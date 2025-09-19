using Newtonsoft.Json;

namespace CellularSwarm.Core.Data;

public class Serializer
{
    public static string Serialize(Simulation simulation)
    {
        return JsonConvert.SerializeObject(SimulationData.FromSimulation(simulation));
    }
    public static Simulation Deserialize(string json)
    {
        var data = JsonConvert.DeserializeObject<SimulationData>(json) ?? new SimulationData();
        return SimulationData.ToSimulation(data);
    }
}
