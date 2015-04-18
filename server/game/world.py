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
        self._current_population = 0

        self._raw_metadata = metadata.encode_self()

    @property
    def can_accept(self):
        return self._current_population < self._max_population

    @property
    def metadata(self):
        return self._raw_metadata