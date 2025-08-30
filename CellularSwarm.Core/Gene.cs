using System.Collections.Generic;

namespace CellularSwarm.Core;

public class Gene
{
    public int id;
    public string name;

    public List<GeneAction> actions = new();
    public List<GeneCondition> activatorConditions = new();
    public List<GeneCondition> inhibitorConditions = new();

    public Gene(int id, string name, List<GeneAction> actions, List<GeneCondition> activatorConditions, List<GeneCondition> inhibitorConditions)
    {
        this.id = id;
        this.name = name;
        this.actions = actions;
        this.activatorConditions = activatorConditions;
        this.inhibitorConditions = inhibitorConditions;
    }

    public Gene(Gene gene)
    {
        id = gene.id;
        name = gene.name;
        actions = new(gene.actions);
        activatorConditions = new(gene.activatorConditions);
        inhibitorConditions = new(gene.inhibitorConditions);
    }

    public bool ShouldBeActive(Cell cell)
    {
        if (NecessaryConditionsMet(inhibitorConditions, cell)) { return false; }
        if (NecessaryConditionsMet(activatorConditions, cell)) { return true; }
        return false;
    }

    public static bool NecessaryConditionsMet(List<GeneCondition> conditions, Cell cell)
    {
        bool conditionsMet = true;

        if (conditions.Count == 0) { return false; }

        foreach (GeneCondition condition in conditions)
        {
            conditionsMet &= condition.IsMet(cell);
        }

        return conditionsMet;
    }
}
