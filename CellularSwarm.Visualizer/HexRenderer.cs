using Raylib_cs;
using System.Numerics;
using CellularSwarm.Core;

public class HexRenderer
{
    public float hexSize;
    public Camera2D camera;
    public Vector2 offset;
    public bool showText;
    private static readonly float Sqrt3 = MathF.Sqrt(3f);

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
        float y = hexSize * Sqrt3 * (r + (q / 2f)); // hex_size * r * math.sqrt(3) + hex_size * q * math.sqrt(3) / 2
        var pos = offset + new Vector2(x, -y);

        Raylib.DrawPoly(pos, 6, hexSize, 0, outlineColor);
        Raylib.DrawPoly(pos, 6, hexSize * 0.9f, 0, fillColor);

        // if (showText) Raylib.DrawText(text, (int)pos.X - 20, (int)pos.Y - 20, 15, new Color(255, 255, 255, 127));
    }

    public void Render(int q, int r, Color fillColor)
    {
        float x = hexSize * q * 3 / 2;
        float y = hexSize * Sqrt3 * (r + (q / 2f));
        var pos = offset + new Vector2(x, -y);

        Raylib.DrawPoly(pos, 6, hexSize, 0, fillColor);

        // if (showText) Raylib.DrawText(text, (int)pos.X - 20, (int)pos.Y - 20, 15, new Color(255, 255, 255, 127));
    }

    public void Render(float x, float y, Color fillColor)
    {
        var pos = offset + new Vector2(x, -y);
        Raylib.DrawPoly(pos, 6, hexSize, 0, fillColor);
    }
    
    public void Render(float x, float y, Color fillColor, float scale)
    {
        var pos = offset + new Vector2(x, -y);
        Raylib.DrawPoly(pos, 6, hexSize * scale, 0, fillColor);
    }

    public void RenderSimulationThreeMorphogens(SimulationRenderer simulationRenderer)
    {
        // int rendered = 0;
        Vector2 topLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), camera);
        Vector2 bottomRight = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);

        var simulation = simulationRenderer.Simulation;
        var max = simulation.maxConcentration / simulationRenderer.amplifier;

        foreach (var cellPair in simulation.Cells)
        {
            float x = hexSize * cellPair.Key.q * 3 / 2;
            float y = hexSize * (float)Sqrt3 * (cellPair.Key.r + (cellPair.Key.q / 2f));
            var pos = offset + new Vector2(x, -y);

            if (!IsOnScreen(pos, hexSize, topLeft, bottomRight)) continue;

            float r = simulationRenderer.redMorphogenId >= 0 ? cellPair.Value.GetMorphogenAmount(simulationRenderer.redMorphogenId) : 0;
            float g = simulationRenderer.greenMorphogenId >= 0 ? cellPair.Value.GetMorphogenAmount(simulationRenderer.greenMorphogenId) : 0;
            float b = simulationRenderer.blueMorphogenId >= 0 ? cellPair.Value.GetMorphogenAmount(simulationRenderer.blueMorphogenId) : 0;

            // string morphoText;
            // morphoText = string.Empty;
            // morphoText = $"{r:F2}\n{g:F2}\n{b:F2}";
            // morphoText = $"{cellPair.Key}";

            var morphoColor = new Color(r / max, g / max, b / max);
            Render(x, y, morphoColor, cellPair.Value.spawnedThisFrame ? 0.5f : 1f);
            // rendered++;
        }
        // Raylib.EndMode2D();
        // Raylib.DrawText(rendered.ToString(), 5, 80, 20, Color.Blue);
        // Raylib.BeginMode2D(camera);
    }

    public void RenderSimulationSingleMorphogen(SimulationRenderer simulationRenderer)
    {
        Vector2 topLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), camera);
        Vector2 bottomRight = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);

        var simulation = simulationRenderer.Simulation;
        var factor = simulationRenderer.amplifier / simulation.maxConcentration;

        foreach (var cellPair in simulation.Cells)
        {
            float x = hexSize * cellPair.Key.q * 3 / 2;
            float y = hexSize * (float)Sqrt3 * (cellPair.Key.r + (cellPair.Key.q / 2f));
            var pos = offset + new Vector2(x, -y);

            if (!IsOnScreen(pos, hexSize, topLeft, bottomRight)) continue;

            float w = simulationRenderer.singleMorphogenId >= 0 ? cellPair.Value.GetMorphogenAmount(simulationRenderer.singleMorphogenId) : 0f;
            w *= factor;

            var morphoColor = new Color(w, w, w);
            Render(x, y, morphoColor, cellPair.Value.spawnedThisFrame ? 0.5f : 1f);
        }
    }
    public void RenderSimulationCellTypes(SimulationRenderer simulationRenderer)
    {
        Vector2 topLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), camera);
        Vector2 bottomRight = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);

        var simulation = simulationRenderer.Simulation;
        var cellTypeColors = simulationRenderer.cellTypeColors;

        foreach (var cellPair in simulation.Cells)
        {
            float x = hexSize * cellPair.Key.q * 3 / 2;
            float y = hexSize * (float)Sqrt3 * (cellPair.Key.r + (cellPair.Key.q / 2f));
            var pos = offset + new Vector2(x, -y);

            if (!IsOnScreen(pos, hexSize, topLeft, bottomRight)) continue;

            var cellColor = cellTypeColors.GetValueOrDefault(cellPair.Value.cellType.id, Color.Black);
            Render(x, y, cellColor, cellPair.Value.spawnedThisFrame ? 0.5f : 1f);
        }
    }

    public void RenderSimulationGeneActivity(SimulationRenderer simulationRenderer)
    {
        Vector2 topLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), camera);
        Vector2 bottomRight = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);

        var simulation = simulationRenderer.Simulation;

        foreach (var cellPair in simulation.Cells)
        {
            float x = hexSize * cellPair.Key.q * 3 / 2;
            float y = hexSize * (float)Sqrt3 * (cellPair.Key.r + (cellPair.Key.q / 2f));
            var pos = offset + new Vector2(x, -y);

            if (!IsOnScreen(pos, hexSize, topLeft, bottomRight)) continue;

            var geneColor = simulation.Genes[simulationRenderer.geneId].ShouldBeActive(cellPair.Value) ? simulationRenderer.activeGeneColor : simulationRenderer.inactiveGeneColor;
            Render(x, y, geneColor, cellPair.Value.spawnedThisFrame ? 0.5f : 1f);
        }
    }

    public void RenderSimulationGeneConditionMet(SimulationRenderer simulationRenderer)
    {
        Vector2 topLeft = Raylib.GetScreenToWorld2D(new Vector2(0, 0), camera);
        Vector2 bottomRight = Raylib.GetScreenToWorld2D(new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), camera);

        var simulation = simulationRenderer.Simulation;

        foreach (var cellPair in simulation.Cells)
        {
            float x = hexSize * cellPair.Key.q * 3 / 2;
            float y = hexSize * (float)Sqrt3 * (cellPair.Key.r + (cellPair.Key.q / 2f));
            var pos = offset + new Vector2(x, -y);

            if (!IsOnScreen(pos, hexSize, topLeft, bottomRight)) continue;

            var geneConditionColor = simulation.GeneConditions[simulationRenderer.geneConditionId].IsMet(cellPair.Value) ? simulationRenderer.metConditionColor : simulationRenderer.notMetConditionColor;
            Render(x, y, geneConditionColor, cellPair.Value.spawnedThisFrame ? 0.5f : 1f);
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

    bool IsOnScreen(Vector2 pos, float radius, Vector2 topLeft, Vector2 bottomRight)
    {
        return !(pos.X + radius < topLeft.X ||
                 pos.X - radius > bottomRight.X ||
                 pos.Y + radius < topLeft.Y ||
                 pos.Y - radius > bottomRight.Y);
    }
}