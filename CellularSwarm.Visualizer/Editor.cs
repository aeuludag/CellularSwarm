using System;
using System.Numerics;
using CellularSwarm.Core;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public class Editor
{
    public SimulationRenderer renderer;

    public bool showMorphogenEditor = false;
    public bool showCellTypeEditor = false;
    public bool showGeneActionEditor = false;
    public bool showGeneConditionEditor = false;
    public bool showGeneEditor = false;
    public bool showSimulationEditor = false;
    private readonly static Random random = new();
    Dictionary<string, int> selectorStates = new(); // help by gpt

    public Editor(SimulationRenderer renderer)
    {
        this.renderer = renderer;
    }

    public void ShowWindowManager()
    {
        if (ImGui.Begin("Window Manager", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Checkbox("Morphogen Editor", ref showMorphogenEditor);
            ImGui.Checkbox("Cell Type Editor", ref showCellTypeEditor);
            ImGui.Checkbox("Gene Action Editor", ref showGeneActionEditor);
            ImGui.Checkbox("Gene Condition Editor", ref showGeneConditionEditor);
            ImGui.Checkbox("Gene Editor", ref showGeneEditor);
            ImGui.Checkbox("Simulation Editor", ref showSimulationEditor);
            if (ImGui.Button("Reset Window Positions"))
            {

            }
        }
        ImGui.End();

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));
        if (showMorphogenEditor) ShowMorphogenEditor();
        if (showCellTypeEditor) ShowCellTypeEditor();
        if (showGeneActionEditor) ShowGeneActionEditor();
        if (showGeneConditionEditor) ShowGeneConditionEditor();
        if (showGeneEditor) ShowGeneEditor();
        if (showSimulationEditor) ShowSimulationEditor();
        ImGui.PopStyleVar();
    }

    public void ShowCellTypeEditor()
    {
        var key = "cellTypeEditor";
        var simulation = renderer.Simulation;
        var cellTypes = simulation.CellTypes;
        var cellTypeId = cellTypes.Keys.ToList()[0];

        if (ImGui.Begin("Cell Type Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);
            if (ImGui.Button($"New"))
            {
                var newId = simulation.Add(new CellType(simulation.DefaultCellType)).id;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            cellTypeId = Selector(key, $"Cell Type", cellTypes.Keys.ToList(), (id) => cellTypes[id].name);

            ImGui.SeparatorText("Properties");

            var cellType = cellTypes[cellTypeId];

            var name = cellType.name;

            ImGui.InputText($"Name", ref name, 32);

            cellTypes[cellTypeId] = new CellType(cellTypeId, name);

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowMorphogenEditor()
    {
        var key = "morphogenEditor";
        var simulation = renderer.Simulation;
        var morphogens = simulation.Morphogens;
        var morphogenId = morphogens.Keys.ToList()[0];

        if (ImGui.Begin("Morphogen Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            if (ImGui.Button($"New"))
            {
                var newId = simulation.Add(new Morphogen(simulation.DefaultMorphogen)).id;
                morphogenId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            morphogenId = Selector(key, $"Morphogen", morphogens.Keys.ToList(), (id) => morphogens[id].name);

            ImGui.SeparatorText("Properties");

            var morphogen = morphogens[morphogenId];

            var name = morphogen.name;
            var difFac = morphogen.diffusionFactor;
            var decFac = morphogen.decayFactor;

            ImGui.InputText($"Name", ref name, 32);
            ImGui.SliderFloat($"Diffusion Factor", ref difFac, 0f, 1f);
            ImGui.SliderFloat($"Decay Factor", ref decFac, 0f, 1f);

            morphogens[morphogenId].name = name;
            morphogens[morphogenId].diffusionFactor = difFac;
            morphogens[morphogenId].decayFactor = decFac;

            // if (morphogens.Count > 1)
            // {
            //     ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0f, 0f, 1f));
            //     ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0f, 0f, 1f));
            //     ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0f, 0f, 1f));
            //     if (ImGui.Button($"Delete##{morphogenId}"))
            //     {
            //         simulation.RemoveMorphogen(morphogenId);
            //     }
            //     ImGui.PopStyleColor(3);
            // }

            ImGui.PopID();
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
            ImGui.PushID(key);

            if (ImGui.Button($"New"))
            {
                var newId = simulation.Add(new GeneAction(simulation.DefaultGeneAction)).id;
                geneActionId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            geneActionId = Selector(key, $"Gene Action", geneActions.Keys.ToList(), (id) => geneActions[id].name);
            ImGui.SeparatorText("Properties");

            var geneAction = geneActions[geneActionId];

            var name = geneAction.name;
            var actionType = geneAction.actionType;
            var cellTypeId = geneAction.cellTypeId;

            ImGui.InputText($"Name", ref name, 32);

            actionType = (GeneAction.ActionType)Selector($"actionTypeSelector", $"Action Type",
            [0, 1, 2, 3], (id) => ActionTypeToString((GeneAction.ActionType)id), (int)actionType);

            switch (actionType)
            {
                case GeneAction.ActionType.ChangeMorphogen:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, max);
                    break;
                case GeneAction.ActionType.Multiply:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.Apoptosis:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.ChangeCellType:
                    cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);
                    break;
            }

            geneAction.actionType = actionType;
            geneAction.cellTypeId = cellTypeId;

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowGeneConditionEditor()
    {
        var key = "geneConditionEditor";
        var simulation = renderer.Simulation;
        var geneConditions = simulation.GeneConditions;
        var geneConditionId = geneConditions.Keys.ToList()[0];
        int geneConditionTypeId = 0;

        if (ImGui.Begin("Gene Condition Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            geneConditionTypeId = Selector($"type{key}", "##Condition Type", [0, 1, 2], GeneConditionTypeToString);
            ImGui.SameLine();
            if (ImGui.Button($"New"))
            {
                var newId = geneConditionTypeId switch
                {
                    0 => simulation.Add(simulation.DefaultConcentrationCondition.Clone()).id,
                    1 => simulation.Add(simulation.DefaultCellTypeCondition.Clone()).id,
                    2 => simulation.Add(simulation.DefaultNeighbourCondition.Clone()).id,
                    _ => simulation.Add(simulation.DefaultGeneCondition.Clone()).id
                };
                geneConditionId = newId;
                selectorStates[key] = newId;
            }

            ImGui.Separator();

            geneConditionId = Selector(key, $"Gene Condition", geneConditions.Keys.ToList(), (id) => geneConditions[id].name);

            var geneCondition = geneConditions[geneConditionId];

            var name = geneCondition.name;
            var not = geneCondition.not;
            var strong = geneCondition.strong;

            ImGui.SeparatorText("Properties");

            ImGui.InputText($"Name", ref name, 32);
            ImGui.Checkbox($"Not", ref not);
            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10, 0));
            ImGui.SameLine();
            ImGui.Checkbox($"Strong", ref strong);

            if (geneCondition is ConcentrationCondition concentrationCondition)
            {
                var comparisonType = concentrationCondition.comparisonType;
                var morphogenId = concentrationCondition.morphogenId;
                var thresholdConcentration = concentrationCondition.thresholdConcentration;

                morphogenId = Selector($"morphogenSelector", $"Morphogen", simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, morphogenId);
                ImGui.SliderFloat($"threshold", ref thresholdConcentration, 0f, simulation.maxConcentration);

                comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector", $"Comparison Type",
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
                cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);

                cellType = simulation.CellTypes[cellTypeId];

                geneConditions[geneConditionId].name = name;
                geneConditions[geneConditionId].not = not;
                geneConditions[geneConditionId].strong = strong;
                ((CellTypeCondition)geneConditions[geneConditionId]).cellType = cellType;
            }
            else if (geneCondition is NeighbourCondition neighbourCondition)
            {
                var comparisonType = neighbourCondition.comparisonType;
                comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector", $"Comparison Type",
                [0, 1, 2], (id) => ComparisonTypeToString((GeneCondition.ComparisonType)id), (int)comparisonType);
                var threshold = neighbourCondition.threshold;

                ImGui.SliderInt("Neighbour Count", ref threshold, 0, 6);

                geneConditions[geneConditionId].name = name;
                geneConditions[geneConditionId].not = not;
                geneConditions[geneConditionId].strong = strong;
                ((NeighbourCondition)geneConditions[geneConditionId]).threshold = threshold;
                ((NeighbourCondition)geneConditions[geneConditionId]).comparisonType = comparisonType;
            }
            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowGeneEditor()
    {
        var key = "geneEditor";
        var simulation = renderer.Simulation;
        var genes = simulation.Genes;
        var geneConditions = simulation.GeneConditions.Values.ToList();
        var geneId = genes.Keys.ToList()[0];

        if (ImGui.Begin("Gene Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            if (ImGui.Button($"New"))
            {
                var newId = simulation.Add(new Gene(simulation.DefaultGene)).id;
                geneId = newId;
                selectorStates[key] = newId;
            }
            ImGui.SameLine();
            geneId = Selector(key, $"Gene", genes.Keys.ToList(), (id) => genes[id].name);

            var gene = genes[geneId];
            var name = gene.name;

            ImGui.SeparatorText("Properties");

            ImGui.InputText($"Name", ref name, 32);

            ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(0.7f, 1f, 0.7f, 1f));
            ImGui.SeparatorText("Activator Conditions");
            ListEditor($"activator##{key}", "Activator Conditions", gene.activatorConditions, geneConditions, (condition) => condition.name);
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(1f, 0.7f, 0.7f, 1f));
            ImGui.SeparatorText("Inhibitor Conditions");
            ListEditor($"inhibitor##{key}", "Inhibitor Conditions", gene.inhibitorConditions, geneConditions, (condition) => condition.name);
            ImGui.PopStyleColor();

            genes[geneId].name = name;

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowSimulationEditor()
    {
        var key = "simulationEditor";
        var simulation = renderer.Simulation;
        var morphogens = simulation.Morphogens;
        var diffuser = simulation.Diffuser;

        if (ImGui.Begin("Simulation Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            var name = simulation.name;
            var diffusionSteps = simulation.diffusionSteps;

            var diffusionThreshold = diffuser.diffusionThreshold;
            var diffusionFactor = diffuser.diffusionFactor;

            ImGui.SeparatorText("Properties");

            ImGui.InputText("Name", ref name, 32);
            ImGui.InputFloat("Diffusion Threshold", ref diffusionThreshold);
            ImGui.InputFloat("Diffusion Factor", ref diffusionFactor);
            ImGui.InputInt("Diffusion Steps", ref diffusionSteps);

            ImGui.SeparatorText("Visualization");

            var morphogenKeys = morphogens.Keys.ToList();

            morphogenKeys.Remove(renderer.redMorphogenId);
            morphogenKeys.Remove(renderer.greenMorphogenId);
            morphogenKeys.Remove(renderer.blueMorphogenId);

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.7f, 0.7f, 1f));
            renderer.redMorphogenId = SoftSelector("redMorphogen", "Red", morphogenKeys, (id) => morphogens[id].name, renderer.redMorphogenId);
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 1f, 0.7f, 1f));
            renderer.greenMorphogenId = SoftSelector("greenMorphogen", "Green", morphogenKeys, (id) => morphogens[id].name, renderer.greenMorphogenId);
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.7f, 1f, 1f));
            renderer.blueMorphogenId = SoftSelector("blueMorphogen", "Blue", morphogenKeys, (id) => morphogens[id].name, renderer.blueMorphogenId);
            ImGui.PopStyleColor();

            ImGui.SeparatorText("Stats");

            ImGui.BeginDisabled();
            ImGui.Text($"Cell Count: {simulation.Cells.Count}");
            ImGui.EndDisabled();

            simulation.name = name;
            simulation.diffusionSteps = Math.Max(diffusionSteps, 0);
            diffuser.diffusionThreshold = Math.Max(diffusionThreshold, 0);
            diffuser.diffusionFactor = Math.Max(diffusionFactor, 0);

            ImGui.PopID();
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

        if (!items.Contains(current)) current = items[0];

        string preview = (current >= 0) ? getName(current) : "None";
        //  $"{label} - ({current})" OR $"{label}"
        if (ImGui.BeginCombo($"{label} - ({current})", preview))
        {
            foreach (var id in items)
            {
                bool selected = (id == current);
                if (ImGui.Selectable($"{getName(id)}##{id}", selected)) selectorStates[key] = id;
            }
            ImGui.EndCombo();
        }

        ImGui.PopID();

        if (items.Contains(selectorStates[key]))
        {
            return selectorStates[key];
        }
        selectorStates[key] = items[0];
        return items[0];
    }

    int SoftSelector(string key, string label, List<int> items, Func<int, string> getName, int? defaultItem = null)
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

        string preview = (current >= 0) ? getName(current) : "None";
        if (ImGui.BeginCombo($"{label} - ({current})", preview))
        {
            foreach (var id in items)
            {
                bool selected = (id == current);
                if (ImGui.Selectable($"{getName(id)}##{id}", selected)) selectorStates[key] = id;
            }

            if (ImGui.Selectable("None##none", current <= 0)) { selectorStates[key] = -1; }
            ImGui.EndCombo();
        }

        ImGui.PopID();

        return selectorStates[key];
    }

    void DictionaryFloatEditor(string key, string label, Dictionary<int, float> dict, List<int> allItems, Func<int, string> getName, float max = 1f)
    {
        ImGui.PushID(key);
        // mostly gpt written
        if (ImGui.BeginTable($"{label}##{key}", 3, ImGuiTableFlags.None))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Value");
            ImGui.TableSetupColumn("Remove");
            ImGui.TableHeadersRow();

            foreach (var dictKey in dict.Keys.ToList())
            {
                float value = dict[dictKey];
                ImGui.PushID(dictKey);

                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.Text(getName(dictKey));

                ImGui.TableSetColumnIndex(1);
                ImGui.PushItemWidth(200);
                if (ImGui.SliderFloat("##value", ref value, 0f, max)) dict[dictKey] = value;
                ImGui.PopItemWidth();

                ImGui.TableSetColumnIndex(2);
                if (ImGui.SmallButton(IconFonts.FontAwesome6.TrashCan + " Remove")) dict.Remove(dictKey);

                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        // ImGui.Separator();

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
        ImGui.PopID();
    }

    void ListEditor<T>(string key, string label, List<T> list, List<T> allItems, Func<T, string> getName)
    {
        ImGui.PushID(key);
        // mostly gpt written

        if (ImGui.BeginTable($"{label}##{key}", 2, ImGuiTableFlags.Reorderable))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Remove");
            ImGui.TableHeadersRow();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                ImGui.PushID(i);

                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);

                ImGui.Text($"{getName(item)}");

                ImGui.TableSetColumnIndex(1);
                if (ImGui.Button(IconFonts.FontAwesome6.TrashCan + " Remove")) list.Remove(item);

                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        // ImGui.Separator();

        // --- Add new item combo ---
        if (ImGui.BeginCombo("Add Item", "Select..."))
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                // ImGui.PushID(i);

                var item = allItems[i];
                if (list.Contains(item)) continue;

                if (ImGui.Selectable($"{getName(item)}##{i}"))
                    list.Add(item);

                // ImGui.PopID();
            }
            ImGui.EndCombo();
        }
        ImGui.PopID();
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
            GeneCondition.ComparisonType.GreaterThan => "Greater Than (>)",
            GeneCondition.ComparisonType.LessThan => "Less Than (<)",
            GeneCondition.ComparisonType.EqualsTo => "Equal To (=)",
            _ => "",
        };
    }

    string GeneConditionTypeToString(int id)
    {
        return id switch
        {
            0 => "Concentration Condition",
            1 => "Cell Type Condition",
            2 => "Neighbour Condition",
            _ => ""
        };
    }

    void Space(int space = 5)
    {
        ImGui.Dummy(new Vector2(0, space));
    }
}