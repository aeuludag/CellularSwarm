using System.Numerics;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public static class Themes
{
    public readonly static List<Action> AllThemes = new List<Action>() { Themes.ApplyGreenYellowTheme, Themes.ApplyPurpleYellowTheme, Themes.ApplyRedYellowTheme, Themes.ApplyCustomDarkTheme, Themes.ApplyCustomLightTheme, Themes.ApplyImGuiClassicTheme, Themes.ApplyImGuiDarkTheme, Themes.ApplyImGuiLightTheme };

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

    public static void ApplyGreenYellowTheme()
    {
        // DebugConsole.Info("Applying Green - Yellow Theme", "THEME");
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        // Example theme colors
        colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 0.90f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 0.90f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.31f, 0.32f, 0.90f);

        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.09f, 0.20f, 0.10f, 0.90f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.12f, 0.45f, 0.12f, 0.90f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.50f);

        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);

        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);

        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.59f, 0.59f, 0.10f, 1.00f);

        colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.50f, 0.28f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.06f, 0.53f, 0.06f, 1.00f);

        colors[(int)ImGuiCol.Header] = new Vector4(0.20f, 0.22f, 0.23f, 0.55f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.26f, 0.59f, 0.26f, 0.80f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.26f, 0.59f, 0.26f, 1.00f);

        colors[(int)ImGuiCol.Separator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.10f, 0.40f, 0.75f, 0.78f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.10f, 0.40f, 0.75f, 1.00f);

        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.26f, 0.59f, 0.26f, 0.20f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.26f, 0.59f, 0.26f, 0.67f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.26f, 0.95f);

        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);

    }

    public static void ApplyPurpleYellowTheme()
    {
        // DebugConsole.Info("Applying Purple - Yellow Theme", "THEME");
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        // Example theme colors
        colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 0.90f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 0.90f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.31f, 0.32f, 0.90f);

        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.20f, 0.09f, 0.20f, 0.90f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.45f, 0.12f, 0.45f, 0.90f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.50f);

        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);

        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);

        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.59f, 0.59f, 0.10f, 1.00f);

        colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.30f, 0.32f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.06f, 0.53f, 0.98f, 1.00f);

        colors[(int)ImGuiCol.Header] = new Vector4(0.20f, 0.22f, 0.23f, 0.55f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);

        colors[(int)ImGuiCol.Separator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.10f, 0.40f, 0.75f, 0.78f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.10f, 0.40f, 0.75f, 1.00f);

        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.59f, 0.26f, 0.98f, 0.20f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.59f, 0.26f, 0.98f, 0.67f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.59f, 0.26f, 0.98f, 0.95f);

        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.59f, 0.26f, 0.98f, 0.80f);

    }

    public static void ApplyRedYellowTheme()
    {
        // DebugConsole.Info("Applying Red - Yellow Theme", "THEME");
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        // Example theme colors
        colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 0.90f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 0.90f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.31f, 0.32f, 0.90f);

        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.20f, 0.09f, 0.09f, 0.90f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.45f, 0.12f, 0.09f, 0.90f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.50f);

        colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.00f);

        colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
        colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);

        colors[(int)ImGuiCol.CheckMark] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.75f, 0.75f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.59f, 0.59f, 0.10f, 1.00f);

        colors[(int)ImGuiCol.Button] = new Vector4(0.20f, 0.22f, 0.23f, 1.00f);
        colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.28f, 0.30f, 0.32f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.06f, 0.53f, 0.98f, 1.00f);

        colors[(int)ImGuiCol.Header] = new Vector4(0.20f, 0.22f, 0.23f, 0.55f);
        colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);
        colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);

        colors[(int)ImGuiCol.Separator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.10f, 0.40f, 0.75f, 0.78f);
        colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.10f, 0.40f, 0.75f, 1.00f);

        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.59f, 0.26f, 0.98f, 0.20f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.59f, 0.26f, 0.98f, 0.67f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.59f, 0.26f, 0.98f, 0.95f);

        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.59f, 0.26f, 0.98f, 0.80f);

    }

    public static void ApplyImGuiClassicTheme()
    {
        // DebugConsole.Info("Applying ImGui Classic Theme.", "THEME");
        ImGui.StyleColorsClassic();
    }

    public static void ApplyImGuiDarkTheme()
    {
        // DebugConsole.Info("Applying ImGui Dark Theme.", "THEME");
        ImGui.StyleColorsDark();
    }

    public static void ApplyImGuiLightTheme()
    {
        // DebugConsole.Info("Applying ImGui Light Theme.", "THEME");
        ImGui.StyleColorsLight();
    }

    public static void ApplyCustomDarkTheme()
    {
        // DebugConsole.Info("Applying Custom Dark Theme", "THEME");

        ApplyCustom(ConfigHandler.Config.customDarkThemeMain, ConfigHandler.Config.customDarkThemeAccent, true);
    }
    public static void ApplyCustomLightTheme()
    {
        // DebugConsole.Info("Applying Custom Light Theme", "THEME");

        ApplyCustom(ConfigHandler.Config.customLightThemeMain, ConfigHandler.Config.customLightThemeAccent, false);
    }

    private static void ApplyCustom(Vector4 main, Vector4 accent, bool dark)
    {
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        Vector4 darkWindowBg = new(0.07f, 0.07f, 0.07f, 0.90f);
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

        // Example theme colors
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
        } else
        {
            colors[(int)ImGuiCol.Text] = new Vector4(0.05f, 0.06f, 0.08f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = Lighter(Mix(lightWindowBg, main), 3);
            colors[(int)ImGuiCol.TableHeaderBg] = lightWindowBg;
            colors[(int)ImGuiCol.ChildBg] = lightChildBg;
            colors[(int)ImGuiCol.PopupBg] = lightPopupBg;
            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.80f, 0.81f, 0.82f, 0.90f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.85f, 0.86f, 0.87f, 0.90f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.90f, 0.91f, 0.92f, 0.90f);
            
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(1.00f, 1.00f, 1.00f, 0.50f);

            button = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
            header = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
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

        colors[(int)ImGuiCol.Button] = Mix(button, notHovered);
        colors[(int)ImGuiCol.ButtonHovered] = Mix(dim, accent, notActive);
        colors[(int)ImGuiCol.ButtonActive] = Mix(dim, accent);

        colors[(int)ImGuiCol.Header] = header;
        colors[(int)ImGuiCol.HeaderHovered] = Mix(accent, header, notActive);
        colors[(int)ImGuiCol.HeaderActive] = Mix(accent, header);

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
}