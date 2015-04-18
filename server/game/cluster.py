from datetime import datetime
import logging
from game.world import World

LOGGER = logging.getLogger(__name__.split('.')[-1])


class WorldCluster():
    def __init__(self):
        self._users = {}
        self._next_user = 0

        self._worlds = {}
        self._next_world = 0

        self._current_time = datetime.now().microsecond

    def get_world(self):
        for world_name in self._worlds:
            w = self._worlds[world_name]
            if w.can_accept:
                return world_name

        new_name = 'world' + str(self._next_world)
        self._next_world += 1
        new_world = World(new_name, 32, 32, 500, 10)
        self._worlds.setdefault(new_name, new_world)
        return new_name

    def get_free_name(self):
        self._next_user += 1
        return 'user' + str(self._next_user)

    def can_enter(self, user_name, world_name):
        return True

    def enter(self, user, world_name):
        return True

    def update(self):
        new_time = datetime.now().microsecond
        dt = new_time - self._current_time
        self._current_time = new_time

        # print 'world update: dt: %r' % dt