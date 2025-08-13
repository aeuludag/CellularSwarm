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

    public static HexCoords operator -(HexCoords a, HexCoords b)
    {
        return new HexCoords(a.q - b.q, a.r - b.r);
    }

    public override string ToString()
    {
        return $"({q}, {r})";
    }

    public static HexCoords FromString(string coords)
    {
        var parts = coords.Trim('(', ')').Split(',');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int q) || !int.TryParse(parts[1], out int r))
        {
            throw new FormatException("Invalid hex coordinate format. Expected format: (q, r)");
        }
        return new HexCoords(q, r);
    }

    public static bool operator ==(HexCoords a, HexCoords b)
    {
        return a.q == b.q && a.r == b.r;
    }

    public static bool operator !=(HexCoords a, HexCoords b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (q, r).GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is HexCoords other)
        {
            return q == other.q && r == other.r;
        }
        return false;
    }
}