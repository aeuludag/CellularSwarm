using System;
using System.Numerics;
using CellularSwarm.Core;
using CellularSwarm.Visualizer;
using Raylib_cs;

public class SimulationRenderer
{
    public const string VERSION = "1.0.260825";
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