namespace CellularSwarm.Core;

public abstract class GeneCondition
{
    public int id;
    public string name = string.Empty;
    public bool strong; // All strong conditions and at least one weak condition must be met.
    public bool not;

    public abstract bool IsMet(Cell cell);

    public enum ComparisonType
    {
        GreaterThan,
        LessThan,
        EqualsTo,
    }
}

public class ConcentrationCondition : GeneCondition
{
    public int morphogenID;
    public float thresholdConcentration;
    public ComparisonType comparisonType;

    public ConcentrationCondition(int id, bool not, int morphogenID, float thresholdConcentration, ComparisonType comparisonType)
    {
        this.id = id;
        this.not = not;
        this.morphogenID = morphogenID;
        this.thresholdConcentration = thresholdConcentration;
        this.comparisonType = comparisonType;
    }

    public override bool IsMet(Cell cell)
    {
        float concentration = cell.GetMorphogen(morphogenID);

        switch (comparisonType)
        {
            case ComparisonType.GreaterThan:
                return concentration >= thresholdConcentration;
            case ComparisonType.LessThan:
                return concentration <= thresholdConcentration;
            case ComparisonType.EqualsTo:
                return concentration == thresholdConcentration;
            default:
                return false;
        }
    }
}

public class CellTypeCondition : GeneCondition
{
    public CellType cellType;

    public CellTypeCondition(int id, bool not, CellType cellType)
    {
        this.id = id;
        this.not = not;
        this.cellType = cellType;
    }

    public override bool IsMet(Cell cell)
    {
        return cell.cellType == cellType;
    }
}

public class NeighbourCondition : GeneCondition
{
    public int threshold;
    public ComparisonType comparisonType;

    public NeighbourCondition(int id, bool not, int threshold, ComparisonType comparisonType)
    {
        this.id = id;
        this.not = not;
        this.threshold = threshold;
        this.comparisonType = comparisonType;
    }

    public override bool IsMet(Cell cell)
    {
        var count = cell.neighbourCount;

        switch (comparisonType)
        {
            case ComparisonType.LessThan:
                return count < threshold;
            case ComparisonType.GreaterThan:
                return count > threshold;
            case ComparisonType.EqualsTo:
                return count == threshold;
            default:
                return false;
        }
    }
}