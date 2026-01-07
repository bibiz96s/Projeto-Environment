using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HexOwner currentPlayer = HexOwner.Player1;

    Inventory player1Inventory = new();
    Inventory player2Inventory = new();

    Dictionary<HexOwner, List<HexTile>> ownedTiles = new();

    HexTile[] allTiles;

    void Awake()
    {
        Instance = this;

        ownedTiles[HexOwner.Player1] = new List<HexTile>();
        ownedTiles[HexOwner.Player2] = new List<HexTile>();
    }

    void Start()
    {
        allTiles = FindObjectsByType<HexTile>(FindObjectsSortMode.None);
        Debug.Log($"Grid carregado com {allTiles.Length} hexes");
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HexTile tile = hit.collider.GetComponent<HexTile>();
            if (tile != null)
            {
                Debug.Log($"🟢 Clique em {tile.q},{tile.r}");
                TryClaim(tile);
            }
        }
    }

    void TryClaim(HexTile tile)
    {
        Inventory inv = GetInventory(currentPlayer);

        if (!inv.HasEnoughForConquest())
        {
            Debug.Log("❌ Recursos insuficientes.");
            return;
        }

        if (tile.owner == HexOwner.None)
        {
            if (!HasAdjacent(tile)) return;

            inv.SpendForConquest();
            ClaimTile(tile);
            EndTurn();
            return;
        }

        if (tile.owner != currentPlayer)
        {
            int roll = Random.Range(1, 21);
            Debug.Log($"⚔️ {currentPlayer} rolou {roll}");

            if (roll > 14)
            {
                inv.SpendForConquest();
                ClaimTile(tile);
                Debug.Log("🏆 Vitória!");
            }
            else
            {
                inv.LoseRandomItems(4);
                Debug.Log("💀 Derrota! Perdeu 4 itens aleatórios.");
            }

            EndTurn();
        }
    }

    void ClaimTile(HexTile tile)
    {
        if (tile.owner != HexOwner.None)
            ownedTiles[tile.owner].Remove(tile);

        tile.SetOwner(currentPlayer);
        ownedTiles[currentPlayer].Add(tile);
    }

    void EndTurn()
    {
        GenerateResources();
        LogPlayerStatus();

        currentPlayer = currentPlayer == HexOwner.Player1
            ? HexOwner.Player2
            : HexOwner.Player1;

        Debug.Log($"🔄 Turno de {currentPlayer}");
    }

    void GenerateResources()
    {
        Inventory inv = GetInventory(currentPlayer);

        foreach (var tile in ownedTiles[currentPlayer])
            inv.GainFromBiome(tile.biome);
    }

    void LogPlayerStatus()
    {
        LogBiomes(HexOwner.Player1);
        player1Inventory.LogInventory("Player 1");

        LogBiomes(HexOwner.Player2);
        player2Inventory.LogInventory("Player 2");
    }

    void LogBiomes(HexOwner player)
    {
        int sea = 0, land = 0, forest = 0;

        foreach (var tile in ownedTiles[player])
        {
            switch (tile.biome)
            {
                case BiomeType.Sea: sea++; break;
                case BiomeType.Land: land++; break;
                case BiomeType.Forest: forest++; break;
            }
        }

        Debug.Log(
            $"🗺️ {player} BIOMAS → Mar:{sea} | Terra:{land} | Floresta:{forest}"
        );
    }

    bool HasAdjacent(HexTile target)
    {
        foreach (var tile in ownedTiles[currentPlayer])
        {
            int dq = Mathf.Abs(tile.q - target.q);
            int dr = Mathf.Abs(tile.r - target.r);
            int ds = Mathf.Abs((-tile.q - tile.r) - (-target.q - target.r));

            if (dq <= 1 && dr <= 1 && ds <= 1)
                return true;
        }

        return ownedTiles[currentPlayer].Count == 0;
    }

    Inventory GetInventory(HexOwner player)
    {
        return player == HexOwner.Player1 ? player1Inventory : player2Inventory;
    }
}
