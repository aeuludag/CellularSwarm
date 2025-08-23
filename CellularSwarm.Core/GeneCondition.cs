namespace CellularSwarm.Core;

public abstract class GeneCondition
{
    public int id;
    public string name = string.Empty;
    public bool strong; // All strong conditions and at least one weak condition must be met.
    public bool not;

    public abstract bool IsMet(Cell cell);
    public abstract GeneCondition Clone();

    public enum ComparisonType
    {
        GreaterThan,
        LessThan,
        EqualsTo,
    }
}

public class ConcentrationCondition : GeneCondition
{
    public int morphogenId;
    public float thresholdConcentration;
    public ComparisonType comparisonType;

    public ConcentrationCondition(int id, bool strong, bool not, int morphogenId, float thresholdConcentration, ComparisonType comparisonType, string name = "")
    {
        this.id = id;
        this.name = name;
        this.strong = strong;
        this.not = not;
        this.morphogenId = morphogenId;
        this.thresholdConcentration = thresholdConcentration;
        this.comparisonType = comparisonType;
    }

    public ConcentrationCondition(GeneCondition condition, int morphogenId, float thresholdConcentration, ComparisonType comparisonType)
    {
        this.id = condition.id;
        this.name = condition.name;
        this.strong = condition.strong;
        this.not = condition.not;
        this.morphogenId = morphogenId;
        this.thresholdConcentration = thresholdConcentration;
        this.comparisonType = comparisonType;
    }

    public override bool IsMet(Cell cell)
    {
        float concentration = cell.GetMorphogen(morphogenId);

        bool result = comparisonType switch
        {
            ComparisonType.GreaterThan => concentration > thresholdConcentration,
            ComparisonType.LessThan => concentration < thresholdConcentration,
            ComparisonType.EqualsTo => concentration == thresholdConcentration,
            _ => false,
        };

        return result ^ not;
    }

    public override GeneCondition Clone()
    {
        return new ConcentrationCondition(id, strong, not, morphogenId, thresholdConcentration, comparisonType, name);
    }
}

public class CellTypeCondition : GeneCondition
{
    public CellType cellType;

    public CellTypeCondition(int id, bool strong, bool not, CellType cellType, string name = "")
    {
        this.id = id;
        this.name = name;
        this.strong = strong;
        this.not = not;
        this.cellType = cellType;
    }

    public CellTypeCondition(GeneCondition condition, CellType cellType)
    {
        this.id = condition.id;
        this.name = condition.name;
        this.strong = condition.strong;
        this.not = condition.not;
        this.cellType = cellType;
    }

    public override bool IsMet(Cell cell)
    {
        return (cell.cellType == cellType) ^ not;
    }

    public override GeneCondition Clone()
    {
        return new CellTypeCondition(id, strong, not, cellType, name);
    }
}

public class NeighbourCondition : GeneCondition
{
    public int threshold;
    public ComparisonType comparisonType;

    public NeighbourCondition(int id, bool strong, bool not, int threshold, ComparisonType comparisonType, string name = "")
    {
        this.id = id;
        this.name = name;
        this.strong = strong;
        this.not = not;
        this.threshold = threshold;
        this.comparisonType = comparisonType;
    }

    public NeighbourCondition(GeneCondition condition, int threshold, ComparisonType comparisonType)
    {
        this.id = condition.id;
        this.name = condition.name;
        this.strong = condition.strong;
        this.not = condition.not;
        this.threshold = threshold;
        this.comparisonType = comparisonType;
    }

    public override bool IsMet(Cell cell)
    {
        var count = cell.neighbourCount;

        bool result = comparisonType switch
        {
            ComparisonType.LessThan => count < threshold,
            ComparisonType.GreaterThan => count > threshold,
            ComparisonType.EqualsTo => count == threshold,
            _ => false,
        };

        return result ^ not;
    }

    public override GeneCondition Clone()
    {
        return new NeighbourCondition(id, strong, not, threshold, comparisonType, name);
    }
}