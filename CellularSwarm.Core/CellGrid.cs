namespace CellularSwarm.Core
{
    public struct HexCoords
    {
        public int q;
        public int r;

        public HexCoords(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public static HexCoords operator +(HexCoords a, HexCoords b)
        {
            return new HexCoords(a.q + b.q, a.r + b.r);
        }
    }
}