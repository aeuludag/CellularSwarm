using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace CellularSwarm.Visualizer;

public class DebugConsole
{
    public static readonly DebugConsole Instance = new();

    public static List<Message> Lines
    {
        get => Instance.lines;
    }
    public SimulationRenderer Renderer = new();

    private List<Message> lines = new();

    private Dictionary<string, Command> commands;
    private char prefix = '!';

    public DebugConsole()
    {
        commands = new();

        var clear = new Command("clear", ["clear", "clr"]);
        var setprefix = new Command("setprefix", ["setprefix", "prefix"], 2);
        var simclear = new Command("simclear", ["simclear", "simclr"]);
        var help = new Command("help", ["help"]);

        clear.CommandAction = args =>
        {
            DebugConsole.Instance.lines.Clear();
        };

        setprefix.CommandAction = args =>
        {
            var newPrefix = args[1][0];
            DebugConsole.Instance.prefix = newPrefix;
            DebugConsole.Info($"New prefix set to [{prefix}].", "CONSOLE");
        };

        simclear.CommandAction = args =>
        {
            Renderer.ClearGrid();
            DebugConsole.Info("Clear simulation grid.", "CONSOLE");
        };

        help.CommandAction = args =>
        {
            Info(
    $@"Here are the available commands:
{prefix}help: This message.
{prefix}setprefix <prefix>: Set prefix.
{prefix}clear: Clear the console.
{prefix}simclear: Clear the simulation grid.",
    "CONSOLE");
        };

        Command[] commandsArray = [clear, setprefix, help, simclear];

        foreach (Command command in commandsArray)
        {
            foreach (string alias in command.alias)
            {
                commands.Add(alias, command);
            }
        }
    }

    public static void Send(string text)
    {
        if (text == string.Empty) { return; }

        Log(text, "USER");
        if (text[0] != Instance.prefix) return;

        string[] arguments = ArgumentisynthesizeBaby(text);
        string commandName = arguments[0];

        if (Instance.commands.TryGetValue(commandName, out Command? command))
        {
            try
            {
                if (command.argumentsCount != 1) { command?.Perform(arguments); }
                else { command?.Perform(); }
            }
            catch (Exception e)
            {
                Error($"Error while trying to perform command [{commandName}].", "CONSOLE");
                Error(e.Message, "CONSOLE");
            }
        }
        else
        {
            DebugConsole.Warning($"Could not recognize the command [{commandName}].", "CONSOLE");
        }
    }

    public static string[] ArgumentisynthesizeBaby(string text)
    {
        string[] arguments = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (arguments.Length == 0) return ["<none>"];

        if (arguments[0] == Instance.prefix.ToString()) { arguments = arguments[1..]; }
        if (arguments[0][0] == Instance.prefix) { arguments[0] = arguments[0][1..]; }

        return arguments;
    }

    public static void Log(string text, string sender)
    {
        Log(new Message(text, sender));
    }

    public static void Log(Vector4 color, string text, string sender)
    {
        Log(new Message(color, text, sender));
    }

    public static void Warning(string text, string sender)
    {
        Log(new Message(Message.Importance.Warning, text, sender));
    }
    
    public static void Error(string text, string sender)
    {
        Log(new Message(Message.Importance.Error, text, sender));
    }

    public static void Info(string text, string sender)
    {
        Log(new Message(Message.Importance.Info, text, sender));
    }

    public static string GetAllLines()
    {
        return Instance.GetAllLinesP();
    }

    private string GetAllLinesP()
    {
        StringBuilder sb = new();
        foreach (var line in lines)
        {
            sb.Append(line.ToString());
            sb.Append('\n');
        }
        return sb.ToString();
    }

    public static void Log(Message message)
    {
        if (!message.text.Contains('\n')) { Instance.lines.Add(message); return; }
        var allLines = message.text.Split('\n');
        foreach (var line in allLines)
        {
            Log(new Message(message.importance, message.color, message.date, line, message.sender));
        }
    }

    public class Message
    {
        public Vector4 color = DefaultColor;
        public Importance importance = Importance.Default;
        public DateTime date;
        public string text;
        public string sender;

        public static readonly Vector4 DefaultColor = new(0.9f, 0.9f, 0.95f, 1f);
        public static readonly Vector4 WarningColor = new(0.9f, 0.9f, 0.3f, 1f);
        public static readonly Vector4 ErrorColor = new(0.9f, 0.3f, 0.3f, 1f);
        public static readonly Vector4 InfoColor = new(0.6f, 0.7f, 1f, 1f);

        public Message(Importance importance, Vector4 color, DateTime date, string text, string sender)
        {
            this.importance = importance;
            this.color = color;
            this.date = date;
            this.text = text;
            this.sender = sender;
        }

        public Message(Vector4 color, DateTime date, string text, string sender)
        {
            this.color = color;
            this.date = date;
            this.text = text;
            this.sender = sender;
        }

        public Message(Vector4 color, DateTime date, string text)
        {
            this.color = color;
            this.date = date;
            this.text = text;
            this.sender = "CONSOLE";
        }

        public Message(DateTime date, string text)
        {
            this.date = date;
            this.text = text;
            this.sender = "CONSOLE";
        }

        public Message(string text)
        {
            this.date = DateTime.Now;
            this.text = text;
            this.sender = "CONSOLE";
        }
        public Message(string text, string sender)
        {
            this.date = DateTime.Now;
            this.text = text;
            this.sender = sender;
        }
        public Message(Vector4 color, string text, string sender)
        {
            this.date = DateTime.Now;
            this.text = text;
            this.sender = sender;
            this.color = color;
        }

        public Message(Importance importance, string text)
        {
            this.text = text;
            this.importance = importance;
            this.date = DateTime.Now;
            this.color = importance switch
            {
                Importance.Default => DefaultColor,
                Importance.Warning => WarningColor,
                Importance.Error => ErrorColor,
                Importance.Info => InfoColor,
                _ => DefaultColor
            };
            this.sender = "CONSOLE";
        }

        public Message(Importance importance, string text, string sender)
        {
            this.text = text;
            this.importance = importance;
            this.date = DateTime.Now;
            this.color = importance switch
            {
                Importance.Default => DefaultColor,
                Importance.Warning => WarningColor,
                Importance.Error => ErrorColor,
                Importance.Info => InfoColor,
                _ => DefaultColor
            };
            this.sender = sender;
        }

        public Message(Importance importance, Vector4 color, string text)
        {
            this.text = text;
            this.date = DateTime.Now;
            this.importance = importance;
            this.color = color;
            this.sender = "CONSOLE";
        }

        public Message(Importance importance, DateTime date, Vector4 color, string text)
        {
            this.text = text;
            this.date = date;
            this.importance = importance;
            this.color = color;
            this.sender = "CONSOLE";
        }

        public override string ToString()
        {
            string[] textMap = ["DEFAULT", "WARNING", " ERROR ", " INFO. "];
            return $"[{date:H:mm:ss:fff}] [{textMap[(int)importance]}] [{sender}] {text}";
        }

        public enum Importance
        {
            Default,
            Warning,
            Error,
            Info
        }
    }
}
