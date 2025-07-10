namespace CellularSwarm.Tests;

using CellularSwarm.Core;

public class GeneTests
{
    public Simulation simulation;

    public GeneTests()
    {
        simulation = new Simulation(-1, "Test Sim");

    }

    [Theory]
    [InlineData(10, 15, false)]
    [InlineData(20, 15, true)]
    public void ShouldBeActive_DifferentConcentrations(int current, int threshold, bool result)
    {
        CellType Stem = new CellType(0, "Stem");

        var A = new Morphogen(0, "A", 1);
        var Multiplogen = new Morphogen(1, "Multiplogen", 1);
        var Unmultiplogen = new Morphogen(2, "Unmultiplogen", 1);

        Dictionary<int, int> Content = new(){
            { A.id, 1 },
            { Multiplogen.id, current },
            { Unmultiplogen.id, 20 },
            };

        var MultiplyAction = new GeneAction(0, GeneAction.ActionType.Multiply);
        var MultiplogenCondition = new ConcentrationCondition(0, false, Multiplogen.id, threshold, ConcentrationCondition.ComparisonType.GreaterThan);

        Gene GENE_0 = new Gene(
            0,
            new List<GeneAction>() { MultiplyAction },
            new List<GeneCondition> { MultiplogenCondition },
            new List<GeneCondition>()
        );

        List<Gene> DNA = new() { GENE_0 };

        var cell = new Cell(Stem, DNA, Content);

        Assert.Equal(GENE_0.ShouldBeActive(cell), result);
    }
}