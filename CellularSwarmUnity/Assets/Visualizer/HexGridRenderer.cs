using System.Collections.Generic;
using CellularSwarm.Core;
using UnityEngine;

public class HexGridRenderer : MonoBehaviour
{
    public GameObject hexPrefab;
    public float hexSize = 1.0f;
    private Dictionary<HexCoords, GameObject> cellCoords = new();

    void Start()
    {
    }

    public void GenerateGridFromSimulation(Simulation simulation)
    {
        foreach (var cell in simulation.cells)
        {
            var coords = cell.Key;
            var hexagon = Instantiate(hexPrefab, new Vector3(coords.q, coords.r, 0), Quaternion.identity);
            hexagon.GetComponent<UnityCell>().cell = cell.Value;
            hexagon.transform.SetParent(transform);
            cellCoords[coords] = hexagon;
        }
    }

    public void UpdateGridFromSimulation(Simulation simulation)
    {
        
    }

    void GenerateHexGrid(int radius)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Mathf.Abs(q + r) <= radius)
                {
                    AddHexagon(q, r);
                }
            }
        }
    }

    void AddHexagon(int q, int r)
    {
        float x = hexSize * q * 3 / 2;
        float y = hexSize * r * Mathf.Sqrt(3) + hexSize * q * Mathf.Sqrt(3) / 2; // hex_size * r * math.sqrt(3) + hex_size * q * math.sqrt(3) / 2

        Vector3 position = new Vector3(x, y, 0);

        UnityCell cell = hexPrefab.GetComponent<UnityCell>();
        if (cell != null)
        {
            cell.text = $"({q}, {r})";
        }
        
        GameObject hexagon = Instantiate(hexPrefab, position, Quaternion.identity);
        hexagon.transform.localScale = new Vector3(hexSize * 2, hexSize * 2, 1);
        hexagon.transform.SetParent(transform);

        cellCoords[new HexCoords(q, r)] = hexagon;
    }

    void RemoveHexagon(int q, int r)
    {
        if (cellCoords.TryGetValue(new HexCoords(q, r), out GameObject hexagon))
        {
            Destroy(hexagon);
            cellCoords.Remove(new HexCoords(q, r));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
