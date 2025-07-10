namespace CellularSwarm.Core.Data;

public abstract class GeneConditionData
{
    public int id;
    public bool strong;
    public bool not;
    public int conditionType;
}

public class ConcentrationConditionData : GeneConditionData
{
    
}