using System;
using System.Numerics;

namespace CellularSwarm.Visualizer;

public class Config
{
    public string simulationsPath = "";
    public int themeIndex = 0;
    public bool useParallel = true;
    public int width = 1080;
    public int height = 720;
    public Vector4 customDarkThemeMain = new(0.12f, 0.74f, 0.12f, 1.00f);
    public Vector4 customDarkThemeAccent = new(0.75f, 0.75f, 0.10f, 1.00f);
    public Vector4 customLightThemeMain = new(0.12f, 0.74f, 0.12f, 1.00f);
    public Vector4 customLightThemeAccent = new(0.75f, 0.75f, 0.10f, 1.00f);
    public Config()
    {
        this.simulationsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simulations");
    }
}
