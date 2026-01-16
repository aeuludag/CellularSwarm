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
    public int width;
    public int height;
    public Color backColor;
    public Color outlineColor;
    public Theme customDarkTheme = new("Custom Dark", new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector4(0.8f, 0.8f, 0.8f, 1f), true);
    public Theme customLightTheme = new("Custom Light", new Vector4(0.8f, 0.8f, 0.8f, 1f),  new Vector4(0.2f, 0.2f, 0.2f, 1f), false);
    public Config()
    {
        Reset();
    }

    public void Reset()
    {
        simulationsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simulations");
        themeIndex = 2;
        useParallel = true;
        showWelcome = true;
        width = 1080;
        height = 720;
        backColor = new(40, 40, 40);
        outlineColor = new(80, 80, 80);
        customDarkTheme.main = new Vector4(0.2f, 0.2f, 0.2f, 1f);
        customDarkTheme.accent = new Vector4(0.8f, 0.8f, 0.8f, 1f);
        customLightTheme.main = new Vector4(0.8f, 0.8f, 0.8f, 1f);
        customLightTheme.accent = new Vector4(0.2f, 0.2f, 0.2f, 1f);
    }
}
