using System;

namespace CellularSwarm.Core;

public class DiffusionHandler
{
    // Gets the current state of the simulation. Each cell will trade morphogens between neighbours.
    // This is called by the Simulation class.
    // The trading will occur when a certain difference threshold is reached.
    // Then it will share its morphogens with its neighbours proportional to the difference in concentration and the diffusion factor.

    public float diffusionThreshold = 0.1f;
    public float diffusionFactor = 1f;

    public Simulation Simulation { get; }

    public DiffusionHandler(Simulation simulation)
    {
        this.Simulation = simulation;
    }

    // <(HexCoords, int), float> would be better
    public void Diffuse()
    {
        var morphogenDelta = new Dictionary<HexCoords, Dictionary<int, float>>();

        foreach (var cellPair in Simulation.Cells)
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
                    var neighbour = Simulation.Cells[neighbourCoords];

                    var diff = morphogenConcentration - neighbour.GetMorphogenAmount(morphogenId);

                    if (diff <= diffusionThreshold) continue;

                    neighboursToShare++;

                    var rawShareAmount = diff * Simulation.Morphogens[morphogenId].diffusionFactor * diffusionFactor;
                    tempMorphogenDelta[neighbourCoords] = rawShareAmount;
                }

                if (neighboursToShare <= 0) continue;

                var totalShareAmount = 0f;

                foreach (var neighbourCoords in tempMorphogenDelta.Keys)
                {
                    var shareAmount = tempMorphogenDelta[neighbourCoords] / (neighboursToShare + 1);

                    totalShareAmount += shareAmount;

                    AddMorphogenTo(morphogenDelta, neighbourCoords, morphogenId, shareAmount);
                }

                AddMorphogenTo(morphogenDelta, coords, morphogenId, -totalShareAmount);
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

                Simulation.Cells[coords].SetMorphogen(morphogenId, Simulation.Cells[coords].GetMorphogenAmount(morphogenId) + delta);
            }
        }
    }

    public void DiffuseAllOf(HexCoords coords)
    {
        var morphogenDelta = new Dictionary<HexCoords, Dictionary<int, float>>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighbours(coords);
        var neighbourCount = neighbours.Count;

        if (neighbourCount == 0) return;

        foreach (var morphogenPair in cell.Morphogens)
        {
            var amount = morphogenPair.Value;
            var shareAmount = amount / neighbourCount;

            foreach (var neighbourCoords in neighbours)
            {
                AddMorphogenTo(morphogenDelta, neighbourCoords, morphogenPair.Key, shareAmount);
            }
        }

        foreach (var morphogenPair in morphogenDelta)
        {
            var cellCoords = morphogenPair.Key;
            var deltas = morphogenPair.Value;

            foreach (var deltaPair in deltas)
            {
                var morphogenId = deltaPair.Key;
                var delta = deltaPair.Value;

                Simulation.Cells[cellCoords].SetMorphogen(morphogenId, Simulation.Cells[cellCoords].GetMorphogenAmount(morphogenId) + delta);
            }
        }

    }

    void AddMorphogenTo(Dictionary<HexCoords, Dictionary<int, float>> delta, HexCoords coords, int id, float amount)
    {
        if (delta.ContainsKey(coords))
        {
            if (!delta[coords].TryAdd(id, amount))
            {
                delta[coords][id] += amount;
            }
        }
        else
        {
            delta.Add(coords, new Dictionary<int, float>());
            delta[coords].Add(id, amount);
        }
    }

    // still not very familiar with parallel programming so I used the help of gpt a bit
    public void DiffuseParallel()
    {
        var allLocalDeltas = new List<Dictionary<(HexCoords, int), float>>();

        Parallel.ForEach(Simulation.Cells, () => new Dictionary<(HexCoords, int), float>(),
            (cellPair, state, localDelta) =>
            {
                var delta = DiffuseSingular(cellPair.Key);
                // still dont know much about parallel methods. 
                foreach (var kv in delta)
                {
                    if (localDelta.TryGetValue(kv.Key, out var existing))
                        localDelta[kv.Key] = existing + kv.Value;
                    else
                        localDelta[kv.Key] = kv.Value;
                }
                return localDelta;
            },
            localDelta =>
            {
                lock (allLocalDeltas) { allLocalDeltas.Add(localDelta); }
            });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        foreach (var localDelta in allLocalDeltas)
        {
            foreach (((HexCoords coords, int morphogenId), float amount) in localDelta)
            {
                AddMorphogenTo(morphogenDelta, coords, morphogenId, amount);
            }
        }

        foreach (((HexCoords coords, int morphogenId), float amount) in morphogenDelta)
        {
            Simulation.Cells[coords].AddMorphogen(morphogenId, amount);
        }
    }

    public void DiffuseCollectionParallel(List<HexCoords> cellsToDiffuse)
    {
        var allLocalDeltas = new List<Dictionary<(HexCoords, int), float>>();

        Parallel.ForEach(cellsToDiffuse, () => new Dictionary<(HexCoords, int), float>(),
            (coords, state, localDelta) =>
            {
                var delta = DiffuseSingular(coords);
                // still dont know much about parallel methods. 
                foreach (var kv in delta)
                {
                    if (localDelta.TryGetValue(kv.Key, out var existing))
                        localDelta[kv.Key] = existing + kv.Value;
                    else
                        localDelta[kv.Key] = kv.Value;
                }
                return localDelta;
            },
            localDelta =>
            {
                lock (allLocalDeltas) { allLocalDeltas.Add(localDelta); }
            });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        foreach (var localDelta in allLocalDeltas)
        {
            foreach (((HexCoords coords, int morphogenId), float amount) in localDelta)
            {
                AddMorphogenTo(morphogenDelta, coords, morphogenId, amount);
            }
        }

        foreach (((HexCoords coords, int morphogenId), float amount) in morphogenDelta)
        {
            Simulation.Cells[coords].AddMorphogen(morphogenId, amount);
        }
    }

    public void DiffuseAllOfCollectionParallel(List<HexCoords> cellsToDiffuse)
    {
        var allLocalDeltas = new List<Dictionary<(HexCoords, int), float>>();

        Parallel.ForEach(cellsToDiffuse, () => new Dictionary<(HexCoords, int), float>(),
            (coords, state, localDelta) =>
            {
                var delta = DiffuseAllOfSingular(coords);
                // still dont know much about parallel methods. 
                foreach (var kv in delta)
                {
                    if (localDelta.TryGetValue(kv.Key, out var existing))
                        localDelta[kv.Key] = existing + kv.Value;
                    else
                        localDelta[kv.Key] = kv.Value;
                }
                return localDelta;
            },
            localDelta =>
            {
                lock (allLocalDeltas) { allLocalDeltas.Add(localDelta); }
            });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        foreach (var localDelta in allLocalDeltas)
        {
            foreach (((HexCoords coords, int morphogenId), float amount) in localDelta)
            {
                AddMorphogenTo(morphogenDelta, coords, morphogenId, amount);
            }
        }

        foreach (((HexCoords coords, int morphogenId), float amount) in morphogenDelta)
        {
            Simulation.Cells[coords].AddMorphogen(morphogenId, amount);
        }
    }

    public Dictionary<(HexCoords, int), float> DiffuseSingular(HexCoords coords)
    {
        var delta = new Dictionary<(HexCoords, int), float>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighbours(coords);
        var neighbourCount = neighbours.Count;

        var morphogens = Simulation.Morphogens;
        var cells = Simulation.Cells;

        if (neighbourCount == 0) return delta;

        foreach ((int morphogenId, float amount) in cell.Morphogens)
        {
            var tempDelta = new List<(HexCoords, float)>(6);
            float totalShareAmount = 0f;
            int neighboursToShare = 0;

            foreach (var neighbourCoords in neighbours)
            {
                var neighbour = cells[neighbourCoords];
                var neighbourMorphogenAmount = neighbour.GetMorphogenAmount(morphogenId);

                var diff = amount - neighbourMorphogenAmount;

                if (diff < diffusionThreshold) continue;

                neighboursToShare++;

                var rawShareAmount = diff * morphogens[morphogenId].diffusionFactor * diffusionFactor;
                tempDelta.Add((neighbourCoords, rawShareAmount));
            }

            if (neighboursToShare == 0) continue;

            float shareFactor = 1f / (neighboursToShare + 1f);

            foreach ((HexCoords neighbourCoords, float rawShareAmount) in tempDelta)
            {
                float shareAmount = shareFactor * rawShareAmount;
                AddMorphogenTo(delta, neighbourCoords, morphogenId, shareAmount);

                totalShareAmount += shareAmount;
            }

            AddMorphogenTo(delta, coords, morphogenId, -totalShareAmount);
        }

        return delta;
    }

    public Dictionary<(HexCoords, int), float> DiffuseAllOfSingular(HexCoords coords)
    {
        var delta = new Dictionary<(HexCoords, int), float>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighbours(coords);
        var neighbourCount = neighbours.Count;

        if (neighbourCount == 0) return delta;

        foreach ((int morphogenId, float amount) in cell.Morphogens)
        {
            var shareAmount = amount / neighbourCount;

            foreach (var neighbourCoords in neighbours)
            {
                AddMorphogenTo(delta, neighbourCoords, morphogenId, shareAmount);
            }

            AddMorphogenTo(delta, coords, morphogenId, -amount);
        }

        return delta;
    }

    public void ActiveTransportationCollectionParallel(List<HexCoords> cellsThatTransport)
    {
        var allLocalDeltas = new List<Dictionary<(HexCoords, int), float>>();

        Parallel.ForEach(cellsThatTransport, () => new Dictionary<(HexCoords, int), float>(),
            (coords, state, localDelta) =>
            {
                var delta = ActiveTransportationSingular(coords);
                // still dont know much about parallel methods. 
                foreach (var kv in delta)
                {
                    if (localDelta.TryGetValue(kv.Key, out var existing))
                        localDelta[kv.Key] = existing + kv.Value;
                    else
                        localDelta[kv.Key] = kv.Value;
                }
                return localDelta;
            },
            localDelta =>
            {
                lock (allLocalDeltas) { allLocalDeltas.Add(localDelta); }
            });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        foreach (var localDelta in allLocalDeltas)
        {
            foreach (((HexCoords coords, int morphogenId), float amount) in localDelta)
            {
                AddMorphogenTo(morphogenDelta, coords, morphogenId, amount);
            }
        }

        foreach (((HexCoords coords, int morphogenId), float amount) in morphogenDelta)
        {
            Simulation.Cells[coords].AddMorphogen(morphogenId, amount);
        }
    }
    
    public Dictionary<(HexCoords, int), float> ActiveTransportationSingular(HexCoords coords)
    {
        var delta = new Dictionary<(HexCoords, int), float>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighbours(coords);
        var neighbourCount = neighbours.Count;

        var morphogens = Simulation.Morphogens;
        var cells = Simulation.Cells;

        if (neighbourCount == 0) return delta;

        // SEARCH IN ALL CELLS AND MORPHOGENS, WEIGHTED AVG FOR ALL POSITIVE RELATIVE BIASES, IGNORE NEGATIVE
        foreach ((int morphogenId, Morphogen morphogen) in morphogens)
        {
            var tempDelta = new List<(HexCoords, float)>(6);
            var totalTransportedAmount = 0f;
            var totalPositiveBiases = 0f;
            var amount = cell.GetMorphogenAmount(morphogenId);
            float maxRelativeBias = 0f;
            // int neighboursToShare = 0;

            foreach (var neighbourCoords in neighbours)
            {
                var neighbour = cells[neighbourCoords];

                if (!cell.shouldTransport && !neighbour.shouldTransport) continue;

                var neighbourMorphogenAmount = neighbour.GetMorphogenAmount(morphogenId);

                var cellBias = cell.GetTransportationBias(morphogenId);
                var neighbourBias = neighbour.GetTransportationBias(morphogenId);

                var relativeBias = (cellBias - neighbourBias) / 2; // value between -1 and +1

                if (relativeBias <= 0f) continue;

                totalPositiveBiases += relativeBias;
                if (relativeBias > maxRelativeBias) maxRelativeBias = relativeBias;
                // neighboursToShare++;

                var rawTransportAmount = amount * relativeBias;
                tempDelta.Add((neighbourCoords, rawTransportAmount));
            }

            float transportFactor = maxRelativeBias / totalPositiveBiases;
            // float transportFactor = 1f / (totalPositiveBiases + 1); // one for the road (jk, + 1 bcs it shouldnt just give all right? No idk.)

            foreach ((HexCoords neighbourCoords, float rawTransportAmount) in tempDelta)
            {
                float transportAmount = transportFactor * rawTransportAmount; // in the end = diff * (relativeBias / totalBias)
                AddMorphogenTo(delta, neighbourCoords, morphogenId, transportAmount);

                totalTransportedAmount += transportAmount;
            }

            AddMorphogenTo(delta, coords, morphogenId, -totalTransportedAmount);
        }

        return delta;
    }

    void AddMorphogenTo(Dictionary<(HexCoords, int), float> delta, HexCoords coords, int morphogenId, float amount)
    {

        if (delta.ContainsKey((coords, morphogenId)))
        {
            delta[(coords, morphogenId)] += amount;
        }
        else
        {
            delta[(coords, morphogenId)] = amount;
        }
    }
}