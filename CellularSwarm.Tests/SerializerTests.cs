// namespace CellularSwarm.Tests;

// using System.IO;
// using CellularSwarm.Core;
// using CellularSwarm.Core.Data;

// public class SerializerTests
// {
//     public Simulation simulation;

//     public SerializerTests()
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

//         var cell = new Cell(simulation);

//         cell.AddMorphogen(0, 10f);
//         cell.AddMorphogen(1, 15f);

//         cell.genes.Add(simulation.Genes[0]);

//         var cell2 = new Cell(cell);
//         cell2.SetMorphogen(0, 0f);

//         var cell3 = new Cell(cell);
//         cell3.SetMorphogen(2, 10f);

//         simulation.Cells.Add(new HexCoords(0, 0), cell);
//         simulation.Cells.Add(new HexCoords(0, 1), cell2);
//         simulation.Cells.Add(new HexCoords(0, 2), cell3);

//         simulation.Step();

//         using (var writer = new StringWriter())
//         {
//             var simulationData = SimulationData.FromSimulation(simulation);
//             var json = Serializer.Serialize(simulationData);
//             writer.Write(json);
//             File.WriteAllText("simulation.json", json);
//         }
//     }

//     [Fact]
//     public void Serialize_ShouldReturnValidJson()
//     {
//         // Arrange
//         var simulationData = new SimulationData
//         {
//             id = 1,
//             name = "Test Simulation",
//             cells = new Dictionary<string, CellData>(),
//             cellTypes = new Dictionary<int, CellTypeData>(),
//             morphogens = new Dictionary<int, MorphogenData>(),
//             geneActions = new Dictionary<int, GeneActionData>(),
//             geneConditions = new Dictionary<int, GeneConditionData>(),
//             genes = new Dictionary<int, GeneData>()
//         };

//         // Act
//         var json = Serializer.Serialize(simulationData);

//         // Assert
//         Assert.False(string.IsNullOrEmpty(json));
//     }

//     [Fact]
//     public void Deserialize_ShouldReturnValidSimulationData()
//     {
//         // Arrange
//         var json = "{\"id\":1,\"name\":\"Test Simulation\",\"cells\":{},\"cellTypes\":{},\"morphogens\":{},\"geneActions\":{},\"geneConditions\":{},\"genes\":{}}";

//         // Act
//         var simulationData = Serializer.Deserialize(json);

//         // Assert
//         Assert.NotNull(simulationData);
//         Assert.Equal(1, simulationData.id);
//         Assert.Equal("Test Simulation", simulationData.name);
//     }

//     [Fact]
//     public void SerializeAndDeserialize_ShouldReturnSameSimulationData()
//     {
//         // Arrange
//         var originalData = SimulationData.FromSimulation(simulation);

//         // Act
//         var json = Serializer.Serialize(originalData);
//         var deserializedData = Serializer.Deserialize(json);
//         var newSimulation = SimulationData.ToSimulation(deserializedData);

//         // Assert
//         Assert.Equal(originalData.id, newSimulation.id);
//         Assert.Equal(originalData.name, newSimulation.name);
//         Assert.Equal(originalData.cells.Count, newSimulation.Cells.Count);
//         Assert.Equal(originalData.cellTypes.Count, newSimulation.CellTypes.Count);
//         Assert.Equal(originalData.morphogens.Count, newSimulation.Morphogens.Count);
//         Assert.Equal(originalData.geneActions.Count, newSimulation.GeneActions.Count);
//         Assert.Equal(originalData.geneConditions.Count, newSimulation.GeneConditions.Count);
//         Assert.Equal(originalData.genes.Count, newSimulation.Genes.Count);
//     }

//     [Theory]
//     [InlineData("(0, 0)", 0, 0)]
//     [InlineData("(1, -1)", 1, -1)]
//     [InlineData("(-1, 1)", -1, 1)]
//     public void HexCoords_ShouldConvertFine(string expected, int q, int r)
//     {
//         // Arrange
//         var coords = new HexCoords(q, r);
//         var expectedCoords = coords.ToString();

//         Assert.Equal(expected, expectedCoords);
//         Assert.Equal(coords, HexCoords.FromString(expected));
//     }
// }