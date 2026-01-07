using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public int water = 10;
    public int sand = 10;
    public int stone = 10;
    public int wood = 10;

    public bool HasEnoughForConquest()
    {
        return water >= 2 && sand >= 2 && stone >= 2 && wood >= 2;
    }

    public void SpendForConquest()
    {
        water -= 2;
        sand -= 2;
        stone -= 2;
        wood -= 2;
    }

    public void GainFromBiome(BiomeType biome)
    {
        switch (biome)
        {
            case BiomeType.Sea:
                water += 1;
                break;
            case BiomeType.Land:
                sand += 1;
                stone += 1;
                break;
            case BiomeType.Forest:
                wood += 1;
                break;
        }
    }

    public void LoseRandomItems(int amount)
    {
        List<System.Action> losses = new()
        {
            () => { if (water > 0) water--; },
            () => { if (sand > 0) sand--; },
            () => { if (stone > 0) stone--; },
            () => { if (wood > 0) wood--; }
        };

        for (int i = 0; i < amount; i++)
        {
            losses[Random.Range(0, losses.Count)]();
        }
    }

    public void LogInventory(string playerName)
    {
        Debug.Log(
            $"📦 {playerName} INVENTÁRIO → " +
            $"Água:{water} | Areia:{sand} | Pedra:{stone} | Madeira:{wood}"
        );
    }
}
