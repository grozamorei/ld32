import logging
import struct
from tornado.websocket import WebSocketHandler
from proto.protocol import Welcome, DebugPackage, RequestEnterWorld, ResponseEnterWorld, EnterWorldStatus
from game.user import User

LOGGER = logging.getLogger(__name__.split('.')[-1])


# noinspection PyClassHasNoInit
class _AuthStatus():
    UNKNOWN, WELCOME_SEND, AUTHORIZED, REJECTED = range(4)


class WSHandler(WebSocketHandler):
    def __init__(self, application, request, **kwargs):
        super(WSHandler, self).__init__(application, request, **kwargs)
        self._cluster = application.settings['cluster']
        self._status = _AuthStatus.UNKNOWN
        self._partial_message = None
        LOGGER.debug('Socket created')

    def data_received(self, chunk):
        LOGGER.info('Data chunk received: %r' % chunk)

    def open(self):
        self.set_nodelay(True)
        w = Welcome()
        w.available_name = self._cluster.get_free_name()
        w.random_world = self._cluster.get_world()
        LOGGER.info('Incoming connection; sending welcome %s::%s' % (w.available_name, w.random_world, ))
        self.write_message(w.encode_self(), True)
        self._status = _AuthStatus.WELCOME_SEND

    def on_message(self, message):
        if len(message) < 4:
            self._store_add_partial(message)
            return

        if self._partial_message is not None:
            LOGGER.warning('adding partial message: %s + %s' % (self._partial_message, message, ))
            message = self._partial_message + message
            self._partial_message = None

        message_len = struct.unpack('i', message[0:4])[0]
        real_len = len(message) - 5

        if real_len < message_len:
            self._store_add_partial(message)
            return
        elif real_len > message_len:
            LOGGER.error('AHAHAHAH CHTO ETO U NAS NAKONEC TO')
            self.close(500, "prosti paren'")

        if message[4] == DebugPackage.ID:
            d = DebugPackage()
            d.unpack_from(message)
            LOGGER.info('Debug package from %s with %s' % (d.sender, d.message, ))
            return

        if self._status == _AuthStatus.WELCOME_SEND:
            message_id = struct.unpack_from('b', message[4])[0]
            if message_id == RequestEnterWorld.ID:
                res = ResponseEnterWorld()
                req_enter = RequestEnterWorld()
                req_enter.unpack_from(message)

                name = req_enter.user_name
                world = req_enter.world_name
                if self._cluster.can_enter(name, world):
                    u = User(self, name)
                    if self._cluster.enter(u, world):
                        LOGGER.info('User %s entering world %s' % (u.name, u.world._name))
                        res.status = EnterWorldStatus.ENTER_SUCCESS
                        res.my_id = u.byte_id
                        self.write_message(res.encode_self(), True)
                        self.write_message(u.world.metadata, True)
                        self._status = _AuthStatus.AUTHORIZED
                    else:
                        LOGGER.info('User %s was refused to enter the world %s' % (name, u.world._name))
                        res.status = EnterWorldStatus.NONE
                        self.write_message(res.encode_self(), True)
                else:
                    LOGGER.info('User was refused to enter the cluster with name %s' % name)
                    res.status = EnterWorldStatus.NONE
                    self.write_message(res.encode_self(), True)
        elif self._status == _AuthStatus.AUTHORIZED:
            LOGGER.info('message from logged in user: ')
        else:
            LOGGER.warning('unknown state : %i' % self._status)

    def on_close(self):
        LOGGER.info('Connection closed: ')

    def check_origin(self, origin):
        return True

    def _store_add_partial(self, chunk):
        LOGGER.warning('storing partial message: %s' % chunk)
        if self._partial_message is not None:
            self._partial_message += chunk
        else:
            self._partial_message = chunk