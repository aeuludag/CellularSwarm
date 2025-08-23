namespace CellularSwarm.Core;

public class Simulation
{
    public int id;
    public string name;

    public Dictionary<HexCoords, Cell> Cells = new();

    public Dictionary<int, CellType> CellTypes = new();
    public Dictionary<int, Morphogen> Morphogens = new();
    public Dictionary<int, GeneAction> GeneActions = new();
    public Dictionary<int, GeneCondition> GeneConditions = new();
    public Dictionary<int, Gene> Genes = new();

    public readonly DiffusionHandler Diffuser;
    public float maxConcentration = 100f;
    public int diffusionSteps = 3;

    private readonly static Random random = new();
    private CellType defaultCellType;
    private Morphogen defaultMorphogen;
    private GeneAction defaultGeneAction;
    private GeneCondition defaultGeneCondition;
    private Gene defaultGene;

    public Simulation(int id, string name)
    {
        this.id = id;
        this.name = name;

        Diffuser = new DiffusionHandler(this);

        defaultCellType = Add(new CellType(-1, "Default Cell Type"));
        defaultMorphogen = Add(new Morphogen(-1, "Default Morphogen", 1f, 0.1f));
        defaultGeneAction = Add(new GeneAction(-1, GeneAction.ActionType.Multiply, "Default Gene Action"));
        defaultGeneCondition = Add(new NeighbourCondition(-1, false, false, 3, GeneCondition.ComparisonType.GreaterThan, "Default Condition"));
        defaultGene = Add(new Gene(-1, "Default Gene", [defaultGeneAction], [defaultGeneCondition], []));
    }

    public List<HexCoords> GetNeighbours(HexCoords coords)
    {
        var neighbours = new List<HexCoords>();
        var neighbouringTiles = coords.GetNeighbouringCoords();
        for (int i = 0; i < 6; i++)
        {
            if (Cells.ContainsKey(neighbouringTiles[i])) { neighbours.Add(neighbouringTiles[i]); }
        }
        return neighbours;
    }

    public Dictionary<HexCoords, Cell> Step()
    {
        List<HexCoords> cellsToMultiply = new();
        List<HexCoords> cellsToApoptosis = new();

        foreach (var cellPair in Cells)
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
            var cell = Cells[coords];
            List<HexCoords> freeTiles = new(); // why did i use tile here?

            foreach (var tile in coords.GetNeighbouringCoords())
            {
                if (Cells.ContainsKey(tile)) continue;
                freeTiles.Add(tile);
            }

            if (freeTiles.Count == 0)
            {
                cell.shouldMultiply = false;
                continue;
            }

            int i = random.Next(freeTiles.Count);
            var newCell = cell.Multiply();
            Cells.Add(freeTiles[i], newCell);
        }

        foreach (var coords in cellsToApoptosis)
        {
            var cell = Cells[coords];

            cell.Apoptosis();
            Diffuser.DiffuseAllOf(coords);
            Cells.Remove(coords);
        }

        for (int i = 0; i < diffusionSteps; i++)
        {
            Diffuser.Diffuse();
        }

        return Cells;
    }

    public CellType Add(CellType cellType)
    {
        int newId = RandomId(CellTypes);
        cellType.id = newId;
        CellTypes.Add(newId, cellType);
        return cellType;
    }

    public Morphogen Add(Morphogen morphogen)
    {
        int newId = RandomId(Morphogens);
        morphogen.id = newId;
        Morphogens.Add(newId, morphogen);
        return morphogen;
    }

    public GeneCondition Add(GeneCondition geneCondition)
    {
        int newId = RandomId(GeneConditions);
        geneCondition.id = newId;
        GeneConditions.Add(newId, geneCondition);
        return geneCondition;
    }

    public GeneAction Add(GeneAction geneAction)
    {
        int newId = RandomId(GeneActions);
        geneAction.id = newId;
        GeneActions.Add(newId, geneAction);
        return geneAction;
    }

    public Gene Add(Gene gene)
    {
        int newId = RandomId(Genes);
        gene.id = newId;
        Genes.Add(newId, gene);
        return gene;
    }

    public Morphogen GetMorphogen(int id)
    {
        if (Morphogens.TryGetValue(id, out var morphogen))
        {
            return morphogen;
        }
        var newMorphogen = new Morphogen(defaultMorphogen);
        newMorphogen.id = id;
        Morphogens.Add(id, newMorphogen);
        return newMorphogen;
    }
    public CellType GetCellType(int id)
    {
        if (CellTypes.TryGetValue(id, out var cellType))
        {
            return cellType;
        }
        var newCellType = new CellType(defaultCellType);
        newCellType.id = id;
        CellTypes.Add(id, newCellType);
        return newCellType;
    }
    public GeneAction GetGeneAction(int id)
    {
        if (GeneActions.TryGetValue(id, out var geneAction))
        {
            return geneAction;
        }
        var newGeneAction = new GeneAction(defaultGeneAction);
        newGeneAction.id = id;
        GeneActions.Add(id, newGeneAction);
        return newGeneAction;
    }
    public GeneCondition GetGeneCondition(int id)
    {
        if (GeneConditions.TryGetValue(id, out var geneCondition))
        {
            return geneCondition;
        }
        var newGeneCondition = defaultGeneCondition.Clone();
        newGeneCondition.id = id;
        GeneConditions.Add(id, newGeneCondition);
        return newGeneCondition;
    }
    public Gene GetGene(int id)
    {
        if (Genes.TryGetValue(id, out var gene))
        {
            return gene;
        }
        return Add(new Gene(defaultGene));
    }

    int RandomId<T>(Dictionary<int, T> dict)
    {
        var newId = random.Next(int.MaxValue);
        while (dict.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }
        return newId;
    }
}