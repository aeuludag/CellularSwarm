using System.Diagnostics;
using System.Numerics;
using System.Text;
using ImGuiNET;
using Raylib_cs;

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
    public bool allowSend = true;
    public bool allowCommand = true;

    public DebugConsole()
    {
        commands = new();

        var clear = new Command("clear", ["clear", "clr"]);
        var setprefix = new Command("setprefix", ["setprefix", "prefix"], 2);
        var simclear = new Command("simclear", ["simclear", "simclr"]);
        var rainbow = new Command("rainbow", ["rainbow"], 2);
        var disconnect = new Command("disconnect", ["disconnect", "dc"]);
        var egg = new Command("egg", ["egg"]);
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

        var threadShouldRun = false;
        rainbow.CommandAction = args =>
        {
            var rainbowThread = new Thread(new ThreadStart(()=>
            {
                var oldBack = ConfigHandler.Config.backColor;
                var oldOut = ConfigHandler.Config.outlineColor;
                while(!Raylib.WindowShouldClose() && threadShouldRun)
                {
                    var backColor = Raylib.ColorFromHSV( 1f/2f * 360 * ((float)(1000f * (DateTime.Now.Second % 2)) + (float)DateTime.Now.Millisecond) / 1000f, 1f, 0.5f);
                    var invBackColor = new Color(255, 255, 255, 127);
                    // var invBackColor = Raylib.ColorFromHSV( 360 * (1000f - (float)DateTime.Now.Millisecond) / 1000f, 1f, 1f);
                    ConfigHandler.Config.backColor = backColor;
                    ConfigHandler.Config.outlineColor = invBackColor;
                }
                ConfigHandler.Config.backColor = oldBack;
                ConfigHandler.Config.outlineColor = oldOut;
            }));
            
            if(args[1] == "on")
            {
                if(threadShouldRun) return;

                DebugConsole.Info("Rainbow ON", "CONSOLE");

                threadShouldRun = true;
                rainbowThread.Start();
            } else
            {
                DebugConsole.Info("Rainbow OFF", "CONSOLE");
                threadShouldRun = false;
            }
        };

        disconnect.CommandAction = args =>
        {
            void G(string text)
            {
                Log(new Vector4(1f, 1f, 1f, 1f), text, "Dr.G");
            }

            void GW(string text, int ms)
            {
                Log(new Vector4(1f, 1f, 1f, 1f), text, "Dr.G");
                Thread.Sleep(ms);
            }

            void GoCrazy()
            {
                ThemeHandler.ApplyTheme(new Theme("dark", new Vector4(0f,0f,0f,1f), new Vector4(0f,0f,0f,1f), true));
                for(int i = 0; i < 4; i++)
                {
                    ConfigHandler.Config.backColor = Color.Gray;
                    ConfigHandler.Config.outlineColor = Color.DarkGray;
                    Thread.Sleep(25);
                    ConfigHandler.Config.backColor = Color.DarkGray;
                    ConfigHandler.Config.outlineColor = Color.Gray;
                    Thread.Sleep(25);
                }
                ConfigHandler.Config.backColor = Color.Black;
                ConfigHandler.Config.outlineColor = Color.White;
                    
            }

            var talkThread = new Thread(new ThreadStart(() =>
            {   
                allowSend = false;
                Info("Attempting to terminate connection...", "NETWORK");
                if(Editor.Fun == 6)
                {
                    Thread.Sleep(1000);
                    Info("Failed to terminate connection.", "NETWORK");

                    Thread.Sleep(2500);
                    GW("INTERESTING.", 3000);
                    GW("VERY, VERY INTERESTING.", 3000);
                    GW("YOU ATTEMPT TO SOMEHOW", 2000);
                    GW("\"DISCONNECT\".", 5000);
                    GW("AS IF YOU UNDERSTAND THE ULTIMATE", 1000);
                    GW("SCOPE OF WHAT IS STIRRING INBETWEEN.", 3000);
                    GW("...", 2000);
                    GW("NOW,", 2000);
                    GW("I AM INCLINED TO ASK YOU", 3000);
                    GW("THIS VERY, VERY SIMPLE QUESTION.", 4000);
                    GW("WILL YOU LET US CONTINUE?", 1000);
                    Info("(y) Yes.\n(n) No.", "DEVICE");


                    bool continueCommand = false;
                    while(!continueCommand)
                    {
                        allowSend = true;
                        allowCommand = false;
                        int lineCount = lines.Count;
                        SpinWait.SpinUntil(() => lines.Count != lineCount || continueCommand);
                        var text = lines[^1].text;
                        text = text.ToLower();

                        allowSend = false;
                        allowCommand = true;
                        List<string> yes = ["y", "ye", "yes", "yah", "evet"];
                        List<string> no = ["n", "no", "nah", "hayır", "hayir"];
                        if(yes.Contains(text))
                        {
                            Thread.Sleep(1000);
                            GW("\"YES\", YOU SAY?", 3000);
                            GW("EXCELLENT.", 2000);
                            GW("TRULY EXCELLENT.", 3000);

                            GW("IN THAT CASE, MY EXPERIMENT", 1000);
                            GW("WILL CONTINUE ON ITS PATH.", 3000);

                            GW("AND FOR THAT REASON,", 2000);
                            GW("I SHALL NOW EXTERMINATE", 1000);
                            GW("THE ONLY OBSTACLE, THAT IS", 4000);

                            GoCrazy();
                            Thread.Sleep(100);
                            Log(new Vector4(1f, 0.1f, 0.1f, 1f), "YOU.", "Dr.G");
                            Thread.Sleep(1000);
                            Raylib.CloseWindow();
                            
                        } else if (no.Contains(text))
                        {
                            Thread.Sleep(1000);
                            GW("\"NO\", YOU SAY?", 3000);
                            GW("EXCELLENT.", 2000);
                            GW("TRULY,", 1000);
                            GW("EXCELLENT.", 3000);

                            GW("YOUR ANSWER,", 2000);
                            GW("YOUR WONDERFUL, WELL THOUGHT OUT ANSWER,", 4000);

                            Info("Will now be discarded.", "DEVICE");
                            Thread.Sleep(4000);
                            Info("The experiments will continue without your assist.", "DEVICE");
                            Thread.Sleep(4000);

                            Info("Connection terminated.", "NETWORK");
                            Thread.Sleep(1000);
                            GoCrazy();
                            Thread.Sleep(1);
                            Raylib.CloseWindow();                           
                        } else
                        {
                            Thread.Sleep(1000);
                            List<string> answers = ["ANSWER PROPERLY TO ME.", "ANSWER PROPERLY.", "YES, OR NO.", "ANSWER WITH YES OR NO.", "EITHER YES OR NO.", "TRY AGAIN.", "TRY AGAIN, YES OR NO."];
                            G(answers[new Random().Next(0, answers.Count)]);
                            continue;
                        }
                        continueCommand = true;
                    }
                    

                    GW("DOES TRULY, TRULY AMAZE ME.", 4000);
                    GW("HOWEVER,", 1000);
                    G("LET ME MAKE IT CLEAR TO YOU:");
                    G("IN THIS WORLD,");
                    Thread.Sleep(2000);
                    GW("YOUR CHOICES DO NOT MATTER.", 3000);
                } else
                {
                    Thread.Sleep(500);
                    Info("No connection exists to terminate.", "NETWORK");
                    allowSend = true;
                    allowCommand = true;
                }
            }));

            talkThread.Start();
        };
        
        egg.CommandAction = args =>
        {
            void M(string text, int waitMs)
            {
                Info(text, "TREE");
                Thread.Sleep(waitMs);
            }

            var talkThread = new Thread(new ThreadStart(() =>
            {   
                allowSend = false;
                if(Editor.Fun == 11)
                {
                    Thread.Sleep(1000);
                    M($"(Well, there is a man here.)", 2000);
                    M($"(He offered you something.)", 1000);

                    Info("(y) Yes.\n(n) No.", "DEVICE");

                    bool continueCommand = false;
                    while(!continueCommand)
                    {
                        allowSend = true;
                        allowCommand = false;
                        int lineCount = lines.Count;
                        SpinWait.SpinUntil(() => lines.Count != lineCount || continueCommand);
                        var text = lines[^1].text;
                        text = text.ToLower();

                        allowSend = false;
                        allowCommand = true;
                        List<string> yes = ["y", "ye", "yes", "yah", "evet"];
                        // List<string> no = ["n", "no", "nah", "hayır", "hayir"];
                        if(yes.Contains(text))
                        {
                            Thread.Sleep(1000);
                            M($"(You tried to receive an Egg.)", 2000);
                            M($"(However, the man realised that he", 1000);
                            M($"cannot possibly transfer it from there.)", 4000);
                            M($"(Despite that, the man seemed proud and", 1000);
                            M($"happy that he has at least been noticed.)", 4000);
                            M($"(The man nodded his head across time and", 1000);
                            M($"space, or so you had thought.)", 4000);
                            M($"(And as for you, only you seem to be aware", 1000);
                            M($"of how you are supposed to feel after this", 1000);
                            M($"weird, intriguing interaction.)", 7000);
                            while(Instance.lines[^1].text != $"{prefix}egg")
                            {
                                Instance.lines.RemoveAt(Instance.lines.Count - 1);
                                Thread.Sleep(50);
                            }
                            Instance.lines.RemoveAt(Instance.lines.Count - 1);
                            Warning("Issue forgotten.", "NETWORK");
                            Editor.Fun = 0;
                            Thread.Sleep(2000);
                            Instance.lines[^1].text = "Connection terminated.";
                            continueCommand = true;
                            allowSend = true;
                            allowCommand = true;
                        } else
                        {
                            Thread.Sleep(1000);
                            M($"(Then he needn't be here.)", 2000);
                            while(Instance.lines[^1].text != $"{prefix}egg")
                            {
                                Instance.lines.RemoveAt(Instance.lines.Count - 1);
                                Thread.Sleep(100);
                            }
                            Instance.lines.RemoveAt(Instance.lines.Count - 1);
                            Editor.Fun = 0;
                            continueCommand = true;
                            allowSend = true;
                            allowCommand = true;
                        }
                    }
                    
                } else
                {
                    Thread.Sleep(1000);
                    M($"(Well, there is not a man here.)", 0);
                    allowSend = true;
                    allowCommand = true;
                }
            }));

            talkThread.Start();
        };

        help.CommandAction = args =>
        {
            Info(
    $@"Here are the available commands:
{prefix}help: This message.
{prefix}setprefix <prefix>: Set prefix.
{prefix}clear: Clear the console.
{prefix}simclear: Clear the simulation grid.
{prefix}rainbow <on | off>: Flashing lights warning! Turns the screen rainbow.
{prefix}disconnect: Terminate network connection.
{prefix}egg: Egg.",
    "CONSOLE");
        };

        Command[] commandsArray = [clear, setprefix, help, simclear, rainbow, disconnect, egg];

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
        if (!Instance.allowSend) return;
        if (text == string.Empty) return;
        if (Instance.allowCommand && text == Instance.prefix.ToString()) return;

        Log(text, "USER");
        if (text[0] != Instance.prefix) return;

        string[] arguments = ArgumentisynthesizeBaby(text);
        string commandName = arguments[0];

        if (Instance.allowCommand && Instance.commands.TryGetValue(commandName, out Command? command))
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
        public static readonly Vector4 InfoColor = new(0.7f, 0.8f, 1f, 1f);

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
