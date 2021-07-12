using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// example from https://docs.unity3d.com/Manual/JSONSerialization.html
[SelectionBaseAttribute]
public class saveJson : MonoBehaviour
{
    public CoordObjs jsondata;
    public CoordObjs jsondata2;
    
    // custom class for list of coords for gameobjects (e.g. docks)
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

        public NameAndCoord(string namein, string filepathin, float xin, float yin, float zin, float qxin, float qyin, float qzin, float qwin)
        {
            this.name = namein;
            this.filepath = filepathin;
            this.x = xin;
            this.y = yin;
            this.z = zin;
            this.qx = qxin; this.qy = qyin; this.qz = qzin; this.qw = qwin;
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
        public void AddNameCoords(string namein, string filepathin, float xin, float yin, float zin, float qxin, float qyin, float qzin, float qwin)
        {
            NameAndCoord coordin;
            coordin = new NameAndCoord(namein, filepathin, xin, yin, zin, qxin, qyin, qzin, qwin);
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

        jsondata = JsonUtility.FromJson<CoordObjs>(strdata);
        Debug.Log("json read from file ToJson:                 " + JsonUtility.ToJson(jsondata));
       
        // instantiate dock.   https://stackoverflow.com/questions/42489397/load-3d-object-in-unity-using-script
        foreach(NameAndCoord ob in jsondata.namecoords)
        {
            string modelfile; string[] strArr;
            modelfile = ob.filepath;
            strArr = modelfile.Split('/');     // NOTE:  use single quotes for char '/',  "/" is a string
            // Debug.Log("StrArr[-1]:                  " + strArr[strArr.Length-1].Replace(".model",""));
            var dockObj = Instantiate(Resources.Load(strArr[strArr.Length-1].Replace(".model",""))) as GameObject;
            dockObj.name = ob.name;
            // **NOTE** render objects with x--> -x  since the orientation is like that in-game
            dockObj.transform.position = new Vector3(-1*ob.x, ob.y, ob.z);
            dockObj.AddComponent<SelectionBaseObject>();

            // Quaternion rotation "orientation": {"x": 0, "y": 0.707, "z": 0, "w": 0.707},   https://docs.unity3d.com/ScriptReference/Quaternion.html
            // https://answers.unity.com/questions/476128/how-to-change-quaternion-by-180-degrees.html
            Quaternion quat = new Quaternion(ob.qx, -1*ob.qy, ob.qz, ob.qw);
            // dockObj.transform.rotation = new Vector3(quat.eulerAngles.x, quat.eulerAngles.y, quat.eulerAngles.z);
            dockObj.transform.rotation = quat;
            Debug.Log("dockObj.transform.rotation.y:            " + dockObj.transform.rotation.y);   // This is stores as a quat, 0.7071068
            
            // set filepath for the model file in D2R's data storage.    The filepath variable appears in the unity editor -> inspector (script component)
            // set from jsondata.namecoords[0].filepath
            // dockObj.GetComponent<SelectionBaseObject>().SetFilePath("data/hd/env/model/act3/docktown/act3_docktown_docks/dock01.model");
            dockObj.GetComponent<SelectionBaseObject>().SetFilePath(ob.filepath);
            Debug.Log("fpath:            " + dockObj.GetComponent<SelectionBaseObject>().GetFilePath());
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
                // **NOTE** save objects with x--> -x, undoing the rendering orientation.   also rotation/quat is y --> -y
                jsondata2.AddNameCoords(gobjs[i].name, gobjs[i].GetComponent<SelectionBaseObject>().GetFilePath(), -1*gobjs[i].transform.position.x, gobjs[i].transform.position.y, gobjs[i].transform.position.z, gobjs[i].transform.rotation.x, -1*gobjs[i].transform.rotation.y, gobjs[i].transform.rotation.z, gobjs[i].transform.rotation.w);
            }
        }
        Debug.Log("looped through GameObjs:                 " + JsonUtility.ToJson(jsondata2));

        // save current objects positions to json file
        // Debug.Log("saving to file:                 " + JsonUtility.ToJson(jsondata2));
        System.IO.File.WriteAllText(Application.dataPath + "/ex1.json", JsonUtility.ToJson(jsondata2));

        // call python script that appends pos, rotation to docktown3_base.json and saves docktown3.json
        string strCmd;
        strCmd = "/C cd /d " + Application.dataPath + " && python preset_update.py";
        System.Diagnostics.Process.Start("CMD.exe", strCmd);
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
