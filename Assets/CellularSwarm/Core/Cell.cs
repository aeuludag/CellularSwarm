using System.Collections.Generic;

namespace CellularSwarm.Core
{
    public class Cell
    {
        public int id;
        public HexCoords coords;
        public List<Gene> genes;
        public List<MorphogenConcentrationPair> cellularContent;
    }
}
