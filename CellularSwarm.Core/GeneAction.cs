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
        this.name = name;
        this.actionType = actionType;
    }

    public GeneAction(int id, ActionType actionType, Dictionary<int, float> actionMorphogens, string name = "", int cellTypeId = -1)
    {
        this.id = id;
        this.name = name;
        this.actionType = actionType;
        this.actionMorphogens = actionMorphogens;
        this.cellTypeId = cellTypeId;
    }

    public GeneAction(int id, ActionType actionType, int cellTypeId, string name = "")
    {
        this.id = id;
        this.name = name;
        this.actionType = actionType;
        this.cellTypeId = cellTypeId;
    }
    
    public GeneAction(GeneAction geneAction)
    {
        id = geneAction.id;
        name = geneAction.name;
        actionType = geneAction.actionType;
        actionMorphogens = new(geneAction.actionMorphogens);
        cellTypeId = geneAction.cellTypeId;
    }

    public enum ActionType
    {
        ChangeMorphogen,
        ChangeCellType,
        Apoptosis,
        Multiply,
        TransportMorphogen,
    }
}