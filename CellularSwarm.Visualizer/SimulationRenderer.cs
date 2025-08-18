using System;
using System.Numerics;
using CellularSwarm.Core;

public class SimulationRenderer
{
    public Simulation simulation;
    public SimulationRenderer()
    {
        simulation = new Simulation(
            id: 0,
            name: "Test Simulation"
        );

        simulation.CellTypes.Add(0, new CellType(0, "Stem"));
        simulation.CellTypes.Add(1, new CellType(1, "Meat"));
        simulation.CellTypes.Add(2, new CellType(2, "Skin"));

        simulation.Morphogens.Add(0, new Morphogen(0, "A", 0.9f, 0.1f));
        simulation.Morphogens.Add(1, new Morphogen(1, "B", 0.6f, 0.1f));
        simulation.Morphogens.Add(2, new Morphogen(2, "C", 0.3f, 0.1f));

        simulation.GeneActions.Add(0, new GeneAction(0, GeneAction.ActionType.Multiply));
        simulation.GeneActions.Add(1, new GeneAction(1, GeneAction.ActionType.Apoptosis));

        simulation.GeneConditions.Add(0, new ConcentrationCondition(0, false, 1, 50f, GeneCondition.ComparisonType.GreaterThan));
        simulation.GeneConditions.Add(1, new ConcentrationCondition(1, false, 0, 30f, GeneCondition.ComparisonType.GreaterThan));

        simulation.GeneConditions.Add(2, new ConcentrationCondition(2, false, 0, 70f, GeneCondition.ComparisonType.GreaterThan));

        simulation.Genes.Add(0, new Gene(
            id: 0,
            name: "MulGEN",
            actions: new List<GeneAction> { simulation.GeneActions[0] },
            activatorConditions: new List<GeneCondition> { simulation.GeneConditions[0] },
            inhibitorConditions: new List<GeneCondition> { simulation.GeneConditions[1] }
        ));

        simulation.Genes.Add(1, new Gene(
            id: 1,
            name: "KilGEN",
            actions: new List<GeneAction> { simulation.GeneActions[1] },
            activatorConditions: new List<GeneCondition> { simulation.GeneConditions[2] },
            inhibitorConditions: new List<GeneCondition> { /*simulation.GeneConditions[1]*/ }
        ));
    }

    public void SetPredefined()
    {
        float max = 100f;
        (HexCoords pos, float a, float b, float c)[] predefined = [
            (new HexCoords(0, 0), 0f, 0f, 0f),
            (new HexCoords(0, 1), 0f, 0f, 0f),
            (new HexCoords(0, -1), 0f, 0f, 0f),
            (new HexCoords(-1, 0), 0f, 0f, 0f),
            (new HexCoords(1, -1), 0f, 0f, 0f),
            (new HexCoords(-1, 1), 0f, 0f, 0f),
            (new HexCoords(-1, -1), 0f, 0f, 0f),
            // (new HexCoords(1, 1), 0f, 0f, 0f),

            (new HexCoords(1, 2), 0f, 0f, 0f),
            (new HexCoords(1, 3), 0f, 0f, 0f),
            (new HexCoords(1, 4), 0f, 0f, 0f),
            (new HexCoords(1, 5), 0f, 0f, 0f),
            (new HexCoords(0, 5), 0f, 0f, 0f),
            (new HexCoords(-1, 5), 0f, 0f, 0f),
            (new HexCoords(-2, 5), 0f, 0f, 0f),
            (new HexCoords(-3, 5), 0f, 0f, 0f),
            (new HexCoords(-4, 5), 0f, 0f, 0f),
            (new HexCoords(-4, 4), 0f, 0f, 0f),
            (new HexCoords(-4, 3), 0f, 0f, 0f),
            (new HexCoords(-4, 2), 0f, 0f, 0f),
            (new HexCoords(-4, 1), 0f, 0f, 0f),
            (new HexCoords(-5, 3), 0f, 0f, 0f),
            (new HexCoords(-5, 2), max, 0f, 0f),
            (new HexCoords(-5, 1), 0f, 0f, 0f),
            (new HexCoords(-6, 3), 0f, 0f, 0f),
            (new HexCoords(-6, 2), 0f, 0f, 0f),

            (new HexCoords(2, 0), 0f, 0f, 0f),
            (new HexCoords(3, 0), 0f, 0f, 0f),
            (new HexCoords(4, 0), 0f, 0f, 0f),
            (new HexCoords(5, 0), 0f, 0f, 0f),
            (new HexCoords(6, 0), 0f, 0f, max),
            (new HexCoords(6, 1), 0f, 0f, 0f),
            (new HexCoords(6, -1), 0f, 0f, 0f),
            (new HexCoords(7, 0), 0f, 0f, 0f),
            (new HexCoords(7, -1), 0f, 0f, 0f),
            (new HexCoords(5, 1), 0f, 0f, 0f),

            (new HexCoords(5, -3), 0f, max, 0f),
            (new HexCoords(5, -2), 0f, 0f, 0f),
            (new HexCoords(5, -4), 0f, 0f, 0f),
            (new HexCoords(4, -2), 0f, 0f, 0f),
            (new HexCoords(4, -3), 0f, 0f, 0f),
            (new HexCoords(6, -3), 0f, 0f, 0f),
            (new HexCoords(6, -4), 0f, 0f, 0f),

            (new HexCoords(2, -2), 0f, 0f, 0f),
            (new HexCoords(3, -3), 0f, 0f, 0f),

            // (new HexCoords(-2, -1), 0f, 0f, 0f),
            // (new HexCoords(-3, -1), 0f, 0f, 0f),
            // (new HexCoords(-4, -1), 0f, 0f, 0f),
            // (new HexCoords(-5, -1), 0f, 0f, 0f),
            // (new HexCoords(-6, -1), 0f, 0f, 0f),
            // (new HexCoords(-7, -1), 0f, 500f, 0f),
        ];

        for (int i = 0; i < predefined.Length; i++)
        {
            AddCell(predefined[i].pos, predefined[i].a, predefined[i].b, predefined[i].c);
        }
    }

    public void GenerateCellGrid(int radius, HexCoords offset, (float a, float b, float c) abc)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    AddCell(offset + new HexCoords(q, r), abc.a, abc.b, abc.c);
                }
            }
        }
    }
    public void GenerateCellGrid(int radius, HexCoords offset, Vector3 palette, float max)
    {
        palette *= max;
        GenerateCellGrid(radius, offset, (palette.X, palette.Y, palette.Z));
    }

    public void AddCell(HexCoords coords, float a, float b, float c)
    {
        if (simulation.cells.ContainsKey(coords))
        {
            var theCell = simulation.cells[coords];
            theCell.SetMorphogen(0, a);
            theCell.SetMorphogen(1, b);
            theCell.SetMorphogen(2, c);
            return;
        }

        var cell = new Cell(simulation);

        cell.SetMorphogen(0, a);
        cell.SetMorphogen(1, b);
        cell.SetMorphogen(2, c);

        cell.genes.Add(simulation.Genes[0]);
        cell.genes.Add(simulation.Genes[1]);

        simulation.cells.Add(coords, cell);
    }
    public void AddCell(HexCoords coords, Vector3 palette, float max)
    {
        palette *= max;
        AddCell(coords, palette.X, palette.Y, palette.Z);
    }

    public void RemoveCell(HexCoords coords)
    {
        if (!simulation.cells.ContainsKey(coords))
        {
            return;
        }

        simulation.cells.Remove(coords);
    }

    public void ClearGrid()
    {
        foreach (var cellPair in simulation.cells)
        {
            var coords = cellPair.Key;
            var theCell = simulation.cells[coords];
            theCell.SetMorphogen(0, 0);
            theCell.SetMorphogen(1, 0);
            theCell.SetMorphogen(2, 0);
        }
    }
}
