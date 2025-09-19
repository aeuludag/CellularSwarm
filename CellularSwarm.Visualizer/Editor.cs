using System;
using Raylib_cs;
using System.Numerics;
using CellularSwarm.Core;
using ImGuiNET;

namespace CellularSwarm.Visualizer;

public class Editor
{
    public SimulationRenderer renderer;

    public int brushSize;
    public GridState gridState;

    public bool showMorphogenEditor = false;
    public bool showCellTypeEditor = false;
    public bool showGeneActionEditor = false;
    public bool showGeneConditionEditor = false;
    public bool showGeneEditor = false;
    public bool showSimulationEditor = false;
    public bool showInspector = false;
    public bool showCellEditor = false;
    public bool showGridEditor = true;
    public bool showVisualizationEditor = true;

    public HexCoords selectedCellCoords = new(int.MaxValue, int.MaxValue);

    private readonly static Random random = new();
    Dictionary<string, int> selectorStates = new(); // help by gpt
    private readonly Vector4 RED_WARNING = new(0.9f, 0f, 0f, 1f);
    private readonly Vector4 RED_LIGHT = new(1f, 0.7f, 0.7f, 1f);
    private readonly Vector4 RED_DARK = new(0.7f, 0.2f, 0.2f, 1f);
    private readonly Vector4 GREEN_LIGHT = new(0.7f, 1f, 0.7f, 1f);
    private readonly Vector4 GREEN_DARK = new(0.2f, 0.7f, 0.2f, 1f);
    private readonly Vector4 BLUE_LIGHT = new(0.7f, 0.7f, 1f, 1f);
    private readonly Vector4 BLUE_DARK = new(0.3f, 0f, 0f, 1f);
    private readonly Vector4 PURPLE_DARK = new(0.2f, 0.2f, 0.5f, 1f);

    public Editor(SimulationRenderer renderer)
    {
        this.renderer = renderer;
    }

    public void ShowWindowManager(HexCoords mouseHex)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));

        if (ImGui.Begin("Window Manager", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.Checkbox("Visualization Editor", ref showVisualizationEditor);
            ImGui.Checkbox("Morphogen Editor", ref showMorphogenEditor);
            ImGui.Checkbox("Cell Type Editor", ref showCellTypeEditor);
            ImGui.Checkbox("Gene Action Editor", ref showGeneActionEditor);
            ImGui.Checkbox("Gene Condition Editor", ref showGeneConditionEditor);
            ImGui.Checkbox("Gene Editor", ref showGeneEditor);
            ImGui.Checkbox("Simulation Editor", ref showSimulationEditor);
            if (ImGui.Button("Hide All"))
            {
                showMorphogenEditor = false;
                showCellTypeEditor = false;
                showGeneActionEditor = false;
                showGeneConditionEditor = false;
                showGeneEditor = false;
                showSimulationEditor = false;
            }
            ImGui.SameLine();
            if (ImGui.Button("Reset Windows"))
            {

            }
        }
        ImGui.End();

        if (showVisualizationEditor) ShowVisualizationEditor();
        if (showMorphogenEditor) ShowMorphogenEditor();
        if (showCellTypeEditor) ShowCellTypeEditor();
        if (showGeneActionEditor) ShowGeneActionEditor();
        if (showGeneConditionEditor) ShowGeneConditionEditor();
        if (showGeneEditor) ShowGeneEditor();
        if (showSimulationEditor) ShowSimulationEditor();
        if (showInspector) ShowInspectWindow(mouseHex);
        if (showCellEditor) ShowCellEditor(selectedCellCoords);
        if (showGridEditor) ShowGridEditor();

        ImGui.PopStyleVar();
    }

    public void ShowCellTypeEditor()
    {
        var key = "cellTypeEditor";
        var simulation = renderer.Simulation;
        var cellTypes = simulation.CellTypes;
        var cellTypeId = -1;

        if (ImGui.Begin("Cell Type Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            CloseButton(ref showCellTypeEditor);

            ImGui.SameLine();

            if (ImGui.Button($"New Cell Type"))
            {
                var newId = simulation.Add(new CellType(simulation.DefaultCellType)).id;
                selectorStates[key] = newId;
            }
            ImGui.Separator();

            cellTypeId = Selector(key, $"Cell Type", cellTypes.Keys.ToList(), (id) => cellTypes[id].name);

            ImGui.SeparatorText("Properties");

            var cellType = cellTypes[cellTypeId];

            var name = cellType.name;

            if (ImGui.InputText($"Name", ref name, 32)) { cellTypes[cellTypeId] = new CellType(cellTypeId, name); }

            List<string> referencedInConditions = new();
            List<string> referencedInActions = new();

            foreach (var condition in simulation.GeneConditions.Values)
            {
                if (condition is CellTypeCondition cellTypeCondition)
                {
                    if (cellTypeCondition.cellType == cellType) { referencedInConditions.Add(cellTypeCondition.name); }
                }
            }

            foreach (var action in simulation.GeneActions.Values)
            {
                if (action.actionType == GeneAction.ActionType.ChangeCellType)
                {
                    if (action.cellTypeId == cellTypeId) { referencedInActions.Add(action.name); }
                }
            }

            RemoveArea(cellTypes, cellTypeId, referencedInConditions.Count > 0 || referencedInActions.Count > 0, "Type");

            InfoHeader(() =>
            {
                if (referencedInConditions.Count > 0) WrappingText($"Used in Conditions: {string.Join(", ", referencedInConditions)}");
                if (referencedInActions.Count > 0) WrappingText($"Used in Actions: {string.Join(", ", referencedInActions)}");
                ImGui.Text($"ID: {cellTypeId}");
            });

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowMorphogenEditor()
    {
        var key = "morphogenEditor";
        var simulation = renderer.Simulation;
        var morphogens = simulation.Morphogens;
        var morphogenId = -1;

        if (ImGui.Begin("Morphogen Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            CloseButton(ref showMorphogenEditor);

            ImGui.SameLine();

            if (ImGui.Button($"New Morphogen"))
            {
                var newId = simulation.Add(new Morphogen(simulation.DefaultMorphogen)).id;
                morphogenId = newId;
                selectorStates[key] = newId;
            }

            // if (morphogens.Count == 0) { ImGui.PopID(); ImGui.End(); return; }

            ImGui.Separator();

            morphogenId = Selector(key, $"Morphogen", morphogens.Keys.ToList(), (id) => morphogens[id].name);

            ImGui.SeparatorText("Properties");

            var morphogen = morphogens[morphogenId];

            var name = morphogen.name;
            var difFac = morphogen.diffusionFactor;
            var decFac = morphogen.decayFactor;

            if (ImGui.InputText($"Name", ref name, 32)) { morphogen.name = name; }
            if (ImGui.SliderFloat($"Diffusion Factor", ref difFac, 0f, 1f)) { morphogen.diffusionFactor = difFac; }
            HoverTooltip("How fast the morphogen flows & diffuses.");
            if (ImGui.SliderFloat($"Decay Factor", ref decFac, 0f, 1f)) { morphogen.decayFactor = decFac; }
            HoverTooltip("How fast the morphogen decays.");

            List<string> referencedInConditions = new();
            List<string> referencedInActions = new();

            foreach (var condition in simulation.GeneConditions.Values)
            {
                if (condition is ConcentrationCondition concentrationCondition)
                {
                    if (concentrationCondition.morphogenId == morphogenId) { referencedInConditions.Add(concentrationCondition.name); }
                }
            }

            foreach (var action in simulation.GeneActions.Values)
            {
                if (action.actionType == GeneAction.ActionType.ChangeMorphogen)
                {
                    if (action.actionMorphogens.ContainsKey(morphogenId)) { referencedInActions.Add(action.name); }
                }
            }

            ImGui.Separator();
            RemoveArea(morphogens, morphogenId, referencedInConditions.Count > 0 || referencedInActions.Count > 0, "Morphogen");

            InfoHeader(() =>
            {
                if (referencedInConditions.Count > 0) WrappingText($"Used in Conditions: {string.Join(", ", referencedInConditions)}");
                if (referencedInActions.Count > 0) WrappingText($"Used in Actions: {string.Join(", ", referencedInActions)}");
                ImGui.Text($"ID: {morphogenId}");
            });

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

            CloseButton(ref showGeneActionEditor);

            ImGui.SameLine();

            if (ImGui.Button($"New Action"))
            {
                var newId = simulation.Add(new GeneAction(simulation.DefaultGeneAction)).id;
                geneActionId = newId;
                selectorStates[key] = newId;
            }

            ImGui.Separator();

            geneActionId = Selector(key, $"Gene Action", geneActions.Keys.ToList(), (id) => geneActions[id].name);
            ImGui.SeparatorText("Properties");

            var geneAction = geneActions[geneActionId];

            var name = geneAction.name;
            var actionType = geneAction.actionType;
            var cellTypeId = geneAction.cellTypeId;

            if (ImGui.InputText($"Name", ref name, 32)) { geneAction.name = name; }

            actionType = (GeneAction.ActionType)Selector($"actionTypeSelector", $"Action Type",
            [0, 1, 2, 3], (id) => ActionTypeToString((GeneAction.ActionType)id), (int)actionType);

            geneAction.actionType = actionType;

            switch (actionType)
            {
                case GeneAction.ActionType.ChangeMorphogen:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, -max, max);
                    break;
                case GeneAction.ActionType.Multiply:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.Apoptosis:
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.ChangeCellType:
                    cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);
                    geneAction.cellTypeId = cellTypeId;
                    break;
            }

            List<string> referencedInGenes = new();

            foreach (var gene in simulation.Genes.Values)
            {
                if (gene.actions.Contains(geneAction)) { referencedInGenes.Add(gene.name); }
            }

            ImGui.Separator();
            RemoveArea(geneActions, geneActionId, referencedInGenes.Count > 0, "Action");

            InfoHeader(() =>
            {
                if (referencedInGenes.Count > 0) WrappingText($"Used in Genes: {string.Join(", ", referencedInGenes)}");
                ImGui.Text($"ID: {geneActionId}");
            });
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

            CloseButton(ref showGeneConditionEditor);

            ImGui.SameLine();
            geneConditionTypeId = Selector($"type{key}", "##Condition Type", [0, 1, 2], GeneConditionTypeToString);
            ImGui.SameLine();
            if (ImGui.Button($"New Condition"))
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

            ImGui.SeparatorText("Properties");

            ImGui.InputText($"Name", ref name, 32);
            ImGui.Checkbox($"Not", ref not);
            HoverTooltip("Inverts the condition, e.g. \"Only work when cell type is *not* stem\"");

            if (geneCondition is ConcentrationCondition concentrationCondition)
            {
                var comparisonType = concentrationCondition.comparisonType;
                var morphogenId = concentrationCondition.morphogenId;
                var thresholdConcentration = concentrationCondition.thresholdConcentration;
                if (ImGui.BeginTable("##concentrationTable", 3))
                {
                    // ImGui.TableSetupColumn("Morphogen");
                    // ImGui.TableSetupColumn("Comparison");
                    // ImGui.TableSetupColumn("Threshold");
                    // ImGui.TableHeadersRow();
                    ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(0);
                    ImGui.PushItemWidth(150);
                    morphogenId = Selector($"morphogenSelector", $"##Morphogen", simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, morphogenId);
                    ImGui.PopItemWidth();

                    ImGui.TableSetColumnIndex(1);
                    ImGui.PushItemWidth(150);
                    comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector", $"##Comparison Type",
                    [0, 1, 2], (id) => ComparisonTypeToString((GeneCondition.ComparisonType)id), (int)comparisonType);
                    ImGui.PopItemWidth();

                    ImGui.TableSetColumnIndex(2);
                    ImGui.PushItemWidth(150);
                    ImGui.SliderFloat($"##Threshold", ref thresholdConcentration, 0f, simulation.maxConcentration);
                    ImGui.PopItemWidth();
                }
                ImGui.EndTable();

                concentrationCondition.name = name;
                concentrationCondition.not = not;
                concentrationCondition.comparisonType = comparisonType;
                concentrationCondition.morphogenId = morphogenId;
                concentrationCondition.thresholdConcentration = thresholdConcentration;

                // hilkat garibesi
                var inEnglish = $"{(comparisonType == GeneCondition.ComparisonType.EqualsTo ?
                $"Does {simulation.Morphogens[morphogenId].name} concentration{(not ? " not " : " ")}equal to {thresholdConcentration:F2}?" :
                $"Is {simulation.Morphogens[morphogenId].name} concentration{(not ? " not " : " ")}{(comparisonType == GeneCondition.ComparisonType.GreaterThan ? "greater than" : "less than")} {thresholdConcentration:F2}?"
                )}";

                ImGui.TextWrapped(inEnglish);

            }
            else if (geneCondition is CellTypeCondition cellTypeCondition)
            {
                var cellType = cellTypeCondition.cellType;
                var cellTypeId = cellType.id;

                cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);

                cellType = simulation.CellTypes[cellTypeId];

                cellTypeCondition.name = name;
                cellTypeCondition.not = not;
                cellTypeCondition.cellType = cellType;

                ImGui.TextWrapped($"Is cell type{(not ? " not " : " ")}{cellType.name}?");
            }
            else if (geneCondition is NeighbourCondition neighbourCondition)
            {
                var comparisonType = neighbourCondition.comparisonType;
                var threshold = neighbourCondition.threshold;

                if (ImGui.BeginTable("neighbourTable", 3))
                {
                    ImGui.TableNextRow();

                    ImGui.TableSetColumnIndex(0);
                    ImGui.PushItemWidth(150);
                    ImGui.Text("Neighbour Count");
                    ImGui.PopItemWidth();

                    ImGui.TableSetColumnIndex(1);
                    ImGui.PushItemWidth(150);
                    comparisonType = (GeneCondition.ComparisonType)Selector($"comparisonTypeSelector", $"##Comparison Type",
                    [0, 1, 2], (id) => ComparisonTypeToString((GeneCondition.ComparisonType)id), (int)comparisonType);
                    ImGui.PopItemWidth();

                    ImGui.TableSetColumnIndex(2);
                    ImGui.PushItemWidth(100);
                    ImGui.SliderInt("##Neighbour Count", ref threshold, 0, 6);
                    ImGui.PopItemWidth();
                }
                ImGui.EndTable();

                neighbourCondition.name = name;
                neighbourCondition.not = not;
                neighbourCondition.threshold = threshold;
                neighbourCondition.comparisonType = comparisonType;

                var inEnglish = $"{(comparisonType == GeneCondition.ComparisonType.EqualsTo ?
                $"Does neighbour count{(not ? " not " : " ")}equal to {threshold}?" :
                $"Is neighbour count{(not ? " not " : " ")}{(comparisonType == GeneCondition.ComparisonType.GreaterThan ? "greater than" : "less than")} {threshold}?"
                )}";
                ImGui.TextWrapped(inEnglish);
            }

            List<string> referencedInGenes = new();

            foreach (var gene in simulation.Genes.Values)
            {
                if (gene.activatorConditions.Contains(geneCondition) || gene.inhibitorConditions.Contains(geneCondition)) { referencedInGenes.Add(gene.name); }
            }

            ImGui.Separator();
            RemoveArea(geneConditions, geneConditionId, referencedInGenes.Count > 0, "Condition");

            InfoHeader(() =>
            {
                if (referencedInGenes.Count > 0) WrappingText($"Used in Genes: {string.Join(", ", referencedInGenes)}");
                ImGui.Text($"ID: {geneConditionId}");
            });
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
        var geneActions = simulation.GeneActions.Values.ToList();
        var geneId = genes.Keys.ToList()[0];

        if (ImGui.Begin("Gene Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            CloseButton(ref showGeneEditor);

            ImGui.SameLine();

            if (ImGui.Button($"New Gene"))
            {
                var newId = simulation.Add(new Gene(simulation.DefaultGene)).id;
                geneId = newId;
                selectorStates[key] = newId;
            }
            ImGui.Separator();

            geneId = Selector(key, $"Gene", genes.Keys.ToList(), (id) => genes[id].name);

            var gene = genes[geneId];
            var name = gene.name;

            ImGui.SeparatorText("Properties");

            if (ImGui.InputText($"Name", ref name, 32)) { gene.name = name; }

            ImGui.PushStyleColor(ImGuiCol.Header, GREEN_DARK);
            // ImGui.SeparatorText("Activator Conditions");
            if (ImGui.CollapsingHeader("Activator Conditions"))
            {
                ListEditor($"activator##{key}", "Activator Conditions", gene.activatorConditions, geneConditions, (condition) => condition.name);
            }
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Header, RED_DARK);
            if (ImGui.CollapsingHeader("Inhibitor Conditions"))
            {
                ListEditor($"inhibitor##{key}", "Inhibitor Conditions", gene.inhibitorConditions, geneConditions, (condition) => condition.name);
            }
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Header, PURPLE_DARK);
            if (ImGui.CollapsingHeader("Actions"))
            {
                ListEditor($"action##{key}", "Actions", gene.actions, geneActions, (action) => action.name);
            }
            ImGui.PopStyleColor();

            ImGui.Separator();
            RemoveArea(genes, geneId, false, "Gene");

            InfoHeader(() =>
            {
                ImGui.Text($"ID: {geneId}");
            });

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

            CloseButton(ref showSimulationEditor);
            ImGui.Separator();

            var name = simulation.name;
            var diffusionSteps = simulation.diffusionSteps;

            var diffusionThreshold = diffuser.diffusionThreshold;
            var diffusionFactor = diffuser.diffusionFactor;

            ImGui.SeparatorText("Properties");

            if (ImGui.InputText("Name", ref name, 32)) { simulation.name = name; }
            if (ImGui.InputFloat("Diffusion Threshold", ref diffusionThreshold)) { diffuser.diffusionThreshold = Math.Max(0, diffusionThreshold); }
            if (ImGui.InputFloat("Diffusion Factor", ref diffusionFactor)) { diffuser.diffusionFactor = Math.Max(0, diffusionFactor); }
            if (ImGui.InputInt("Diffusion Steps", ref diffusionSteps)) { simulation.diffusionSteps = Math.Max(0, diffusionSteps); }

            InfoHeader(() =>
            {
                ImGui.Text($"Cell Count: {simulation.Cells.Count}");
            });

            ImGui.PopID();
        }
        ImGui.End();
    }

    void ShowInspectWindow(HexCoords mouseHex)
    {
        var simulation = renderer.Simulation;

        ImGui.SetNextWindowPos(ImGui.GetMousePos() + new Vector2(10, 10), ImGuiCond.Always);
        if (ImGui.Begin("Inspector", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing))
        {
            ImGui.PushID("inspector");

            if (!simulation.Cells.TryGetValue(mouseHex, out Cell? cell))
            {
                ImGui.Text($"Position: {mouseHex}");
                ImGui.TextDisabled("No cell here.");
                ImGui.PopID(); ImGui.End(); return;
            }

            ImGui.Text($"Position: {mouseHex}");

            ImGui.TextDisabled("Click to edit cell.");

            ImGui.SeparatorText("Cell Type");

            ImGui.Text($"{cell.cellType.name}");

            ImGui.SeparatorText("Morphogens");

            foreach (var morphogenPair in cell.Morphogens)
            {
                ImGui.PushID($"morphogen{morphogenPair.Key}");
                int morphogenId = morphogenPair.Key;
                var morphogenAmount = morphogenPair.Value;
                ImGui.Text($"{simulation.GetMorphogen(morphogenId).name} : {morphogenAmount:F2}");
                ImGui.PopID();
            }

            ImGui.SeparatorText("Genes");

            foreach (var gene in simulation.Genes.Values)
            {
                if (gene.ShouldBeActive(cell))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 1f, 0.3f, 1f));
                }
                else
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.3f, 0.3f, 1f));
                }
                ImGui.PushID($"gene{gene.id}");
                ImGui.Text($"{gene.name}");
                ImGui.PopID();
                ImGui.PopStyleColor();
            }

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowCellEditor(HexCoords coords)
    {
        var key = "cellEditor";
        var simulation = renderer.Simulation;
        var cells = simulation.Cells;

        if (!cells.ContainsKey(coords)) { showCellEditor = false; selectedCellCoords = new(int.MaxValue, int.MaxValue); return; }
        var cell = cells[coords];

        var morphogens = simulation.Morphogens;
        var cellContent = cell.Morphogens;

        if (ImGui.Begin("Cell Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            // CloseButton(ref showCellEditor);
            // ImGui.Separator();

            if (ImGui.Button("Add to Palette"))
            {
                renderer.cellPalette.Add((new(cell), "New Cell"));
            }
            ImGui.Separator();

            ImGui.Text($"Cell Coordinates: {coords}");

            ImGui.SeparatorText("Properties");

            var cellType = cell.cellType;
            var cellTypeId = cellType.id;

            cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);

            cellType = simulation.CellTypes[cellTypeId];

            cell.cellType = cellType;

            ImGui.SeparatorText("Cell Content");

            DictionaryFloatEditor(key, "Cell Content", cellContent, morphogens.Keys.ToList(), (id) => morphogens[id].name, max: 100f);

            ImGui.Separator();

            RedButton(() =>
            {
                if (ImGui.Button("Remove Cell"))
                {
                    simulation.Cells.Remove(coords);
                }
            });

            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowGridEditor()
    {
        if (ImGui.Begin("Grid Controls", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID("gridEditor");
            if (ImGui.RadioButton(IconFonts.FontAwesome6.ArrowsUpDownLeftRight + " Move", gridState == GridState.Move))
            {
                gridState = GridState.Move;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.Pen + " Brush", gridState == GridState.Brush))
            {
                gridState = GridState.Brush;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.Eraser + " Eraser", gridState == GridState.Erase))
            {
                gridState = GridState.Erase;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.MagnifyingGlass + " Inspect", gridState == GridState.Inspect))
            {
                gridState = GridState.Inspect;
            }

            ImGui.Separator();

            if (gridState != GridState.Brush) { ImGui.BeginDisabled(); }

            if (renderer.cellIndex == 0) { ImGui.BeginDisabled(); }
            RedButton(() =>
            {
                if (ImGui.Button(IconFonts.FontAwesome6.TrashCan))
                {
                    renderer.cellPalette.RemoveAt(renderer.cellIndex);
                }
            });
            if (renderer.cellIndex == 0) { ImGui.EndDisabled(); }

            ImGui.SameLine();

            renderer.cellIndex = Selector("cellPaletteIndex", "Cell to Draw", Enumerable.Range(0, renderer.cellPalette.Count).ToList(), (i) => renderer.cellPalette[i].name);
            var name = renderer.cellPalette[renderer.cellIndex].name;
            if (ImGui.InputText("Cell Name", ref name, 32))
            {
                renderer.cellPalette[renderer.cellIndex] = (renderer.CellToDraw, name);
            }

            if (gridState != GridState.Brush) { ImGui.EndDisabled(); }

            ImGui.Separator();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("Brush Size").X - 10);
            brushSize++;
            ImGui.SliderInt("Brush Size", ref brushSize, 1, 12);
            brushSize--;


            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowVisualizationEditor()
    {
        var key = "visualizer";
        var simulation = renderer.Simulation;
        var morphogens = simulation.Morphogens;

        if (ImGui.Begin("Visualization Editor", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            CloseButton(ref showVisualizationEditor);
            
            ImGui.Separator();

            var visualizationType = Selector("visualizationType", "Mode", [0, 1, 2], VisualizationTypeToString, (int)renderer.visualizationType);
            ImGui.Separator();

            var morphogenKeys = morphogens.Keys.ToList();
            switch (visualizationType)
            {
                case 0: // Three Morphogens
                    renderer.visualizationType = SimulationRenderer.VisualizationType.ThreeMorphogens;

                    ImGui.SliderFloat("Amplifier", ref renderer.amplifier, 1f, simulation.maxConcentration);

                    ImGui.Separator();

                    morphogenKeys.Remove(renderer.redMorphogenId);
                    morphogenKeys.Remove(renderer.greenMorphogenId);
                    morphogenKeys.Remove(renderer.blueMorphogenId);

                    ImGui.PushStyleColor(ImGuiCol.Text, RED_LIGHT);
                    renderer.redMorphogenId = SoftSelector("redMorphogen", "Red", morphogenKeys, (id) => morphogens[id].name, renderer.redMorphogenId);
                    ImGui.PopStyleColor();

                    ImGui.PushStyleColor(ImGuiCol.Text, GREEN_LIGHT);
                    renderer.greenMorphogenId = SoftSelector("greenMorphogen", "Green", morphogenKeys, (id) => morphogens[id].name, renderer.greenMorphogenId);
                    ImGui.PopStyleColor();

                    ImGui.PushStyleColor(ImGuiCol.Text, BLUE_LIGHT);
                    renderer.blueMorphogenId = SoftSelector("blueMorphogen", "Blue", morphogenKeys, (id) => morphogens[id].name, renderer.blueMorphogenId);
                    ImGui.PopStyleColor();

                    break;
                case 1: // Single Morphogen
                    renderer.visualizationType = SimulationRenderer.VisualizationType.SingleMorphogen;
                    ImGui.SliderFloat("Amplifier", ref renderer.amplifier, 1f, simulation.maxConcentration);

                    ImGui.Separator();

                    renderer.singleMorphogenId = Selector("singleMorphogen", "Morphogen", morphogenKeys, (id) => morphogens[id].name, renderer.singleMorphogenId);
                    break;
                case 2: // Cell Types
                    renderer.visualizationType = SimulationRenderer.VisualizationType.CellTypes;
                    foreach (var cellTypePair in simulation.CellTypes)
                    {
                        var cellTypeId = cellTypePair.Key;
                        var cellType = cellTypePair.Value;
                        Color color = renderer.cellTypeColors.GetValueOrDefault(cellTypeId, Color.Black);
                        var colorVector = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);

                        if (ImGui.ColorEdit3($"{cellType.name}##{cellTypeId}", ref colorVector, ImGuiColorEditFlags.Float))
                        {
                            renderer.cellTypeColors[cellTypeId] = new Color(colorVector.X, colorVector.Y, colorVector.Z);
                        }
                    }
                    break;
                default:
                    break;
            }
            ImGui.PopID();
        }

        ImGui.End();
    }

    // GENERICS

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
        if (ImGui.BeginCombo($"{label}", preview))
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
        //  $"{label} - ({current})" OR $"{label}"
        if (ImGui.BeginCombo($"{label}", preview))
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

    void DictionaryFloatEditor(string key, string label, Dictionary<int, float> dict, List<int> allItems, Func<int, string> getName, float min = 0f, float max = 1f)
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
                if (ImGui.SliderFloat("##value", ref value, min, max)) dict[dictKey] = value;
                ImGui.PopItemWidth();

                ImGui.TableSetColumnIndex(2);
                RedButton(() => { if (ImGui.Button(IconFonts.FontAwesome6.TrashCan + " Remove")) dict.Remove(dictKey); });

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

    void RemoveArea<T>(Dictionary<int, T> dictToRemoveFrom, int id, bool isReferenced, string name)
    {
        var simulation = renderer.Simulation;
        RedButton(() =>
        {
            var isGridEmpty = simulation.Cells.Count == 0;
            var isLast = dictToRemoveFrom.Count == 1;
            var canDelete = !isReferenced && isGridEmpty && !isLast;
            if (ImGui.BeginChild("removalArea", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetFrameHeight())))
            {
                if (!canDelete) ImGui.BeginDisabled();
                if (ImGui.Button($"Delete {name}"))
                {
                    dictToRemoveFrom.Remove(id);
                }
                if (!canDelete) ImGui.EndDisabled();
                // if (!canDelete)
                // {
                //     ImGui.SameLine();
                //     ImGui.TextColored(RED_WARNING, "Can't delete this now.");
                // }
            }
            ImGui.EndChild();
            if (!canDelete && ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.TextColored(RED_WARNING, "Can't delete this now.");
                if (isReferenced) ImGui.BulletText($"{name} is still used (see Info).");
                if (!isGridEmpty) ImGui.BulletText($"Grid must be cleared first.");
                if (isLast) ImGui.BulletText($"At least one {name} should remain.");
                ImGui.EndTooltip();
            }
        });
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
            GeneCondition.ComparisonType.EqualsTo => "Equals To (=)",
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

    string VisualizationTypeToString(int id)
    {
        return id switch
        {
            0 => "Three Morphogens",
            1 => "Single Morphogen",
            2 => "Cell Types",
            _ => ""
        };
    }

    void Space(int space = 5)
    {
        ImGui.Dummy(new Vector2(0, space));
    }

    void WrappingText(string text)
    {
        ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);
        ImGui.TextUnformatted(text);
        ImGui.PopTextWrapPos();
    }

    void InfoHeader(Action content, string label = "Info")
    {
        if (ImGui.CollapsingHeader(label))
        {
            ImGui.BeginDisabled();
            content();
            ImGui.EndDisabled();
        }
    }

    void CloseButton(ref bool toFalse)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0f, 0f, 1f));
        if (ImGui.Button($"Close"))
        {
            toFalse = false;
        }
        ImGui.PopStyleColor(3);
    }

    void RedButton(Action content)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0f, 0f, 1f));
        content();
        ImGui.PopStyleColor(3);
    }

    void HoverTooltip(string text)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text(text);
            ImGui.EndTooltip();
        }
    }

    public enum GridState
    {
        Move,
        Brush,
        Erase,
        Inspect
    }
}