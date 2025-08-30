// namespace CellularSwarm.Tests;

// using System.Diagnostics;
// using CellularSwarm.Core;

// public class GeneTests
// {
//     public Simulation simulation;

//     public GeneTests()
//     {
//         simulation = new Simulation(
//             id: 0,
//             name: "Test Simulation"
//         );

//         simulation.CellTypes.Add(0, new CellType(0, "Stem"));
//         simulation.CellTypes.Add(1, new CellType(1, "Meat"));
//         simulation.CellTypes.Add(2, new CellType(2, "Skin"));

//         simulation.Morphogens.Add(0, new Morphogen(0, "A", 0.9f, 0.1f));
//         simulation.Morphogens.Add(1, new Morphogen(1, "B", 0.8f, 0.1f));
//         simulation.Morphogens.Add(2, new Morphogen(2, "C", 0.7f, 0.1f));

//         simulation.GeneActions.Add(0, new GeneAction(0, GeneAction.ActionType.Multiply));

//         simulation.GeneConditions.Add(0, new ConcentrationCondition(0, false, false, 0, 15f, ConcentrationCondition.ComparisonType.GreaterThan));
//         simulation.GeneConditions.Add(1, new ConcentrationCondition(1, false, false, 1, 20f, ConcentrationCondition.ComparisonType.GreaterThan));

//         simulation.Genes.Add(0, new Gene(
//             id: 0,
//             name: "Test Gene",
//             actions: new List<GeneAction> { simulation.GeneActions[0] },
//             activatorConditions: new List<GeneCondition> { simulation.GeneConditions[0] },
//             inhibitorConditions: new List<GeneCondition> { simulation.GeneConditions[1] }
//         ));
//     }
    
//     [Theory]
//     [InlineData(10f, 15f, false)]
//     [InlineData(20f, 15f, true)]
//     [InlineData(10f, 25f, false)]
//     [InlineData(20f, 25f, false)]

//     public void ShouldBeActive_DifferentConcentrations(float morphogenA, float morphogenB, bool result)
//     {
//         Cell cell = new Cell(simulation);
//         cell.genes.Add(simulation.Genes[0]);

//         cell.SetMorphogen(0, morphogenA);
//         cell.SetMorphogen(1, morphogenB);

//         bool isActive = cell.genes[0].ShouldBeActive(cell);

//         Debugger.Log(0, "GeneTests", $"\nMorphogen A: {morphogenA}, Morphogen B: {morphogenB}, Expected: {result}, Actual: {isActive}\n");
//         Debugger.Log(0, "GeneTests", $"\nActivator conditions met: {Gene.NecessaryConditionsMet(cell.genes[0].activatorConditions, cell)}\n");
//         Debugger.Log(0, "GeneTests", $"\nInhibitor conditions met: {Gene.NecessaryConditionsMet(cell.genes[0].inhibitorConditions, cell)}\n");

//         Assert.Equal(result, isActive);
//     }
// }