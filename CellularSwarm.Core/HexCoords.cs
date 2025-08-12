namespace CellularSwarm.Core;

public struct HexCoords
{
    public int q;
    public int r;

    public HexCoords(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public static HexCoords[] offsets = [new HexCoords(0, -1), new HexCoords(-1, 0), new HexCoords(0, 1), new HexCoords(1, 0), new HexCoords(-1, 1), new HexCoords(1, -1)];
    public static HexCoords[] GetNeighbouringCoords(HexCoords hexCoords)
    {
        var coords = new HexCoords[6];
        for (int i = 0; i < 6; i++)
        {
            coords[i] = hexCoords + offsets[i];
        }
        return coords;
    }

    public HexCoords[] GetNeighbouringCoords()
    {
        var coords = new HexCoords[6];
        for (int i = 0; i < 6; i++)
        {
            coords[i] = this + offsets[i];
        }
        return coords;
    }

    public static HexCoords operator +(HexCoords a, HexCoords b)
    {
        return new HexCoords(a.q + b.q, a.r + b.r);
    }
}