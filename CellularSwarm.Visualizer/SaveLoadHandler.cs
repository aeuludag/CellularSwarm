using System;
using System.Diagnostics;
using CellularSwarm.Core;
using CellularSwarm.Core.Data;
using Newtonsoft.Json;

namespace CellularSwarm.Visualizer;

public class SaveLoadHandler
{
    public string folder;
    public bool badLoad = false;
    public SaveLoadHandler()
    {
        // folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        // folder = Directory.GetParent(folder)!.FullName;
        // folder = Directory.GetParent(folder)!.FullName;
        // folder = Path.Combine(folder, "Simulations");
        DebugConsole.Info("Initializing save & load locations.", "IO");
        var location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        location = Path.Combine(location, "Simulations");
        DebugConsole.Info($"Generated path: [{location}]", "IO");
        if (Path.Exists(location))
        {
            DebugConsole.Info("Path exists.", "IO");
        }
        else
        {
            DebugConsole.Warning("Path does not exist. Trying to create one.", "IO");
            Directory.CreateDirectory(location);
        }
        folder = location;
    }
    public void SaveSimulation(Simulation simulation, SimulationRenderer simulationRenderer, string name = "")
    {
        DebugConsole.Info($"Exporting simulation [{name}] to path: [{folder}].", "IO");

        var serializedSimulation = Serializer.Serialize(simulation);
        var serializedSimulationRenderer = VisualizationData.Serialize(simulationRenderer);

        if (name == "") { name = simulation.name; }

        File.WriteAllText(Path.Combine(folder, $"{name}.json"), serializedSimulation);
        File.WriteAllText(Path.Combine(folder, $"{name}.vis.json"), serializedSimulationRenderer);

        return;
    }
    public SimulationRenderer LoadSimulation(string name)
    {
        DebugConsole.Info($"Loading simulation [{name}] from path: [{folder}]", "IO");
        try
        {
            var simulation = Serializer.Deserialize(File.ReadAllText(Path.Combine(folder, $"{name}.json")));
            SimulationRenderer simulationRenderer;
            if (File.Exists(Path.Combine(folder, $"{name}.vis.json")))
            {
                simulationRenderer = VisualizationData.Deserialize(File.ReadAllText(Path.Combine(folder, $"{name}.vis.json")), simulation);
            }
            else
            {
                simulationRenderer = new(simulation);
            }
            badLoad = false;
            return simulationRenderer;
        }
        catch (Exception e)
        {
            DebugConsole.Error($"Error while loading simulation [{name}].", "IO");
            DebugConsole.Error(e.Message, "IO");
            DebugConsole.Warning("Loading a default simulation.", "IO");
            var simulation = new Simulation(0, "default-error");
            badLoad = true;
            return new SimulationRenderer(simulation);
        }
    }
}
