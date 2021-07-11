import json
import copy

## python script to save positions into json

false = False
template_dock = {
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
    ex1 = json.load(open('ex1.json', 'r'))

    entities_list = []

    for el in ex1['namecoords']:
        # {'name': 'dock5', 'filepath': 'modelpath3', 'x': 212.10000610351562, 'y': -4.0, 'z': 136.89999389648438}
        ent_dict = copy.deepcopy(template_dock)
        ent_dict['name'] = el['name']
        # ent_dict['components'][0]['filename'] = set correct filename/ from 'filepath' key
        ent_dict['components'][1]['position']['x'] = el['x']
        ent_dict['components'][1]['position']['y'] = el['y']
        ent_dict['components'][1]['position']['z'] = el['z']
        ### TODO: add rotation

        entities_list.append(ent_dict)

    json.dump(entities_list, open('entities_list.json', 'w'))

    # load docktown3.json base
    js_base = json.load(open('D://D2R//Data//hd//env//preset//act3//docktown//docktown3_base.json', 'r'))
    # add entities from ex1.json unity scene
    js_base['entities'] += entities_list

    # save to final docktown3.json
    json.dump(js_base, open('D://D2R//Data//hd//env//preset//act3//docktown//docktown3.json', 'w'))
