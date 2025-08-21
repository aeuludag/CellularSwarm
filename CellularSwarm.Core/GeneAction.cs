namespace CellularSwarm.Core;

public class GeneAction
{
    public int id;
    public string name = string.Empty; 
    public ActionType actionType;
    public Dictionary<int, float> actionMorphogens = new();
    public int cellTypeId;

    public GeneAction(int id, ActionType actionType, string name = "")
    {
        this.id = id;
        this.actionType = actionType;
    }

    public GeneAction(int id, ActionType actionType, Dictionary<int, float> actionMorphogens, int cellTypeId = -1, string name = "")
    {
        this.id = id;
        this.actionType = actionType;
        this.actionMorphogens = actionMorphogens;
        this.cellTypeId = cellTypeId;
    }

    public GeneAction(int id, ActionType actionType, int cellTypeId, string name = "")
    {
        this.id = id;
        this.actionType = actionType;
        this.cellTypeId = cellTypeId;
    }

    public enum ActionType
    {
        ChangeMorphogen,
        ChangeCellType,
        Apoptosis,
        Multiply,
    }
}