import logging
import tornado
import os
from tornado.web import Application
from net.ws_handler import WSHandler

LOGGER = logging.getLogger(__name__.split('.')[-1])

class GameApplication(Application):
    HANDLERS = [
        (r'/echo', WSHandler),
        (r'/static/(.*)', tornado.web.StaticFileHandler, {'path': os.path.join(os.getcwd(), 'static')})

    ]

    def __init__(self, cluster):
        h = GameApplication.HANDLERS
        super(GameApplication, self).__init__(h, "", None, cluster=cluster)
        LOGGER.info('TornadoApplication created')