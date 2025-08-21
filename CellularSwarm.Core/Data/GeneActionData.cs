namespace CellularSwarm.Core.Data;

public class GeneActionData
{
    public int id;
    public string name = string.Empty;
    public int actionType;
    public Dictionary<int, float> actionMorphogens = new();
    public int cellTypeId;

    public static GeneActionData FromGeneAction(GeneAction action)
    {
        return new GeneActionData
        {
            id = action.id,
            name = action.name,
            actionType = (int)action.actionType,
            actionMorphogens = new Dictionary<int, float>(action.actionMorphogens),
            cellTypeId = action.cellTypeId,
        };
    }

    public static GeneAction ToGeneAction(GeneActionData data)
    {
        return new GeneAction(data.id, (GeneAction.ActionType)data.actionType, data.actionMorphogens, data.cellTypeId, data.name);
    }
}
