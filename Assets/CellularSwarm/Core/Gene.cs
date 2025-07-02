using System.Collections.Generic;

namespace CellularSwarm.Core
{
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

        public bool NecessaryConditionsMet(List<GeneCondition> conditions, Cell cell)
        {
            bool weakConditionsMet = false;
            bool weakConditionExists = false;
            bool strongConditionsMet = true;
            bool strongConditionExists = false;

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

    public abstract class GeneCondition
    {
        public int id;
        public bool strong; // All strong conditions and at least one weak condition must be met.

        public abstract bool IsMet(Cell cell);
    }

    public class ConcentrationCondition : GeneCondition
    {
        public Morphogen morphogen;
        public int thresholdConcentration;
        public ComparisonType comparisonType;

        public ConcentrationCondition(int id, Morphogen morphogen, int thresholdConcentration, ComparisonType comparisonType)
        {
            this.id = id;
            this.morphogen = morphogen;
            this.thresholdConcentration = thresholdConcentration;
            this.comparisonType = comparisonType;
        }

        public override bool IsMet(Cell cell)
        {
            int concentration = cell.cellularContent.GetValueOrDefault(morphogen, 0);

            switch (comparisonType)
            {
                case ComparisonType.GreaterThan:
                    return concentration >= thresholdConcentration;
                case ComparisonType.LessThan:
                    return concentration <= thresholdConcentration;
                default:
                    return false;
            }
        }
        public enum ComparisonType
        {
            GreaterThan,
            LessThan,
        }
    }

    public class CellTypeCondition : GeneCondition
    {
        public CellType cellType;
        public bool not;

        public CellTypeCondition(int id, CellType cellType, bool not)
        {
            this.cellType = cellType;
            this.not = not;
        }

        public override bool IsMet(Cell cell)
        {
            return not ? (cell.cellType != cellType) : (cell.cellType == cellType);
        }
    }

    public class GeneAction
    {
        public int id;
        public ActionType actionType;
        public Dictionary<Morphogen, int> actionMorphogens;

        public GeneAction(int id, ActionType actionType)
        {
            this.id = id;
            this.actionType = actionType;

            if (actionType == ActionType.ChangeMorphogen)
            {
                throw new System.Exception("Morphogen list is not specified in Change Morphogen actions.");
            }
        }

        public GeneAction(int id, ActionType actionType, Dictionary<Morphogen, int> actionMorphogens)
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
}
