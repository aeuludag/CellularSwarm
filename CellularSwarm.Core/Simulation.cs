using System.Collections.Generic;

namespace CellularSwarm.Core
{
    public class Simulation
    {
        public int id;
        public string name;

        public Dictionary<HexCoords, Cell> cells;
        public Dictionary<int, CellType> CellTypes;
        public Dictionary<int, Morphogen> Morphogens;
        public GeneAction[] GeneActions;

        public Simulation(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public Simulation(int id, string name, Dictionary<int, Morphogen> morphogens, GeneAction[] geneActions, Dictionary<int, CellType> cellTypes)
        {
            this.id = id;
            this.name = name;

            this.Morphogens = morphogens;
            this.GeneActions = geneActions;
            this.CellTypes = cellTypes;
        }
        
        //public Dictionary<HexCoords, Cell> Step(Dictionary<HexCoords, Cell> cellGrid)
        //{

        //}
    }
}