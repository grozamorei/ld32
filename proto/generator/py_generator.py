from base_generator import BaseGenerator
from generator import util
from util import TAB, TAB2, TAB3


class PYGenerator(BaseGenerator):
    def _before_generating(self):
        self._comment = '#'
        self._base_class_name = 'BaseMessage'
        self._get_id_method_name = 'id'

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
        f.write('%sself._length = -1\n' % TAB2)
        f.write('%sself._real_id = 255\n' % TAB2)
        f.write('\n')
        f.write('%sself._format = "<i b"\n' % TAB2)
        f.write('%sself._struct = None\n' % TAB2)

    #
    # Message sub level routines override
    #

    def _message_class_header(self, descriptor, message_id):
        f = self._file

        def write_field_definition(field_name, field_type, *_):
            default_value = util.default_for_type(field_type, self._custom_enums)
            if field_type in self._custom_enums:
                default_value = util.format_to_pascal(
                    self._custom_enums[field_type]['cls']) + '.' + default_value.upper()
            f.write('%sself.%s = %s\n' % (TAB2, field_name, default_value, ))

        def add_field_format(field_name, field_type, *_):
            if field_type == 'byte':
                f.write(' b')
            elif field_type == 'short':
                f.write(' h')
            elif field_type == 'int':
                f.write(' i')
            elif field_type == 'string':
                f.write(' b %is' % _[1])
            elif field_type in self._custom_enums:
                f.write(' b')

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

        f.write('\n')

        f.write('%sself._format += "' % TAB2)
        util.iterate_message_fields(descriptor, add_field_format)
        f.write('"\n')
        f.write('%sself._struct = struct.Struct(self._format)\n' % TAB2)

    def _message_receive_constructor(self, descriptor, m_type):
        if m_type == 'server':
            return

        f = self._file

        f.write('\n')
        f.write('%sdef unpack_from(self, raw):\n' % TAB)
        f.write('%svalues = self._struct.unpack(raw)\n' % TAB2)
        f.write('%sself._length = values[0]\n' % TAB2)
        f.write('%sself._real_id = values[1]\n' % TAB2)
        f.write('\n')

        l = len(descriptor)
        struct_i = 2
        for field_i in xrange(2, l):
            field_name = descriptor[field_i][0]
            field_type = descriptor[field_i][1]
            if field_type == 'string':
                struct_i += 1
                f.write('%sself.%s = values[%i].strip()\n' % (TAB2, field_name, struct_i, ))
                struct_i += 1
            else:
                f.write('%sself.%s = values[%i]\n' % (TAB2, field_name, struct_i, ))
                struct_i += 1

    def _message_send_constructor(self, descriptor, m_type):
        if m_type == 'client':
            return

        f = self._file

        f.write('\n')
        f.write('%sdef encode_self(self):\n' % TAB)
        f.write('%s# noinspection PyListCreation\n' % TAB2)
        f.write('%svalues = [self._struct.size - 5, self.%s]\n' % (TAB2, self._get_id_method_name, ))

        l = len(descriptor)
        for field_i in xrange(2, l):
            field_name = descriptor[field_i][0]
            field_type = descriptor[field_i][1]
            if field_type == 'string':
                field_fixed_size = descriptor[field_i][2]
                f.write('%svalues.append(%i)\n' % (TAB2, field_fixed_size, ))

            f.write('%svalues.append(self.%s)\n' % (TAB2, field_name))

        f.write('%sreturn self._struct.pack(*values)\n' % TAB2)