using System;
using CellularSwarm.Core;
using CellularSwarm.Core.Data;

namespace CellularSwarm.Visualizer;

public class SaveLoadHandler
{
    public string folder;
    public bool badLoad = false;
    public SaveLoadHandler()
    {
        folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
        folder = Directory.GetParent(folder)!.FullName;
        folder = Directory.GetParent(folder)!.FullName;
        folder = Path.Combine(folder, "Simulations");
    }
    public void SaveSimulation(Simulation simulation, string name = "")
    {
        var serialized = Serializer.Serialize(SimulationData.FromSimulation(simulation));

        if (name == "") { File.WriteAllText(Path.Combine(folder, $"{simulation.name}.json"), serialized); return; }
        else { File.WriteAllText(Path.Combine(folder, $"{name}.json"), serialized); return; }
    }
    public Simulation LoadSimulation(string name)
    {
        try
        {
            var deserialized = Serializer.Deserialize(File.ReadAllText(Path.Combine(folder, $"{name}.json")));
            var simulation = SimulationData.ToSimulation(deserialized);
            badLoad = false;
            return simulation;
        }
        catch
        {
            badLoad = true;
            return new Simulation(0, "default-error");
        }
    }
}
