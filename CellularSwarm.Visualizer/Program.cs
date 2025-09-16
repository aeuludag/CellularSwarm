using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using IconFonts;

using CellularSwarm.Core;
using CellularSwarm.Visualizer;

int width = 1500;
int height = 900;

bool isFullScreen = false;

float scale = 1f;

float hexSize = 50f;

Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
Raylib.InitWindow(width, height, "Cellular Swarm");
Raylib.SetWindowPosition(0, 0);
Raylib.SetWindowMinSize(400, 400);
Raylib.SetTargetFPS(60);

var windowPos = Raylib.GetWindowPosition();

var renderer = new HexRenderer();
renderer.hexSize = hexSize;
renderer.showText = false;

var backRenderer = new HexRenderer();
backRenderer.hexSize = hexSize;
backRenderer.showText = false;

var camera = new Camera2D();
var center = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
camera.Target = Vector2.Zero;
camera.Offset = center;
camera.Zoom = 1f;
camera.Rotation = 0f;

renderer.camera = camera;

bool play = false;
var speed = 15f;
var mouseHex = new HexCoords(0, 0);
var showHexCursor = false;
var showInspectCursor = false;

Vector3 palette = new(0f, 0f, 0f);
int brushSize = 0;

GridStates state = GridStates.Move;

SaveLoadHandler saveLoadHandler = new();
string saveLoadPath = "default";

var backColor = new Color(40, 40, 40);

SimulationRenderer simulationRenderer = new();
Simulation simulation;
ResetSimulation();

var editor = new Editor(simulationRenderer);

var camMin = 0.01f;
var camMax = 2f;

rlImGui.Setup(true);

while (!Raylib.WindowShouldClose())
{
    // --- Update ---

    center = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2);
    camera.Offset = center;
    renderer.camera = camera;

    speed = 750f / camera.Zoom;

    mouseHex = MouseToHex();
    HandleKeyboardInput();
    HandleMouseInput();

    if (play) { simulation.Step(); }

    // --- Draw ---
    Raylib.BeginDrawing();
    Raylib.ClearBackground(backColor);
    Raylib.BeginMode2D(camera);

    // // renderer.RenderRadialGrid(9, Color.RayWhite, Color.LightGray);
    // renderer.RenderRadialGrid(12, new Color(60, 60, 60), new Color(40, 40, 40), PointsToHex(camera.Target));
    // renderer.RenderRadialGrid(3, new Color(80, 80, 80), new Color(40, 40, 40), PointsToHex(camera.Target));

    var bottomLeft = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(0, Raylib.GetScreenHeight()), camera));
    var topRight = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), 0), camera));

    if (camera.Zoom >= 0.3f) backRenderer.RenderRectangle(bottomLeft, topRight, backColor, Color.DarkGray);

    switch (simulationRenderer.visualizationType)
    {
        case SimulationRenderer.VisualizationType.ThreeMorphogens:
            renderer.RenderSimulationThreeMorphogens(simulationRenderer);
            break;
        case SimulationRenderer.VisualizationType.SingleMorphogen:
            renderer.RenderSimulationSingleMorphogen(simulationRenderer);
            break;
        case SimulationRenderer.VisualizationType.CellTypes:
            renderer.RenderSimulationCellTypes(simulationRenderer);
            break;
        default:
            break;
    }

    // renderer.Render(bottomLeft.q, bottomLeft.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "BL");
    // renderer.Render(topRight.q, topRight.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "TR");

    if (showHexCursor)
    {
        renderer.RenderRadialGrid(brushSize, new Color(palette.X, palette.Y, palette.Z, 0.1f), new Color(palette.X, palette.Y, palette.Z, 0.1f), mouseHex);
    }

    if (showInspectCursor)
    {
        renderer.Render(mouseHex.q, mouseHex.r, new Color(0f, 0f, 0f, 0.1f), new Color(1f, 1f, 1f, 0.2f));
    }
    // renderer.Render(mouseHex.q, mouseHex.r, new Color(palette.X, palette.Y, palette.Z, 0.1f), new Color(palette.X, palette.Y, palette.Z, 0.1f));

    Raylib.EndMode2D();

    DrawUI();

    Raylib.DrawText($"FPS: {Raylib.GetFPS()}\nW: {Raylib.GetScreenWidth()} H: {Raylib.GetScreenHeight()}", 5, 5, 20, Color.White);

    Raylib.EndDrawing();
}

rlImGui.Shutdown();
Raylib.CloseWindow();

void DrawUI()
{
    rlImGui.Begin();

    Controls();
    GridEditor();
    SaveLoadWindow();
    editor.ShowWindowManager(mouseHex);

    rlImGui.End();
}

void HandleKeyboardInput()
{
    if (ImGui.GetIO().WantCaptureKeyboard) return;

    MoveCamera();
    SetZoomWithKeyboard();
    ControlSimulationWithKeyboard();
    SetGridModeWithKeyboard();

    if (Raylib.IsKeyPressed(KeyboardKey.F)) ToggleFullscreen();
}

void HandleMouseInput()
{
    if (ImGui.GetIO().WantCaptureMouse) return;

    SetGridMode();
    SetZoomWithMouse();
    if (Raylib.IsMouseButtonDown(MouseButton.Middle)) { MoveCameraWithMouse(); }
}

// void TestInput()
// {
//     if (Raylib.IsKeyPressed(KeyboardKey.Space))
//     {
//         Console.WriteLine("Space pressed");
//     }
//     float wheel = Raylib.GetMouseWheelMove();
//     if (wheel != 0)
//     {
//         Console.WriteLine($"Scrolled: {wheel}");
//     }
// }

void ToggleFullscreen()
// by gpt
{
    if (!isFullScreen)
    {
        // Save window size before switching
        width = Raylib.GetScreenWidth();
        height = Raylib.GetScreenHeight();

        windowPos = Raylib.GetWindowPosition();

        int mon = Raylib.GetCurrentMonitor();
        int monW = Raylib.GetMonitorWidth(mon);
        int monH = Raylib.GetMonitorHeight(mon);

        Raylib.SetWindowPosition(0, 0);
        Raylib.SetWindowSize(monW, monH);
        isFullScreen = true;
    }
    else
    {
        Raylib.SetWindowPosition((int)windowPos.X, (int)windowPos.Y);
        Raylib.SetWindowSize(width, height); // restore windowed size
        isFullScreen = false;
    }
}

void SetZoomWithMouse()
{
    float targetZoom = camera.Zoom;
    float scroll = Raylib.GetMouseWheelMove();
    if (scroll == 0f) return;
    if (scroll > 0 && targetZoom == camMax) return;
    if (scroll < 0 && targetZoom == camMin) return;

    float scrollAmount = Math.Clamp(scroll * 0.1f, -0.2f, +0.2f);

    var mousePosBefore = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
    targetZoom *= 1f + scrollAmount;
    camera.Zoom = Math.Clamp(targetZoom, camMin, camMax);
    var mousePosAfter = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);

    camera.Target += mousePosBefore - mousePosAfter;
}

void MoveCamera()
{
    var dt = Raylib.GetFrameTime();
    if (Raylib.IsKeyDown(KeyboardKey.W)) { camera.Target -= Vector2.UnitY * speed * dt; }
    if (Raylib.IsKeyDown(KeyboardKey.S)) { camera.Target += Vector2.UnitY * speed * dt; }
    if (Raylib.IsKeyDown(KeyboardKey.A)) { camera.Target -= Vector2.UnitX * speed * dt; }
    if (Raylib.IsKeyDown(KeyboardKey.D)) { camera.Target += Vector2.UnitX * speed * dt; }
}

void MoveCameraWithMouse()
{
    var md = Raylib.GetMouseDelta();
    camera.Target -= md / camera.Zoom;
}

void SetZoomWithKeyboard()
{
    if (Raylib.IsKeyDown(KeyboardKey.E)) { camera.Zoom *= 1.05f; }
    if (Raylib.IsKeyDown(KeyboardKey.Q)) { camera.Zoom *= 0.95f; }
}

void SetGridModeWithKeyboard()
{
    if (Raylib.IsKeyPressed(KeyboardKey.One)) { state = GridStates.Move; }
    if (Raylib.IsKeyPressed(KeyboardKey.Two)) { state = GridStates.Brush; }
    if (Raylib.IsKeyPressed(KeyboardKey.Three)) { state = GridStates.Erase; }
    if (Raylib.IsKeyPressed(KeyboardKey.Four)) { state = GridStates.Inspect; }
}

void ControlSimulationWithKeyboard()
{
    if (Raylib.IsKeyPressed(KeyboardKey.Space))
    {
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) { play = false; simulation.Step(); return; }
        play = !play;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.C)) { simulationRenderer.ClearGrid(); }
    if (Raylib.IsKeyPressed(KeyboardKey.R)) { ResetSimulation(); }
}

void SetGridMode()
{
    showHexCursor = false;
    showInspectCursor = false;
    editor.showInspector = false;
    switch (state)
    {
        case GridStates.Brush:
            showHexCursor = true;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                simulationRenderer.GenerateCellGrid(brushSize, mouseHex, palette);
            }
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                simulationRenderer.GenerateCellGrid(brushSize, mouseHex, Vector3.Zero);
            }
            break;
        case GridStates.Erase:
            showHexCursor = true;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                simulationRenderer.RemoveCellGrid(brushSize, mouseHex);
            }
            break;
        case GridStates.Move:
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                MoveCameraWithMouse();
            }
            break;
        case GridStates.Inspect:
            showInspectCursor = true;
            editor.showInspector = true;
            break;
    }
}

HexCoords MouseToHex()
{
    var mousePos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
    return PointsToHex(mousePos);
}

HexCoords PointsToHex(Vector2 position)
{
    float x = position.X;
    float y = -position.Y;

    int q = (int)Math.Round((2f / 3f) * (x / hexSize));
    int r = (int)Math.Round((y / (hexSize * Math.Sqrt(3f))) - (q / 2f));

    return new HexCoords(q, r);
}

void Controls()
{
    if (ImGui.Begin("Controls", ImGuiWindowFlags.AlwaysAutoResize))
    {
        ImGui.SetWindowFontScale(scale);
        ImGui.Text(play ? "Playing..." : "Stopped");
        if (ImGui.Button(play ? IconFonts.FontAwesome6.Pause + " Pause" : IconFonts.FontAwesome6.Play + " Play"))
        {
            play = !play;
        }
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.ArrowRight + " Step"))
        {
            play = false;
            simulation.Step();
        }
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.TrashCan + " Clear"))
        {
            simulationRenderer.ClearGrid();
        }
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.Repeat + " Reset"))
        {
            ResetSimulation();
        }
    }
    ImGui.End();
}

void GridEditor()
{
    if (ImGui.Begin("Grid Editor", ImGuiWindowFlags.AlwaysAutoResize))
    {
        ImGui.PushID("gridEditor");
        ImGui.SetWindowFontScale(scale);
        ImGui.Text($"{state} tool is selected");
        if (ImGui.RadioButton(IconFonts.FontAwesome6.ArrowsUpDownLeftRight + " Move", state == GridStates.Move))
        {
            state = GridStates.Move;
        }
        if (ImGui.RadioButton(IconFonts.FontAwesome6.Pen + " Brush", state == GridStates.Brush))
        {
            state = GridStates.Brush;
        }
        ImGui.SameLine();
        brushSize++;
        ImGui.SliderInt("Size", ref brushSize, 1, 12);
        brushSize--;
        if (ImGui.RadioButton(IconFonts.FontAwesome6.Eraser + " Eraser", state == GridStates.Erase))
        {
            state = GridStates.Erase;
        }
        if (ImGui.RadioButton(IconFonts.FontAwesome6.MagnifyingGlass + " Inspect", state == GridStates.Inspect))
        {
            state = GridStates.Inspect;
        }

        if (simulationRenderer.redMorphogenId >= 0)
        {
            var rMorph = simulation.GetMorphogen(simulationRenderer.redMorphogenId);
            if (ImGui.ColorButton($"R ({rMorph.name})", new Vector4(1f, 0f, 0f, 1f)))
            {
                palette.X = 1 - palette.X;
                if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(1f, 0f, 0f); }
            }
            ImGui.SameLine();
            ImGui.SliderFloat($"{rMorph.name}##r", ref palette.X, 0f, 1f);
        }
        else { palette.X = 0f; }

        if (simulationRenderer.greenMorphogenId >= 0)
        {
            var gMorph = simulation.GetMorphogen(simulationRenderer.greenMorphogenId);
            if (ImGui.ColorButton($"G ({gMorph.name})", new Vector4(0f, 1f, 0f, 1f)))
            {
                palette.Y = 1 - palette.Y;
                if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(0f, 1f, 0f); }
            }
            ImGui.SameLine();
            ImGui.SliderFloat($"{gMorph.name}##g", ref palette.Y, 0f, 1f);
        }
        else { palette.Y = 0f; }

        if (simulationRenderer.blueMorphogenId >= 0)
        {
            var bMorph = simulation.GetMorphogen(simulationRenderer.blueMorphogenId);
            if (ImGui.ColorButton($"B ({bMorph.name})", new Vector4(0f, 0f, 1f, 1f)))
            {
                palette.Z = 1 - palette.Z;
                if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(0f, 0f, 1f); }
            }
            ImGui.SameLine();
            ImGui.SliderFloat($"{bMorph.name}##b", ref palette.Z, 0f, 1f);
        }
        else { palette.Z = 0f; }

        ImGui.PopID();
    }
    ImGui.End();
}

void SaveLoadWindow()
{
    if (ImGui.Begin("Save & Load", ImGuiWindowFlags.AlwaysAutoResize))
    {
        ImGui.SetWindowFontScale(scale);
        if (ImGui.Button(FontAwesome6.Upload + " Export"))
        {
            saveLoadHandler.SaveSimulation(simulation, saveLoadPath);
        }
        ImGui.SameLine();
        if (ImGui.Button(FontAwesome6.Download + " Import"))
        {
            simulation = saveLoadHandler.LoadSimulation(saveLoadPath);
            simulationRenderer.Simulation = simulation;

            var morphogenIds = simulation.Morphogens.Keys.ToList();
            if (morphogenIds.Count >= 0) simulationRenderer.redMorphogenId = morphogenIds[0]; else simulationRenderer.redMorphogenId = -1;
            if (morphogenIds.Count >= 1) simulationRenderer.greenMorphogenId = morphogenIds[1]; else simulationRenderer.greenMorphogenId = -1;
            if (morphogenIds.Count >= 2) simulationRenderer.blueMorphogenId = morphogenIds[2]; else simulationRenderer.blueMorphogenId = -1;
        }
        ImGui.InputText(".json", ref saveLoadPath, 64);
    }
    ImGui.End();
}

void ResetSimulation()
{
    // simulationRenderer.Simulation = saveLoadHandler.LoadSimulation("default");
    simulationRenderer.Simulation = new(0, "new");

    simulationRenderer.redMorphogenId = -1;
    simulationRenderer.greenMorphogenId = -1;
    simulationRenderer.blueMorphogenId = -1;

    simulation = simulationRenderer.Simulation;
}

enum GridStates
{
    Move,
    Brush,
    Erase,
    Inspect
}