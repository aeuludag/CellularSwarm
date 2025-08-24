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

    public CellType DefaultCellType { get; protected set; }
    public Morphogen DefaultMorphogen { get; protected set; }
    public GeneAction DefaultGeneAction { get; protected set; }
    public GeneCondition DefaultGeneCondition { get; protected set; }
    public ConcentrationCondition DefaultConcentrationCondition { get; protected set; }
    public CellTypeCondition DefaultCellTypeCondition { get; protected set; }
    public NeighbourCondition DefaultNeighbourCondition { get; protected set; }
    public Gene DefaultGene { get; protected set; }

    private readonly static Random random = new();

    public Simulation(int id, string name)
    {
        this.id = id;
        this.name = name;

        Diffuser = new DiffusionHandler(this);

        DefaultCellType = new CellType(0, "New Cell Type");
        DefaultMorphogen = new Morphogen(0, "New Morphogen", 1f, 0.1f);
        DefaultGeneAction = new GeneAction(0, GeneAction.ActionType.Multiply, "New Gene Action");
        DefaultConcentrationCondition = new ConcentrationCondition(0, false, false, 0, 10f, GeneCondition.ComparisonType.GreaterThan, "New Concentration Condition");
        DefaultCellTypeCondition = new CellTypeCondition(0, false, false, DefaultCellType, "New Cell Type Condition");
        DefaultNeighbourCondition = new NeighbourCondition(0, false, false, 3, GeneCondition.ComparisonType.GreaterThan, "New Neighbour Condition");
        DefaultGeneCondition = DefaultNeighbourCondition.Clone();
        DefaultGene = new Gene(0, "New Gene", [], [], []);

        CellTypes.Add(0, new(DefaultCellType));
        Morphogens.Add(0, new(DefaultMorphogen));
        GeneActions.Add(0, new(DefaultGeneAction));
        GeneConditions.Add(0, DefaultGeneCondition.Clone());
        Genes.Add(0, new(DefaultGene));
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

    public void RemoveMorphogen(int id)
    {
        Morphogens.Remove(id);
    }

    public Morphogen GetMorphogen(int id)
    {
        if (Morphogens.TryGetValue(id, out var morphogen))
        {
            return morphogen;
        }
        var newMorphogen = new Morphogen(DefaultMorphogen);
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
        var newCellType = new CellType(DefaultCellType);
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
        var newGeneAction = new GeneAction(DefaultGeneAction);
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
        var newGeneCondition = DefaultGeneCondition.Clone();
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
        return Add(new Gene(DefaultGene));
    }

    int RandomId<T>(Dictionary<int, T> dict)
    {
        var newId = random.Next(int.MaxValue);
        while (dict.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }
        return newId;
    }
}