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
        actions = gene.actions;
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
        bool weakConditionsMet = false;
        bool weakConditionExists = false;
        bool strongConditionsMet = true;
        bool strongConditionExists = false;

        if (conditions.Count == 0) { return false; }

        foreach (GeneCondition condition in conditions)
        {
            if (condition.strong)
            {
                strongConditionExists = true;
                strongConditionsMet &= condition.IsMet(cell);
            }
            else
            {
                weakConditionExists = true;
                weakConditionsMet |= condition.IsMet(cell);
            }
        }

        bool strongs = !strongConditionExists || strongConditionsMet; // only false when StrongConditionExists and its not met (p -> q)
        bool weaks = !weakConditionExists || weakConditionsMet; // only false when WeakConditionExists and its not met (p -> q)

        return strongs && weaks;
    }
}
