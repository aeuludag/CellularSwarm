using System.Numerics;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public static class ThemeHandler
{
    public readonly static List<Theme> Themes = new()
    {
        ConfigHandler.Config.customDarkTheme,
        ConfigHandler.Config.customLightTheme,
        new Theme("Mint & Lemon Peel",          Hex("#23950FFF"), Hex("#FFDD00FF"), true),
        new Theme("Favourite Worst Nightmare",  Hex("#928702FF"), Hex("#928702FF"), true),
        new Theme("Humbug",                     Hex("#530D40FF"), Hex("#DCD6B8FF"), true),
        new Theme("Tranquility Base Hotel",     Hex("#724212FF"), Hex("#DA9C38FF"), true),
        new Theme("The Car",                    Hex("#D4CCC4FF"), Hex("#B6AA8CFF"), false),
        new Theme("Showbiz",                    Hex("#377EB5FF"), Hex("#76BBD3FF"), true),
        new Theme("Origin of Symmetry",         Hex("#D79141FF"), Hex("#78B4D2FF"), false),
        new Theme("Black Holes & Revelations",  Hex("#4663DDFF"), Hex("#AA603CFF"), true),
        new Theme("Simulation Theory",          Hex("#B9417DFF"), Hex("#6496E6FF"), true),
        new Theme("Currents",                   Hex("#573A78FF"), Hex("#D73C3CFF"), true),
        new Theme("Mario Circuit",              Hex("#FF8282FF"), Hex("#1A15BFFF"), false),
        new Theme("Luigi's Mansion",            Hex("#0A710AFF"), Hex("#1E6FE4FF"), true),
        new Theme("Wario Stadium",              Hex("#A314BCFF"), Hex("#F1D70EFF"), true),
        new Theme("Peach Beach",                Hex("#F67CD6FF"), Hex("#F9C622FF"), false),
    };

    public static Theme GetCurrentTheme()
    {
        return Themes[ConfigHandler.Config.themeIndex];
    }

    public static void ApplyMorph()
    {
        var style = ImGui.GetStyle();

        style.FrameRounding = 2.0f;
        style.WindowRounding = 4.0f;
        style.ScrollbarRounding = 3.0f;
        style.GrabRounding = 3.0f;
        style.TabRounding = 3.0f;

        style.WindowBorderSize = 1.0f;
        style.FrameBorderSize = 1.0f;
        style.ScrollbarSize = 14.0f;
        style.GrabMinSize = 10.0f;
    }

    public static void ApplyCurrentTheme()
    {
        Theme theme = GetCurrentTheme();
        ApplyCustom(theme.main, theme.accent, theme.dark);
    }
    public static void ApplyTheme(Theme theme)
    {
        ApplyCustom(theme.main, theme.accent, theme.dark);
    }

    private static void ApplyCustom(Vector4 main, Vector4 accent, bool dark)
    {
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        Vector4 darkWindowBg = new(0.10f, 0.10f, 0.10f, 0.90f);
        Vector4 darkChildBg = new(0.01f, 0.01f, 0.01f, 0.90f);
        Vector4 darkPopupBg = new(0.01f, 0.01f, 0.01f, 0.90f);

        Vector4 lightWindowBg = new(0.95f, 0.95f, 0.95f, 0.90f);
        Vector4 lightPopupBg = new(0.97f, 0.97f, 0.97f, 0.90f);
        Vector4 lightChildBg = new(0.97f, 0.97f, 0.97f, 0.90f);

        Vector4 notHovered = new(0.9f, 0.9f, 0.9f, 0.95f);
        Vector4 notActive = new(0.9f, 0.9f, 0.9f, 0.95f);

        Vector4 titleBg = new Vector4(0.9f, 0.9f, 0.9f, 0.95f);
        Vector4 checkmark = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
        Vector4 slider = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
        Vector4 button = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        Vector4 header = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
        Vector4 resizeGrip = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
        Vector4 separator = new Vector4(0.5f, 0.5f, 0.5f, 0.9f);

        Vector4 dim = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);

        Vector4 lighterAccent = Lighter(accent, 1);

        if (dark)
        {
            colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.95f, 0.95f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = Mix(main, darkWindowBg);
            colors[(int)ImGuiCol.TableHeaderBg] = Mix(main, darkWindowBg);
            colors[(int)ImGuiCol.ChildBg] = Mix(main, darkChildBg);
            colors[(int)ImGuiCol.PopupBg] = Mix(main, darkPopupBg);
            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 0.90f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 0.90f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.31f, 0.32f, 0.90f);

            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.50f);

            colors[(int)ImGuiCol.ButtonHovered] = Mix(dim, accent, notActive);
            colors[(int)ImGuiCol.ButtonActive] = Mix(dim, accent);

            colors[(int)ImGuiCol.HeaderHovered] = Mix(lighterAccent, header, notActive);
            colors[(int)ImGuiCol.HeaderActive] = Mix(lighterAccent, header);

            colors[(int)ImGuiCol.TextLink] = lighterAccent;

            colors[(int)ImGuiCol.Button] = Mix(button, notHovered);
        }
        else
        {
            button = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
            header = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);

            colors[(int)ImGuiCol.Text] = new Vector4(0.05f, 0.06f, 0.08f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = Lighter(Mix(lightWindowBg, main), 2);
            colors[(int)ImGuiCol.TableHeaderBg] = lightWindowBg;
            colors[(int)ImGuiCol.ChildBg] = lightChildBg;
            colors[(int)ImGuiCol.PopupBg] = lightPopupBg;
            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

            colors[(int)ImGuiCol.FrameBg] = Lighter(accent, 3);
            colors[(int)ImGuiCol.FrameBgHovered] = Lighter(accent, 4);
            colors[(int)ImGuiCol.FrameBgActive] = Lighter(accent, 4);

            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(1.00f, 1.00f, 1.00f, 0.50f);

            colors[(int)ImGuiCol.ButtonHovered] = Mix(lighterAccent, notActive);
            colors[(int)ImGuiCol.ButtonActive] = lighterAccent;

            colors[(int)ImGuiCol.HeaderHovered] = Mix(lighterAccent, header, notActive);
            colors[(int)ImGuiCol.HeaderActive] = Mix(lighterAccent, header);

            colors[(int)ImGuiCol.TextLink] = Mix(dim, accent);

            colors[(int)ImGuiCol.Button] = Mix(Lighter(accent, 2), notHovered);
        }

        colors[(int)ImGuiCol.TitleBg] = Mix(main, titleBg, notActive);
        colors[(int)ImGuiCol.TitleBgActive] = Mix(main, titleBg);

        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);

        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);

        colors[(int)ImGuiCol.CheckMark] = Mix(checkmark, accent);
        colors[(int)ImGuiCol.SliderGrab] = Mix(slider, accent, notActive);
        colors[(int)ImGuiCol.SliderGrabActive] = Mix(accent, slider);

        colors[(int)ImGuiCol.Header] = header;

        colors[(int)ImGuiCol.Separator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.10f, 0.40f, 0.75f, 0.78f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.10f, 0.40f, 0.75f, 1.00f);

        colors[(int)ImGuiCol.ResizeGrip] = Mix(accent, resizeGrip, notActive, notHovered);
        colors[(int)ImGuiCol.ResizeGripHovered] = Mix(accent, resizeGrip, notActive);
        colors[(int)ImGuiCol.ResizeGripActive] = Mix(accent, resizeGrip);

        colors[(int)ImGuiCol.Tab] = new Vector4(0.20f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);

    }

    private static Vector4 Lighter(Vector4 v, int iter = 1)
    {
        float shift(float x) => MathF.Cbrt(x + 0.5f) - MathF.Cbrt(1.5f) + 1f;

        for (int i = 0; i < iter; i++)
        {
            v = new Vector4(shift(v.X), shift(v.Y), shift(v.Z), shift(v.W));
        }

        return v;
    }

    private static Vector4 Mix(Vector4 v1, Vector4 v2)
    {
        return new Vector4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
    }
    private static Vector4 Mix(Vector4 v1, Vector4 v2, Vector4 v3)
    {
        return Mix(Mix(v1, v2), v3);
    }
    private static Vector4 Mix(Vector4 v1, Vector4 v2, Vector4 v3, Vector4 v4)
    {
        return Mix(Mix(v1, v2), Mix(v3, v4));
    }

    // https://gist.github.com/SpaceSpeekerman/a2ec38b4f0d40de743b74c73f1f0846f
    public static Vector4 Hex(string hexColor)
    {
        hexColor = hexColor.TrimStart('#'); // Remove '#' if present

        // Parse hexadecimal values for red, green, blue, and alpha components
        int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        int a = hexColor.Length == 8 ? int.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) : 255;

        // Normalize the color values from 0-255 to 0-1 range
        float rf = r / 255f;
        float gf = g / 255f;
        float bf = b / 255f;
        float af = a / 255f;

        return new Vector4(rf, gf, bf, af);
    }
}

public class Theme
{
    public string name;
    public Vector4 main;
    public Vector4 accent;

    public bool dark;
    public Theme(string name, Vector4 main, Vector4 accent, bool dark)
    {
        this.name = name;
        this.main = main;
        this.accent = accent;
        this.dark = dark;
    }
}