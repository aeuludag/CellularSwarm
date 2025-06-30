namespace CellularSwarm.Core
{
    public class CellGrid
    {

    }

    public struct HexCoords
    {
        public int x;
        public int y;

        public HexCoords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static HexCoords operator +(HexCoords a, HexCoords b)
        {
            return new HexCoords(a.x + b.x, a.y + b.y);
        }
    }
}