using System.Collections.Generic;

namespace CellularSwarm.Core;

public class Gene
{
    public int id;
    public string name;

    public List<GeneAction> actions;
    public List<GeneCondition> activitorConditions;
    public List<GeneCondition> inhibitorConditions;

    public Gene(int id, List<GeneAction> actions, List<GeneCondition> activitorConditions, List<GeneCondition> inhibitorConditions)
    {
        this.id = id;
        this.actions = actions;
        this.activitorConditions = activitorConditions;
        this.inhibitorConditions = inhibitorConditions;
    }

    public bool ShouldBeActive(Cell cell)
    {
        // TODO: Write Unit Tests for this.

        if(NecessaryConditionsMet(inhibitorConditions, cell)) { return false; }
        if(NecessaryConditionsMet(activitorConditions, cell)) { return true; }
        return false;
    }

    public static bool NecessaryConditionsMet(List<GeneCondition> conditions, Cell cell)
    {
        bool weakConditionsMet = false;
        bool weakConditionExists = false;
        bool strongConditionsMet = true;
        bool strongConditionExists = false;

        if(conditions.Count == 0) { return false; }

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

public class GeneAction
{
    public int id;
    public ActionType actionType;
    public Dictionary<int, int> actionMorphogens;

    public GeneAction(int id, ActionType actionType)
    {
        this.id = id;
        this.actionType = actionType;

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
