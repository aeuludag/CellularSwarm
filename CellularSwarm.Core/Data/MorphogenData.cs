namespace CellularSwarm.Core.Data;

public class MorphogenData
{
    public int id;
    public string name = string.Empty;
    public float diffusionFactor;

    public static MorphogenData FromMorphogen(Morphogen morphogen)
    {
        return new MorphogenData
        {
            id = morphogen.id,
            name = morphogen.name,
            diffusionFactor = morphogen.diffusionFactor
        };
    }

    public static Morphogen ToMorphogen(MorphogenData data)
    {
        return new Morphogen(data.id, data.name, data.diffusionFactor);
    }
}
