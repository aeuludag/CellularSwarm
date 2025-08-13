using System.Collections.Generic;
using CellularSwarm.Core;
using static System.ValueTuple;
using UnityEngine;
using CellularSwarm.Core.Data;

public class Visualizer : MonoBehaviour
{
    public HexGridRenderer hexGridRenderer;
    public Simulation simulation;

    // Start is called before the first frame update
    void Start()
    {
        simulation = new Simulation(
            id: 0,
            name: "Test Simulation"
        );

        simulation.CellTypes.Add(0, new CellType(0, "Stem"));
        simulation.CellTypes.Add(1, new CellType(1, "Meat"));
        simulation.CellTypes.Add(2, new CellType(2, "Skin"));

        simulation.Morphogens.Add(0, new Morphogen(0, "A", 0.9f));
        simulation.Morphogens.Add(1, new Morphogen(1, "B", 0.8f));
        simulation.Morphogens.Add(2, new Morphogen(2, "C", 0.7f));

        simulation.GeneActions.Add(0, new GeneAction(0, GeneAction.ActionType.Multiply));

        simulation.GeneConditions.Add(0, new ConcentrationCondition(0, false, 0, 15f, GeneCondition.ComparisonType.GreaterThan));
        simulation.GeneConditions.Add(1, new ConcentrationCondition(1, false, 1, 20f, GeneCondition.ComparisonType.GreaterThan));

        simulation.Genes.Add(0, new Gene(
            id: 0,
            name: "Test Gene",
            actions: new List<GeneAction> { simulation.GeneActions[0] },
            activatorConditions: new List<GeneCondition> { simulation.GeneConditions[0] },
            inhibitorConditions: new List<GeneCondition> { simulation.GeneConditions[1] }
        ));

        var cell = new Cell(simulation);

        cell.SetMorphogen(0, 10f);
        cell.SetMorphogen(1, 15f);

        cell.genes.Add(simulation.Genes[0]);

        hexGridRenderer.GenerateGridFromSimulation(simulation);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
