import logging
from proto.protocol import NewSeed, SeedDestroyed

LOGGER = logging.getLogger(__name__.split('.')[-1])


class Seed():
    def __init__(self, owner_id, location):
        self.id = owner_id
        self.location = location
        self.health = 1
        self.configuration = 0

        self.new_cmd = NewSeed()
        self.new_cmd.location = location
        self.new_cmd.owner = owner_id
        self.new_cmd = self.new_cmd.encode_self()

        self.destr_cmd = SeedDestroyed()
        self.destr_cmd.location = location
        self.destr_cmd = self.destr_cmd.encode_self()

    def set_config(self, new_config):
        pass

    def damage(self, amount):
        self.health -= amount

    def is_dead(self):
        return self.health <= 0