namespace CellularSwarm.Core.Data;

public class GeneActionData
{
    public int id;
    public int actionType;
    public Dictionary<int, int> actionMorphogens = new();

    public static GeneActionData FromGeneAction(GeneAction action)
    {
        return new GeneActionData
        {
            id = action.id,
            actionType = (int)action.actionType,
            actionMorphogens = new Dictionary<int, int>(action.actionMorphogens)
        };
    }

    public static GeneAction ToGeneAction(GeneActionData data)
    {
        return new GeneAction(data.id, (GeneAction.ActionType)data.actionType, data.actionMorphogens);
    }
}
