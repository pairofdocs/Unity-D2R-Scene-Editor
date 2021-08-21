using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

[SelectionBase]
public class MeshLoader : MonoBehaviour
{
    public string filepath;

    public void Load(LSLib.Granny.Model.Root root, string path, bool loadTextures)
    {
        this.filepath = path;
        foreach (var m in root.Meshes)
        {
            var meshObj = new GameObject(m.Name);
            var meshRenderer = meshObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

            var meshFilter = meshObj.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            // Flip mesh x and mesh normal x.  Unity scene did not match in-game objects
            mesh.vertices = m.PrimaryVertexData.Vertices.Select(v => new Vector3(-1*v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
            mesh.normals = m.PrimaryVertexData.Vertices.Select(v => new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z)).ToArray();
            mesh.uv = m.PrimaryVertexData.Vertices.Select(v => new Vector2(v.TextureCoordinates0.X, v.TextureCoordinates0.Y)).ToArray();
            mesh.triangles = m.PrimaryTopology.Indices.ToArray();
            int[] tris = mesh.triangles;
            // flip normals https://stackoverflow.com/questions/51100346/flipping-3d-gameobjects-in-unity3d/51100522
            for (int i = 0; i < tris.Length / 3; i++) {
                int a = tris[i * 3 + 0];
                int b = tris[i * 3 + 1];
                int c = tris[i * 3 + 2];
                tris[i * 3 + 0] = c;
                // tris[i * 3 + 1] = b;  // is this needed? b is kept the same?
                tris[i * 3 + 2] = a;
            }
            mesh.triangles = tris;
            meshFilter.mesh = mesh;

            meshObj.transform.localScale = new Vector3(m.ExtendedData.VertexScale, m.ExtendedData.VertexScale, m.ExtendedData.VertexScale);

            if (meshRenderer.material == null) meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.EnableKeyword("_NORMALMAP");
            meshRenderer.material.EnableKeyword("_METALLICGLOSSMAP");
            
            if (loadTextures == true) {
                var texture = meshObj.AddComponent<TextureLoader>();
                if (m.MaterialBindings.Count > 0) {
                    var albedo = m.MaterialBindings[0].Material?.Maps?.FirstOrDefault(map => map.Usage == "AlbedoTexture");
                    if (albedo != null) texture.Load(albedo.Map.Texture.FromFileName, meshRenderer);
                    var normal = m.MaterialBindings[0].Material?.Maps?.FirstOrDefault(map => map.Usage == "NormalTexture");
                    if (normal != null) texture.Load(normal.Map.Texture.FromFileName, meshRenderer, "_BumpMap");
                    var orm = m.MaterialBindings[0].Material?.Maps?.FirstOrDefault(map => map.Usage == "ORMTexture");
                    if (orm != null) texture.Load(orm.Map.Texture.FromFileName, meshRenderer, "_MetallicGlossMap");
                }    
            }
            
            meshObj.transform.parent = transform;
        }

    }

    public string GetFilePath()
    {
        return this.filepath;
    }
}