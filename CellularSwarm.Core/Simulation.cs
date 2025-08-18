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

    public readonly DiffusionHandler Diffuser;
    public float diffusionFactor = 1f;
    public float diffusionThreshold = 0.1f;

    private readonly static Random random = new();

    public Simulation(int id, string name)
    {
        this.id = id;
        this.name = name;

        Diffuser = new DiffusionHandler(this);
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

    public Dictionary<HexCoords, Cell> Step()
    {
        List<HexCoords> cellsToMultiply = new();
        List<HexCoords> cellsToApoptosis = new();

        foreach (var cellPair in cells)
        {
            var coords = cellPair.Key;
            var cell = cellPair.Value;

            cell.neighbourCount = GetNeighbours(coords).Count;

            cell.Step();

            if (cell.shouldApoptosis) cellsToApoptosis.Add(coords);
            if (cell.shouldMultiply) cellsToMultiply.Add(coords);
        }

        foreach (var coords in cellsToMultiply)
        {
            var cell = cells[coords];
            List<HexCoords> freeTiles = new(); // why did i use tile here?

            foreach (var tile in coords.GetNeighbouringCoords())
            {
                if (cells.ContainsKey(tile)) continue;
                freeTiles.Add(tile);
            }

            if (freeTiles.Count == 0)
            {
                cell.shouldMultiply = false;
                continue;
            }

            int i = random.Next(freeTiles.Count);
            var newCell = cell.Multiply();
            cells.Add(freeTiles[i], newCell);
        }

        foreach (var coords in cellsToApoptosis)
        {
            var cell = cells[coords];

            cell.Apoptosis();
            cells.Remove(coords);
        }

        Diffuser.Diffuse();
        
        return cells;
    }
}