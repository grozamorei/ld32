import logging
from proto.protocol import WorldData

LOGGER = logging.getLogger(__name__.split('.')[-1])


class World():
    def __init__(self, name, size_x, size_y, step, max_population):
        metadata = WorldData()
        self._name = metadata.name = name
        self._size_x = metadata.size_x = size_x
        self._size_y = metadata.size_y = size_y
        self._step = metadata.world_step = step
        self._max_population = metadata.max_population = max_population

        self._free_ids = range(0, 255)
        self._users = {}

        self._raw_metadata = metadata.encode_self()

    @property
    def can_accept(self):
        return len(self._free_ids) > 0

    @property
    def metadata(self):
        return self._raw_metadata

    def add_user(self, user):
        u_id = self._free_ids[0]
        self._free_ids = self._free_ids[1:]
        user.initialize_in_world(self, u_id)
        self._users.setdefault(u_id, user)

    def remove_user(self, user):
        u_id = user.byte_id
        self._free_ids.append(u_id)
        if self._users.has_key(u_id):
            del self._users[user.byte_id]
        else:
            LOGGER.error('removing already absent user! %s ' % user.name)