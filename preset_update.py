"""python script to save positions, rotations and scale into a base json"""

import json
import copy

### Change these paths to load your map preset. These paths are examples using my own docktown3 location
docktown_base = './/docktown3_base.json'
docktown_final = 'D://D2R//Data//hd//env//preset//act3//docktown//docktown3.json'

false = False
template_ent = {
        "type": "Entity", "name": "dock_template", "id": 3217705948,
        "components": [
          {
            "type": "ModelDefinitionComponent",
            "name": "dock_template_components_ModelDefinitionComponent",
            "filename": "data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model",
            "visibleLayers": 1,
            "lightMask": 19,
            "shadowMask": 1,
            "ghostShadows": false,
            "floorModel": false,
            "terrainBlendEnableYUpBlend": false,
            "terrainBlendMode": 1
          },
          {
            "type": "TransformDefinitionComponent",
            "name": "dock_template_components_TransformDefinitionComponent",
            "position": {"x": 271, "y": -4, "z": 260},
            "orientation": {"x": 0, "y": 0, "z": 0, "w": 1},
            "scale": {"x": 1, "y": 1, "z": 1},
            "inheritOnlyPosition": false
          }
        ]
      }

def write_json_d2r():
    ex1 = json.load(open('unityscene.json', 'r'))

    entities_list = []
    models_list = []

    for el in ex1['namecoords']:
        # {'name': 'dock5', 'filepath': 'modelpath3', 'x': 212.10000610351562, 'y': -4.0, 'z': 136.89999389648438}
        ent_dict = copy.deepcopy(template_ent)
        ent_dict['name'] = el['name']
        ent_dict['components'][0]['filename'] = el['filepath']
        ent_dict['components'][1]['position']['x'] = el['x']
        ent_dict['components'][1]['position']['y'] = el['y']
        ent_dict['components'][1]['position']['z'] = el['z']
        ## quaternion rotation
        ent_dict['components'][1]['orientation']['x'] = el['qx']
        ent_dict['components'][1]['orientation']['y'] = el['qy']
        ent_dict['components'][1]['orientation']['z'] = el['qz']
        ent_dict['components'][1]['orientation']['w'] = el['qw']
        ## scale
        ent_dict['components'][1]['scale']['x'] = el['scalex']
        ent_dict['components'][1]['scale']['y'] = el['scaley']
        ent_dict['components'][1]['scale']['z'] = el['scalez']

        entities_list.append(ent_dict)

        if {"path": el['filepath']} not in models_list:
          models_list.append({"path": el['filepath']})

    json.dump(entities_list, open('entities_list.json', 'w'))

    # load docktown3.json base
    js_base = json.load(open(docktown_base, 'r'))
    # add entities from unityscene.json unity scene
    js_base['entities'] += entities_list
    js_base['dependencies']['models'] += models_list

    # save to final docktown3.json
    json.dump(js_base, open(docktown_final, 'w'))


write_json_d2r()
