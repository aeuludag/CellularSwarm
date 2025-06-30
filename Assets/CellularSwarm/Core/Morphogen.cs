using System.Collections.Generic;

namespace CellularSwarm.Core
{
    public struct Morphogen
    {
        public int id;
        public string name;

        public Morphogen(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public struct MorphogenConcentrationPair 
    {
        public Morphogen morphogen;
        public int concentration;

        public MorphogenConcentrationPair(Morphogen morphogen, int concentration)
        {
            this.morphogen = morphogen;
            this.concentration = concentration >= 0 ? concentration : 0;
        }
        public MorphogenConcentrationPair Decay()
        {
            return new MorphogenConcentrationPair(this.morphogen, this.concentration - 1);
        }

        public static (List<Morphogen>, List<int>) PairToLists(List<MorphogenConcentrationPair> pairs)
        {
            List<Morphogen> morphogens = new();
            List<int> concentrations = new();

            foreach (var pair in pairs)
            {
                morphogens.Add(pair.morphogen);
                concentrations.Add(pair.concentration);
            }

            return (morphogens, concentrations);
        }

    }
}
