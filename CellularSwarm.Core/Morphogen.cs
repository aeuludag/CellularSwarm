namespace CellularSwarm.Core
{
    public struct Morphogen
    {
        public int id;
        public string name;
        public float diffusionFactor;

        public Morphogen(int id, string name, float diffusionFactor)
        {
            this.id = id;
            this.name = name;
            this.diffusionFactor = diffusionFactor;
        }

        public bool Equals(Morphogen other) => other.id == id;
        public override bool Equals(object obj) => obj is Morphogen other && (other.id == id);
        public override int GetHashCode() => id;

        public static bool operator ==(Morphogen left, Morphogen right) => left.Equals(right);
        public static bool operator !=(Morphogen left, Morphogen right) => !left.Equals(right);
    }
}
