using System.Collections.Generic;

namespace CellularSwarm.Core;

public class Gene
{
    public int id;
    public string name;

    public bool activatorAny = false;
    public bool inhibitorAny = false;
    public List<GeneCondition> activatorConditions = new();
    public List<GeneCondition> inhibitorConditions = new();
    public List<GeneAction> actions = new();

    public Gene(int id, string name, bool activatorAny, bool inhibitorAny, List<GeneCondition> activatorConditions, List<GeneCondition> inhibitorConditions, List<GeneAction> actions)
    {
        this.id = id;
        this.name = name;
        this.activatorAny = activatorAny;
        this.inhibitorAny = inhibitorAny;
        this.activatorConditions = activatorConditions;
        this.inhibitorConditions = inhibitorConditions;
        this.actions = actions;
    }

    public Gene(Gene gene)
    {
        id = gene.id;
        name = gene.name;
        activatorAny = gene.activatorAny;
        inhibitorAny = gene.inhibitorAny;
        activatorConditions = new(gene.activatorConditions);
        inhibitorConditions = new(gene.inhibitorConditions);
        actions = new(gene.actions);
    }

    public bool ShouldBeActive(Cell cell)
    {
        if (NecessaryConditionsMet(inhibitorConditions, inhibitorAny, cell)) { return false; }
        if (NecessaryConditionsMet(activatorConditions, activatorAny, cell)) { return true; }
        return false;
    }

    public static bool NecessaryConditionsMet(List<GeneCondition> conditions, bool any, Cell cell)
    {
        bool conditionsMet;

        if (conditions.Count == 0) { return false; }

        if (any)
        {
            conditionsMet = false;
            foreach (GeneCondition condition in conditions)
            {
                conditionsMet |= condition.IsMet(cell);
            }
        } else
        {
            conditionsMet = true;
            foreach (GeneCondition condition in conditions)
            {
                conditionsMet &= condition.IsMet(cell);
            }
        }

        return conditionsMet;
    }
}
