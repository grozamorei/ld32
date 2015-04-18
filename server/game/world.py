import logging

LOGGER = logging.getLogger(__name__.split('.')[-1])


class World():
    def __init__(self, name, size_x, size_y, step, max_population):
        self._name = name
        self._size_x = size_x
        self._size_y = size_y
        self._step = step
        self._max_population = max_population
        self._current_population = 0

    @property
    def can_accept(self):
        return self._current_population < self._max_population