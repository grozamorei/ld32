import string

TAB = '    '
TAB2 = '        '
TAB3 = '            '

__NUMERIC = ['int', 'short', 'byte']
__NULLABLE = ['string', 'string[]', 'byte[]', 'short[]']


def format_to_pascal(value):
    """
    :type value: str
    :rtype: str
    """
    return "".join(string.capwords(value, '_').split('_'))


def format_to_camel(value):
    """
    :type value: str
    :rtype: str
    """
    if value.find('_') == -1:
        return value

    parts = value.split('_')
    final = parts[0]
    for i in xrange(1, len(parts)):
        final += parts[i].capitalize()
    return final


def iterate_message_fields(message_descriptor, iterator):
    """
    :type message_descriptor: dict
    :type iterator: lambda
    """
    l = len(message_descriptor)
    for f in xrange(2, l):
        is_last = f == l - 1
        field_name = format_to_camel(message_descriptor[f][0])
        field_type = message_descriptor[f][1]
        if 'string' in field_type:
            field_fixed_size = message_descriptor[f][2]
            iterator(field_name, field_type, is_last, field_fixed_size)
        else:
            iterator(field_name, field_type, is_last)


def default_for_type(type_name, custom_enums):
    if type_name in __NUMERIC:
        return '-1'
    elif type_name in __NULLABLE:
        return None
    elif type_name == 'bool':
        return 'false'
    elif type_name in custom_enums:
        return custom_enums[type_name]['fields'][0]

    assert False