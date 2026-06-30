namespace CellularSwarm.Core.Data;

public class GeneData
{
    public int id;
    public string name = string.Empty;
    public bool activatorAny = false;
    public bool inhibitorAny = false;
    public List<int> activatorConditionIDs = new();
    public List<int> inhibitorConditionIDs = new();
    public List<int> actionIDs = new();

    public static GeneData FromGene(Gene gene)
    {
        return new GeneData
        {
            id = gene.id,
            name = gene.name,
            activatorAny = gene.activatorAny,
            inhibitorAny = gene.inhibitorAny,
            activatorConditionIDs = gene.activatorConditions.Select(c => c.id).ToList(),
            inhibitorConditionIDs = gene.inhibitorConditions.Select(c => c.id).ToList(),
            actionIDs = gene.actions.Select(a => a.id).ToList(),
        };
    }

    public static Gene ToGene(Simulation simulation, GeneData data)
    {
        var activatorConditions = data.activatorConditionIDs.Select(id => simulation.GeneConditions[id]).ToList();
        var inhibitorConditions = data.inhibitorConditionIDs.Select(id => simulation.GeneConditions[id]).ToList();
        var actions = data.actionIDs.Select(id => simulation.GeneActions[id]).ToList();

        return new Gene(data.id, data.name, data.activatorAny, data.inhibitorAny, activatorConditions, inhibitorConditions, actions);
    }

}