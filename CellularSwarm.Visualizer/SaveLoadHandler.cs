using System;
using System.Diagnostics;
using CellularSwarm.Core;
using CellularSwarm.Core.Data;
using Newtonsoft.Json;

namespace CellularSwarm.Visualizer;

public class SaveLoadHandler
{
    public static SaveLoadHandler Instance = new();
    public static bool BadLoad
    {
        get => Instance.badLoad;
        set => Instance.badLoad = value;
    }
    public static string LoadedCoreVersion
    {
        get => Instance.loadedCoreVersion;
        set => Instance.loadedCoreVersion = value;
    }
    public static string LoadedRendererVersion
    {
        get => Instance.loadedRendererVersion;
        set => Instance.loadedRendererVersion = value;
    }
    public static string LoadedRendererName
    {
        get => Instance.loadedRendererName;
        set => Instance.loadedRendererName = value;
    }
    public static DateTime LastSaveTime
    {
        get => Instance.lastSaveTime;
        set => Instance.lastSaveTime = value;
    }
    private bool badLoad;
    private string loadedCoreVersion = Simulation.VERSION;
    private string loadedRendererVersion = SimulationRenderer.VERSION;
    private string loadedRendererName = SimulationRenderer.NAME;
    private DateTime lastSaveTime = DateTime.MinValue;
    public SaveLoadHandler()
    {
        var location = ConfigHandler.Config.simulationsPath;
        DebugConsole.Info($"Simulation locations are configured to [{location}].", "SAVELOAD");
        if (Path.Exists(location))
        {
            DebugConsole.Info("Path exists.", "SAVELOAD");
        }
        else
        {
            DebugConsole.Warning("Path does not exist. Trying to create one...", "SAVELOAD");
            Directory.CreateDirectory(location);
        }
    }

    public static void SaveSimulationToSimulationsFolder(Simulation simulation, SimulationRenderer simulationRenderer, string name = "")
    {
        SaveSimulation(simulation, simulationRenderer, Path.Combine(ConfigHandler.Config.simulationsPath, $"{name}.csim"));
    }

    public static SimulationRenderer LoadSimulationFromSimulationsFolder(string name)
    {
        return LoadSimulation(Path.Combine(ConfigHandler.Config.simulationsPath, $"{name}.csim"));
    }

    public static void SaveSimulation(Simulation simulation, SimulationRenderer simulationRenderer, string path = "")
    {
        DebugConsole.Info($"Exporting simulation to path: [{path}].", "SAVELOAD");

        // var serializedSimulation = Serializer.Serialize(simulation);
        // var serializedSimulationRenderer = VisualizationData.Serialize(simulationRenderer);

        var serializedContainedSimulation = ContainedSimulationData.Serialize(ContainedSimulationData.FromContainedSimulation(new ContainedSimulation(simulation, simulationRenderer)));

        if (path == "") { path = ConfigHandler.Config.simulationsPath; }

        File.WriteAllText(path, serializedContainedSimulation);

        Instance.lastSaveTime = DateTime.Now;

        // File.WriteAllText(Path.Combine(folder, $"{name}.json"), serializedSimulation);
        // File.WriteAllText(Path.Combine(folder, $"{name}.vis.json"), serializedSimulationRenderer);

        return;
    }
    public static SimulationRenderer LoadSimulation(string path)
    {
        DebugConsole.Info($"Loading simulation from path [{path}].", "SAVELOAD");
        Instance.lastSaveTime = DateTime.MinValue;
        try
        {
            var containedSimulationData = ContainedSimulationData.Deserialize(File.ReadAllText(path));

            DebugConsole.Info($"Loaded simulation version [{containedSimulationData.simulationData.version}].", "SAVELOAD");
            DebugConsole.Info($"Loaded renderer [{containedSimulationData.visualizationData.renderer}].", "SAVELOAD");
            DebugConsole.Info($"Loaded renderer version [{containedSimulationData.visualizationData.version}].", "SAVELOAD");

            var simulation = SimulationData.ToSimulation(containedSimulationData.simulationData);
            var simulationRenderer = VisualizationData.ToSimulationRenderer(containedSimulationData.visualizationData, simulation);
            
            Instance.loadedCoreVersion = containedSimulationData.simulationData.version;
            Instance.loadedRendererVersion = containedSimulationData.visualizationData.version;
            Instance.loadedRendererName = containedSimulationData.visualizationData.renderer;
            Instance.badLoad = false;

            return simulationRenderer;
        }
        catch (Exception e)
        {
            DebugConsole.Error($"Error while loading simulation from [{path}].", "SAVELOAD");
            DebugConsole.Error(e.Message, "SAVELOAD");
            DebugConsole.Warning("Loading a default simulation...", "SAVELOAD");
            var simulation = new Simulation(0, "default-error");
            Instance.loadedCoreVersion = Simulation.VERSION;
            Instance.badLoad = true;
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