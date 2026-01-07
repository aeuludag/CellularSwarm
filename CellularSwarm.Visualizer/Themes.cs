using System.Numerics;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public static class Themes
{
    // gpt generated
    public static void ApplyTheme1()
    {
        DebugConsole.Log("Applying Theme1...");
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        // General style settings
        style.FrameRounding = 3.0f;
        style.WindowRounding = 4.0f;
        style.ScrollbarRounding = 3.0f;
        style.GrabRounding = 3.0f;
        style.TabRounding = 3.0f;

        style.WindowBorderSize = 1.0f;
        style.FrameBorderSize = 1.0f;
        style.ScrollbarSize = 14.0f;
        style.GrabMinSize = 10.0f;

        // Example theme colors
        colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        colors[(int)ImGuiCol.WindowBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.ChildBg] = new Vector4(0.03f, 0.04f, 0.05f, 0.90f);
        colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
        colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
        colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

        colors[(int)ImGuiCol.FrameBg] = new Vector4(0.20f, 0.21f, 0.22f, 1.00f);
        colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.25f, 0.26f, 0.27f, 1.00f);
        colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.30f, 0.31f, 0.32f, 1.00f);

        colors[(int)ImGuiCol.TitleBg] = new Vector4(0.09f, 0.20f, 0.10f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.12f, 0.45f, 0.12f, 1.00f);
        colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.51f);

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

        colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.26f, 0.59f, 0.98f, 0.20f);
        colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.67f);
        colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);

        colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.20f, 0.21f, 1.00f);
        colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.80f);

    }
}
