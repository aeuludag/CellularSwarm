namespace CellularSwarm.Core.Data;

public class GeneConditionData
{
    public int id;
    public bool strong;
    public bool not;
    public int conditionType; // 0: Concentration, 1: CellType, 2: Neighbour

    // Concentration
    public int morphogenId;
    public float thresholdConcentration;

    // CellType
    public int cellTypeId;

    // Neighbour
    public int neighbourThreshold;
    public int comparisonType; // also in concentration

    public static GeneConditionData FromGeneCondition(GeneCondition condition)
    {
        var data = new GeneConditionData
        {
            id = condition.id,
            strong = condition.strong,
            not = condition.not
        };

        if (condition is ConcentrationCondition concentrationCondition)
        {
            data.conditionType = 0;
            data.morphogenId = concentrationCondition.morphogenID;
            data.thresholdConcentration = concentrationCondition.thresholdConcentration;
            data.comparisonType = (int)concentrationCondition.comparisonType;
        }
        else if (condition is CellTypeCondition cellTypeCondition)
        {
            data.conditionType = 1;
            data.cellTypeId = cellTypeCondition.cellType.id;
        }
        else if (condition is NeighbourCondition neighbourCondition)
        {
            data.conditionType = 2;
            data.neighbourThreshold = neighbourCondition.threshold;
            data.comparisonType = (int)neighbourCondition.comparisonType;
        }

        return data;
    }

    public static GeneCondition ToGeneCondition(Simulation simulation, GeneConditionData data)
    {
        GeneCondition condition = data.conditionType switch
        {
            0 => new ConcentrationCondition(
                id: data.id,
                not: data.not,
                morphogenID: data.morphogenId,
                thresholdConcentration: data.thresholdConcentration,
                comparisonType: (GeneCondition.ComparisonType)data.comparisonType
            ),
            1 => new CellTypeCondition(
                id: data.id,
                not: data.not,
                cellType: simulation.CellTypes[data.cellTypeId]
            ),
            2 => new NeighbourCondition(
                id: data.id,
                not: data.not,
                threshold: data.neighbourThreshold,
                comparisonType: (GeneCondition.ComparisonType)data.comparisonType
            ),
            _ => throw new ArgumentException("Invalid condition type")
        };

        return condition;
    }
}
