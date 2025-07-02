using System.Collections.Generic;

namespace CellularSwarm.Core
{
    public class Cell
    {
        public CellType cellType;
        public List<Gene> genes;
        public Dictionary<Morphogen, int> cellularContent;

        public int neighbourCount;

        public List<GeneAction> GetAvailableActions()
        {
            List<GeneAction> actions = new();

            foreach (Gene gene in genes)
            {
                if (gene.ShouldBeActive(this)) actions.AddRange(gene.actions);
            }

            return actions;
        }

        public void PerformAction(GeneAction action)
        {
            switch (action.actionType)
            {
                case GeneAction.ActionType.ChangeMorphogen:

                    foreach (var pair in action.actionMorphogens)
                    {
                        AddMorphogen(pair.Key, pair.Value);
                    }
                    break;

                case GeneAction.ActionType.Apoptosis:
                    Apoptosis();
                    break;

                case GeneAction.ActionType.Multiply:
                    Multiply();
                    break;
            }
        }

        public void AddMorphogen(Morphogen morphogen, int concentration)
        {
            SetMorphogen(morphogen, cellularContent.GetValueOrDefault(morphogen, 0) + concentration);
        }

        public void SetMorphogen(Morphogen morphogen, int concentration)
        {
            if(concentration <= 0) concentration = 0;
            cellularContent[morphogen] = concentration;
        }

        public void Apoptosis()
        {

        }

        public void Multiply()
        {

        }

    }
    public struct CellType
    {
        public int id;
        public string name;

        public CellType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public bool Equals(CellType other) => other.id == id;
        public override bool Equals(object obj) => obj is CellType other && (other.id == id);
        public override int GetHashCode() => id;
        public static bool operator ==(CellType left, CellType right) => left.Equals(right);
        public static bool operator !=(CellType left, CellType right) => !left.Equals(right);
    }
}
