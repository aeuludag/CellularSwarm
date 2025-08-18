namespace CellularSwarm.Core;

public class Cell
{
    public Simulation simulation;
    public CellType cellType;
    public List<Gene> genes = new();
    private Dictionary<int, float> _morphogens = new();
    public Dictionary<int, float> Morphogens
    {
        get => _morphogens;
    }
    public int neighbourCount = 0;
    public bool shouldMultiply;
    public bool shouldApoptosis;
    private GeneAction? _currentMultiplyAction;

    public Cell(Simulation simulation, CellType type, List<Gene> genes, Dictionary<int, float> morphogens)
    {
        this.cellType = type;
        this.genes = genes;
        this._morphogens = morphogens;
        this.simulation = simulation;
    }

    public Cell(Simulation simulation, int cellTypeID, List<int> geneIDs)
    {
        this.cellType = simulation.CellTypes[cellTypeID];
        this.genes = geneIDs.Select(id => simulation.Genes[id]).ToList();
        this.simulation = simulation;
    }

    public Cell(Simulation simulation)
    {
        this.simulation = simulation;
        this.cellType = simulation.CellTypes[0];
    }

    public Cell(Cell cell)
    {
        simulation = cell.simulation;
        cellType = cell.cellType;
        genes = new(cell.genes);
        _morphogens = new(cell.Morphogens);
    }

    public void Step()
    {
        var actions = GetAvailableActions();
        foreach (var action in actions)
        {
            PerformAction(action);
        }
    }

    public List<GeneAction> GetAvailableActions()
    {
        List<GeneAction> actions = new();

        foreach (Gene gene in genes)
        {
            if (gene.ShouldBeActive(this)) actions.AddRange(gene.actions);
        }

        return actions;
    }

    public void PerformAction(GeneAction action)
    {
        switch (action.actionType)
        {
            case GeneAction.ActionType.ChangeMorphogen:

                foreach (var pair in action.actionMorphogens)
                {
                    AddMorphogen(pair.Key, pair.Value);
                }
                break;

            case GeneAction.ActionType.Apoptosis:
                shouldApoptosis = true;
                break;

            case GeneAction.ActionType.Multiply:
                if (neighbourCount == 6) break;
                _currentMultiplyAction = action;
                shouldMultiply = true;
                break;
        }
    }

    public void AddMorphogen(int id, float concentration)
    {
        SetMorphogen(id, _morphogens.GetValueOrDefault(id, 0) + concentration);
    }

    public void SetMorphogen(int id, float concentration)
    {
        if(concentration <= 0) concentration = 0;
        _morphogens[id] = concentration;
    }

    public float GetMorphogen(int id)
    {
        return _morphogens.GetValueOrDefault(id, 0);
    }

    public void Apoptosis()
    {
        // ded xd
    }

    public Cell Multiply()
    {
        if (_currentMultiplyAction is null) throw new System.NullReferenceException("Current multiply action is null, which should be impossible!!");
        if (!shouldMultiply) throw new System.Exception("Should NOT multiply now.");

        Dictionary<int, float> morphogenShare = _currentMultiplyAction.actionMorphogens;

        shouldMultiply = false;

        Dictionary<int, float> newMorphogens = new();

        foreach (var morphogenPair in _morphogens)
        {
            newMorphogens[morphogenPair.Key] = morphogenPair.Value * morphogenShare.GetValueOrDefault(morphogenPair.Key, 0.5f);
            _morphogens[morphogenPair.Key] *= 1 - morphogenShare.GetValueOrDefault(morphogenPair.Key, 0.5f);
        }

        return new Cell(simulation, cellType, genes, newMorphogens);
    }
}
public struct CellType
{
    public int id;
    public string name;

    public CellType(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public static bool operator ==(CellType left, CellType right) => left.Equals(right);
    public static bool operator !=(CellType left, CellType right) => !left.Equals(right);
    public bool Equals(CellType other) => other.id == id;
    public override bool Equals(object? obj) => obj is CellType other && (other.id == id);
    public override int GetHashCode() => id;
}
