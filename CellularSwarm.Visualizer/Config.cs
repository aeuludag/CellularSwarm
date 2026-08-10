using System;
using System.Numerics;
using Raylib_cs;

namespace CellularSwarm.Visualizer;

public class Config
{
    public string simulationsPath = "";
    public int themeIndex;
    public bool useParallel;
    public bool showWelcome;
    public bool keepWindowsInPlace;
    public bool showInfo;
    public bool showFPSinInfo;
    public int width;
    public int height;
    public bool limitFPS;
    public int maxFPS;
    public Color backColor;
    public Color outlineColor;
    public List<Theme> customThemes = new();

    public Config()
    {
        Reset();
    }

    public void Reset()
    {
        simulationsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simulations");
        themeIndex = 0;
        useParallel = true;
        showWelcome = true;
        keepWindowsInPlace = true;
        showInfo = true;
        showFPSinInfo = true;
        width = 1080;
        height = 720;
        limitFPS = true;
        maxFPS = 60;
        backColor = new(20, 20, 20);
        outlineColor = new(255, 255, 255, 20);
        // Theme customDarkTheme = new("Custom Dark 1", new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector4(0.8f, 0.8f, 0.8f, 1f), true);
        // Theme customLightTheme = new("Custom Light 2", new Vector4(0.8f, 0.8f, 0.8f, 1f),  new Vector4(0.2f, 0.2f, 0.2f, 1f), false);
        // customThemes = [customDarkTheme, customLightTheme];
    }
}
