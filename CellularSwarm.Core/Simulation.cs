using System.Collections.Generic;

namespace CellularSwarm.Core;

public class Simulation
{
    public int id;
    public string name;

    public Dictionary<HexCoords, Cell> cells;
    public Dictionary<int, CellType> CellTypes;
    public Dictionary<int, Morphogen> Morphogens;
    public GeneAction[] GeneActions;

    public Simulation(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public Simulation(int id, string name, Dictionary<int, Morphogen> morphogens, GeneAction[] geneActions, Dictionary<int, CellType> cellTypes)
    {
        this.id = id;
        this.name = name;

        this.Morphogens = morphogens;
        this.GeneActions = geneActions;
        this.CellTypes = cellTypes;
    }

    public List<HexCoords> GetNeighbours(HexCoords coords)
    {
        var neighbours = new List<HexCoords>();
        var neighbouringTiles = coords.GetNeighbouringCoords();
        for (int i = 0; i < 6; i++)
        {
            if (cells.ContainsKey(neighbouringTiles[i])) { neighbours.Add(neighbouringTiles[i]); }
        }
        return neighbours;
    }
    
    //public Dictionary<HexCoords, Cell> Step(Dictionary<HexCoords, Cell> cellGrid)
    //{

    //}
}