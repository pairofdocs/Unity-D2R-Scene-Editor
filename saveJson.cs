using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// example from https://docs.unity3d.com/Manual/JSONSerialization.html
[SelectionBaseAttribute]
public class saveJson : MonoBehaviour
{
    // get game objects from scene / hierarchy.  https://docs.unity3d.com/ScriptReference/GameObject.Find.html
    public GameObject cube;
    
    // custom class for list of coords for gameobjects (e.g. docks)
    [System.Serializable]
    public class NameAndCoord {
        public string name;
        public string filepath;
        public float x;
        public float y;
        public float z;
        // can add rotation x,y,z here

        public NameAndCoord(string namein, string filepathin, float xin, float yin, float zin)
        {
            this.name = namein;
            this.filepath = filepathin;
            this.x = xin;
            this.y = yin;
            this.z = zin;
        }
    }
    // structs added to list.  https://answers.unity.com/questions/996317/how-add-values-to-genericlist.html

    [System.Serializable]
    public class CoordObjs
    {
        // var accounts = new Dictionary<string, double>();
        // public Dictionary<string, GameObject> dictio = new Dictionary<string, GameObject>();
        // public Dictionary<string, float[]> namecoords = new Dictionary<string, float[]>();
        // public string name;
        // public float[] coords;
        // array has a fixed size. list can be resized
        public List<NameAndCoord> namecoords = new List<NameAndCoord>();
        

        // store GameObject name "dock01_1",  and filepath "data/hd.../dock01.model".   need Dict with Array[] of string and float
        // https://forum.unity.com/threads/c-arrays-with-multiple-types-of-data-at-each-point.144459/ 
        //create a new type
        // public class MyNewType
        // {
        //     //define all of the values for the class
        //     public int Value1;
        //     public string Name;


        // from https://learn.unity.com/tutorial/classes-5#5c8a695cedbc2a067d475193
        // public CoordObjs(float xin, float yin, float zin)
        // {
        //     coords = new float[] {xin, yin, zin};
        //     this.namecoords.Add("name", coords);
        // }
        public CoordObjs()
        {
            // this.namecoords = new Dictionary<string, float[]>();
        }
        public void AddNameCoords(string namein, string filepathin, float xin, float yin, float zin)
        {
            NameAndCoord coordin;
            coordin = new NameAndCoord(namein, filepathin, xin, yin, zin);
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
        string strdata = System.IO.File.ReadAllText(Application.dataPath + "/ex1.json");
        Debug.Log("strdata:              " + strdata);

        CoordObjs jsondata = JsonUtility.FromJson<CoordObjs>(strdata);
        Debug.Log("json read from file ToJson:                 " + JsonUtility.ToJson(jsondata));

        // // This returns the GameObject named Hand. Hand must not have a parent in the Hierarchy view
        // cube = GameObject.Find("/Cube");
        // Debug.Log("Cube: " + cube.transform.position);

        // // set position using jsondata
        // cube.transform.position = new Vector3(jsondata.x, jsondata.y, jsondata.z);
        // Debug.Log("Cube: " + cube.transform.position);
        

        // save to file in D:/apps/Unity Projects/proj3d-1/Assets/
        // Application.persistentDataPath was C:/Users/Newone/AppData/LocalLow/DefaultCompany/proj3d-1
        // Debug.Log("datapath " + Application.dataPath);  
        // string jsonex = JsonUtility.ToJson(test);
        // System.IO.File.WriteAllText(Application.dataPath + "/ex1.json", jsonex);

        // // instantiate cube
        // GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // cube1.transform.position = new Vector3(-10f, 0.5f, 0);
        // cube1.name = "cube1_instantiated";

        // instantiate dock.   https://stackoverflow.com/questions/42489397/load-3d-object-in-unity-using-script
        foreach(NameAndCoord ob in jsondata.namecoords)
        {
            var dockObj = Instantiate(Resources.Load("dock01")) as GameObject;        // TODO: specify model name to load
            dockObj.name = ob.name;
            dockObj.transform.position = new Vector3(ob.x, ob.y, ob.z);
            dockObj.AddComponent<SelectionBaseObject>();

            // set filepath for the model file in D2R's data storage.    The filepath variable appears in the unity editor -> inspector (script component)
            // set from jsondata.namecoords[0].filepath
            // dockObj.GetComponent<SelectionBaseObject>().SetFilePath("data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model");
            dockObj.GetComponent<SelectionBaseObject>().SetFilePath(ob.filepath);
            Debug.Log("fpath:            " + dockObj.GetComponent<SelectionBaseObject>().GetFilePath());
        }

        // var dockObj = Instantiate(Resources.Load("dock01")) as GameObject;
        // dockObj.name = jsondata.namecoords[0].name;
        // dockObj.transform.position = new Vector3(jsondata.namecoords[0].x, jsondata.namecoords[0].y, jsondata.namecoords[0].z);
        // dockObj.AddComponent<SelectionBaseObject>();

        // // set filepath for the model file in D2R's data storage.    The filepath variable appears in the unity editor -> inspector (script component)
        // // set from jsondata.namecoords[0].filepath
        // // dockObj.GetComponent<SelectionBaseObject>().SetFilePath("data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model");
        // dockObj.GetComponent<SelectionBaseObject>().SetFilePath(jsondata.namecoords[0].filepath);
        // Debug.Log("fpath:            " + dockObj.GetComponent<SelectionBaseObject>().GetFilePath());


        // do for loop of json coord items and instantiate the dock models
        

    }

    // OnDisable is called when the GameObject is toggled - disabled (checkbox in the Inspector).  (MainCamera has the attached script saveJson.cs)
    void OnDisable()
    {
        // Debug.Log("PrintOnDisable: script was disabled");
        // Debug.Log("Cube before saving: " + cube.transform.position);

        // // save to file in D:/apps/Unity Projects/proj3d-1/Assets/
        // CoordObjs coords = new CoordObjs(cube.transform.position.x, cube.transform.position.y, cube.transform.position.z);
        // string coordsjson = JsonUtility.ToJson(coords);
        // System.IO.File.WriteAllText(Application.dataPath + "/ex1.json", coordsjson);

        // GameObject dockObj1 = GameObject.Find("/dock01_instantiated");
        // Debug.Log("dockObj1 " + dockObj1.transform.position);

    }
    // in Unity editor, I click "play" which loads the cube object position from my json. then "pause" which allows me to move my cube object.
    // once I'm done moving and positioning the cube I can save the position to my json by unchecking (disabling) the camera  (disabling the cube works too. using camera disable has more accurate coords on save?)

    // Update is called once per frame
    void Update()
    {
        
    }
}
