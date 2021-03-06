# coding=utf-8
import time
import logging
from game.world import World

LOGGER = logging.getLogger(__name__.split('.')[-1])


class WorldCluster():
    def __init__(self):
        self._users = {}
        self._next_user = 0

        self._worlds = {}
        self._next_world = 0

        self._current_time = int(time.time() * 1000)

    def get_world(self):
        for world_name in self._worlds:
            w = self._worlds[world_name]
            if w.can_accept:
                return world_name

        new_name = u'world#'.encode('utf-8') + str(self._next_world)
        self._next_world += 1
        new_world = World(new_name, 64, 64, 2000, 10)
        self._worlds.setdefault(new_name, new_world)
        return new_name

    def get_free_name(self):
        self._next_user += 1
        return u'user#'.encode('utf-8') + str(self._next_user)

    def can_enter(self, user_name, world_name):
        return True

    def enter(self, user, world_name):
        self._worlds[world_name].add_user(user)
        return True

    def update(self):
        new_time = int(time.time() * 1000)
        dt = new_time - self._current_time
        self._current_time = new_time

        for world in self._worlds:
            self._worlds[world].step(dt)