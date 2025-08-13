using System;

namespace CellularSwarm.Core;

public class DiffusionHandler
{
    // Gets the current state of the simulation. Each cell will trade morphogens between neighbours.
    // This is called by the Simulation class.
    // The trading will occur when a certain difference threshold is reached.
    // Then it will share its morphogens with its neighbours proportional to the difference in concentration and the diffusion factor.

    public float DiffusionThreshold { get; set; } = 0.1f;

    public Simulation Simulation { get; }

    public DiffusionHandler(Simulation simulation)
    {
        this.Simulation = simulation;
    }

    public void Diffuse()
    {
        var morphogenDelta = new Dictionary<HexCoords, Dictionary<int, float>>();

        foreach (var cellPair in Simulation.cells)
        {
            var coords = cellPair.Key;
            var cell = cellPair.Value;
            var morphogens = cell.Morphogens;

            var neighbours = Simulation.GetNeighbours(coords);

            foreach (var morphogenPair in morphogens)
            {
                var morphogenId = morphogenPair.Key;
                var morphogenConcentration = morphogenPair.Value;
                int neighboursToShare = 0;
                var tempMorphogenDelta = new Dictionary<HexCoords, float>();

                foreach (var neighbourCoords in neighbours)
                {
                    var neighbour = Simulation.cells[neighbourCoords];

                    var diff = morphogenConcentration - neighbour.GetMorphogen(morphogenId);

                    if (diff <= DiffusionThreshold) continue;

                    neighboursToShare++;

                    var rawShareAmount = diff * Simulation.Morphogens[morphogenId].diffusionFactor;
                    tempMorphogenDelta[neighbourCoords] = rawShareAmount;
                }

                if (neighboursToShare > 0)
                {
                    var totalShareAmount = 0f;
                    foreach (var neighbourCoords in tempMorphogenDelta.Keys)
                    {
                        var shareAmount = tempMorphogenDelta[neighbourCoords] / (neighboursToShare + 1);

                        totalShareAmount += shareAmount;

                        if (morphogenDelta.ContainsKey(neighbourCoords))
                        {
                            if (!morphogenDelta[neighbourCoords].TryAdd(morphogenId, shareAmount))
                            {
                                morphogenDelta[neighbourCoords][morphogenId] += shareAmount;
                            }
                        }
                        else
                        {
                            morphogenDelta.Add(neighbourCoords, new Dictionary<int, float>());
                            morphogenDelta[neighbourCoords].Add(morphogenId, shareAmount);
                        }
                    }

                    if (morphogenDelta.ContainsKey(coords))
                    {
                        if (morphogenDelta[coords].ContainsKey(morphogenId))
                        {
                            morphogenDelta[coords][morphogenId] -= totalShareAmount;
                        }
                        else
                        {
                            morphogenDelta[coords].Add(morphogenId, -totalShareAmount);
                        }
                    }
                    else
                    {
                        morphogenDelta.Add(coords, new Dictionary<int, float>());
                        morphogenDelta[coords].Add(morphogenId, -totalShareAmount);
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

                Simulation.cells[coords].SetMorphogen(morphogenId, Simulation.cells[coords].GetMorphogen(morphogenId) + delta);
            }
        }
    }
}
