using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


// example from https://docs.unity3d.com/Manual/JSONSerialization.html
[SelectionBaseAttribute]
public class saveJson : MonoBehaviour
{
    public CoordObjs jsondata2;
    public string docktown_base = ".//docktown3_base.json";
    public string docktown_final = "D://D2R//Data//hd//env//preset//act3//docktown//docktown3.json";
    
    // custom class for list of coords for gameobject models (e.g. docks, trees, etc)
    [System.Serializable]
    public class NameAndCoord {
        public string name;
        public string filepath;
        public float x;
        public float y;
        public float z;
        // quaternion rotation x,y,z,w
        public float qx;
        public float qy;
        public float qz;
        public float qw;
        // scale
        public float scalex;
        public float scaley;
        public float scalez;

        public NameAndCoord(string namein, string filepathin, float xin, float yin, float zin, float qxin, float qyin, float qzin, float qwin, float scalexin, float scaleyin, float scalezin)
        {
            this.name = namein;
            this.filepath = filepathin;
            this.x = xin;
            this.y = yin;
            this.z = zin;
            this.qx = qxin; this.qy = qyin; this.qz = qzin; this.qw = qwin;
            this.scalex = scalexin; this.scaley = scaleyin; this.scalez = scalezin;
        }
    }
    // structs added to list.  https://answers.unity.com/questions/996317/how-add-values-to-genericlist.html
    // class                   https://learn.unity.com/tutorial/classes-5#5c8a695cedbc2a067d475193

    [System.Serializable]
    public class CoordObjs
    {
        // array has a fixed size. list can be resized
        public List<NameAndCoord> namecoords = new List<NameAndCoord>();

        public CoordObjs()
        {
            // this.namecoords = new Dictionary<string, float[]>();
        }
        public void AddNameCoords(string namein, string filepathin, float xin, float yin, float zin, float qxin, float qyin, float qzin, float qwin, float scalexin, float scaleyin, float scalezin)
        {
            NameAndCoord coordin;
            coordin = new NameAndCoord(namein, filepathin, xin, yin, zin, qxin, qyin, qzin, qwin, scalexin, scaleyin, scalezin);
            this.namecoords.Add(coordin);
        }
    }
    // public CoordObjs test = new CoordObjs(1,0,0);    // test can be public here, outside of Start() 

    
    // Start is called before the first frame update
    void Start()
    {
        // CoordObjs test = new CoordObjs(1f, 0f, 0f);    // when test was Public here, Unity had compilation errors
        // CoordObjs test = new CoordObjs();
        // test.AddNameCoords("dock1", "modelpath1", 1f, 0f, 1f);
        // test.AddNameCoords("dock2", "modelpath2", 2f, 0f, 2f);
        // Debug.Log("test json:             " + JsonUtility.ToJson(test));
        // {"namecoords":[{"name":"name","filepath":"modelpath","x":1.0,"y":0.0,"z":1.0}]}.   the namecoords variable is serialized  -v'

        // read from json. Application.dataPath has to be in Start() function. Errored when in saveJson() 'constructor'
        // string strdata = System.IO.File.ReadAllText(Application.dataPath + "/unityscene.json");
        JObject jsondata = JObject.Parse(File.ReadAllText(Application.dataPath + "/unityscene.json"));

        // instantiate dock.   https://stackoverflow.com/questions/42489397/load-3d-object-in-unity-using-script
        foreach(JObject ob in jsondata["namecoords"])
        {
            string modelfile; string[] strArr;
            modelfile = (string) ob["filepath"];
            strArr = modelfile.Split('/');     // NOTE:  use single quotes for char '/',  "/" is a string
            var modelObj = Instantiate(Resources.Load(strArr[strArr.Length-1].Replace(".model",""))) as GameObject;
            modelObj.name = (string) ob["name"];
            // **NOTE** render objects with x--> -x  since the orientation is like that in-game
            modelObj.transform.position = new Vector3(-1*(float) ob["x"], (float) ob["y"], (float) ob["z"]);
            modelObj.AddComponent<SelectionBaseObject>();

            // Quaternion rotation "orientation": {"x": 0, "y": 0.707, "z": 0, "w": 0.707},   https://docs.unity3d.com/ScriptReference/Quaternion.html
            // https://answers.unity.com/questions/476128/how-to-change-quaternion-by-180-degrees.html
            Quaternion quat = new Quaternion((float) ob["qx"], -1*(float) ob["qy"], -1*(float) ob["qz"], (float) ob["qw"]);
            // modelObj.transform.rotation = new Vector3(quat.eulerAngles.x, quat.eulerAngles.y, quat.eulerAngles.z);
            modelObj.transform.rotation = quat;
            // Debug.Log("modelObj.transform.rotation.y:            " + modelObj.transform.rotation.y);   // This is stores as a quat, 0.7071068

            // scale.   NOTE:  4x scale helm in unity was much smaller than 4x scale helm in-game
            modelObj.transform.localScale = new Vector3((float) ob["scalex"], (float) ob["scaley"], (float) ob["scalez"]);

            // set filepath for the model file in D2R's data storage.    The filepath variable appears in the unity editor -> inspector (script component)
            // set from jsondata.namecoords[0].filepath
            // modelObj.GetComponent<SelectionBaseObject>().SetFilePath("data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model");
            modelObj.GetComponent<SelectionBaseObject>().SetFilePath((string) ob["filepath"]);
            // Debug.Log("fpath:            " + modelObj.GetComponent<SelectionBaseObject>().GetFilePath());
        }

        // add button listener.  Set up button in scene.       https://www.tutorialspoint.com/unity/unity_the_button.htm  and https://forum.unity.com/threads/how-to-assign-onclick-for-ui-button-generated-in-runtime.358974/
        // Button b = gameObject.GetComponent<Button>();
        // get game objects from scene / hierarchy.  https://docs.unity3d.com/ScriptReference/GameObject.Find.html
        Button save = GameObject.Find("Button").GetComponent<Button>();
        save.onClick.AddListener(SaveObjsToJson);
    }

    void SaveObjsToJson()
    {
        Debug.Log("SaveObjsToJson() called ");
        CoordObjs jsondata2 = new CoordObjs();
        // TODO: loop through GameObjects in the heirarchy.  works when new docks/objs are added.    (and removed)
        // https://answers.unity.com/questions/216985/find-game-objects-containing-string.html
        
        GameObject[] gobjs = (GameObject[])FindObjectsOfType(typeof(GameObject));
        for(int i = 0; i < gobjs.Length; i++) {
            if (gobjs[i].GetComponents<SelectionBaseObject>().Length != 0) {
                Debug.Log("Contains SelectionBaseObj:                      " + i);    // Can check for lowercase name or another identifier (maybe selectionBaeObj)
                // **NOTE** save objects with x--> -x, undoing the rendering orientation.   also rotation/quat is y --> -y.  Scale in unity for the helm was much smaller than scale of helm in-game
                jsondata2.AddNameCoords(gobjs[i].name, gobjs[i].GetComponent<SelectionBaseObject>().GetFilePath(), -1*gobjs[i].transform.position.x, gobjs[i].transform.position.y, gobjs[i].transform.position.z, gobjs[i].transform.rotation.x, -1*gobjs[i].transform.rotation.y, -1*gobjs[i].transform.rotation.z, gobjs[i].transform.rotation.w, gobjs[i].transform.localScale.x, gobjs[i].transform.localScale.y, gobjs[i].transform.localScale.z);
            }
        }
        Debug.Log("looped through GameObjs:                 " + JsonUtility.ToJson(jsondata2));

        // save current objects positions to json file
        JObject job2 = JObject.FromObject(jsondata2);
        System.IO.File.WriteAllText(Application.dataPath + "/unityscene.json", job2.ToString());  // TODO:  do not need to save to file here

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

        // read basejson and append entities
        JObject jsonbase = JObject.Parse(File.ReadAllText(Application.dataPath + "/" + docktown_base));
        
        foreach(JObject el in job2["namecoords"]) {
            JObject ent_dict = JObject.Parse(template_ent);

            ent_dict["name"] = el["name"];
            ent_dict["components"][0]["filename"] = el["filepath"];
            ent_dict["components"][1]["position"]["x"] = el["x"];
            ent_dict["components"][1]["position"]["y"] = el["y"];
            ent_dict["components"][1]["position"]["z"] = el["z"];
            ent_dict["components"][1]["orientation"]["x"] = el["qx"];
            ent_dict["components"][1]["orientation"]["y"] = el["qy"];
            ent_dict["components"][1]["orientation"]["z"] = el["qz"];
            ent_dict["components"][1]["orientation"]["w"] = el["qw"];
            ent_dict["components"][1]["scale"]["x"] = el["scalex"];
            ent_dict["components"][1]["scale"]["y"] = el["scaley"];
            ent_dict["components"][1]["scale"]["z"] = el["scalez"];

            ((JArray)jsonbase["entities"]).Add(ent_dict);

            // add to models list, if not already present  
            bool found = false;
            string elfilepath = (string)el["filepath"];
            foreach(JObject modelpath in jsonbase["dependencies"]["models"]) {
                string modelpathstr = ((string)modelpath["path"]);
                if ( elfilepath.Equals(modelpathstr) ) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                string modelpath_dict = "{'path':'" + el["filepath"] + "'}";
                ((JArray)jsonbase["dependencies"]["models"]).Add(JObject.Parse(modelpath_dict));
            }
        }
        // write preset json to final path in D2R's data/hd/... 
        System.IO.File.WriteAllText(docktown_final, jsonbase.ToString());
    }
    // in Unity editor, I click "play" which loads the object position from my json. then "pause" which allows me to move my objects.
    // once I'm done moving and positioning the object I can save the position to my json by clicking the 'save' button

    // OnDisable is called when the GameObject is toggled - disabled (checkbox in the Inspector).  (MainCamera has the attached script saveJson.cs)
    void OnDisable()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
    }
}
