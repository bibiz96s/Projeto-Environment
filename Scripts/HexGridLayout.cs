using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public int radius = 3;

    [Header("Hex Settings")]
    public float outerSize = 1f;
    public float height = 1f;
    public bool isFlatTopped = true;
    public Material material;

    bool needsRebuild;

    void OnEnable()
    {
        RequestRebuild();
    }

    void OnValidate()
    {
        RequestRebuild();
    }

    void RequestRebuild()
    {
        if (needsRebuild) return;
        needsRebuild = true;

#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            needsRebuild = false;
            GenerateGrid();
        };
#endif
    }

    void GenerateGrid()
    {
        ClearGrid();

        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                int s = -q - r;
                if (Mathf.Abs(s) > radius) continue;

                GameObject hex = new GameObject($"Hex {q},{r}");
                hex.transform.SetParent(transform, false);
                hex.transform.position = CubeToWorld(q, r);

                // Renderer
                var renderer = hex.AddComponent<HexRenderer>();
                renderer.outerSize = outerSize;
                renderer.height = height;
                renderer.isFlatTopped = isFlatTopped;
                renderer.material = material;

                // Collider
                var collider = hex.GetComponent<MeshCollider>();
                collider.convex = true;

                // Tile
                var tile = hex.AddComponent<HexTile>();
                tile.q = q;
                tile.r = r;

                // 🔥 BIOMA DEFINIDO AQUI 🔥
                tile.SetBiome(GetRandomBiome(q, r));
            }
        }
    }

    BiomeType GetRandomBiome(int q, int r)
    {
        // Seed fixa baseada na posição → mapa consistente
        int seed = q * 73856093 ^ r * 19349663;
        Random.InitState(seed);

        float roll = Random.value;

        if (roll < 0.33f)
            return BiomeType.Sea;
        if (roll < 0.66f)
            return BiomeType.Land;

        return BiomeType.Forest;
    }

    void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }

    Vector3 CubeToWorld(int q, int r)
    {
        float x, z;

        if (isFlatTopped)
        {
            x = outerSize * 1.5f * q;
            z = outerSize * Mathf.Sqrt(3f) * (r + q / 2f);
        }
        else
        {
            x = outerSize * Mathf.Sqrt(3f) * (q + r / 2f);
            z = outerSize * 1.5f * r;
        }

        return new Vector3(x, 0f, z);
    }
}
