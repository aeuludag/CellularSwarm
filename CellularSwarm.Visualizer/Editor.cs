using System;
using CellularSwarm.Core;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public class Editor
{
    public SimulationRenderer renderer;
    private readonly static Random random = new();
    Dictionary<string, int> selectorStates = new();

    public Editor(SimulationRenderer renderer)
    {
        this.renderer = renderer;
    }

    public void ShowMorphogenEditor()
    {
        var key = "morphogenEditor";
        var simulation = renderer.Simulation;
        var morphogens = simulation.Morphogens;
        var morphogenId = morphogens.Keys.ToList()[0];

        if (ImGui.Begin("Morphogen Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.Button($"New##{key}"))
            {
                var newId = random.Next(int.MaxValue);
                while (morphogens.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }
                simulation.Morphogens.Add(newId, new Morphogen(newId, $"New Morphogen ({newId})", 1f, 0.1f));
                morphogenId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            morphogenId = Selector(key, $"Morphogen##{key}", morphogens.Keys.ToList(), (id) => morphogens[id].name);

            ImGui.Separator();

            var morphogen = morphogens[morphogenId];

            var name = morphogen.name;
            var difFac = morphogen.diffusionFactor;
            var decFac = morphogen.decayFactor;

            ImGui.InputText($"Name##{key}", ref name, 32);
            ImGui.SliderFloat($"Diffusion Factor##{key}", ref difFac, 0f, 1f);
            ImGui.SliderFloat($"Decay Factor##{key}", ref decFac, 0f, 1f);

            morphogens[morphogenId].name = name;
            morphogens[morphogenId].diffusionFactor = difFac;
            morphogens[morphogenId].decayFactor = decFac;
        }
        ImGui.End();
    }

    public void ShowGeneActionEditor()
    {
        var key = "geneActionEditor";

        var simulation = renderer.Simulation;
        var geneActions = simulation.GeneActions;
        var geneActionId = geneActions.Keys.ToList()[0];

        var max = simulation.maxConcentration;

        if (ImGui.Begin("Gene Action Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.Button($"New##{key}"))
            {
                var newId = random.Next(int.MaxValue);
                while (geneActions.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }

                simulation.GeneActions.Add(newId,
                    new GeneAction(newId, GeneAction.ActionType.Multiply, $"New Action ({newId})"));
                geneActionId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            geneActionId = Selector(key, $"Gene Action##{key}", geneActions.Keys.ToList(), (id) => geneActions[id].name);
            ImGui.Separator();

            var geneAction = geneActions[geneActionId];

            var name = geneAction.name;
            var actionType = geneAction.actionType;
            var cellTypeId = geneAction.cellTypeId;

            ImGui.InputText($"Name##{key}", ref name, 32);

            actionType = (GeneAction.ActionType)Selector($"actionTypeSelector##{key}", $"Action Type##{key}",
            [0, 1, 2, 3], (id) => ActionTypeToString((GeneAction.ActionType)id), (int)actionType);

            switch (actionType)
            {
                case GeneAction.ActionType.ChangeMorphogen:
                    DictionaryFloatEditor($"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, max);
                    break;
                case GeneAction.ActionType.Multiply:
                    DictionaryFloatEditor($"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.Apoptosis:
                    DictionaryFloatEditor($"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.ChangeCellType:
                    cellTypeId = Selector($"cellType##{key}", $"Cell Type##{key}", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);
                    break;
            }

            geneAction.actionType = actionType;
            geneAction.cellTypeId = cellTypeId;
        }
        ImGui.End();
    }

    public void ShowCellTypeEditor()
    {
        var key = "cellTypeEditor";
        var simulation = renderer.Simulation;
        var cellTypes = simulation.CellTypes;
        var cellTypeId = cellTypes.Keys.ToList()[0];

        if (ImGui.Begin("Cell Type Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.Button($"New##{key}"))
            {
                var newId = random.Next(int.MaxValue);
                while (cellTypes.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }
                simulation.CellTypes.Add(newId, new CellType(newId, $"New Cell Type ({newId})"));
                cellTypeId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            cellTypeId = Selector(key, $"Cell Type##{key}", cellTypes.Keys.ToList(), (id) => cellTypes[id].name);

            ImGui.Separator();

            var cellType = cellTypes[cellTypeId];

            var name = cellType.name;

            ImGui.InputText($"Name##{key}", ref name, 32);

            cellTypes[cellTypeId] = new CellType(cellTypeId, name);
        }
        ImGui.End();
    }
    public void ShowGeneConditionEditor()
    {
        var key = "geneConditionEditor";
        var simulation = renderer.Simulation;
        var geneConditions = simulation.GeneConditions;
        var geneConditionId = geneConditions.Keys.ToList()[0];

        if (ImGui.Begin("Gene Condition Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.Button($"New##{key}"))
            {
                var newId = random.Next(int.MaxValue);
                while (geneConditions.ContainsKey(newId)) { newId = random.Next(int.MaxValue); }
                simulation.GeneConditions.Add(newId, new NeighbourCondition(newId, false, false, 0, GeneCondition.ComparisonType.EqualsTo, $"New Gene Condition ({newId})"));
                geneConditionId = newId;
                selectorStates[key] = newId;
            }

            ImGui.SameLine();
            geneConditionId = Selector(key, $"Gene Condition##{key}", geneConditions.Keys.ToList(), (id) => geneConditions[id].name);

            ImGui.Separator();

            var geneCondition = geneConditions[geneConditionId];

            var name = geneCondition.name;
            var not = geneCondition.not;
            var strong = geneCondition.strong;

            ImGui.InputText($"Name##{key}", ref name, 32);
            ImGui.Checkbox($"Not##{key}", ref not);
            ImGui.Checkbox($"Strong##{key}", ref strong);

            if (geneCondition is ConcentrationCondition concentrationCondition)
            {
                var comparisonType = concentrationCondition.comparisonType;
                var morphogenId = concentrationCondition.morphogenId;
                var thresholdConcentration = concentrationCondition.thresholdConcentration;

                morphogenId = Selector($"morphogenSelector##{key}", $"Morphogen##{key}", simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, morphogenId);
                ImGui.SliderFloat($"Threshold##{key}", ref thresholdConcentration, 0f, simulation.maxConcentration);

                comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector##{key}", $"Comparison Type##{key}",
                [0, 1, 2], (id) => ComparisonTypeToString((GeneCondition.ComparisonType)id), (int)comparisonType);

                geneConditions[geneConditionId].name = name;
                geneConditions[geneConditionId].not = not;
                geneConditions[geneConditionId].strong = strong;
                ((ConcentrationCondition)geneConditions[geneConditionId]).comparisonType = comparisonType;
                ((ConcentrationCondition)geneConditions[geneConditionId]).morphogenId = morphogenId;
                ((ConcentrationCondition)geneConditions[geneConditionId]).thresholdConcentration = thresholdConcentration;

            }
            else if (geneCondition is CellTypeCondition cellTypeCondition)
            {
                var cellType = cellTypeCondition.cellType;
                var cellTypeId = cellType.id;
                cellTypeId = Selector($"cellType##{key}", $"Cell Type##{key}", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);

                cellType = simulation.CellTypes[cellTypeId];

                geneConditions[geneConditionId].name = name;
                geneConditions[geneConditionId].not = not;
                geneConditions[geneConditionId].strong = strong;
                ((CellTypeCondition)geneConditions[geneConditionId]).cellType = cellType;
            }
            else if (geneCondition is NeighbourCondition neighbourCondition)
            {
                var comparisonType = neighbourCondition.comparisonType;
                comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector##{key}", $"Comparison Type##{key}",
                [0, 1, 2], (id) => ComparisonTypeToString((GeneCondition.ComparisonType)id), (int)comparisonType);
                var threshold = neighbourCondition.threshold;

                ImGui.SliderInt("Neighbour Count", ref threshold, 0, 6);

                geneConditions[geneConditionId].name = name;
                geneConditions[geneConditionId].not = not;
                geneConditions[geneConditionId].strong = strong;
                ((NeighbourCondition)geneConditions[geneConditionId]).threshold = threshold;
                ((NeighbourCondition)geneConditions[geneConditionId]).comparisonType = comparisonType;
            }

        }
        ImGui.End();
    }

    int Selector(string key, string label, List<int> items, Func<int, string> getName, int? defaultItem = null)
    {
        // mostly gpt written
        if (defaultItem is not null)
        {
            selectorStates[key] = defaultItem ?? 0;
        }
        else if (!selectorStates.ContainsKey(key))
        {
            selectorStates[key] = items[0];
        }

        ImGui.PushID(key);

        int current = selectorStates[key];

        // if (!items.Contains(current)) current = items[0];

        string preview = (current >= 0) ? getName(current) : "None";
        if (ImGui.BeginCombo(label, preview))
        {
            foreach (var id in items)
            {
                bool selected = (id == current);
                if (ImGui.Selectable($"{getName(id)}##{id}", selected)) selectorStates[key] = id;
            }
            ImGui.EndCombo();
        }

        ImGui.PopID();

        return selectorStates[key];
    }

    void DictionaryFloatEditor(string label, Dictionary<int, float> dict, List<int> allItems, Func<int, string> getName, float max = 1f)
    {
        // mostly gpt written
        if (ImGui.BeginTable("##dict_table_" + label, 3, ImGuiTableFlags.None))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Value");
            ImGui.TableSetupColumn("Remove");
            ImGui.TableHeadersRow();

            foreach (var key in dict.Keys.ToList())
            {
                float value = dict[key];
                ImGui.PushID(key);

                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.Text(getName(key));

                ImGui.TableSetColumnIndex(1);
                ImGui.PushItemWidth(200);
                if (ImGui.SliderFloat("##value", ref value, 0f, max)) dict[key] = value;
                ImGui.PopItemWidth();

                ImGui.TableSetColumnIndex(2);
                if (ImGui.SmallButton(IconFonts.FontAwesome6.TrashCan + " Remove")) dict.Remove(key);

                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        ImGui.Separator();

        // --- Add new item combo ---
        if (ImGui.BeginCombo("Add Item", "Select..."))
        {
            foreach (var item in allItems)
            {
                if (dict.ContainsKey(item)) continue;

                if (ImGui.Selectable(getName(item)))
                    dict[item] = 0f; // default
            }
            ImGui.EndCombo();
        }
    }

    string ActionTypeToString(GeneAction.ActionType actionType)
    {
        return actionType switch
        {
            GeneAction.ActionType.ChangeMorphogen => "Change Morphogen",
            GeneAction.ActionType.ChangeCellType => "Change Cell Type",
            GeneAction.ActionType.Apoptosis => "Apoptosis",
            GeneAction.ActionType.Multiply => "Multiply",
            _ => "",
        };
    }

    string ComparisonTypeToString(GeneCondition.ComparisonType comparisonType)
    {
        return comparisonType switch
        {
            GeneCondition.ComparisonType.GreaterThan => ">",
            GeneCondition.ComparisonType.LessThan => "<",
            GeneCondition.ComparisonType.EqualsTo => "=",
            _ => "",
        };
    }
}