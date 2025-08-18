namespace CellularSwarm.Core;

public class GeneAction
{
    public int id;
    public string name = string.Empty; 
    public ActionType actionType;
    public Dictionary<int, float> actionMorphogens = new();

    public GeneAction(int id, ActionType actionType)
    {
        this.id = id;
        this.actionType = actionType;
    }

    public GeneAction(int id, ActionType actionType, Dictionary<int, float> actionMorphogens)
    {
        this.id = id;
        this.actionType = actionType;
        this.actionMorphogens = actionMorphogens;
    }

    public enum ActionType
    {
        ChangeMorphogen,
        Apoptosis,
        Multiply,
    }
}