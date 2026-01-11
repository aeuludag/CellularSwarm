using System;

namespace CellularSwarm.Visualizer;


public class Command
{
    public string id;
    public string[] alias;
    public int argumentsCount; // including the command itself

    public Action<string[]>? CommandAction;
    public Command(string id, string[] alias, int argumentsCount = 1)
    {
        this.id = id;
        this.alias = alias;
        this.argumentsCount = argumentsCount;
    }

    public void Perform(string[] arguments)
    {
        if (argumentsCount == 1 && arguments.Length != 1) { DebugConsole.Warning("No need to give arguments for this command.", "CONSOLE"); }
        if (argumentsCount != arguments.Length) { DebugConsole.Error($"Expected {argumentsCount - 1} argument(s), {arguments.Length - 1} given.", "CONSOLE"); return; }

        CommandAction?.Invoke(arguments);
    }
    public void Perform()
    {
        if (argumentsCount != 1)
        {
            DebugConsole.Error($"Expected {argumentsCount - 1} argument(s), none given.", "CONSOLE");
            return;
        }
        CommandAction?.Invoke([]);
    }
}