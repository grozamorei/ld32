import logging
from game.seed import Seed

LOGGER = logging.getLogger(__name__.split('.')[-1])


class User():
    def __init__(self, ws, name):
        """
        :type ws: WSHandler
        :type name: str
        """
        self.name = name
        self.ws = ws

        self.byte_id = -1
        self.world = None
        self.seeds = []

    def initialize_in_world(self, world, id):
        self.world = world
        self.byte_id = id

    def add_seed(self, location):
        s = Seed(self.byte_id, location)
        self.seeds.append(s)

    def remove_seed(self, location):
        for s in self.seeds:
            if s.location == location:
                self.seeds.remove(s)