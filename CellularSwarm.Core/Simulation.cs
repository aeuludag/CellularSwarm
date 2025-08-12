namespace CellularSwarm.Core;

public class Simulation
{
    public int id;
    public string name;

    public Dictionary<HexCoords, Cell> cells = new();
    public Dictionary<int, CellType> CellTypes = new();
    public Dictionary<int, Morphogen> Morphogens = new();
    public Dictionary<int, GeneAction> GeneActions = new();
    public Dictionary<int, GeneCondition> GeneConditions = new();
    public Dictionary<int, Gene> Genes = new();

    public Simulation(int id, string name)
    {
        this.id = id;
        this.name = name;
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
    
    //public Dictionary<HexCoords, Cell> Step(Dictionary<HexCoords, Cell> cellGrid) Step
    //{

    //}
}