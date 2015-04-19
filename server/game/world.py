import logging
import zlib
from proto.protocol import WorldData, WorldSnapshot

LOGGER = logging.getLogger(__name__.split('.')[-1])


class World():
    def __init__(self, name, size_x, size_y, step, max_population):
        metadata = WorldData()
        self._name = metadata.name = name
        self._size_x = metadata.size_x = size_x
        self._size_y = metadata.size_y = size_y
        self._step = metadata.world_step = step
        self._max_population = metadata.max_population = max_population

        self._board = []
        for i in range(size_x * size_y):
            self._board.append(0)

        self._free_ids = range(1, 120)
        self._users = {}

        self._raw_metadata = metadata.encode_self()

    @property
    def can_accept(self):
        return len(self._free_ids) > 0

    @property
    def metadata(self):
        return self._raw_metadata

    @property
    def is_empty(self):
        return len(self._users) == 0

    def add_user(self, user):
        u_id = self._free_ids[0]
        self._free_ids = self._free_ids[1:]
        user.initialize_in_world(self, u_id)
        self._users.setdefault(u_id, user)
        LOGGER.info('ADDED user [%i]:%s. new population: %i' % (u_id, user.name, len(self._users), ))

    def remove_user(self, user):
        u_id = user.byte_id
        self._free_ids.append(u_id)
        if u_id in self._users:
            del self._users[user.byte_id]
        else:
            LOGGER.error('removing already absent user! %s ' % user.name)
        LOGGER.info('REMOVED user [%i]:%s. new population: %i' % (u_id, user.name, len(self._users), ))

    def debug_deploy_configuration(self, user_id, configuration):
        LOGGER.info('user [%i]%s deployed configuration: %r' % (user_id, self._users[user_id].name, configuration))
        for c in configuration:
            if self._board[c] != 0:
                continue
            self._board[c] = user_id

    def broadcast(self, message_raw):
        for u_id in self._users:
            self._users[u_id].ws.write_message(message_raw, True)

    def step(self, dt):
        if self.is_empty:
            return

        tt = WorldSnapshot()
        raw = ''.join(str(x) for x in self._board)
        # comp = zlib.compress(raw)
        tt.snapshot = raw
        zz = tt.encode_self()
        l = len(zz)
        self.broadcast(zz)