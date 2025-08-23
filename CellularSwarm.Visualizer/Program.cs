using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using IconFonts;

using CellularSwarm.Core;
using CellularSwarm.Core.Data;
using CellularSwarm.Visualizer;

const int WIDTH = 1200;
const int HEIGHT = 900;

float scale = 1f;

float hexSize = 50f;

Raylib.InitWindow(WIDTH, HEIGHT, "Cellular Swarm");
Raylib.SetTargetFPS(60);

var renderer = new HexRenderer();
renderer.hexSize = hexSize;
renderer.showText = false;

var backRenderer = new HexRenderer();
backRenderer.hexSize = hexSize;
backRenderer.showText = false;

SimulationRenderer simulationRenderer = new();
Simulation simulation;
ResetSimulation();

var camera = new Camera2D();
var center = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
camera.Target = Vector2.Zero;
camera.Offset = center / 2;
camera.Zoom = 1f;
camera.Rotation = 0f;

bool play = false;
var speed = 15f;
var mouseHex = new HexCoords(0, 0);
var showHexCursor = false;

Vector3 palette = new(0f, 0f, 0f);
int brushSize = 0;

GridStates state = GridStates.Move;

string saveLoadPath = "default-simulation";

var backColor = new Color(40, 40, 40);

var editor = new Editor(simulationRenderer);

rlImGui.Setup(true);

while (!Raylib.WindowShouldClose())
{
    // --- Update ---

    // center = new Vector2(Raylib.GetRenderWidth() / 2, Raylib.GetRenderHeight() / 2);
    // camera.Target = center;
    // camera.Offset = center / 2;

    speed = 750f / camera.Zoom;

    mouseHex = MouseToHex();
    HandleKeyboardInput();
    HandleMouseInput();

    camera.Zoom = Math.Clamp(camera.Zoom, 0.01f, 2f);

    if (play) { simulation.Step(); }

    // --- Draw ---
    Raylib.BeginDrawing();
    Raylib.ClearBackground(backColor);
    Raylib.BeginMode2D(camera);

    // // renderer.RenderRadialGrid(9, Color.RayWhite, Color.LightGray);
    // renderer.RenderRadialGrid(12, new Color(60, 60, 60), new Color(40, 40, 40), PointsToHex(camera.Target));
    // renderer.RenderRadialGrid(3, new Color(80, 80, 80), new Color(40, 40, 40), PointsToHex(camera.Target));

    var bottomLeft = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(0, HEIGHT), camera));
    var topRight = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(WIDTH, 0), camera));

    if(camera.Zoom >= 0.5f) backRenderer.RenderRectangle(bottomLeft, topRight, backColor, Color.DarkGray);

    renderer.RenderFromSimulation(simulationRenderer);

    // renderer.Render(bottomLeft.q, bottomLeft.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "BL");
    // renderer.Render(topRight.q, topRight.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "TR");

    if (showHexCursor)
        renderer.RenderRadialGrid(brushSize, new Color(palette.X, palette.Y, palette.Z, 0.1f), new Color(palette.X, palette.Y, palette.Z, 0.1f), mouseHex);
    // renderer.Render(mouseHex.q, mouseHex.r, new Color(palette.X, palette.Y, palette.Z, 0.1f), new Color(palette.X, palette.Y, palette.Z, 0.1f));

    Raylib.EndMode2D();

    DrawUI();
    Raylib.DrawText(Raylib.GetFPS().ToString(), 5, 5, 20, Color.White);

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
    editor.ShowMorphogenEditor();
    editor.ShowGeneActionEditor();
    editor.ShowCellTypeEditor();
    editor.ShowGeneConditionEditor();

    rlImGui.End();
}

void HandleKeyboardInput()
{
    if (ImGui.GetIO().WantCaptureKeyboard) return;

    MoveCamera();
    SetZoomWithKeyboard();
    ControlSimulationWithKeyboard();
    SetGridModeWithKeyboard();

    if (Raylib.IsKeyPressed(KeyboardKey.F)) Raylib.ToggleFullscreen();
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

void SetZoomWithMouse()
{
    float scroll = Raylib.GetMouseWheelMove();
    if (scroll > float.Epsilon)
    {
        camera.Zoom *= 1.1f;
    }
    else if (scroll < -float.Epsilon)
    {
        camera.Zoom *= 0.9f;
    }
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
                simulationRenderer.RemoveCell(mouseHex);
            }
            break;
        case GridStates.Move:
            showHexCursor = false;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                MoveCameraWithMouse();
            }
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
        ImGui.SetColorEditOptions(ImGuiColorEditFlags.Float | ImGuiColorEditFlags.PickerHueWheel);
        ImGui.ColorEdit3("Morphogen", ref palette);
        if (ImGui.ColorButton("R", new Vector4(1f, 0f, 0f, 1f)))
        {
            palette.X = 1 - palette.X;
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(1f, 0f, 0f); }
        }
        ImGui.SameLine();
        if (ImGui.ColorButton("G", new Vector4(0f, 1f, 0f, 1f)))
        {
            palette.Y = 1 - palette.Y;
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(0f, 1f, 0f); }
        }
        ImGui.SameLine();
        if (ImGui.ColorButton("B", new Vector4(0f, 0f, 1f, 1f)))
        {
            palette.Z = 1 - palette.Z;
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift)) { palette = new Vector3(0f, 0f, 1f); }
        }
        ImGui.SameLine();
    }
    ImGui.End();
}

void SaveLoadWindow()
{
    var folder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
    folder = Directory.GetParent(folder)!.FullName;
    folder = Directory.GetParent(folder)!.FullName;
    folder = Path.Combine(folder, "Simulations");

    if (ImGui.Begin("Save & Load", ImGuiWindowFlags.AlwaysAutoResize))
    {
        ImGui.SetWindowFontScale(scale);
        if (ImGui.Button(FontAwesome6.Download + " Save"))
        {
            var serialized = Serializer.Serialize(SimulationData.FromSimulation(simulation));

            File.WriteAllText(Path.Combine(folder, $"{saveLoadPath}.json"), serialized);
        }
        ImGui.SameLine();
        if (ImGui.Button(FontAwesome6.Upload + " Load"))
        {
            System.Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));

            var deserialized = Serializer.Deserialize(File.ReadAllText(Path.Combine(folder, $"{saveLoadPath}.json")));
            simulation = SimulationData.ToSimulation(deserialized);
            simulationRenderer.Simulation = simulation;
        }
        ImGui.InputText(".json", ref saveLoadPath, 64);
    }
    ImGui.End();
}

void ResetSimulation()
{
    simulationRenderer.Simulation = simulationRenderer.SetSampleSimulation();
    simulationRenderer.morphogenIdForColors = new(0, 1, 2);
    simulation = simulationRenderer.Simulation;
}

enum GridStates
{
    Move,
    Brush,
    Erase
}