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
                    Render(q + offset.q, r + offset.r, fillColor, outlineColor);
                }
            }
        }
    }

    public void Render(int q, int r, Color fillColor, Color outlineColor)
    {
        float x = hexSize * q * 3 / 2;
        float y = hexSize * (float)Math.Sqrt(3f) * (r + (q / 2f)); // hex_size * r * math.sqrt(3) + hex_size * q * math.sqrt(3) / 2
        var pos = offset + new Vector2(x, -y);

        Raylib.DrawPoly(pos, 6, hexSize, 0, outlineColor);
        Raylib.DrawPoly(pos, 6, hexSize * 0.9f, 0, fillColor);

        // if (showText) Raylib.DrawText(text, (int)pos.X - 20, (int)pos.Y - 20, 15, new Color(255, 255, 255, 127));
    }

    public void RenderFromSimulation(SimulationRenderer simulationRenderer)
    {
        var simulation = simulationRenderer.Simulation;
        var morphogenIdForColors = simulationRenderer.morphogenIdForColors;
        var max = simulation.maxConcentration;

        foreach (var cellPair in simulation.Cells)
        {
            float r = cellPair.Value.GetMorphogen((int)morphogenIdForColors.X);
            float g = cellPair.Value.GetMorphogen((int)morphogenIdForColors.Y);
            float b = cellPair.Value.GetMorphogen((int)morphogenIdForColors.Z);

            // string morphoText;
            // morphoText = string.Empty;
            // morphoText = $"{r:F2}\n{g:F2}\n{b:F2}";
            // morphoText = $"{cellPair.Key}";

            var morphoColor = new Color(r / max, g / max, b / max);
            Render(cellPair.Key.q, cellPair.Key.r, morphoColor, morphoColor);
        }
    }

    public void RenderRectangle(HexCoords start, HexCoords end, Color fillColor, Color outlineColor)
    {
        var qDiff = end.q - start.q + 1;
        var rDiff = end.r - start.r + 1 + (qDiff / 2);

        int q = start.q;
        int r = start.r;

        var current = new HexCoords(q, r);
        var newRowStart = GoRight(current);

        for (int i = 0; i < qDiff; i++)
        {
            newRowStart = GoRight(current);
            for (int j = 0; j < rDiff; j++)
            {
                Render(current.q, current.r, fillColor, outlineColor);
                current = GoUp(current);
            }
            current = newRowStart;
        }
    }

    HexCoords GoRight(HexCoords hexCoords)
    {
        if (hexCoords.q % 2 == 0) { return new HexCoords(hexCoords.q + 1, hexCoords.r); }
        else { return new HexCoords(hexCoords.q + 1, hexCoords.r - 1); }

    }
    HexCoords GoUp(HexCoords hexCoords)
    {
        return new HexCoords(hexCoords.q, hexCoords.r + 1);
    }
}