using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CellularSwarm.Visualizer;

public class UpdateChecker
{
    public static CheckStatus CurrentStatus = CheckStatus.NotChecked;
    public static string? Version { get; private set; } = null;
    public static string? Platforms { get; private set; } = null;
    public static string? Title { get; private set; } = null;
    public static string? Description { get; private set; } = null;
    private static readonly HttpClient client = new HttpClient();
    private static readonly string url = "http://aeuludag.github.io/cellular-swarm.version";

    public static async Task CheckForUpdatesAsync()
    {
        if(CurrentStatus != CheckStatus.NotChecked) return;
        CurrentStatus = CheckStatus.Checking;
        try
        {
            DebugConsole.Info($"Checking for updates at [{url}].", "NETWORK");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("CellularSwarm-UpdateChecker");
            string versionInfoFetched = await client.GetStringAsync(url);
            DebugConsole.Info($"Fetched response [{versionInfoFetched.Replace("\n", " \\n ")}].", "NETWORK");
            if(versionInfoFetched.StartsWith("cellular-swarm.version\n"))
            {
                Version = versionInfoFetched.Split('\n')[1];
                Platforms = versionInfoFetched.Split('\n')[2];
                Title = versionInfoFetched.Split('\n')[3];
                Description = string.Join("\n", versionInfoFetched.Split('\n')[4..]);
            } else
            {
                throw new Exception($"Invalid version format fetched.");
            }
            // Thread.Sleep(10000);
            // throw new Exception("Zuhaahaaa eror");
            DebugConsole.Info($"Fetched App version [{Version}].", "NETWORK");
            CurrentStatus = CheckStatus.Checked;
        }
        catch (Exception e)
        {
            CurrentStatus = CheckStatus.Error;
            DebugConsole.Error("Error while checking for updates.", "NETWORK");
            DebugConsole.Error(e.Message, "NETWORK");
        }
    }

    public enum CheckStatus
    {
        NotChecked,
        Checking,
        Checked,
        Error
    }
}