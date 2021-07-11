using UnityEngine;

// class to ensure that clicking on an object selects the parent. and for storing the model filepath
[SelectionBase]
public class SelectionBaseObject : MonoBehaviour
{
    public string filepath;

    // functions to set and get filepath
    public void SetFilePath(string strin)
    {
        this.filepath = strin;
    }

    public string GetFilePath()
    {
        return this.filepath;
    }
    
}
