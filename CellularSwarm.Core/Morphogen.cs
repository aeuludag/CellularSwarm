namespace CellularSwarm.Core;

public class Morphogen
{
    public int id;
    public string name;
    public float diffusionFactor;
    public float decayFactor;

    public Morphogen(int id, string name, float diffusionFactor, float decayFactor)
    {
        this.id = id;
        this.name = name;
        this.diffusionFactor = diffusionFactor;
        this.decayFactor = decayFactor;
    }

    public bool Equals(Morphogen other) => other.id == id;
    public override bool Equals(object? obj) => obj is Morphogen other && (other.id == id);
    public override int GetHashCode() => id;

    public static bool operator ==(Morphogen left, Morphogen right) => left.Equals(right);
    public static bool operator !=(Morphogen left, Morphogen right) => !left.Equals(right);
}