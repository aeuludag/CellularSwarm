using Raylib_cs;
using System.Numerics;
using CellularSwarm.Core;

public class HexRenderer
{
    public float hexSize;
    public Vector2 offset;
    public bool showText;

    public void RenderRadialGrid(int radius, Color fillColor, Color outlineColor, HexCoords offset)
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Math.Abs(q + r) <= radius)
                {
                    string text = showText ? $"({q}, {r})" : string.Empty;
                    Render(q + offset.q, r + offset.r, fillColor, outlineColor, text);
                }
            }
        }
    }

    public void Render(int q, int r, Color fillColor, Color outlineColor, string text = "")
    {
        float x = hexSize * q * 3 / 2;
        float y = hexSize * (float)Math.Sqrt(3f) * (r + (q / 2f)); // hex_size * r * math.sqrt(3) + hex_size * q * math.sqrt(3) / 2
        var pos = offset + new Vector2(x, -y);

        Raylib.DrawPoly(pos, 6, hexSize, 0, outlineColor);
        Raylib.DrawPoly(pos, 6, hexSize * 0.9f, 0, fillColor);

        Raylib.DrawText(text, (int)pos.X - 20, (int)pos.Y - 20, 15, new Color(255, 255, 255, 127));
    }

    public void RenderFromSimulation(Simulation simulation, Color fillColor, Color outlineColor, float max)
    {
        foreach (var cellPair in simulation.cells)
        {
            float a = cellPair.Value.GetMorphogen(0);
            float b = cellPair.Value.GetMorphogen(1);
            float c = cellPair.Value.GetMorphogen(2);

            string morphoText;
            morphoText = string.Empty;
            morphoText = $"{a:F2}\n{b:F2}\n{c:F2}";
            morphoText = $"{cellPair.Key}";

            morphoText = showText ? morphoText : string.Empty;
            // max = 1f;
            var morphoColor = new Color(a / max, b / max, c / max);
            Render(cellPair.Key.q, cellPair.Key.r, morphoColor, morphoColor, morphoText);
        }
    }
}