import logging

LOGGER = logging.getLogger(__name__.split('.')[-1])


class User():
    def __init__(self, name):
        self.byte_id = -1
        self.name = name
        self.world = None

    def initialize_in_world(self, world, id):
        self.world = world
        self.byte_id = id