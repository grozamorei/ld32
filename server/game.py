import logging
from game.cluster import WorldCluster
from tornado.httpserver import HTTPServer

from tornado.ioloop import IOLoop, PeriodicCallback
from net.game_application import GameApplication


class Game():
    def __init__(self):
        self._io_loop = IOLoop.instance()
        self._cluster = WorldCluster()
        self._tornado_app = GameApplication(self._cluster)
        self._http_server = HTTPServer(self._tornado_app, False, self._io_loop)

    def start(self, port, step):
        self._http_server.listen(port)
        c = PeriodicCallback(self._cluster.update, step, self._io_loop)
        c.start()
        logging.info('starting on port %i. cluster step = %ims' % (port, step, ))
        self._io_loop.start()


if __name__ == '__main__':
    logging.basicConfig(format='%(levelname)s::%(asctime)s {%(module)s:%(lineno)d} :: %(message)s ::',
                        datefmt='[%d.%m.%Y %I:%M:%S]', level=20)
    Game().start(3000, 1000)