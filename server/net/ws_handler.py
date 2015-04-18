import logging
from tornado.websocket import WebSocketHandler

LOGGER = logging.getLogger(__name__.split('.')[-1])


class WSHandler(WebSocketHandler):
    def __init__(self, application, request, **kwargs):
        super(WSHandler, self).__init__(application, request, **kwargs)
        self._cluster = application.settings['cluster']
        LOGGER.info('Socket created')

    def data_received(self, chunk):
        LOGGER.info('Data chunk received: %r' % chunk)

    def open(self):
        self.set_nodelay(True)
        LOGGER.info('Incoming connection')

    def on_message(self, message):
        LOGGER.info('Message received: %r' % message)
        # d = DebugPackage()
        # d.unpack_from(message)
        # print d
        #
        # encoded = d.encode_self()
        # self.write_message(encoded, True)
        pass

    def on_close(self):
        LOGGER.info('Connection closed: ')

    def check_origin(self, origin):
        return True