import os
import json
from generator.cs_generator import CSGenerator
from generator.py_generator import PYGenerator


def deploy():
    generators = {'cs': CSGenerator, 'py': PYGenerator}
    bindings = json.load(open('bindings.json', 'r'))

    protocol_descriptor_path = os.path.join(os.getcwd(), *bindings['protocol_descriptor'].split(r'/'))
    protocol_descriptor = json.load(open(protocol_descriptor_path, 'r'))

    for target_key in bindings['target_files']:
        target_descriptor = bindings['target_files'][target_key]
        target_descriptor['path'] = os.path.join(os.getcwd(), *target_descriptor['path'].split(r'/'))

        g = generators[target_key](target_descriptor, protocol_descriptor)
        g.generate()

if __name__ == '__main__':
    deploy()