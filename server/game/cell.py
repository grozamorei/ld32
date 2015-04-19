

class Cell():
    def __init__(self, idx):
        self.idx = idx
        self.t_x = 0
        self.t_y = 0
        self.user_id = 0
        self.health = 1
        self.damage = 1

    def is_hostile(self, other_cell):
        if self.user_id == 0 or other_cell.user_id == 0:
            return False
        return other_cell.user_id != self.user_id

    def is_same_user(self, other_cell):
        if self.user_id == 0 or other_cell.user_id == 0:
            return False
        return other_cell.user_id == self.user_id

    def is_occupied(self):
        return self.user_id != 0

    def attack(self):
        self.health -= 1

    def is_dead(self):
        return self.health <= 0

    def reset(self):
        self.user_id = 0
        self.health = 1
        self.damage = 1

    def clone(self):
        c = Cell(self.idx)
        c.user_id = self.user_id
        c.t_x = self.t_x
        c.t_y = self.t_y
        c.health = self.health
        c.damage = self.damage
        return c