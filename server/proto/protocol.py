#
# ATTENTION: This file is automatically generated. do not try to edit it.
#

import struct
import logging


# noinspection PyClassHasNoInit
class AuthStatus():
    NONE, AUTH_SUCCESS, NAME_OCCUPIED, UNKNOWN = range(4)


# noinspection PyClassHasNoInit
class EnterWorldStatus():
    NONE, ENTER_SUCCESS, TOO_MANY_USERS, UNKNOWN = range(4)


class BaseMessage():
    @property
    def get_id(self):
        raise NotImplementedError()

    def __init__(self):
        self._length = -1
        self._real_id = 255

        self._format = "<i b"
        self._struct = None


class DebugPackage(BaseMessage):
    @property
    def get_id(self):
        return 0

    def __init__(self):
        BaseMessage.__init__(self)
        self.sender = ""
        self.message = ""

        self._format += " b 50s b 120s"
        self._struct = struct.Struct(self._format)

    def unpack_from(self, raw):
        values = self._struct.unpack(raw)
        self.sender = values[3].strip()
        self.message = values[5].strip()


class RequestEnterWorld(BaseMessage):
    @property
    def get_id(self):
        return 1

    def __init__(self):
        BaseMessage.__init__(self)
        self.user_name = ""
        self.world_name = ""

        self._format += " b 50s b 50s"
        self._struct = struct.Struct(self._format)

    def unpack_from(self, raw):
        values = self._struct.unpack(raw)
        self.user_name = values[3].strip()
        self.world_name = values[5].strip()


class RequestCreateWorld(BaseMessage):
    @property
    def get_id(self):
        return 2

    def __init__(self):
        BaseMessage.__init__(self)
        self.user_name = ""
        self.world_name = ""
        self.world_step = -1
        self.world_size_x = -1
        self.world_size_y = -1
        self.world_population = -1

        self._format += " b 50s b 50s h h h h"
        self._struct = struct.Struct(self._format)

    def unpack_from(self, raw):
        values = self._struct.unpack(raw)
        self.user_name = values[3].strip()
        self.world_name = values[5].strip()
        selfworld_step = values[6]
        selfworld_size_x = values[7]
        selfworld_size_y = values[8]
        selfworld_population = values[9]


class Welcome(BaseMessage):
    @property
    def get_id(self):
        return 3

    def __init__(self):
        BaseMessage.__init__(self)
        self.available_name = ""
        self.random_world = ""

        self._format += " b 50s b 50s"
        self._struct = struct.Struct(self._format)


class ResponseAuthorize(BaseMessage):
    @property
    def get_id(self):
        return 4

    def __init__(self):
        BaseMessage.__init__(self)
        self.status = AuthStatus.NONE
        self.token = ""

        self._format += " b b 10s"
        self._struct = struct.Struct(self._format)


class ResponseEnterWorld(BaseMessage):
    @property
    def get_id(self):
        return 5

    def __init__(self):
        BaseMessage.__init__(self)
        self.status = EnterWorldStatus.NONE

        self._format += " b"
        self._struct = struct.Struct(self._format)


class WorldData(BaseMessage):
    @property
    def get_id(self):
        return 6

    def __init__(self):
        BaseMessage.__init__(self)
        self.world_step = -1
        self.size_x = -1
        self.size_y = -1
        self.max_population = -1
        self.players_ids = None
        self.players_names = None

        self._format += " h h h h"
        self._struct = struct.Struct(self._format)


class RoomSnapshot(BaseMessage):
    @property
    def get_id(self):
        return 7

    def __init__(self):
        BaseMessage.__init__(self)
        self.snapshot = None

        self._format += ""
        self._struct = struct.Struct(self._format)
