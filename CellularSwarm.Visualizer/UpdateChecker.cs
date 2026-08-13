using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CellularSwarm.Visualizer;

public class UpdateChecker
{
    public static CheckStatus CurrentStatus = CheckStatus.NotChecked;
    public static string? Version => version;
    private static readonly HttpClient client = new HttpClient();
    private static string? version = null;
    private static readonly string url = "http://localhost:5173/cellular-swarm.version";

    public static async Task CheckForUpdatesAsync()
    {
        if(CurrentStatus != CheckStatus.NotChecked) return;
        CurrentStatus = CheckStatus.Checking;
        try
        {
            DebugConsole.Info($"Checking for updates at [{url}].", "NETWORK");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("CellularSwarm-UpdateChecker");
            string versionFetched = await client.GetStringAsync(url);
            version = versionFetched;
            DebugConsole.Info($"Fetched response: {versionFetched.Replace("\n", " \\n ")}", "NETWORK");
            if(versionFetched.StartsWith("cellular-swarm.version\n"))
            {
                versionFetched = versionFetched.Split('\n')[1];
            } else
            {
                throw new Exception($"Invalid version format fetched.");
            }
            // Thread.Sleep(10000);
            // throw new Exception("Zuhaahaaa eror");
            DebugConsole.Info($"Fetched App version [{versionFetched}].", "NETWORK");
            CurrentStatus = CheckStatus.Checked;
            version = versionFetched;
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