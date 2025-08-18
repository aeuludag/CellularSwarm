namespace CellularSwarm.Core.Data;

public class SimulationData
{
    public int id;
    public string name = string.Empty;
    public Dictionary<string, CellData> cells = new();
    public Dictionary<int, CellTypeData> cellTypes = new();
    public Dictionary<int, MorphogenData> morphogens = new();
    public Dictionary<int, GeneActionData> geneActions = new();
    public Dictionary<int, GeneConditionData> geneConditions = new();
    public Dictionary<int, GeneData> genes = new();
    public float diffusionThreshold;
    public float diffusionFactor;

    public static SimulationData FromSimulation(Simulation simulation)
    {
        return new SimulationData
        {
            id = simulation.id,
            name = simulation.name,
            cells = simulation.cells.ToDictionary(c => c.Key.ToString(), c => CellData.FromCell(c.Value)),
            cellTypes = simulation.CellTypes.ToDictionary(c => c.Key, c => CellTypeData.FromCellType(c.Value)),
            morphogens = simulation.Morphogens.ToDictionary(m => m.Key, m => MorphogenData.FromMorphogen(m.Value)),
            geneActions = simulation.GeneActions.ToDictionary(a => a.Key, a => GeneActionData.FromGeneAction(a.Value)),
            geneConditions = simulation.GeneConditions.ToDictionary(c => c.Key, c => GeneConditionData.FromGeneCondition(c.Value)),
            genes = simulation.Genes.ToDictionary(g => g.Key, g => GeneData.FromGene(g.Value)),
            diffusionFactor = simulation.diffusionFactor,
            diffusionThreshold = simulation.diffusionThreshold,
        };
    }

    public static Simulation ToSimulation(SimulationData data)
    {
        var simulation = new Simulation(data.id, data.name)
        {
            CellTypes = data.cellTypes.ToDictionary(ct => ct.Key, ct => CellTypeData.ToCellType(ct.Value)),
            Morphogens = data.morphogens.ToDictionary(m => m.Key, m => MorphogenData.ToMorphogen(m.Value)),
            GeneActions = data.geneActions.ToDictionary(a => a.Key, a => GeneActionData.ToGeneAction(a.Value))
        };

        simulation.GeneConditions = data.geneConditions.ToDictionary(c => c.Key, c => GeneConditionData.ToGeneCondition(simulation, c.Value));
        simulation.Genes = data.genes.ToDictionary(g => g.Key, g => GeneData.ToGene(simulation, g.Value));

        simulation.cells = data.cells.ToDictionary(c => HexCoords.FromString(c.Key), c => CellData.ToCell(simulation, c.Value));

        simulation.diffusionFactor = data.diffusionFactor;
        simulation.diffusionThreshold = data.diffusionThreshold;

        return simulation;
    }
}
