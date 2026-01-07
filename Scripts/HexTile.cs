using UnityEngine;

public enum HexOwner
{
    None,
    Player1,
    Player2
}

public enum BiomeType
{
    Sea,
    Land,
    Forest
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class HexTile : MonoBehaviour
{
    public int q;
    public int r;

    public HexOwner owner = HexOwner.None;
    public BiomeType biome;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        UpdateColor();
    }

    public void SetOwner(HexOwner newOwner)
    {
        owner = newOwner;
        UpdateColor();
    }

    public void SetBiome(BiomeType newBiome)
    {
        biome = newBiome;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (rend == null) return;

        // Cor base do bioma
        Color baseColor = biome switch
        {
            BiomeType.Sea => new Color(0.2f, 0.6f, 1f),
            BiomeType.Land => new Color(0.9f, 0.8f, 0.4f),
            BiomeType.Forest => new Color(0.1f, 0.5f, 0.1f),
            _ => Color.gray
        };

        // Overlay do dono (bem leve)
        if (owner == HexOwner.None)
        {
            rend.material.color = biome switch
            {
                BiomeType.Sea => new Color(0.3f, 0.6f, 1f),
                BiomeType.Land => Color.yellow,
                BiomeType.Forest => Color.green,
                _ => Color.gray
            };
            return;
        }
        else if (owner == HexOwner.Player2)
            baseColor = Color.Lerp(baseColor, Color.red, 0.25f);

        rend.material.color = baseColor;
    }
}
