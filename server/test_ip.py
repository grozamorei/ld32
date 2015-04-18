import logging
import struct
from tornado.httpserver import HTTPServer

from tornado.ioloop import IOLoop
from tornado.web import Application
from tornado.websocket import WebSocketHandler

class WSHandler(WebSocketHandler):
    def data_received(self, chunk):
        print 'data received: ' + chunk

    def open(self):
        self.set_nodelay(True)
        print 'new connection'

    def on_message(self, message):
        self.write_message(message, True)

        len = message[:4]
        zaza = struct.unpack('i b b 50s b 50s', message)
        print 'message received %s : + i' % (message, )

    def on_close(self):
      print 'connection closed'

    def check_origin(self, origin):
        return True


application = Application([
    (r'/echo', WSHandler),
])
# my ip: 188.242.130.83


def start(port):
    io_loop = IOLoop.instance()
    http_server = HTTPServer(application, io_loop=io_loop)
    http_server.listen(port)
    io_loop.start()


if __name__ == '__main__':
    logging.basicConfig(format='%(levelname)s::%(asctime)s {%(module)s:%(lineno)d} :: %(message)s ::',
                            datefmt='[%d.%m.%Y %I:%M:%S]', level=20)
    start(3000)