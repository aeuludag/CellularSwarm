namespace CellularSwarm.Core.Data;

public class GeneData
{
    public int id;
    public string name = string.Empty;
    public List<int> actionIDs = new();
    public List<int> activatorConditionIDs = new();
    public List<int> inhibitorConditionIDs = new();

    public static GeneData FromGene(Gene gene)
    {
        return new GeneData
        {
            id = gene.id,
            name = gene.name,
            actionIDs = gene.actions.Select(a => a.id).ToList(),
            activatorConditionIDs = gene.activatorConditions.Select(c => c.id).ToList(),
            inhibitorConditionIDs = gene.inhibitorConditions.Select(c => c.id).ToList()
        };
    }

    public static Gene ToGene(Simulation simulation, GeneData data)
    {
        var actions = data.actionIDs.Select(id => simulation.GeneActions[id]).ToList();
        var activatorConditions = data.activatorConditionIDs.Select(id => simulation.GeneConditions[id]).ToList();
        var inhibitorConditions = data.inhibitorConditionIDs.Select(id => simulation.GeneConditions[id]).ToList();

        return new Gene(data.id, data.name, actions, activatorConditions, inhibitorConditions);
    }

}