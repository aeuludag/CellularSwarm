using System;
using System.ComponentModel;
using System.Diagnostics;
using Newtonsoft.Json;

namespace CellularSwarm.Visualizer;

public class ConfigHandler
{
    public static ConfigHandler Instance = new();

    public string location;
    public Config config = new();
    public static Config Config { get => Instance.config; }

    public ConfigHandler()
    {
        location = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        location = Path.Combine(location, "aeuludag", "cellular-swarm");
    
        DebugConsole.Info($"Config location configured to [{location}].", "CONFIG");

        if (Path.Exists(location))
        {
            DebugConsole.Info("Path exists.", "CONFIG");
        }
        else
        {
            DebugConsole.Warning("Path does not exist. Trying to create one.", "CONFIG");
            Directory.CreateDirectory(location);
        }
    }

    public static void SaveConfig()
    {
        var serialized = JsonConvert.SerializeObject(Instance.config);

        try
        {
            File.WriteAllText(Path.Combine(Instance.location, "config.json"), serialized);
            DebugConsole.Info("Succesfully saved config.", "CONFIG");
        }
        catch (Exception e)
        {
            DebugConsole.Error($"Error while trying to save config file to [{Instance.location}].", "CONFIG");
            DebugConsole.Error(e.Message, "CONFIG");
        }
    }

    public static void LoadConfig()
    {
        if(!File.Exists(Path.Combine(Instance.location, "config.json"))) Instance.config = new Config();

        try
        {
            var text = File.ReadAllText(Path.Combine(Instance.location, "config.json"));
            DebugConsole.Info(text, "CONFIG");
            Config deserialized = JsonConvert.DeserializeObject<Config>(text) ?? new Config();
            Instance.config = deserialized;
            if(deserialized.simulationsPath == string.Empty) { deserialized.simulationsPath = new Config().simulationsPath; }
            DebugConsole.Info("Successfully loaded config.", "CONFIG");
        }
        catch (Exception e)
        {
            DebugConsole.Error($"Error while loading config.", "CONFIG");
            DebugConsole.Error(e.Message, "CONFIG");
            DebugConsole.Warning("Loading default config.", "CONFIG");
            Instance.config = new Config();
        }

    }

    public static void ResetConfig()
    {
        Instance.config = new Config();
        DebugConsole.Info("Config reset.", "CONFIG");
    }
}
