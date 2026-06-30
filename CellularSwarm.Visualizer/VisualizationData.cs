using CellularSwarm.Core;
using Raylib_cs;
using CellularSwarm.Core.Data;
using Newtonsoft.Json;

namespace CellularSwarm.Visualizer;

public class VisualizationData
{
    public int visualizationType = 0;
    public Dictionary<int, Color> cellTypeColors = new();
    public List<(CellData cell, string name)> cellPalette = new();
    public int redMorphogenId = -1;
    public int greenMorphogenId = -1;
    public int blueMorphogenId = -1;
    public int singleMorphogenId = -1;
    public float amplifier = 1f;
    public int geneId = -1;
    public Color activeGeneColor = new Color(0.75f, 0.3f, 0.3f, 1f);
    public Color inactiveGeneColor = new Color(0.2f, 0.5f, 0.75f, 1f);
    public int geneConditionId = -1;
    public Color metConditionColor = new Color(0.3f, 0.8f, 0.25f, 1f);
    public Color notMetConditionColor = new Color(0.7f, 0.25f, 0.25f, 1f);

    public static VisualizationData FromSimulationRenderer(SimulationRenderer renderer)
    {
        var data = new VisualizationData
        {
            visualizationType = (int)renderer.visualizationType,
            cellTypeColors = renderer.cellTypeColors,
            cellPalette = renderer.cellPalette.Select(c => (CellData.FromCell(c.cell), c.name)).ToList(),
            redMorphogenId = renderer.redMorphogenId,
            greenMorphogenId = renderer.greenMorphogenId,
            blueMorphogenId = renderer.blueMorphogenId,
            singleMorphogenId = renderer.singleMorphogenId,
            amplifier = renderer.amplifier,
            geneId = renderer.geneId,
            activeGeneColor = renderer.activeGeneColor,
            inactiveGeneColor = renderer.inactiveGeneColor,
            geneConditionId = renderer.geneConditionId,
            metConditionColor = renderer.metConditionColor,
            notMetConditionColor = renderer.notMetConditionColor,
        };

        return data;
    }

    public static SimulationRenderer ToSimulationRenderer(VisualizationData data, Simulation simulation)
    {
        var renderer = new SimulationRenderer(simulation)
        {
            visualizationType = (SimulationRenderer.VisualizationType)data.visualizationType,
            redMorphogenId = data.redMorphogenId,
            greenMorphogenId = data.greenMorphogenId,
            blueMorphogenId = data.blueMorphogenId,
            singleMorphogenId = data.singleMorphogenId,
            amplifier = data.amplifier,
            geneId = data.geneId,
            activeGeneColor = data.activeGeneColor,
            inactiveGeneColor = data.inactiveGeneColor,
            geneConditionId = data.geneConditionId,
            metConditionColor = data.metConditionColor,
            notMetConditionColor = data.notMetConditionColor,
            cellTypeColors = data.cellTypeColors,
            cellPalette = data.cellPalette.Select(c => (CellData.ToCell(simulation, c.cell), c.name)).ToList()
        };

        return renderer;
    }

    public static string Serialize(SimulationRenderer data)
    {
        return JsonConvert.SerializeObject(FromSimulationRenderer(data));
    }

    public static SimulationRenderer Deserialize(string data, Simulation simulation)
    {
        var visualizationData = JsonConvert.DeserializeObject<VisualizationData>(data) ?? new VisualizationData();
        return ToSimulationRenderer(visualizationData, simulation);
    }
}
