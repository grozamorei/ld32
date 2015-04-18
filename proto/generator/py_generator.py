from base_generator import BaseGenerator
from generator import util
from util import TAB, TAB2, TAB3


class PYGenerator(BaseGenerator):
    def _before_generating(self):
        self._comment = '#'
        self._base_class_name = 'BaseMessage'
        self._get_id_method_name = 'get_id'
        self._send_stream_creator_name = '_init_send_stream'
        self._receive_stream_creator_name = '_init_receive_stream'

    #
    # Upper level routines override
    #

    def _write_includes_and_package(self):
        f = self._file
        for single_import in self._target_d['imports']:
            f.write('import %s\n' % single_import)
        f.write('\n')
        f.write('\n')

    #
    # Sub level routines override
    #

    def _generate_enum(self, enum_descriptor):
        BaseGenerator._generate_enum(self, enum_descriptor)

        f = self._file
        cls_name = util.format_to_pascal(enum_descriptor['cls'])

        f.write('# noinspection PyClassHasNoInit\n')
        f.write('class %s():\n' % cls_name)
        f.write('%s' % TAB)

        fields = enum_descriptor['fields']
        l = len(fields)
        for i in xrange(l):
            if i == l-1:
                f.write('%s = range(%i)\n' % (fields[i].upper(), l))
            else:
                f.write('%s, ' % fields[i].upper())
        f.write('\n')
        f.write('\n')

    def _generate_base_class(self):
        BaseGenerator._generate_base_class(self)