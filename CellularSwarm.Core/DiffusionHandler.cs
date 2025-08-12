using System;

namespace CellularSwarm.Core;

public class DiffusionHandler
{
    // Gets the current state of the simulation. Each cell will trade morphogens between neighbours.
    // This is called by the Simulation class.
    // The trading will occur when a certain difference threshold is reached.
    // Then it will share its morphogens with its neighbours proportional to the difference in concentration and the diffusion factor.

    public static float DiffusionThreshold { get; set; } = 10f;

    public static void Diffuse(Simulation simulation)
    {
        var morphogenDelta = new Dictionary<HexCoords, Dictionary<int, float>>();

        foreach (var cellPair in simulation.cells)
        {
            var coords = cellPair.Key;
            var cell = cellPair.Value;
            var morphogens = cell.Morphogens;

            var neighbours = simulation.GetNeighbours(coords);

            foreach (var morphogenPair in morphogens)
            {
                var morphogenId = morphogenPair.Key;
                var morphogenConcentration = morphogenPair.Value;
                int neighboursToShare = 0;
                var tempMorphogenDelta = new Dictionary<HexCoords, float>();

                foreach (var neighbourCoords in neighbours)
                {
                    var neighbour = simulation.cells[neighbourCoords];

                    var diff = morphogenConcentration - neighbour.GetMorphogen(morphogenId);

                    if (diff <= DiffusionThreshold) continue;

                    neighboursToShare++;

                    var rawShareAmount = diff * simulation.Morphogens[morphogenId].diffusionFactor;
                    tempMorphogenDelta[neighbourCoords] = rawShareAmount;
                }

                if (neighboursToShare > 0)
                {
                    var totalShareAmount = tempMorphogenDelta.Values.Sum();
                    foreach (var neighbourCoords in tempMorphogenDelta.Keys)
                    {
                        var shareAmount = tempMorphogenDelta[neighbourCoords] / (neighboursToShare + 1);
                        morphogenDelta[neighbourCoords][morphogenId] = shareAmount;
                        morphogenDelta[coords][morphogenId] = -shareAmount;
                    }
                }
            }
        }

        foreach (var morphogenPair in morphogenDelta)
        {
            var coords = morphogenPair.Key;
            var deltas = morphogenPair.Value;

            foreach (var deltaPair in deltas)
            {
                var morphogenId = deltaPair.Key;
                var delta = deltaPair.Value;

                simulation.cells[coords].SetMorphogen(morphogenId, simulation.cells[coords].GetMorphogen(morphogenId) + delta);
            }
        }
    }
}
