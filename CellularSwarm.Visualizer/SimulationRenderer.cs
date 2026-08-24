using System;
using System.Numerics;
using CellularSwarm.Core;
using CellularSwarm.Visualizer;
using Raylib_cs;

public class SimulationRenderer
{
    public const string VERSION = "1.0.260823";
    public const string NAME = "Default Limon Renderer";
    public Simulation Simulation { get => _simulation; set => _simulation = value; }
    public VisualizationType visualizationType = VisualizationType.ThreeMorphogens;

    public Dictionary<int, Color> cellTypeColors = new();
    public List<(Cell cell, string name)> cellPalette;
    public int cellIndex;
    public int redMorphogenId;
    public int greenMorphogenId;
    public int blueMorphogenId;
    public int singleMorphogenId;
    public float amplifier = 1f;
    public int geneId;
    public Color activeGeneColor = new Color(0.75f, 0.3f, 0.3f, 1f);
    public Color inactiveGeneColor = new Color(0.2f, 0.5f, 0.75f, 1f);
    public int geneConditionId;
    public Color metConditionColor = new Color(0.3f, 0.8f, 0.25f, 1f);
    public Color notMetConditionColor = new Color(0.7f, 0.25f, 0.25f, 1f);

    public Cell CellToDraw { get => cellPalette[cellIndex].cell; }
    public Cell EmptyCell { get => cellPalette[0].cell; }

    Simulation _simulation;
    private bool _useParallel = true;
    private Func<Dictionary<HexCoords, Cell>> _stepFunction;

    public SimulationRenderer()
    {
        _simulation = new(0, "new");
        cellPalette = [(new Cell(_simulation), "Empty Cell")];
        cellIndex = 0;
        _stepFunction = _simulation.StepParallel;
        // _useParallel = ConfigHandler.Config.useParallel;
        // _stepFunction = _useParallel ? Simulation.StepParallel : Simulation.Step;
    }

    public SimulationRenderer(Simulation simulation)
    {
        _simulation = simulation;
        cellPalette = [(new Cell(_simulation), "Empty Cell")];
        cellIndex = 0;
        _stepFunction = _simulation.StepParallel;
        // _useParallel = ConfigHandler.Config.useParallel;
        // _stepFunction = _useParallel ? Simulation.StepParallel : Simulation.Step;
    }

    public void SetParallel()
    {
        _useParallel = ConfigHandler.Config.useParallel;
        _stepFunction = _useParallel ? _simulation.StepParallel : _simulation.Step;
    }
    public void Step()
    {
        _stepFunction.Invoke();
    }

    public void ChangeSimulation(Simulation simulation)
    {
        _simulation = simulation;
        cellPalette = [(new Cell(_simulation), "Empty Cell")];
        cellIndex = 0;
    }

    // public Simulation SetSampleSimulation()
    // {
    //     var simulation = new Simulation(
    //         id: 0,
    //         name: "default"
    //     );

    //     simulation.CellTypes.Add(0, new CellType(0, "Stem"));
    //     simulation.CellTypes.Add(1, new CellType(1, "Meat"));
    //     simulation.CellTypes.Add(2, new CellType(2, "Skin"));

    //     simulation.Morphogens.Add(0, new Morphogen(0, "Dieogen", 0.1f, 0.05f));
    //     simulation.Morphogens.Add(1, new Morphogen(1, "Liveogen", 0.3f, 0.01f));
    //     simulation.Morphogens.Add(2, new Morphogen(2, "blu", 0f, 0.01f));

    //     simulation.GeneActions.Add(0, new GeneAction(0, GeneAction.ActionType.Multiply,
    //         new Dictionary<int, float>() { { 2, 0f } }, "Stem Multiply"));
    //     simulation.GeneActions.Add(1, new GeneAction(1, GeneAction.ActionType.Apoptosis, "Apoptosis"));
    //     simulation.GeneActions.Add(2, new GeneAction(2, GeneAction.ActionType.ChangeMorphogen,
    //         new Dictionary<int, float>() { { 1, 50f }, { 2, 100f } }, "Stem Morphogens"));
    //     simulation.GeneActions.Add(3, new GeneAction(3, GeneAction.ActionType.ChangeMorphogen,
    //         new Dictionary<int, float>() { { 0, 10f } }, "Generate Red"));

    //     simulation.GeneConditions.Add(0,
    //         new ConcentrationCondition(0, not: false, 1, 40f, GeneCondition.ComparisonType.GreaterThan, "Liveogen To Multiply"));
    //     simulation.GeneConditions.Add(1,
    //         new ConcentrationCondition(1, not: false, 0, 60f, GeneCondition.ComparisonType.GreaterThan, "Dieogen To Prevent"));
    //     simulation.GeneConditions.Add(2,
    //         new ConcentrationCondition(2, not: false, 0, 80f, GeneCondition.ComparisonType.GreaterThan, "Dieogen To Kill"));
    //     simulation.GeneConditions.Add(3,
    //         new CellTypeCondition(3, not: false, simulation.CellTypes[0], "Is Stem Cell Type"));
    //     simulation.GeneConditions.Add(4,
    //         new ConcentrationCondition(4, not: false, 2, 10f, GeneCondition.ComparisonType.GreaterThan, "Has Stem"));
    //     simulation.GeneConditions.Add(5,
    //         new ConcentrationCondition(5, not: false, 1, 30f, GeneCondition.ComparisonType.LessThan, "Dieogen Generator"));
    //     simulation.GeneConditions.Add(6,
    //         new NeighbourCondition(6, not: false, 6, GeneCondition.ComparisonType.LessThan, "Edgy"));


    //     simulation.Genes.Add(0, new Gene(
    //         id: 0,
    //         name: "MulGEN",
    //         actions: new List<GeneAction> { simulation.GeneActions[0] },
    //         activatorConditions: new List<GeneCondition> { simulation.GeneConditions[0] },
    //         inhibitorConditions: new List<GeneCondition> { simulation.GeneConditions[1] }
    //     ));

    //     simulation.Genes.Add(1, new Gene(
    //         id: 1,
    //         name: "KilGEN",
    //         actions: new List<GeneAction> { simulation.GeneActions[1] },
    //         activatorConditions: new List<GeneCondition> { simulation.GeneConditions[2] },
    //         inhibitorConditions: new List<GeneCondition> { /*simulation.GeneConditions[1]*/ }
    //     ));

    //     simulation.Genes.Add(2, new Gene(
    //         id: 2,
    //         name: "DieogenGENErator",
    //         actions: new List<GeneAction> { simulation.GeneActions[3] },
    //         activatorConditions: new List<GeneCondition> { simulation.GeneConditions[6], simulation.GeneConditions[5] },
    //         inhibitorConditions: new List<GeneCondition> { /*simulation.GeneConditions[1]*/ }
    //     ));

    //     simulation.Genes.Add(3, new Gene(
    //         id: 3,
    //         name: "StemGEN",
    //         actions: new List<GeneAction> { simulation.GeneActions[2] },
    //         activatorConditions: new List<GeneCondition> { simulation.GeneConditions[4] },
    //         inhibitorConditions: new List<GeneCondition> { /*simulation.GeneConditions[1]*/ }
    //     ));

    //     simulation.diffusionSteps = 3;
    //     simulation.Diffuser.diffusionFactor = 1f;
    //     simulation.Diffuser.diffusionThreshold = 0.1f;

    //     return simulation;
    // }

    public List<HexCoords> GetCoordsOfRadius(int radius, HexCoords offset)
    {
        List<HexCoords> coords = new();
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    coords.Add(offset + new HexCoords(q, r));
                }
            }
        }
        return coords;
    }

    public void GenerateCellGrid(int radius, HexCoords offset, Vector3 palette)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    AddCell(offset + new HexCoords(q, r), palette);
                }
            }
        }
    }

    public void GenerateCellGrid(int radius, HexCoords offset, Cell palette)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    AddCell(offset + new HexCoords(q, r), palette);
                }
            }
        }
    }

    public void RemoveCellGrid(int radius, HexCoords offset)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    RemoveCell(offset + new HexCoords(q, r));
                }
            }
        }
    }

    public void AddCell(HexCoords coords, Vector3 palette)
    {
        palette *= Simulation.maxConcentration;
        if (Simulation.Cells.ContainsKey(coords))
        {
            var theCell = Simulation.Cells[coords];
            if (redMorphogenId >= 0) theCell.SetMorphogen(redMorphogenId, palette.X);
            if (greenMorphogenId >= 0) theCell.SetMorphogen(greenMorphogenId, palette.Y);
            if (blueMorphogenId >= 0) theCell.SetMorphogen(blueMorphogenId, palette.Z);
            return;
        }

        var cell = new Cell(Simulation);

        if (redMorphogenId >= 0) cell.SetMorphogen(redMorphogenId, palette.X);
        if (greenMorphogenId >= 0) cell.SetMorphogen(greenMorphogenId, palette.Y);
        if (blueMorphogenId >= 0) cell.SetMorphogen(blueMorphogenId, palette.Z);

        Simulation.Cells.Add(coords, cell);
    }

    public void AddCell(HexCoords coords, Cell cell)
    {
        if (Simulation.Cells.ContainsKey(coords))
        {
            Simulation.Cells[coords] = new(cell);
            return;
        }
        Simulation.AddCell(coords, cell);
    }

    public void RemoveCell(HexCoords coords)
    {
        if (!Simulation.Cells.ContainsKey(coords))
        {
            return;
        }

        Simulation.Cells.Remove(coords);
    }

    public void DetachMorphogen(int morphogenId)
    {
        foreach((Cell cell, var _) in cellPalette)
        {
            cell.Morphogens.Remove(morphogenId);
        }

        var defaultMorphogen = Simulation.Morphogens.Keys.ToList()[0];
        
        redMorphogenId = redMorphogenId == morphogenId ? defaultMorphogen : redMorphogenId;
        blueMorphogenId = blueMorphogenId == morphogenId ? defaultMorphogen : blueMorphogenId;
        greenMorphogenId = greenMorphogenId == morphogenId ? defaultMorphogen : greenMorphogenId;

        singleMorphogenId = singleMorphogenId == morphogenId ? defaultMorphogen : singleMorphogenId;


    }
    
    public void DetachCellType(int cellTypeId)
    {
        foreach((Cell cell, var _) in cellPalette)
        {
            if(cell.cellType.id == cellTypeId) cell.cellType = cell.simulation.CellTypes.Values.ToList()[0];
        }

        cellTypeColors.Remove(cellTypeId);
    }

    public void DetachGeneCondition(int geneConditionId)
    {
        this.geneConditionId = this.geneConditionId == geneConditionId ? Simulation.GeneConditions.Keys.ToList()[0] : geneConditionId;
    }

    public void DetachGene(int geneId)
    {
        this.geneId = this.geneId == geneId ? Simulation.Genes.Keys.ToList()[0] : geneId;
    }

    public void ClearGrid()
    {
        // foreach (var cellPair in Simulation.cells)
        // {
        //     var coords = cellPair.Key;
        //     var theCell = Simulation.cells[coords];
        //     theCell.Morphogens.Clear();
        // }
        Simulation.Cells.Clear();
    }

    public Color GetCellColor(Cell cell, float a = 1f)
    {
        switch (visualizationType)
        {
            case VisualizationType.ThreeMorphogens:
                {
                    float max = Simulation.maxConcentration;

                    float r = redMorphogenId >= 0 ? cell.GetMorphogenAmount(redMorphogenId) : 0;
                    float g = greenMorphogenId >= 0 ? cell.GetMorphogenAmount(greenMorphogenId) : 0;
                    float b = blueMorphogenId >= 0 ? cell.GetMorphogenAmount(blueMorphogenId) : 0;

                    var morphoColor = new Color(r / max, g / max, b / max, a);
                    return morphoColor;
                }
            case VisualizationType.SingleMorphogen:
                {
                    var factor = amplifier / Simulation.maxConcentration;
                    float w = singleMorphogenId >= 0 ? cell.GetMorphogenAmount(singleMorphogenId) : 0f;
                    w *= factor;
                    var morphoColor = new Color(w, w, w, a);
                    return morphoColor;
                }
            case VisualizationType.CellTypes:
                {
                    var cellColor = cellTypeColors.GetValueOrDefault(cell.cellType.id, Color.Black);
                    cellColor.A = (byte)(a * 255);
                    return cellColor;
                }
            case VisualizationType.GeneActivity:
                {
                    var cellColor = Simulation.GetGene(geneId).ShouldBeActive(cell) ? activeGeneColor : inactiveGeneColor;
                    cellColor.A = (byte)(a * 255);
                    return cellColor;
                }
            case VisualizationType.GeneConditionMet:
                {
                    var cellColor = Simulation.GeneConditions[geneConditionId].IsMet(cell) ? metConditionColor : notMetConditionColor;
                    cellColor.A = (byte)(a * 255);
                    return cellColor;
                }
            default:
                return Color.Black;
        }
    }
    
    public enum VisualizationType
    {
        ThreeMorphogens,
        SingleMorphogen,
        CellTypes,
        GeneActivity,
        GeneConditionMet
    }
}