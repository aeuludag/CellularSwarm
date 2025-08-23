namespace CellularSwarm.Core.Data;

public class GeneConditionData
{
    public int id;
    public string name = string.Empty;
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
            name = condition.name,
            strong = condition.strong,
            not = condition.not
        };

        if (condition is ConcentrationCondition concentrationCondition)
        {
            data.conditionType = 0;
            data.morphogenId = concentrationCondition.morphogenId;
            data.strong = condition.strong;
            data.thresholdConcentration = concentrationCondition.thresholdConcentration;
            data.comparisonType = (int)concentrationCondition.comparisonType;
        }
        else if (condition is CellTypeCondition cellTypeCondition)
        {
            data.conditionType = 1;
            data.strong = condition.strong;
            data.cellTypeId = cellTypeCondition.cellType.id;
        }
        else if (condition is NeighbourCondition neighbourCondition)
        {
            data.conditionType = 2;
            data.strong = condition.strong;
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
                strong: data.strong,
                morphogenId: data.morphogenId,
                thresholdConcentration: data.thresholdConcentration,
                comparisonType: (GeneCondition.ComparisonType)data.comparisonType,
                name: data.name
            ),
            1 => new CellTypeCondition(
                id: data.id,
                not: data.not,
                strong: data.strong,
                cellType: simulation.CellTypes[data.cellTypeId],
                name: data.name
            ),
            2 => new NeighbourCondition(
                id: data.id,
                not: data.not,
                strong: data.strong,
                threshold: data.neighbourThreshold,
                comparisonType: (GeneCondition.ComparisonType)data.comparisonType,
                name: data.name
            ),
            _ => throw new ArgumentException("Invalid condition type")
        };

        return condition;
    }
}
