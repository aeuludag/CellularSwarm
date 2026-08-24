using System;
using System.Collections.Concurrent;

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

            var neighbours = Simulation.GetNeighboursNonAlloc(coords);

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
    
    /*  I am sorry.
    *   I am sorry for what I must have done.
    *   From the very beginning of this project, I was strictly against using AI for the fun bits of simulation.
    *   I did not want to leave the fun of implementing the logics to the AI.
    *   I wanted to feel proud.
    *   Proud of what I succesfully do.
    *   But now, with this devils hand disguised as a memory exception,
    *   the manifestation of hells anguish in this world,
    *   I had no option left other than to use help of Gemini.
    *   By help, I meant complete rewrite while not fully understanding it.
    *   If someone were to come up and ask me to rewrite, I would not be able to.
    *   With that, only non-parallel methods remain sanitized.
    *   Lord forgive me for the sins I am commiting.
    *   God forgive me.
    *   -emir, 24th Aug 2026
    */
    public void DiffuseParallel(KeyValuePair<HexCoords, Cell>[] cellArray)
    {
        var allLocalDeltas = new System.Collections.Concurrent.ConcurrentQueue<Dictionary<(HexCoords, int), float>>();

        ParallelChunker.Run(cellArray, (start, end) =>
        {
            for (int j = start; j < end; j++)
            {
                var delta = DiffuseSingular(cellArray[j].Key);
                allLocalDeltas.Enqueue(delta);
            }
        });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        while (allLocalDeltas.TryDequeue(out var localDelta))
        {
            foreach (var kvp in localDelta)
            {
                AddMorphogenTo(morphogenDelta, kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
            }
        }

        foreach (var kvp in morphogenDelta)
        {
            Simulation.Cells[kvp.Key.Item1].AddMorphogen(kvp.Key.Item2, kvp.Value);
        }
    }

    public Dictionary<(HexCoords, int), float> DiffuseSingular(HexCoords coords)
    {
        var delta = new Dictionary<(HexCoords, int), float>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighboursNonAlloc(coords);
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

    public void ActiveTransportationCollection(List<HexCoords> cellsThatTransport)
    {
        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        foreach (var coords in cellsThatTransport)
        {
            var delta = ActiveTransportationSingular(coords);

            foreach (var kv in delta)
            {
                if (morphogenDelta.TryGetValue(kv.Key, out var existing))
                    morphogenDelta[kv.Key] = existing + kv.Value;
                else
                    morphogenDelta[kv.Key] = kv.Value;
            }
        }

        foreach (((HexCoords coords, int morphogenId), float amount) in morphogenDelta)
        {
            Simulation.Cells[coords].AddMorphogen(morphogenId, amount);
        }
    }

    public void ActiveTransportationCollectionParallel(HexCoords[] cellsThatTransport)
    {
        var allLocalDeltas = new System.Collections.Concurrent.ConcurrentQueue<Dictionary<(HexCoords, int), float>>();

        ParallelChunker.Run(cellsThatTransport, (start, end) =>
        {
            var localDelta = new Dictionary<(HexCoords, int), float>();

            for (int j = start; j < end; j++)
            {
                var coords = cellsThatTransport[j];
                var delta = ActiveTransportationSingular(coords);
                
                foreach (var kv in delta)
                {
                    if (localDelta.TryGetValue(kv.Key, out var existing))
                        localDelta[kv.Key] = existing + kv.Value;
                    else
                        localDelta[kv.Key] = kv.Value;
                }
            }
            
            allLocalDeltas.Enqueue(localDelta);
        });

        var morphogenDelta = new Dictionary<(HexCoords, int), float>();

        while (allLocalDeltas.TryDequeue(out var localDelta))
        {
            foreach (var kvp in localDelta)
            {
                AddMorphogenTo(morphogenDelta, kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
            }
        }

        foreach (var kvp in morphogenDelta)
        {
            Simulation.Cells[kvp.Key.Item1].AddMorphogen(kvp.Key.Item2, kvp.Value);
        }
    }
    
    public Dictionary<(HexCoords, int), float> ActiveTransportationSingular(HexCoords coords)
    {
        var delta = new Dictionary<(HexCoords, int), float>();

        var cell = Simulation.Cells[coords];
        var neighbours = Simulation.GetNeighboursNonAlloc(coords);
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