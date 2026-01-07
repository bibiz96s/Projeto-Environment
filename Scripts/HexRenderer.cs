using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class HexRenderer : MonoBehaviour
{
    public float outerSize = 1f;
    public float height = 1f;
    public bool isFlatTopped = true;
    public Material material;

    Mesh mesh;

    void OnEnable()
    {
        Generate();
    }

    void OnValidate()
    {
        Generate();
    }

    public void Generate()
    {
        if (mesh != null)
        {
            if (Application.isPlaying)
                Destroy(mesh);
            else
                DestroyImmediate(mesh);
        }

        mesh = new Mesh
        {
            name = "Hex Mesh"
        };

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshFilter.sharedMesh = mesh;

        if (material != null)
            meshRenderer.sharedMaterial = material;

        BuildMesh();

        // 🔥 A PARTE MAIS IMPORTANTE 🔥
        meshCollider.sharedMesh = null; // força refresh
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }

    void BuildMesh()
    {
        List<Vector3> verts = new();
        List<int> tris = new();

        // centros
        verts.Add(new Vector3(0, height, 0)); // topo
        verts.Add(Vector3.zero);              // base

        // vértices do hex
        for (int i = 0; i < 6; i++)
        {
            float angle = isFlatTopped ? 60f * i : 60f * i - 30f;
            float rad = Mathf.Deg2Rad * angle;

            float x = outerSize * Mathf.Cos(rad);
            float z = outerSize * Mathf.Sin(rad);

            verts.Add(new Vector3(x, height, z));
            verts.Add(new Vector3(x, 0, z));
        }

        // topo
        for (int i = 0; i < 6; i++)
        {
            tris.Add(0);
            tris.Add(2 + ((i + 1) % 6) * 2);
            tris.Add(2 + i * 2);
        }

        // laterais
        for (int i = 0; i < 6; i++)
        {
            int t1 = 2 + i * 2;
            int b1 = t1 + 1;
            int t2 = 2 + ((i + 1) % 6) * 2;
            int b2 = t2 + 1;

            tris.Add(t1); tris.Add(t2); tris.Add(b1);
            tris.Add(b1); tris.Add(t2); tris.Add(b2);
        }

        // base
        for (int i = 0; i < 6; i++)
        {
            tris.Add(1);
            tris.Add(3 + i * 2);
            tris.Add(3 + ((i + 1) % 6) * 2);
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
    }
}
