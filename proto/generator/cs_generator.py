from base_generator import BaseGenerator
from generator import util
from util import TAB, TAB2, TAB3, TAB4


class CSGenerator(BaseGenerator):
    def _before_generating(self):
        BaseGenerator._before_generating(self)
        self._comment = '//'
        self._base_class_name = 'BaseMessage'
        self._get_id_method_name = 'getID'
        self._send_stream_creator_name = 'initSendStream'
        self._receive_stream_creator_name = 'initReceiveStream'

    #
    # Upper level routines override
    #

    def _write_includes_and_package(self):
        f = self._file
        for single_import in self._target_d['imports']:
            f.write('using %s;\n' % single_import)
        f.write('\n')
        f.write('namespace %s\n' % self._target_d['namespace'])

    def _before_body(self):
        self._file.write('{\n\n')

    def _after_body(self):
        self._file.write('}')

    #
    # Sub level routines override
    #

    def _generate_enum(self, enum_descriptor):
        BaseGenerator._generate_enum(self, enum_descriptor)

        f = self._file
        cls_name = util.format_to_pascal(enum_descriptor['cls'])
        f.write('%spublic enum %s\n' % (TAB, cls_name, ))
        f.write('%s{\n' % TAB)

        for enum_field in enum_descriptor['fields']:
            f.write('%s%s,\n' % (TAB2, enum_field.upper(), ))

        f.write('%s}\n\n' % TAB)

    def _generate_base_class(self):
        f = self._file
        f.write('%spublic abstract class %s\n' % (TAB, self._base_class_name, ))
        f.write('%s{\n' % TAB)

        f.write('%spublic abstract byte %s();\n' % (TAB2, self._get_id_method_name))

        f.write('\n')
        f.write('%sprotected MemoryStream stream;\n' % TAB2)
        f.write('%sprotected BinaryWriter writer;\n' % TAB2)
        f.write('%sprotected BinaryReader reader;\n' % TAB2)
        f.write('\n')

        f.write('%sprotected int length = -1;\n' % TAB2)
        f.write('%sprotected byte realID = 255;\n' % TAB2)
        f.write('\n')

        f.write('%sprotected void %s()\n' % (TAB2, self._send_stream_creator_name, ))
        f.write('%s{\n' % TAB2)
        f.write('%sstream = new MemoryStream();\n' % TAB3)
        f.write('%swriter = new BinaryWriter(stream, System.Text.Encoding.UTF8);\n' % TAB3)
        f.write('%sfor (int i = 0; i < 5; i++) writer.Write((byte) 0); // reserved for length and type;\n' % TAB3)
        f.write('%s}\n' % TAB2)
        f.write('\n')

        f.write('%sprotected void %s(byte[] source)\n' % (TAB2, self._receive_stream_creator_name, ))
        f.write('%s{\n' % TAB2)
        f.write('%sstream = new MemoryStream(source);\n' % TAB3)
        f.write('%sreader = new BinaryReader(stream, System.Text.Encoding.UTF8);\n' % TAB3)
        f.write('%slength = reader.ReadInt32();\n' % TAB3)
        f.write('%srealID = reader.ReadByte();\n' % TAB3)
        f.write('%s}\n' % TAB2)
        f.write('\n')


        f.write('%sprotected string wrapString(string value, int wrapAround)\n' % TAB2)
        f.write('%s{\n' % TAB2)
        f.write('%sbyte[] byteValue = System.Text.Encoding.UTF8.GetBytes(value);\n' % TAB3)
        f.write('%sint emptyLength = wrapAround - byteValue.Length;\n' % TAB3)
        f.write('%sstring filler = "";\n' % TAB3)
        f.write('%sfor (int i = 0; i < emptyLength; i++) filler += " ";\n' % TAB3)
        f.write('%sreturn value + filler;\n' % TAB3)
        f.write('%s}\n' % TAB2)
        f.write('\n')

        f.write('%sprotected byte[] wrapCommand()\n' % TAB2)
        f.write('%s{\n' % TAB2)
        f.write('%sint commandLen = (int)stream.Length - 5;\n' % TAB3)
        f.write('%swriter.Seek(0, SeekOrigin.Begin);\n' % TAB3)
        f.write('%swriter.Write(commandLen);\n' % TAB3)
        f.write('%swriter.Write(%s());\n' % (TAB3, self._get_id_method_name, ))
        f.write('%sreturn stream.ToArray();\n' % TAB3)
        f.write('%s}\n' % TAB2)
        f.write('%s}\n\n' % TAB)

    #
    # Message sub level routines override
    #

    def _message_class_header(self, descriptor, message_id):
        f = self._file

        def write_field_definition(field_name, field_type, *_):
            field_name = util.format_to_camel(field_name)
            field_type = util.format_to_pascal(field_type) if field_type in self._custom_enums else field_type
            f.write('%spublic readonly %s %s;\n' % (TAB2, field_type, field_name, ))

        cls_name = util.format_to_pascal(descriptor[0])
        f.write('%spublic class %s : %s\n' % (TAB, cls_name, self._base_class_name, ))
        f.write('%s{\n' % TAB)
        f.write('%spublic static byte ID { get { return %i; } }\n' % (TAB2, message_id, ))
        f.write('%spublic override byte %s() { return %i; }\n' % (TAB2, self._get_id_method_name, message_id, ))

        if len(descriptor) <= 2:
            return

        util.iterate_message_fields(descriptor, write_field_definition)
        f.write('%s\n' % TAB)

    def _message_class_footer(self):
        self._file.write('%s}\n\n' % TAB)

    def _message_send_constructor(self, descriptor, m_type):
        if m_type == 'server':
            return

        f = self._file

        if len(descriptor) <= 2:
            return

        def write_constructor_args(field_name, field_type, last, *_):
            field_name = util.format_to_camel(field_name)
            spacing = ' ' if last else ', '
            f.write('%s %s%s' % (field_type, field_name, spacing))

        def write_fields_init(field_name, *_):
            field_name = util.format_to_camel(field_name)
            f.write('%sthis.%s = %s;\n' % (TAB3, field_name, field_name))

        f.write('%spublic %s( ' % (TAB2, util.format_to_pascal(descriptor[0])))
        util.iterate_message_fields(descriptor, write_constructor_args)
        f.write(')\n')

        f.write('%s{\n' % TAB2)
        f.write('%s%s();\n' % (TAB3, self._send_stream_creator_name, ))
        util.iterate_message_fields(descriptor, write_fields_init)
        f.write('%s}\n' % TAB2)

    def _message_send_encode(self, descriptor, m_type):
        if m_type == 'server':
            return

        f = self._file

        def write_field(field_name, field_type, *_):
            field_name = util.format_to_camel(field_name)
            if field_type == 'string' and _[1]:
                f.write('%swriter.Write(wrapString(%s, %i));\n' % (TAB3, field_name, _[1]))
            if field_type == 'int[]':
                f.write('%swriter.Write((int)%s.Length);\n' % (TAB3, field_name))
                f.write('%sfor (int i = 0; i < %s.Length; i++)\n' % (TAB3, field_name))
                f.write('%swriter.Write(%s[i]);\n' % (TAB4, field_name))
                f.write('\n')
            else:
                f.write('%swriter.Write(%s);\n' % (TAB3, field_name, ))

        f.write('%spublic byte[] encode()\n' % TAB2)
        f.write('%s{\n' % TAB2)

        util.iterate_message_fields(descriptor, write_field)

        f.write('%sreturn wrapCommand();\n' % TAB3)
        f.write('%s}\n' % TAB2)

    def _message_receive_constructor(self, descriptor, m_type):
        if m_type == 'client':
            return

        f = self._file

        def reader_method(field_type):
            if field_type == 'byte' or field_type in self._custom_enums:
                return 'ReadByte'
            if field_type == 'short':
                return 'ReadInt16'
            if field_type == 'string':
                return 'ReadString'
            if field_type in ['string[]', 'byte[]', 'short[]']:
                return None
            assert False

        def read_from_byte_array(field_name, field_type, *_):
            field_name = util.format_to_camel(field_name)
            method = reader_method(field_type)
            if field_type in self._custom_enums:
                f.write('%s%s = (%s)reader.%s();\n' % (TAB3, field_name, util.format_to_pascal(field_type), method, ))
            elif method is not None:
                f.write('%s%s = reader.%s();\n' % (TAB3, field_name, method, ))

        cls_name = util.format_to_pascal(descriptor[0])
        f.write('%spublic %s(byte[] source)\n' % (TAB2, cls_name))
        f.write('%s{\n' % TAB2)
        f.write('%s%s(source);\n' % (TAB3, self._receive_stream_creator_name, ))

        util.iterate_message_fields(descriptor, read_from_byte_array)

        f.write('%s}\n' % TAB2)