import logging
from random import randint
from proto.protocol import NewSeed, SeedDestroyed

LOGGER = logging.getLogger(__name__.split('.')[-1])


class Seed():
    def __init__(self, owner_id, location):
        self.owner_id = owner_id
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

        self.pento = [0, -1,
                      1, -1,
                      -1, 0,
                      0, 0,
                      0, 1]

        self.glider = [1, -1,
                       -1, 0,
                       1, 0,
                       0, 1,
                       1, 1]

        self.s_counter = 2

    def set_config(self, new_config):
        pass

    def damage(self, amount):
        self.health -= amount

    def is_dead(self):
        return self.health <= 0

    def can_spawn(self):
        if self.s_counter == 0:
            self.s_counter = 5
            return True
        self.s_counter -= 1

    def spawn(self, board, maxX):
        mycell = board[self.location]

        iteratefrom = self.pento if randint(0, 1) == 0 else self.glider
        for i in range(0, len(iteratefrom), 2):
            coord = (iteratefrom[i+1] + mycell.t_y) * maxX + iteratefrom[i] + mycell.t_x
            board[coord].reset()
            board[coord].user_id = self.owner_id

    #         glider = new int[10];
    #         glider[0] = 2; glider[1] = 0;
    #         glider[2] = 0; glider[3] = 1;
    #         glider[4] = 2; glider[5] = 1;
    #         glider[6] = 1; glider[7] = 2;
    #         glider[8] = 2; glider[9] = 2;

            # glider = new int[10];
            # glider[0] = 2; glider[1] = 0;
            # glider[2] = 0; glider[3] = 1;
            # glider[4] = 2; glider[5] = 1;
            # glider[6] = 1; glider[7] = 2;
            # glider[8] = 2; glider[9] = 2;