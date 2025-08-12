using Newtonsoft.Json;

namespace CellularSwarm.Core.Data;

public class Serializer
{
    public static string Serialize(SimulationData data)
    {
        return JsonConvert.SerializeObject(data);
    }
    public static SimulationData Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<SimulationData>(json) ?? new SimulationData();
    }
}
