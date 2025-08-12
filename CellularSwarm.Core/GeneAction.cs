namespace CellularSwarm.Core;

public class GeneAction
{
    public int id;
    public string name = string.Empty; 
    public ActionType actionType;
    public Dictionary<int, int> actionMorphogens;

    public GeneAction(int id, ActionType actionType)
    {
        this.id = id;
        this.actionType = actionType;
        actionMorphogens = new Dictionary<int, int>();

        if (actionType == ActionType.ChangeMorphogen)
        {
            throw new Exception("Morphogen list is not specified in Change Morphogen actions.");
        }
    }

    public GeneAction(int id, ActionType actionType, Dictionary<int, int> actionMorphogens)
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