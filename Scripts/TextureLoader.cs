using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[SelectionBase]
public class TextureLoader : MonoBehaviour
{
    public void Load(String txt, MeshRenderer mr, String type = "_MainTex")
    {
        var bytes = File.ReadAllBytes(SaveJson.d2rDataPath.Replace(@"\Data\hd", "") + "\\" + txt);
        var formatVal = BitConverter.ToInt16(bytes, 4);
        var width = BitConverter.ToInt32(bytes, 8);
        var height = BitConverter.ToInt32(bytes, 0xC);
        var mipLevels = BitConverter.ToInt32(bytes, 0x1C);
        var channels = BitConverter.ToInt32(bytes, 0x20);
        var sizeSection1 = BitConverter.ToInt32(bytes, 0x24);
        var startFirstSection = BitConverter.ToInt32(bytes, 0x28);
        var format = formatVal == 31 ? TextureFormat.RGBA32 : formatVal <= 58 ? TextureFormat.DXT1 : formatVal <= 62 ? TextureFormat.DXT5 : TextureFormat.BC4;

        var tex = new Texture2D(width, height, format, mipLevels, mipLevels > 0);
        tex.LoadRawTextureData(bytes.Skip(0x28 + startFirstSection).ToArray());
        tex.Apply();
        mr.material.SetTexture(type, tex);
    }
}
