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
        // folder = Directory.GetParent(folder)!.ContainedName;
        // folder = Directory.GetParent(folder)!.ContainedName;
        // folder = Path.Combine(folder, "Simulations");
        // var location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // location = Path.Combine(location, "Simulations");
        var location = ConfigHandler.Config.simulationsPath;
        DebugConsole.Info($"Simulation locations are configured to [{location}].", "SAVELOAD");
        if (Path.Exists(location))
        {
            DebugConsole.Info("Path exists.", "SAVELOAD");
        }
        else
        {
            DebugConsole.Warning("Path does not exist. Trying to create one.", "SAVELOAD");
            Directory.CreateDirectory(location);
        }
        folder = location;
    }
    public void SaveSimulation(Simulation simulation, SimulationRenderer simulationRenderer, string name = "")
    {
        DebugConsole.Info($"Exporting simulation [{name}] to path: [{folder}].", "SAVELOAD");

        // var serializedSimulation = Serializer.Serialize(simulation);
        // var serializedSimulationRenderer = VisualizationData.Serialize(simulationRenderer);

        var serializedContainedSimulation = ContainedSimulationData.Serialize(ContainedSimulationData.FromContainedSimulation(new ContainedSimulation(simulation, simulationRenderer)));

        if (name == "") { name = simulation.name; }

        File.WriteAllText(Path.Combine(folder, $"{name}.csim"), serializedContainedSimulation);

        // File.WriteAllText(Path.Combine(folder, $"{name}.json"), serializedSimulation);
        // File.WriteAllText(Path.Combine(folder, $"{name}.vis.json"), serializedSimulationRenderer);

        return;
    }
    public SimulationRenderer LoadSimulation(string name)
    {
        DebugConsole.Info($"Loading simulation [{name}] from path: [{folder}]", "IO");
        try
        {
            var containedSimulationData = ContainedSimulationData.Deserialize(File.ReadAllText(Path.Combine(folder, $"{name}.csim")));
            var simulation = SimulationData.ToSimulation(containedSimulationData.simulationData);
            var simulationRenderer = VisualizationData.ToSimulationRenderer(containedSimulationData.visualizationData, simulation);
            
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

public class ContainedSimulation
{
    public Simulation simulation;
    public SimulationRenderer simulationRenderer;

    public ContainedSimulation(Simulation simulation, SimulationRenderer simulationRenderer)
    {
        this.simulation = simulation;
        this.simulationRenderer = simulationRenderer;
    }
}

public class ContainedSimulationData
{
    public SimulationData simulationData;
    public VisualizationData visualizationData;

    public ContainedSimulationData(SimulationData simulationData, VisualizationData visualizationData)
    {
        this.simulationData = simulationData;
        this.visualizationData = visualizationData;
    }

    public static ContainedSimulationData FromContainedSimulation(ContainedSimulation containedSimulation)
    {
        return new ContainedSimulationData(SimulationData.FromSimulation(containedSimulation.simulation), VisualizationData.FromSimulationRenderer(containedSimulation.simulationRenderer));
    }

    public static ContainedSimulation ToContainedSimulation(ContainedSimulationData containedSimulationData)
    {
        var simulation = SimulationData.ToSimulation(containedSimulationData.simulationData);
        return new ContainedSimulation(simulation, VisualizationData.ToSimulationRenderer(containedSimulationData.visualizationData, simulation));
    }

    public static string Serialize(ContainedSimulationData containedSimulationData)
    {
        return JsonConvert.SerializeObject(containedSimulationData);
    }
    public static ContainedSimulationData Deserialize(string text)
    {
        return JsonConvert.DeserializeObject<ContainedSimulationData>(text) ?? new(new(), new());
    }
}