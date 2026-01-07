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

    private List<Message> lines = new();

    public static void Log(string text)
    {
        Instance.LogP(new Message(text));
    }
    public static void Warning(string text)
    {
        Instance.LogP(new Message(Message.Importance.Warning, text));
    }
    public static void Error(string text)
    {
        Instance.LogP(new Message(Message.Importance.Error, text));
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
        Instance.LogP(message);
    }

    private void LogP(Message message)
    {
        lines.Add(message);
    }

    public class Message
    {
        public Vector4 color = DefaultColor;
        public Importance importance = Importance.Default;
        public DateTime date;
        public string text;

        public static readonly Vector4 DefaultColor = new(0.9f, 0.9f, 0.95f, 1f);
        public static readonly Vector4 WarningColor = new(0.9f, 0.9f, 0.3f, 1f);
        public static readonly Vector4 ErrorColor = new(0.9f, 0.3f, 0.3f, 1f);

        public Message(Vector4 color, DateTime date, string text)
        {
            this.color = color;
            this.date = date;
            this.text = text;
        }

        public Message(DateTime date, string text)
        {
            this.date = date;
            this.text = text;
        }

        public Message(string text)
        {
            this.date = DateTime.Now;
            this.text = text;
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
                _ => DefaultColor
            };
        }
        
        public Message(Importance importance, Vector4 color, string text)
        {
            this.text = text;
            this.date = DateTime.Now;
            this.importance = importance;
            this.color = color;
        }

        public Message(Importance importance, DateTime date, Vector4 color, string text)
        {
            this.text = text;
            this.date = date;
            this.importance = importance;
            this.color = color;
        }

        public override string ToString()
        {
            return importance switch
            {
                Importance.Error => $"[{date:H:mm:ss:fff}] [ {importance} ] {text}",
                _ => $"[{date:H:mm:ss:fff}] [{importance}] {text}"
            };
        }

        public enum Importance
        {
            Default,
            Warning,
            Error
        }
    }
}
