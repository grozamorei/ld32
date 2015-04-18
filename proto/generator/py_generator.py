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
            if i == l - 1:
                f.write('%s = range(%i)\n' % (fields[i].upper(), l))
            else:
                f.write('%s, ' % fields[i].upper())
        f.write('\n')
        f.write('\n')

    def _generate_base_class(self):
        BaseGenerator._generate_base_class(self)

        f = self._file

        f.write('class %s():\n' % self._base_class_name)
        f.write('%s@property\n' % TAB)
        f.write('%sdef %s(self):\n' % (TAB, self._get_id_method_name, ))
        f.write('%sraise NotImplementedError()\n' % TAB2)

        f.write('\n')

        f.write('%sdef __init__(self):\n' % TAB)
        f.write('%s_length = -1\n' % TAB2)
        f.write('%s_real_id = 255\n' % TAB2)
        f.write('\n')
        f.write('%s_encode_format = "i b"\n' % TAB2)
        f.write('%s_decode_format = "i b"\n' % TAB2)


    #
    # Message sub level routines override
    #
    def _message_class_header(self, descriptor, message_id):
        f = self._file

        def write_field_definition(field_name, field_type, *_):
            default_value = util.default_for_type(field_type, self._custom_enums)
            if field_type in self._custom_enums:
                default_value = util.format_to_pascal(self._custom_enums[field_type]['cls']) + '.' + default_value.upper()
            f.write('%s%s = %s\n' % (TAB2, field_name, default_value, ))

        cls_name = util.format_to_pascal(descriptor[0])
        f.write('\n')
        f.write('\n')
        f.write('class %s(%s):\n' % (cls_name, self._base_class_name, ))
        f.write('%s@property\n' % TAB)
        f.write('%sdef %s(self):\n' % (TAB, self._get_id_method_name, ))
        f.write('%sreturn %i\n' % (TAB2, message_id, ))

        f.write('\n')

        f.write('%sdef __init__(self):\n' % TAB)
        f.write('%s%s.__init__(self)\n' % (TAB2, self._base_class_name, ))
        util.iterate_message_fields(descriptor, write_field_definition)
