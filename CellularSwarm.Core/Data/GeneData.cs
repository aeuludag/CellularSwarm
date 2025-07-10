namespace CellularSwarm.Core.Data;

public class GeneData
{
    public int id;
    public string name;

    public List<int> actionIDs;
    public List<int> activitorIDs;
    public List<int> inhibitorIDs;
}

public class ActionData
{
    public int id;
    public int actionType;
    public List<CellularContentPair> actionMorphogens;
}