using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using IconFonts;
using System.Runtime.InteropServices;

using CellularSwarm.Core;
using CellularSwarm.Visualizer;
using static CellularSwarm.Visualizer.Editor;
using System.Diagnostics;
using System.Text.RegularExpressions;

if (new Random().Next(0, 10) == 6)
{
    DebugConsole.Info("Connection established.", "NETWORK");
    DebugConsole.Log(new Vector4(1f, 1f, 1f, 1f), "ARE YOU THERE?", "Dr.G");
    DebugConsole.Log(new Vector4(1f, 1f, 1f, 1f), "ARE WE CONNECTED?", "Dr.G");
    DebugConsole.Log(new Vector4(1f, 1f, 1f, 1f), "EXCELLENT. TRULY EXCELLENT.", "Dr.G");
    DebugConsole.Log(new Vector4(1f, 1f, 1f, 1f), "WE MAY BEGIN.", "Dr.G");
}

ConfigHandler.LoadConfig();

int width = ConfigHandler.Config.width;
int height = ConfigHandler.Config.height;

float hexSize = 50f;

Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
Raylib.InitWindow(width, height, "Cellular Swarm");
Raylib.SetWindowMinSize(800, 400);
Raylib.SetTargetFPS(ConfigHandler.Config.limitFPS ? ConfigHandler.Config.maxFPS : int.MaxValue);
Raylib.SetExitKey(KeyboardKey.Null);

Image icon = Raylib.LoadImage("icon_nobg.png");
Raylib.SetWindowIcon(icon);
Raylib.UnloadImage(icon);

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

string simulationName = "your-simulation";

var backColor = new Color(40, 40, 40);
var outlineColor = new Color(80, 80, 80);

SimulationRenderer simulationRenderer = new();
Simulation GetSimulation() { return simulationRenderer.Simulation; }

var editor = new Editor(simulationRenderer);

var camMin = 0.01f;
var camMax = 2f;

rlImGui.Setup(true);

// byte[] iniNameBuffer = System.Text.Encoding.ASCII.GetBytes("custom_layout.ini\0");
unsafe
{
    // if (ConfigHandler.Config.rememberWindows)
    // {
    //     fixed (byte* p = iniNameBuffer)
    //     {
    //         ImGui.GetIO().NativePtr->IniFilename = p;
    //     }
    // }
    // else
    // {
        ImGui.GetIO().NativePtr->IniFilename = null;
    // }
}

ThemeHandler.ApplyMorph();
ThemeHandler.ApplyCurrentTheme();
ResetSimulation();

DebugConsole.Log(new Vector4(1.0f, 0.2f, 0.2f, 1f), "Cellular Swarm", "aeuludag");
DebugConsole.Log(new Vector4(0.9f, 0.6f, 0.2f, 1f), $"Simulation version: v{Simulation.VERSION}", "aeuludag");
DebugConsole.Log(new Vector4(0.6f, 0.8f, 0.2f, 1f), $"Simulation Renderer version: v{SimulationRenderer.VERSION}", "aeuludag");
DebugConsole.Log(new Vector4(0.4f, 0.8f, 0.5f, 1f), $"OS Architecture: {RuntimeInformation.OSArchitecture}", "aeuludag");
DebugConsole.Log(new Vector4(0.2f, 0.6f, 0.9f, 1f), $"OS Version: {Environment.OSVersion}", "aeuludag");

DebugConsole.Log(new Vector4(0.0f, 0.9f, 0.9f, 1f), $"Type !help to see available commands.", "aeuludag");

// List<int> UIDrawTimes = new();
// List<int> simulationStepTimes = new();
// List<int> cellStepTimes = new();
// List<int> multiplicationTimes = new();
// List<int> diffusionTimes = new();
// List<int> apoptosisTimes = new();

DebugConsole.Info("Checking for arguments.", "RENDERER");
var commandlineArgs = Environment.GetCommandLineArgs();
if(commandlineArgs.Length != 1)
{
    var path = commandlineArgs[1];
    DebugConsole.Info($"Found arg to be [{path}].", "RENDERER");

    simulationRenderer = SaveLoadHandler.LoadSimulation(path);
    editor.renderer = simulationRenderer;
    SetTitle(simulationName);
}

DebugConsole.Info("Starting program loop.", "RENDERER");

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

    if (play) Step();

    // --- Draw ---
    backColor = ConfigHandler.Config.backColor;
    outlineColor = ConfigHandler.Config.outlineColor;

    Raylib.BeginDrawing();
    Raylib.ClearBackground(backColor);
    Raylib.BeginMode2D(camera);

    // // renderer.RenderRadialGrid(9, Color.RayWhite, Color.LightGray);
    // renderer.RenderRadialGrid(12, new Color(60, 60, 60), new Color(40, 40, 40), PointsToHex(camera.Target));
    // renderer.RenderRadialGrid(3, new Color(80, 80, 80), new Color(40, 40, 40), PointsToHex(camera.Target));

    var bottomLeft = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(0, Raylib.GetScreenHeight()), camera));
    var topRight = PointsToHex(Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), 0), camera));

    if (camera.Zoom >= 0.2f) backRenderer.RenderRectangle(bottomLeft, topRight, backColor, outlineColor);

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
        case SimulationRenderer.VisualizationType.GeneActivity:
            renderer.RenderSimulationGeneActivity(simulationRenderer);
            break;
        case SimulationRenderer.VisualizationType.GeneConditionMet:
            renderer.RenderSimulationGeneConditionMet(simulationRenderer);
            break;
        default:
            break;
    }

    // renderer.Render(bottomLeft.q, bottomLeft.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "BL");
    // renderer.Render(topRight.q, topRight.r, new Color(255, 0, 0, 100), new Color(0, 0, 255, 100), "TR");

    if (showHexCursor)
    {
        if (editor.gridState == GridState.Erase)
        {
            renderer.RenderRadialGrid(editor.brushSize, new Color(0.7f, 0.7f, 0.7f, 0.1f), new Color(0f, 0f, 0f, 0.1f), mouseHex);
        }
        else
        {
            var cell = simulationRenderer.CellToDraw;
            renderer.RenderRadialGrid(editor.brushSize, simulationRenderer.GetCellColor(cell, 0.1f), simulationRenderer.GetCellColor(cell, 0.1f), mouseHex);
        }
    }

    if (showInspectCursor)
    {
        renderer.Render(mouseHex.q, mouseHex.r, new Color(0f, 0f, 0f, 0.1f), new Color(1f, 1f, 1f, 0.2f));
    }

    Raylib.EndMode2D();

    DrawUI();

    // Raylib.DrawText($"FPS: {Raylib.GetFPS()}\nW: {Raylib.GetScreenWidth()} H: {Raylib.GetScreenHeight()}", 5, 25, 15, Color.RayWhite);

    Raylib.EndDrawing();
}

ConfigHandler.Config.width = Raylib.GetScreenWidth();
ConfigHandler.Config.height = Raylib.GetScreenHeight();

ConfigHandler.SaveConfig();

foreach (var txt2d in Editor.LoadedTextures)
{
    Raylib.UnloadTexture(txt2d);
}

rlImGui.Shutdown();
Raylib.CloseWindow();

void Step()
{
    simulationRenderer.Step();
}

void DrawUI()
{
    rlImGui.Begin();

    var w = ImGui.GetIO().DisplaySize.X;

    ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));
    Editor.WinPosC(w - 16, 16, 1, 0);
    Controls();

    Editor.WinPosC(w - 16, 80, 1, 0);
    SaveLoadWindow();
    ImGui.PopStyleVar();

    if(ConfigHandler.Config.showInfo)
    {
        ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Always, Vector2.Zero);
        editor.ShowInfo();
    }
    editor.ShowWindowManager(mouseHex);

    // ImGui.ShowDemoWindow();

    rlImGui.End();
}

void HandleKeyboardInput()
{
    if (ImGui.GetIO().WantCaptureKeyboard) return;

    MoveCamera();
    SetZoomWithKeyboard();
    ControlSimulationWithKeyboard();
    SetGridModeWithKeyboard();

    // if (Raylib.IsKeyPressed(KeyboardKey.F)) ToggleFullscreen();
    // if (Raylib.IsKeyPressed(KeyboardKey.P)) useParallel ^= true; // feelin' fancy y'know :smirk:
    // if (Raylib.IsKeyPressed(KeyboardKey.T)) diagnosticStep ^= true;
    // if (Raylib.IsKeyPressed(KeyboardKey.G)) { simulationStepTimes.Clear(); cellStepTimes.Clear(); multiplicationTimes.Clear(); diffusionTimes.Clear(); apoptosisTimes.Clear(); }
}

void HandleMouseInput()
{
    if (ImGui.GetIO().WantCaptureMouse) return;

    SetGridMode();
    SetZoomWithMouse();
    SetBrushSizeWithMouse();
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
    if ((editor.gridState == GridState.Brush || editor.gridState == GridState.Erase) && Raylib.IsKeyDown(KeyboardKey.LeftShift)) return;
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

void SetBrushSizeWithMouse()
{
    if(editor.gridState == GridState.Move || editor.gridState == GridState.Inspect) return;
    if(!Raylib.IsKeyDown(KeyboardKey.LeftShift)) return;
    float scroll = Raylib.GetMouseWheelMove();
    if (scroll == 0f) return;

    scroll = Math.Clamp(scroll * 0.7f, -1f, +1f);
    editor.brushSize += (int)Math.Round(scroll);
    editor.brushSize = Math.Clamp(editor.brushSize, 0, 23);
}

void MoveCamera()
{
    var dt = Raylib.GetFrameTime() * (Raylib.IsKeyDown(KeyboardKey.LeftShift) ? 2 : 1);
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
    var dt = Raylib.GetFrameTime();
    if (Raylib.IsKeyDown(KeyboardKey.E)) { camera.Zoom *= (1.00f + 5f * dt); }
    if (Raylib.IsKeyDown(KeyboardKey.Q)) { camera.Zoom *= (1.00f - 5f * dt); }
    camera.Zoom = Math.Clamp(camera.Zoom, camMin, camMax);
}

void SetGridModeWithKeyboard()
{
    if (Raylib.IsKeyDown(KeyboardKey.LeftShift) && editor.gridState != GridState.Inspect)
    {
        var number = (int)Raylib.GetKeyPressed() - (int)KeyboardKey.One;

        if(number >= 0 && number <= 8 && simulationRenderer.cellPalette.Count > number)
        {
            editor.gridState = GridState.Brush;
            editor.SetCellIndex(number);
        }
        return;
    }

    if (Raylib.IsKeyPressed(KeyboardKey.One)) { editor.gridState = GridState.Move; }
    if (Raylib.IsKeyPressed(KeyboardKey.Two)) { editor.gridState = GridState.Brush; }
    if (Raylib.IsKeyPressed(KeyboardKey.Three)) { editor.gridState = GridState.Erase; }
    if (Raylib.IsKeyPressed(KeyboardKey.Four)) { editor.gridState = GridState.Inspect; }
}

void ControlSimulationWithKeyboard()
{
    if (Raylib.IsKeyPressed(KeyboardKey.Space))
    {
        if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
        {
            DebugConsole.Info("Simulation step.", "RENDERER");
            play = false;
            Step();
            return;
        }
        play = !play;

        DebugConsole.Info(play ? "Resuming simulation." : "Pausing simulation.", "RENDERER");
    }
    if (Raylib.IsKeyPressed(KeyboardKey.Zero)) { DebugConsole.Info("Reset zoom.", "RENDERER"); camera.Zoom = 1.0f; center = camera.Target = new(0, 0); };
    if (Raylib.IsKeyPressed(KeyboardKey.C)) { DebugConsole.Info("Clearing grid.", "RENDERER"); simulationRenderer.ClearGrid(); }

    // if (Raylib.IsKeyPressed(KeyboardKey.R)) { ResetSimulation(); }
}

// void LogLastDiagnosticData()
// {
//     if (cellStepTimes.Count == 0) return;
//     Console.WriteLine(
//         $"### DIAGNOSIS LAST:     {DateTime.Now:T} {DateTime.Now:fffffff}\n" +
//         $"## Cell Step:      {cellStepTimes.Last()} us \n" +
//         $"## Multiplication: {multiplicationTimes.Last()} us \n" +
//         $"## Diffusion:      {diffusionTimes.Last()} us \n" +
//         $"## Apoptosis:      {apoptosisTimes.Last()} us \n");
// }

// void LogAverageDiagnosticData()
// {
//     if (cellStepTimes.Count == 0) return;
//     Console.WriteLine(
//         $"### DIAGNOSIS AVERAGE:     {DateTime.Now:T} {DateTime.Now:fffffff}\n" +
//         $"## Cell Step:      {cellStepTimes.Average()} us \n" +
//         $"## Multiplication: {multiplicationTimes.Average()} us \n" +
//         $"## Diffusion:      {diffusionTimes.Average()} us \n" +
//         $"## Apoptosis:      {apoptosisTimes.Average()} us \n");
// }

void SetGridMode()
{
    showHexCursor = false;
    showInspectCursor = false;
    editor.showInspector = false;
    editor.showCellEditor = false;
    switch (editor.gridState)
    {
        case GridState.Brush:
            showHexCursor = true;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                simulationRenderer.GenerateCellGrid(editor.brushSize, mouseHex, simulationRenderer.CellToDraw);
            }
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                simulationRenderer.RemoveCellGrid(editor.brushSize, mouseHex);
            }
            break;
        case GridState.Erase:
            showHexCursor = true;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                simulationRenderer.RemoveCellGrid(editor.brushSize, mouseHex);
            }
            break;
        case GridState.Move:
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                MoveCameraWithMouse();
            }
            break;
        case GridState.Inspect:
            showInspectCursor = true;
            editor.showInspector = true;
            editor.showCellEditor = true;
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                editor.selectedCellCoords = mouseHex;
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
    if (ImGui.Begin("Simulation Controls", ImGuiWindowFlags.AlwaysAutoResize))
    {
        if (ImGui.Button(play ? IconFonts.FontAwesome6.Pause + " Pause" : IconFonts.FontAwesome6.Play + " Play"))
        {
            play = !play;
            DebugConsole.Info(play ? "Resuming simulation." : "Pausing simulation.", "RENDERER");
        }
        HoverTooltip(play ? "Pause simulation" : "Resume simulation", "Space", true);
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.ArrowRight + " Step"))
        {
            play = false;
            DebugConsole.Info("Simulation step.", "RENDERER");
            Step();
        }
        HoverTooltip("Perform one step of simulation", "Shift + Space", true);
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.TrashCan + " Clear Grid"))
        {
            DebugConsole.Info("Clearing grid.", "RENDERER");
            simulationRenderer.ClearGrid();
        }
        HoverTooltip("Clear simulation grid");
        ImGui.SameLine();
        if (ImGui.Button(IconFonts.FontAwesome6.FileCirclePlus + " New Simulation"))
        {
            ResetSimulation();
        }
        HoverTooltip("Open a new simulation");
    }
    ImGui.End();
}

void SaveLoadWindow()
{
    if(Raylib.IsFileDropped())
    {
        // gemini generated
        DebugConsole.Warning("File dropped.", "DRAGDROP");
        FilePathList droppedFiles = Raylib.LoadDroppedFiles();
        unsafe 
        {
            string filePath = Marshal.PtrToStringAnsi((IntPtr)droppedFiles.Paths[0]) ?? "";
            DebugConsole.Warning($"Received file from [{filePath}].", "DRAGDROP");

            simulationRenderer = SaveLoadHandler.LoadSimulation(filePath);
            editor.renderer = simulationRenderer;
            SetTitle(simulationName);
        }
        Raylib.UnloadDroppedFiles(droppedFiles);
    }

    if (ImGui.Begin("Save & Load", ImGuiWindowFlags.AlwaysAutoResize))
    {
        // ImGui.PushTextWrapPos(64*4);
        // if (SaveLoadHandler.BadLoad)
        // {
        //     ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
        //     ImGui.Text($"Error while loading.");
        //     ImGui.PopStyleColor();
        // } else
        // {
        //     ImGui.TextDisabled("Load from Simulations folder or drag & drop file here.");
        // }
        // ImGui.PushItemWidth(196);
        if(ImGui.InputText(".csim", ref simulationName, 64))
        {
            string invalidPatterns = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]";
            simulationName = Regex.Replace(simulationName, invalidPatterns, "_");
        }
        // ImGui.PopItemWidth();
        HoverTooltip("File name in Simulations folder");
        ImGui.Separator();
        
        if (ImGui.Button(FontAwesome6.Upload + " Export") || ImGui.IsKeyChordPressed(ImGuiKey.ModCtrl | ImGuiKey.S))
        {
            SaveSimulation();
        }
        HoverTooltip("Save to Simulations folder", "Ctrl + S", true);
        
        ImGui.SameLine();
        if (ImGui.Button(FontAwesome6.Download + " Import"))
        {
            simulationRenderer = SaveLoadHandler.LoadSimulationFromSimulationsFolder(simulationName);
            editor.renderer = simulationRenderer;
            SetTitle(simulationName);
        }
        HoverTooltip("Load from Simulations folder");

        ImGui.SameLine();
        if (ImGui.Button($"{FontAwesome6.Folder}##sim"))
        {
            Process.Start(new ProcessStartInfo(fileName: ConfigHandler.Config.simulationsPath) {UseShellExecute = true, Verb = "open"});
        }
        HoverTooltip("Open Simulations folder");

        ImGui.PushTextWrapPos(248);
        if (SaveLoadHandler.LastSaveTime != DateTime.MinValue)
        {
            var timeWithoutSave = DateTime.Now - SaveLoadHandler.LastSaveTime;
            if(timeWithoutSave.TotalMinutes < 1)
            {
                ImGui.Text($"Last Save: just now");
            } else
            {
                ImGui.Text($"Last Save: {(timeWithoutSave.Hours > 0 ? timeWithoutSave.Hours + "h " : "")}{timeWithoutSave.Minutes}m ago");
            }
        }
        if (Simulation.VERSION != SaveLoadHandler.LoadedVersion)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ThemeHandler.GetCurrentTheme().dark ? DebugConsole.Message.WarningColor : new Vector4(0.5f, 0.5f, 0f, 1f));
            ImGui.Text($"{FontAwesome6.TriangleExclamation} Version Mismatch Warning!\nProgram version: v{Simulation.VERSION}\nFile version: v{SaveLoadHandler.LoadedVersion}.");
            ImGui.PopStyleColor();
        }
        if (SaveLoadHandler.BadLoad)
        {
            if(simulationName == "GASTER")
            {
                Raylib.CloseWindow(); // ✞︎☜︎☼︎✡︎📪︎ ✞︎☜︎☼︎✡︎ ✋︎☠︎❄︎☜︎☼︎☜︎💧︎❄︎✋︎☠︎☝︎📬︎
            } else
            {
                ImGui.PushStyleColor(ImGuiCol.Text, DebugConsole.Message.ErrorColor);
                ImGui.Text($"{FontAwesome6.TriangleExclamation} Error while loading!\nSee Console ({FontAwesome6.Terminal}) for more info.");
                ImGui.PopStyleColor();
            }

        }
        ImGui.TextDisabled("Load from Simulations folder or Drag & Drop a simulation.");
        ImGui.PopTextWrapPos();
    }
    ImGui.End();
}

void SaveSimulation()
{
    SaveLoadHandler.SaveSimulationToSimulationsFolder(GetSimulation(), simulationRenderer, simulationName);
    SaveLoadHandler.BadLoad = false;
    SaveLoadHandler.LoadedVersion = Simulation.VERSION;
}

void ResetSimulation()
{
    DebugConsole.Info("Simulation reset.", "RENDERER");
    simulationRenderer = new(new(DateTime.Now.Millisecond, $"new-{DateTime.Now:fffffff}"));
    editor.renderer = simulationRenderer;
    simulationRenderer.SetParallel();
    SetTitle(simulationName);
}