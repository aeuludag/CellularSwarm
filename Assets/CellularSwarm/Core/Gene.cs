using System.Collections.Generic;

namespace CellularSwarm.Core
{
    public class Gene
    {
        public int id;
        public string name;

        public GeneAction action;
        public List<GeneCondition> activitorConditions;
        public List<GeneCondition> inhibitorConditions;

        public Gene(int id, GeneAction action, List<GeneCondition> activitorConditions, List<GeneCondition> inhibitorConditions)
        {
            this.id = id;
            this.action = action;
            this.activitorConditions = activitorConditions;
            this.inhibitorConditions = inhibitorConditions;
        }

        public bool ShouldBeActive(List<Morphogen> morphogens, List<int> concentrations)
        {
            bool necessaryConditionsMet = false;

            foreach (GeneCondition condition in inhibitorConditions)
            {
                Morphogen conditionMorphogen = condition.morphogen;

                if (morphogens.Contains(conditionMorphogen))
                {
                    int concentration = concentrations[morphogens.IndexOf(conditionMorphogen)];

                    if (condition.strong)
                    {
                        necessaryConditionsMet &= condition.IsMet(concentration);
                        if (necessaryConditionsMet) { return false; }
                    }
                    else
                    {
                        necessaryConditionsMet |= condition.IsMet(concentration);
                    }
                }
            }

            if (necessaryConditionsMet) { return false; }

            foreach (GeneCondition condition in activitorConditions)
            {
                Morphogen conditionMorphogen = condition.morphogen;

                if (morphogens.Contains(conditionMorphogen))
                {
                    int concentration = concentrations[morphogens.IndexOf(conditionMorphogen)];

                    if (condition.strong)
                    {
                        necessaryConditionsMet &= condition.IsMet(concentration);
                        if (!necessaryConditionsMet) { return false; }
                    }
                    else
                    {
                        necessaryConditionsMet |= condition.IsMet(concentration);
                    }
                }
            }

            return necessaryConditionsMet;
        }
    }

    public class GeneCondition
    {
        public int id;
        public string name;

        public Morphogen morphogen;
        public int thresholdConcentration;
        public ComparisonType comparisonType;
        public bool strong;
        // When a cell has multiple conditions, strong conditions must be all met (& operator) whereas weak conditions need only one of them to be met (| op.).
        // If all conditions are weak, only one of them is enough to activate/inhibit the gene.
        // If all conditions are strong, all of them must be met to activate/inhibit the gene.
        // My naming may be confusing, but I mean strong as in "a must" to continue and not as in "can satisfy the need on its own".
        // I'll probably get confuse myself too.

        public GeneCondition(int id, Morphogen morphogen, int thresholdConcentration, ComparisonType comparisonType)
        {
            this.id = id;
            this.morphogen = morphogen;
            this.thresholdConcentration = thresholdConcentration;
            this.comparisonType = comparisonType;
        }

        public bool IsMet(int concentration)
        {
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

    public class GeneAction
    {
        public int id;
        public ActionType actionType;
        public List<MorphogenConcentrationPair> actionMorphogens;

        public GeneAction(int id, ActionType actionType)
        {
            this.id = id;
            this.actionType = actionType;

            if (actionType == ActionType.ReleaseMorphogen || actionType == ActionType.RemoveMorphogen)
            {
                throw new System.Exception("Morphogen list is not specified in Release or Remove Morphogen events.");
            }
        }

        public GeneAction(int id, ActionType actionType, List<MorphogenConcentrationPair> actionMorphogens)
        {
            this.id = id;
            this.actionType = actionType;
            this.actionMorphogens = actionMorphogens;
        }

        public enum ActionType
        {
            ReleaseMorphogen,
            RemoveMorphogen,
            Apoptosis,
            Reproduce,

        }
    }
}
