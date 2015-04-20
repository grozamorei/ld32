import logging
import zlib
from game.cell import Cell
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

        self.temp = []
        self._board = []
        for i in range(size_x * size_y):
            self._board.append(Cell(i))

        self._free_ids = range(1, 9)
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
            self._board[c].user_id = user_id

    def deploy_bomb(self, at_location):
        # pass
        cell = self._board[at_location]
        for i in range(-1, 2):
            for j in range(-1, 2):
                if i == 0 and j == 0:
                    continue
                target_x = cell.t_x + i
                target_y = cell.t_y + j

                if target_x < 0:
                    target_x = self._size_x-1
                if target_x >= self._size_x:
                    target_x = 0
                if target_y < 0:
                    target_y = self._size_y-1
                if target_y >= self._size_y:
                    target_y = 0

                n = self.temp[target_x][target_y]
                self._board[n.idx].reset()

    def broadcast(self, message_raw):
        for u_id in self._users:
            self._users[u_id].ws.write_message(message_raw, True)

    def step(self, dt):
        if self.is_empty:
            return

        self._calc_shit()

        tt = WorldSnapshot()
        raw = ''.join(str(x.user_id) for x in self._board)
        # comp = zlib.compress(raw)
        tt.snapshot = raw
        zz = tt.encode_self()
        l = len(zz)
        self.broadcast(zz)

    def _calc_shit(self):
        #
        # fight:

        self.temp = []
        for i in range(self._size_x):
            self.temp.append([])
            for j in range(self._size_y):
                idx = j * self._size_x + i
                c = self._board[idx]
                c.t_x = i
                c.t_y = j
                self.temp[i].append(c.clone())

        for cell in self._board:
            hostiles = self._get_hostiles(cell, self.temp)
            if len(hostiles) == 0:
                continue

            self._board[hostiles[0]].attack()

        for cell in self._board:
            if cell.is_dead():
                cell.reset()
            if self.temp[cell.t_x][cell.t_y].is_dead():
                cell.reset()


        # new_board = self._board[:]
        for cell in self._board:
            if cell.user_id == 0:
                neighbours = self._get_neighbours(cell, self.temp)
                # LOGGER.warning('NEIGH: %s : %r' % (cell.idx, neighbours))
                if len(neighbours) == 3:
                    self._board[cell.idx].user_id = neighbours[0]
                else:
                    # if len(neighbours) > 0:
                    #     LOGGER.warning('%s: %r' % (cell.idx, neighbours))
                    # if len(neighbours) > 3:
                    #     LOGGER.warning(neighbours)
                    # self._board[cell.idx].
                    pass
            else:
                friends = self._get_friends(cell, self.temp)
                # LOGGER.warning('friends: %s : %r' % (cell.idx, friends))
                l = len(friends)
                if l == 2 or l == 3:
                    pass
                else:
                    self._board[cell.idx].reset()
                    cell.reset()

        # for i in range(self._size_x):
        #     for j in range(self._size_y):
        #         self._board[j * self._size_x + i] = new_board[i][j]

    def _get_hostiles(self, cell, board2d):
        hostiles = []
        for i in range(-1, 2):
            for j in range(-1, 2):
                target_x = cell.t_x + i
                target_y = cell.t_y + j

                if target_x < 0:
                    target_x = self._size_x-1
                if target_x >= self._size_x:
                    target_x = 0
                if target_y < 0:
                    target_y = self._size_y-1
                if target_y >= self._size_y:
                    target_y = 0

                n = board2d[target_x][target_y]
                if n.is_hostile(cell):
                    hostiles.append(n.idx)

        return hostiles

    def _get_friends(self, cell, board2d):
        friends = []
        for i in range(-1, 2):
            for j in range(-1, 2):
                if i == 0 and j == 0:
                    continue
                target_x = cell.t_x + i
                target_y = cell.t_y + j

                if target_x < 0:
                    target_x = self._size_x-1
                if target_x >= self._size_x:
                    target_x = 0
                if target_y < 0:
                    target_y = self._size_y-1
                if target_y >= self._size_y:
                    target_y = 0

                n = board2d[target_x][target_y]
                if n.is_same_user(cell):
                    friends.append(n.idx)

        return friends

    def _get_neighbours(self, cell, board2d):
        neighbours = []
        for i in range(-1, 2):
            for j in range(-1, 2):
                target_x = cell.t_x + i
                target_y = cell.t_y + j

                if target_x < 0:
                    target_x = self._size_x-1
                if target_x >= self._size_x:
                    target_x = 0
                if target_y < 0:
                    target_y = self._size_y-1
                if target_y >= self._size_y:
                    target_y = 0

                n = board2d[target_x][target_y]
                if n.is_occupied():
                    neighbours.append(n.user_id)

        return neighbours