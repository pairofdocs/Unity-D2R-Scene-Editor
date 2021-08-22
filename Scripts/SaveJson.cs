using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[SelectionBase]
public class SaveJson : MonoBehaviour
{
    public string preset = "docktown3.json";                 // Change this to be the json preset that is edited
    public static string d2rDataPath = "D:/D2R - beta/";     // Change this to where your D2R data is extracted (casc storage)
    public string lastModlPath;
    public bool loadTextures = true;                         // Set this to false to avoid loading textures. Not loading textures may make Unity load scenes faster
        
    // Start is called before the first frame update
    void Start()
    {
        lastModlPath = d2rDataPath + "Data/hd/";

        Debug.Log("Preset to edit: " + Application.dataPath + "/" + preset);
        JObject jsondata = JObject.Parse(File.ReadAllText(Application.dataPath + "/" + preset));

        // json documentation https://www.newtonsoft.com/json/help/html/ModifyJson.htm
        JArray ents = (JArray)jsondata["entities"];

        foreach(JObject ob in ents) {
            string entname = (string)ob["name"];
            JArray comps = (JArray)ob["components"];
            GameObject modelObj = new GameObject(entname);                      // terrain, waypoint01, ...
            Vector3 entpos = new Vector3(); 
            Quaternion entquat = new Quaternion(); 
            Vector3 entscale = new Vector3();                                  // ModelDef and mesh have to be set first. Then transformDef

            foreach(JObject comp in comps) {
                string modelfile; string[] strArr;
                if ( ((string) comp["type"]) == "ModelDefinitionComponent" ) {
                    // Note: some ents don't have ModelDefinitionComponents (but model variation comps.  e.g. l_wall01_06  in towns1.json)
                    modelfile = (string) comp["filename"];
                    strArr = modelfile.Split('/');     // NOTE:  use single quotes for char '/',  "/" is a string
                    // Debug.Log("component:              " + strArr[strArr.Length-1].Replace(".model",""));
                    try {
                        // Load mesh for model   // "path": "data/hd/env/model/act1/outdoors/act1_outdoors_props/waypoint01.model"
                        string fullpath = d2rDataPath + modelfile;
                        fullpath = fullpath.Replace(".model", "_lod0.model");   // append _lod3 to model basename.      **NOTE:** was only able to load lod0.model
                        
                        var mesh = modelObj.AddComponent<MeshLoader>();                               // add component
                        var root = LSLib.Granny.GR2Utils.LoadModel(fullpath);  
                        mesh.Load(root, fullpath, loadTextures);
                    }
                    catch (Exception e) {
                        Destroy(GameObject.Find(entname));
                        if (!strArr[strArr.Length-1].Contains("terrain.model")) {
                            Debug.LogException(e, this);
                        }
                    }
                }
                else if ( ((string) comp["type"]) == "TransformDefinitionComponent" ) {
                    // Debug.Log("transformDef component:              " + entname);
                    // **NOTE** render objects with x--> -x  since the orientation is like that in-game
                    JObject pos = (JObject)comp["position"];                  // orientation, scale
                    entpos = new Vector3(-1*(float) pos["x"], (float) pos["y"], (float) pos["z"]);

                    // Quaternion rotation "orientation": {"x": 0, "y": 0.707, "z": 0, "w": 0.707},   https://docs.unity3d.com/ScriptReference/Quaternion.html
                    // https://answers.unity.com/questions/476128/how-to-change-quaternion-by-180-degrees.html
                    JObject ori = (JObject)comp["orientation"];
                    entquat = new Quaternion((float) ori["x"], -1*(float) ori["y"], -1*(float) ori["z"], (float) ori["w"] );
                    // modelObj.transform.rotation = Quaternion.Euler(quat.eulerAngles.x, quat.eulerAngles.y, quat.eulerAngles.z);

                    // scale.   NOTE:  4x scale helm in unity was much smaller than 4x scale helm in-game
                    JObject scale = (JObject)comp["scale"];
                    entscale = new Vector3((float) scale["x"], (float) scale["y"], (float) scale["z"]);
                }
            }
            // ModelDef and mesh have to be set first. Then transformDef
            modelObj.transform.position = entpos;
            modelObj.transform.rotation = entquat;
            modelObj.transform.localScale = entscale;
        }
        // https://answers.unity.com/questions/1034060/create-unity-ui-panel-via-script.html    (renderMode, raycaster)
        GameObject newCanvas = new GameObject("Canvas");
        Canvas c = newCanvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;      
        newCanvas.AddComponent<GraphicRaycaster>();       // this is needed along with an eventSystem

        var es = new GameObject("EventSystem", typeof(EventSystem));
        es.AddComponent<StandaloneInputModule>();            // https://forum.unity.com/threads/creating-a-gui-from-code.263563/
        // Add button for saving json
        GameObject buttonPrefab = Resources.Load("Save") as GameObject;    // using as Button didnt work
        GameObject saveObj = Instantiate(buttonPrefab) as GameObject;
        saveObj.transform.SetParent(newCanvas.transform, false);

        Button saveButton = saveObj.GetComponent<Button>();
        saveButton.onClick.AddListener(SaveObjsToJson);            // -v' works. needed event listener     ~~~~~
        
        // Add button for loading model files
        GameObject buttonAddPrefab = Resources.Load("AddModel") as GameObject;    // using as Button didnt work
        GameObject addObj = Instantiate(buttonAddPrefab) as GameObject;
        addObj.transform.SetParent(newCanvas.transform, false);

        Button addButton = addObj.GetComponent<Button>();
        addButton.onClick.AddListener(AddModelToScene);            // -v' works. needed event listener     ~~~~~
    }

    void SaveObjsToJson()
    {   // public void https://answers.unity.com/questions/1226228/function-not-appearing-in-onclick-editor.html
        Debug.Log("SaveObjsToJson() called ");

        // read preset json and append entities. Read terrain entity from a template json file
        JObject jsonpreset = JObject.Parse(File.ReadAllText(Application.dataPath + "/" + preset));
        jsonpreset["entities"] = new JArray();
        jsonpreset["dependencies"]["models"] = new JArray();
        List<string> models_list = new List<string>();
        
        JObject terrain_ent = JObject.Parse(File.ReadAllText(Application.dataPath + "/terrain_template.json"));
        ((JArray)jsonpreset["entities"]).Add(terrain_ent);
        // Debug.Log("jsonpreset:                " + jsonpreset);

        string template_ent = @"{
            'type': 'Entity', 'name': 'dock_template', 'id': 3217705948,
            'components': [
                {
                    'type': 'ModelDefinitionComponent',
                    'name': 'dock_template_components_ModelDefinitionComponent',
                    'filename': 'data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model',
                    'visibleLayers': 1,
                    'lightMask': 19,
                    'shadowMask': 1,
                    'ghostShadows': false,
                    'floorModel': false,
                    'terrainBlendEnableYUpBlend': false,
                    'terrainBlendMode': 1
                },
                {
                    'type': 'TransformDefinitionComponent',
                    'name': 'dock_template_components_TransformDefinitionComponent',
                    'position': {'x': 271, 'y': -4, 'z': 260},
                    'orientation': {'x': 0, 'y': 0, 'z': 0, 'w': 1},
                    'scale': {'x': 1, 'y': 1, 'z': 1},
                    'inheritOnlyPosition': false
                }
            ]
        }";

        // Loop through game objects.  look for Mesh component and save to json if entity name is not present in entities
        // https://answers.unity.com/questions/216985/find-game-objects-containing-string.html
        GameObject[] gobjs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        for(int i = 0; i < gobjs.Length; i++) {
            if (gobjs[i].GetComponents<MeshLoader>().Length != 0) {
                // Debug.Log("Contains MeshLoader:                      " + i);    // Can check for lowercase name or another identifier (maybe selectionBaeObj)
                var fullp = (gobjs[i].GetComponent<MeshLoader>().GetFilePath()).Replace("_lod0.model", ".model");  // e.g. ship01_lod0.model  -->  ship01.model
                string[] splitpath = fullp.Replace("Data/hd","data/hd").Split(new string[] {"data/hd"}, StringSplitOptions.None);
                
                JObject ent_dict = JObject.Parse(template_ent);
                ent_dict["name"] = gobjs[i].name;
                ent_dict["components"][0]["filename"] = "data/hd" + splitpath[1];
                ent_dict["components"][1]["position"]["x"] = -1*gobjs[i].transform.position.x;
                ent_dict["components"][1]["position"]["y"] = gobjs[i].transform.position.y;
                ent_dict["components"][1]["position"]["z"] = gobjs[i].transform.position.z;
                ent_dict["components"][1]["orientation"]["x"] = gobjs[i].transform.rotation.x;
                ent_dict["components"][1]["orientation"]["y"] = -1*gobjs[i].transform.rotation.y;
                ent_dict["components"][1]["orientation"]["z"] = -1*gobjs[i].transform.rotation.z;
                ent_dict["components"][1]["orientation"]["w"] = gobjs[i].transform.rotation.w;
                ent_dict["components"][1]["scale"]["x"] = gobjs[i].transform.localScale.x;
                ent_dict["components"][1]["scale"]["y"] = gobjs[i].transform.localScale.y;
                ent_dict["components"][1]["scale"]["z"] = gobjs[i].transform.localScale.z;
                // Debug.Log("ent_dict:"              + ent_dict);
                ((JArray)jsonpreset["entities"]).Add(ent_dict);

                // append to models list if not already in
                if (!models_list.Contains("data/hd" + splitpath[1])) {
                    models_list.Add("data/hd" + splitpath[1]);
                }
            }
        }
        // Debug.Log("models_list:           " + String.Join(",", models_list));
        // for element in model_list, append to jsonpreset["models"]
        foreach(string fp in models_list){
            string modelpath_dict = "{'path':'" + fp + "'}";
            ((JArray)jsonpreset["dependencies"]["models"]).Add(JObject.Parse(modelpath_dict));
        }
        // write json
        System.IO.File.WriteAllText(Application.dataPath + "/" + preset, jsonpreset.ToString());
    }
    
    void AddModelToScene()
    {
        // display file open window to chose filename.  keep a lastPath variable for faster model adding
        lastModlPath = EditorUtility.OpenFilePanel("Choose a model file", lastModlPath, "model");
        // Ensure _lod0.model is loaded.  _lod1, 2,3,4 did not load with LSLib
        lastModlPath = lastModlPath.Replace("_lod1.model", "_lod0.model").Replace("_lod2.model", "_lod0.model").Replace("_lod3.model", "_lod0.model").Replace("_lod4.model", "_lod0.model");
        // Debug.Log("lastModlPath:             " + lastModlPath);
        var strArr = lastModlPath.Split('/');
        var modlname = strArr[strArr.Length-1].Replace("_lod0.model","");
        // Check if model name already exists. output warning and suggestion to copy/paste the model in unity so the game object name can be unique
        var found = false;
        GameObject[] gobjs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        for(int i = 0; i < gobjs.Length; i++) {
            if (gobjs[i].name == modlname) {
                var boolDialog = EditorUtility.DisplayDialog("This model already exists in the scene.", "Copy and paste the model '" + modlname + "' in Unity to ensure unique object names.", "Ok", "");
                found = true;
            }
        }
        if (!found) {
            var go = new GameObject(modlname);  // terrain, waypoint01, ...
            var mesh = go.AddComponent<MeshLoader>();                                    // add script component
            var root = LSLib.Granny.GR2Utils.LoadModel(lastModlPath);
            mesh.Load(root, lastModlPath, loadTextures);
        }
    }

    // OnDisable is called when the GameObject is toggled - disabled (checkbox in the Inspector).  (MainCamera has the attached script SaveJson.cs)
    void OnDisable()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
    }
}
