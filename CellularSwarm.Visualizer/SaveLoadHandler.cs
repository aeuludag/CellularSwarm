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
        folder = "/Users/aeuludag/Documents/Simulations";
    }
    public void SaveSimulation(Simulation simulation, SimulationRenderer simulationRenderer, string name = "")
    {
        DebugConsole.Info($"Exporting simulation [{name}] to path: [{folder}]", "SIMULATION");

        var serializedSimulation = Serializer.Serialize(simulation);
        var serializedSimulationRenderer = VisualizationData.Serialize(simulationRenderer);

        if (name == "") { name = simulation.name; }

        File.WriteAllText(Path.Combine(folder, $"{name}.json"), serializedSimulation);
        File.WriteAllText(Path.Combine(folder, $"{name}.vis.json"), serializedSimulationRenderer);

        return;
    }
    public SimulationRenderer LoadSimulation(string name)
    {
        DebugConsole.Info($"Loading simulation [{name}] from path: [{folder}]", "SIMULATION");
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
            DebugConsole.Error($"Error while loading simulation [{name}]. Raw exception message below.", "SIMULATION");
            DebugConsole.Error(e.Message, "SIMULATION");
            DebugConsole.Warning("Loading a default simulation.", "SIMULATION");
            var simulation = new Simulation(0, "default-error");
            badLoad = true;
            return new SimulationRenderer(simulation);
        }
    }
}
