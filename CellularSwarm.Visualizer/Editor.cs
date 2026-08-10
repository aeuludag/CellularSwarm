using System;
using Raylib_cs;
using System.Numerics;
using CellularSwarm.Core;
using ImGuiNET;
using IconFonts;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

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
    public bool showVisualizationEditor = false;
    public bool showConsole = false;
    public bool showSettings = false;
    public bool showWelcome = ConfigHandler.Config.showWelcome;

    public HexCoords selectedCellCoords = new(int.MaxValue, int.MaxValue);

    public static readonly List<Texture2D> LoadedTextures = new();

    public static readonly Vector4 RED_WARNING = new(0.9f, 0f, 0f, 1f);
    public static readonly Vector4 RED_LIGHT = new(1f, 0.7f, 0.7f, 1f);
    public static readonly Vector4 RED_DARK = new(0.5f, 0.1f, 0.1f, 1f);
    public static readonly Vector4 GREEN_LIGHT = new(0.7f, 1f, 0.7f, 1f);
    public static readonly Vector4 GREEN_DARK = new(0.1f, 0.5f, 0.1f, 1f);
    public static readonly Vector4 BLUE_LIGHT = new(0.7f, 0.7f, 1f, 1f);
    public static readonly Vector4 BLUE_DARK = new(0.3f, 0f, 0f, 1f);
    public static readonly Vector4 PURPLE_LIGHT = new(0.7f, 0.4f, 0.8f, 1f);
    public static readonly Vector4 PURPLE_DARK = new(0.3f, 0.2f, 0.5f, 1f);

    private readonly static Random random = new();

    private Dictionary<string, int> selectorStates = new(); // help by gpt

    private string consoleText = "";
    private Texture2D icon;
    private Texture2D iconNoBg;

    public Editor(SimulationRenderer renderer)
    {
        this.renderer = renderer;
        DebugConsole.Instance.Renderer = renderer;

        icon = LoadTexture("icon.png");
        iconNoBg = LoadTexture("icon_nobg.png");
    }

    public void ShowWindowManager(HexCoords mouseHex)
    {
        var size = ImGui.GetIO().DisplaySize;
        var w = size.X;
        var h = size.Y;

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));

        WinPosC(16, h - 16, 0, 1);
        if (ImGui.Begin("Window Manager", ImGuiWindowFlags.AlwaysAutoResize))
        {

            ImGui.Checkbox("Visualization Editor", ref showVisualizationEditor);
            HoverTooltip("Set visualization modes", "Visualization Mode, ...");
            ImGui.Checkbox("Morphogen Editor", ref showMorphogenEditor);
            HoverTooltip("Create or edit morphogens", "Name, Diffusion Factor, Decay Factor");
            ImGui.Checkbox("Cell Type Editor", ref showCellTypeEditor);
            HoverTooltip("Create or edit cell types", "Name");
            ImGui.Checkbox("Gene Action Editor", ref showGeneActionEditor);
            HoverTooltip("Create or edit gene actions", "Name, Action Type, ...");
            ImGui.Checkbox("Gene Condition Editor", ref showGeneConditionEditor);
            HoverTooltip("Create or edit gene conditions", "Name, Negate, ...");
            ImGui.Checkbox("Gene Editor", ref showGeneEditor);
            HoverTooltip("Create or edit genes", "Name, Activator/Inhibitor Conditions, Actions");
            ImGui.Checkbox("Simulation Editor", ref showSimulationEditor);
            HoverTooltip("Edit simulation properties", "Name, Diffusion Threshold, Diffusion Factor, Diffusion Steps");

            if (ImGui.Button("Hide All"))
            {
                showMorphogenEditor = false;
                showCellTypeEditor = false;
                showGeneActionEditor = false;
                showGeneConditionEditor = false;
                showGeneEditor = false;
                showSimulationEditor = false;
                showVisualizationEditor = false;
                showConsole = false;
                showSettings = false;
                showWelcome = false;
            }
            HoverTooltip("Hide all windows");
            ImGui.SameLine();
            if(ImGui.Button(FontAwesome6.Newspaper))
            {
                showWelcome ^= true;
            }
            HoverTooltip("Welcome");
            ImGui.SameLine();
            if(ImGui.Button(FontAwesome6.Gear))
            {
                showSettings ^= true;
            }
            HoverTooltip("Settings");
            ImGui.SameLine();
            if (ImGui.Button(FontAwesome6.Terminal))
            {
                showConsole ^= true;
            }
            HoverTooltip("Console");
        }
        ImGui.End();

//                                   WELCOME TO THE SPINE
//                                           * *
//                                          \___/
        if      (showVisualizationEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowVisualizationEditor(); }
        if          (showMorphogenEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowMorphogenEditor(); }
        if           (showCellTypeEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowCellTypeEditor(); }
        if         (showGeneActionEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowGeneActionEditor(); }
        if      (showGeneConditionEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowGeneConditionEditor(); }
        if               (showGeneEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowGeneEditor(); }
        if         (showSimulationEditor) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowSimulationEditor(); }
        if                (showInspector) /* | | */  { ShowInspectWindow(mouseHex); }
        if               (showCellEditor) /* | | */  { ShowCellEditor(selectedCellCoords); }
        if               (showGridEditor) /* | | */  { WinPosC(w - 16, h - 16, 1, 1); ShowGridEditor(); }
        if                  (showConsole) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ImGui.SetNextWindowSize(new Vector2(720, 600), ImGuiCond.Once); ShowConsole(); }
        if                 (showSettings) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowSettings(); }
        if                  (showWelcome) /* | | */  { WinPos(w/2, h/2, 0.5f, 0.5f); ShowWelcome(); }
        // tehehe

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

            if (ImGui.InputText($"Name", ref name, 64)) { cellTypes[cellTypeId] = new CellType(cellTypeId, name); }

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
        var morphogenId = morphogens.Keys.ToArray()[0];

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
            var max = simulation.maxConcentration;

            if (ImGui.InputText($"Name", ref name, 64)) { morphogen.name = name; }
            if (ImGui.SliderFloat($"Diffusion Factor##diffusionSlider", ref difFac, 0f, 1f)) { morphogen.diffusionFactor = Math.Clamp(difFac, 0f, 1f); }
            // ImGui.SameLine();
            // WidthX(ImGui.CalcTextSize($"{max:F3}").X + 20, () => { if (ImGui.InputFloat($"Diffusion Factor", ref difFac)) { difFac = Math.Clamp(difFac, 0f, 1f); morphogen.diffusionFactor = difFac; } });
            HoverTooltip("How fast the morphogen flows & diffuses.");
            if (ImGui.SliderFloat($"Decay Factor##decaySlider", ref decFac, 0f, 1f)) { morphogen.decayFactor = Math.Clamp(decFac, 0f, 1f); }
            // ImGui.SameLine();
            // WidthX(ImGui.CalcTextSize($"{max:F3}").X + 20, () => { if (ImGui.InputFloat($"Decay Factor", ref decFac)) { decFac = Math.Clamp(decFac, 0f, 1f); morphogen.decayFactor = decFac; } });

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

            if (ImGui.InputText($"Name", ref name, 64)) { geneAction.name = name; }

            actionType = (GeneAction.ActionType)Selector($"actionTypeSelector", $"Action Type",
            [0, 1, 2, 3, 4], (id) => ActionTypeToString((GeneAction.ActionType)id), (int)actionType);

            geneAction.actionType = actionType;

            switch (actionType)
            {
                case GeneAction.ActionType.ChangeMorphogen:
                    ImGui.SeparatorText("Morphogen Delta");
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, -max, max);
                    break;
                case GeneAction.ActionType.Multiply:
                    ImGui.SeparatorText("Morphogen Share Rate");
                    HoverTooltip("0.0 : leave all to the parent\n0.5 : share equally (default)\n1.0 : give all to the daughter");
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.Apoptosis:
                    // DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name);
                    break;
                case GeneAction.ActionType.ChangeCellType:
                    cellTypeId = Selector($"cellType", $"Cell Type", simulation.CellTypes.Keys.ToList(), (id) => simulation.CellTypes[id].name, cellTypeId);
                    geneAction.cellTypeId = cellTypeId;
                    break;
                case GeneAction.ActionType.TransportMorphogen:
                    ImGui.SeparatorText("Active Transportation");
                    HoverTooltip("-1.0 : pull all of morphogen\n 0.0 : no transportation (default)\n+1.0 : push all of morphogen");
                    DictionaryFloatEditor(key, $"Action Morphogens", geneAction.actionMorphogens, simulation.Morphogens.Keys.ToList(), (id) => simulation.Morphogens[id].name, min: -1, max: +1);
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

            ImGui.InputText($"Name", ref name, 64);
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
                    WidthX(150, () => { if(ImGui.SliderFloat($"Threshold##ThresholdSlider", ref thresholdConcentration, 0f, simulation.maxConcentration)) {thresholdConcentration = Math.Clamp(thresholdConcentration, 0f, simulation.maxConcentration); } });
                    // ImGui.SameLine();
                    // WidthX(ImGui.CalcTextSize($"{simulation.maxConcentration:F3}").X + 20, () => { if (ImGui.InputFloat($"##ThresholdInput", ref thresholdConcentration)) { thresholdConcentration = Math.Clamp(thresholdConcentration, 0f, simulation.maxConcentration); } });
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

                ImGui.Separator();
                ImGui.PushTextWrapPos(450);
                ImGui.Text($"\"{inEnglish}\"");
                ImGui.PopTextWrapPos();

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

                ImGui.Separator();
                ImGui.PushTextWrapPos(200);
                ImGui.Text($"\"Is cell type{(not ? " not " : " ")}{cellType.name}?\"");
                ImGui.PopTextWrapPos();
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
                    if(ImGui.SliderInt("##Neighbour Count", ref threshold, 0, 6)) { threshold = Math.Clamp(threshold, 0, 6); };
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

                ImGui.Separator();
                ImGui.PushTextWrapPos(400);
                ImGui.Text($"\"{inEnglish}\"");
                ImGui.PopTextWrapPos();
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

            if (ImGui.InputText($"Name", ref name, 64)) { gene.name = name; }

            var dark = ThemeHandler.GetCurrentTheme().dark;

            ImGui.PushStyleColor(ImGuiCol.Header, dark ? GREEN_DARK : GREEN_LIGHT);
            // ImGui.SeparatorText("Activator Conditions");
            if (ImGui.CollapsingHeader("Activator Conditions"))
            {
                ImGui.Checkbox("Any##activator", ref gene.activatorAny);
                HoverTooltip("Unchecked: All conditions should be met\nChecked: Only one met condition is enough.");
                ListEditor($"activator##{key}", "Activator Conditions", gene.activatorConditions, geneConditions, (condition) => condition.name);
            }
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Header, dark ? RED_DARK : RED_LIGHT);
            if (ImGui.CollapsingHeader("Inhibitor Conditions"))
            {
                ImGui.Checkbox("Any##inhibitor", ref gene.inhibitorAny);
                HoverTooltip("Unchecked: All conditions should be met\nChecked: Only one met condition is enough.");
                ListEditor($"inhibitor##{key}", "Inhibitor Conditions", gene.inhibitorConditions, geneConditions, (condition) => condition.name);
            }
            ImGui.PopStyleColor();

            ImGui.PushStyleColor(ImGuiCol.Header, dark? PURPLE_DARK : PURPLE_LIGHT);
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

            if (ImGui.InputText("Name", ref name, 64)) { simulation.name = name; Raylib.SetWindowTitle($"\"{name}\" - Cellular Swarm"); }
            if (ImGui.InputFloat("Diffusion Threshold", ref diffusionThreshold)) { diffuser.diffusionThreshold = Math.Max(0, diffusionThreshold); }
            HoverTooltip("The minimum amount of difference in concentration needed to diffuse.\nAlso the minimum amount of concentration a morphogen can be in (after zero).");
            if (ImGui.InputFloat("Diffusion Factor", ref diffusionFactor)) { diffuser.diffusionFactor = Math.Clamp(diffusionFactor, 0f, 1f); }
            HoverTooltip("The general diffusion factor.");
            if (ImGui.InputInt("Diffusion Steps", ref diffusionSteps)) { simulation.diffusionSteps = Math.Max(0, diffusionSteps); }
            HoverTooltip("The amount of diffusion steps performed in each simulation step.");


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

        if(ImGui.GetMousePos().Y > Raylib.GetScreenHeight() / 2)
        {
            ImGui.SetNextWindowPos(ImGui.GetMousePos() + new Vector2(10, -10), ImGuiCond.Always, new Vector2(0f, 1f));
        } else
        {
            ImGui.SetNextWindowPos(ImGui.GetMousePos() + new Vector2(10, 10), ImGuiCond.Always);
        }
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

            if(ImGui.IsKeyDown(ImGuiKey.LeftShift))
            {
                var isDark = ThemeHandler.GetCurrentTheme().dark;
                foreach (var gene in simulation.Genes.Values)
                {
                    if (gene.ShouldBeActive(cell))
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, isDark? new Vector4(0.5f, 1f, 0.5f, 1f) : new Vector4(0.2f, 0.6f, 0.2f, 1f));
                    }
                    else
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, isDark? new Vector4(1f, 0.5f, 0.5f, 1f) : new Vector4(0.8f, 0.2f, 0.2f, 1f));
                    }
                    ImGui.PushID($"gene{gene.id}");
                    ImGui.Text($"{gene.name}");
                    ImGui.PopID();
                    ImGui.PopStyleColor();
                }
            } else
            {
                float activeCount = 0;
                float inactiveCount = 0;
                foreach (var gene in simulation.Genes.Values)
                {
                    if (gene.ShouldBeActive(cell))
                    {
                        activeCount++;
                    }
                    else
                    {
                        inactiveCount++;
                    }
                }
                float all = activeCount + inactiveCount;
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.6f * (inactiveCount / all) + 0.4f, 0.6f * (activeCount / all) + 0.4f, 0.3f, 1f));
                ImGui.Text($"{activeCount} out of {all} genes active");
                ImGui.PopStyleColor();
                ImGui.TextDisabled("Left Shift for detail.");
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

            RedButton(() => { if(ImGui.Button("Close")) selectedCellCoords = new(int.MaxValue, int.MaxValue); });
            ImGui.SameLine();
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
            HoverTooltip("Move the grid.", "1", true);
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.Pen + " Brush", gridState == GridState.Brush))
            {
                gridState = GridState.Brush;
            }
            HoverTooltip("Place cells in palette.\n(Inspect cells to add to palette)", "2", true);
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.Eraser + " Eraser", gridState == GridState.Erase))
            {
                gridState = GridState.Erase;
            }
            HoverTooltip("Erase cells.", "3", true);
            ImGui.SameLine();
            if (ImGui.RadioButton(IconFonts.FontAwesome6.MagnifyingGlass + " Inspect", gridState == GridState.Inspect))
            {
                gridState = GridState.Inspect;
            }
            HoverTooltip("Inspect cells.", "4", true);

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
            HoverTooltip("The cell to be drawn.", "Shift + 1..9", true);

            var name = renderer.cellPalette[renderer.cellIndex].name;
            if (ImGui.InputText("Cell Name", ref name, 64))
            {
                renderer.cellPalette[renderer.cellIndex] = (renderer.CellToDraw, name);
            }

            if (gridState != GridState.Brush) { ImGui.EndDisabled(); }

            ImGui.Separator();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("Brush Size").X - 10);
            brushSize++;
            if(gridState == GridState.Move || gridState == GridState.Inspect) { ImGui.BeginDisabled(); }
            if(ImGui.SliderInt("Brush Size", ref brushSize, 1, 24)) { brushSize = Math.Clamp(brushSize, 1, 24); }
            if(gridState == GridState.Move || gridState == GridState.Inspect) { ImGui.EndDisabled(); }
            brushSize--;
            HoverTooltip("Hexagonal brush radius", "Shift + Scroll", true);


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

            var visualizationType = Selector("visualizationType", "Mode", [0, 1, 2, 3, 4], VisualizationTypeToString, (int)renderer.visualizationType);
            ImGui.Separator();

            var morphogenKeys = morphogens.Keys.ToList();
            switch (visualizationType)
            {
                case 0: // Three Morphogens
                    renderer.visualizationType = SimulationRenderer.VisualizationType.ThreeMorphogens;

                    if(ImGui.SliderFloat("Amplifier", ref renderer.amplifier, 1f, simulation.maxConcentration)) {renderer.amplifier = Math.Clamp(renderer.amplifier, 1f, simulation.maxConcentration); }

                    ImGui.Separator();

                    // morphogenKeys.Remove(renderer.redMorphogenId);
                    // morphogenKeys.Remove(renderer.greenMorphogenId);
                    // morphogenKeys.Remove(renderer.blueMorphogenId);

                    // ImGui.PushStyleColor(ImGuiCol.Text, RED_LIGHT);
                    renderer.redMorphogenId = SoftSelector("redMorphogen", "Red", morphogenKeys, (id) => morphogens[id].name, renderer.redMorphogenId);
                    // ImGui.PopStyleColor();

                    // ImGui.PushStyleColor(ImGuiCol.Text, GREEN_LIGHT);
                    renderer.greenMorphogenId = SoftSelector("greenMorphogen", "Green", morphogenKeys, (id) => morphogens[id].name, renderer.greenMorphogenId);
                    // ImGui.PopStyleColor();

                    // ImGui.PushStyleColor(ImGuiCol.Text, BLUE_LIGHT);
                    renderer.blueMorphogenId = SoftSelector("blueMorphogen", "Blue", morphogenKeys, (id) => morphogens[id].name, renderer.blueMorphogenId);
                    // ImGui.PopStyleColor();

                    break;
                case 1: // Single Morphogen
                    renderer.visualizationType = SimulationRenderer.VisualizationType.SingleMorphogen;
                    if(ImGui.SliderFloat("Amplifier", ref renderer.amplifier, 1f, simulation.maxConcentration)) {renderer.amplifier = Math.Clamp(renderer.amplifier, 1f, simulation.maxConcentration); }

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
                case 3: // Gene Activity
                    renderer.visualizationType = SimulationRenderer.VisualizationType.GeneActivity;

                    renderer.geneId = Selector("gene", "Gene", simulation.Genes.Keys.ToList(), (id) => simulation.Genes[id].name);
                    
                    ImGui.Separator();

                    Color activeGeneColor = renderer.activeGeneColor;
                    var activeGeneColorVector = new Vector3(activeGeneColor.R / 255f, activeGeneColor.G / 255f, activeGeneColor.B / 255f);
                    if (ImGui.ColorEdit3($"Active Gene", ref activeGeneColorVector, ImGuiColorEditFlags.Float))
                    {
                        renderer.activeGeneColor = new Color(activeGeneColorVector.X, activeGeneColorVector.Y, activeGeneColorVector.Z);
                    }

                    Color inactiveGeneColor = renderer.inactiveGeneColor;
                    var inactiveGeneColorVector = new Vector3(inactiveGeneColor.R / 255f, inactiveGeneColor.G / 255f, inactiveGeneColor.B / 255f);
                    if (ImGui.ColorEdit3($"Inactive Gene", ref inactiveGeneColorVector, ImGuiColorEditFlags.Float))
                    {
                        renderer.inactiveGeneColor = new Color(inactiveGeneColorVector.X, inactiveGeneColorVector.Y, inactiveGeneColorVector.Z);
                    }

                    break;
                case 4: // Gene Condition Met
                    renderer.visualizationType = SimulationRenderer.VisualizationType.GeneConditionMet;

                    renderer.geneConditionId = Selector("condition", "Gene Condition", simulation.GeneConditions.Keys.ToList(), (id) => simulation.GeneConditions[id].name);
                    
                    ImGui.Separator();

                    Color metConditionColor = renderer.metConditionColor;
                    var metConditionColorVector = new Vector3(metConditionColor.R / 255f, metConditionColor.G / 255f, metConditionColor.B / 255f);
                    if (ImGui.ColorEdit3($"Met Condition", ref metConditionColorVector, ImGuiColorEditFlags.Float))
                    {
                        renderer.metConditionColor = new Color(metConditionColorVector.X, metConditionColorVector.Y, metConditionColorVector.Z);
                    }

                    Color notMetConditionColor = renderer.notMetConditionColor;
                    var notMetConditionColorVector = new Vector3(notMetConditionColor.R / 255f, notMetConditionColor.G / 255f, notMetConditionColor.B / 255f);
                    if (ImGui.ColorEdit3($"Not Met Condition", ref notMetConditionColorVector, ImGuiColorEditFlags.Float))
                    {
                        renderer.notMetConditionColor = new Color(notMetConditionColorVector.X, notMetConditionColorVector.Y, notMetConditionColorVector.Z);
                    }

                    break;
                default:
                    break;
            }
            ImGui.PopID();
        }

        ImGui.End();
    }

    public void ShowConsole()
    {
        DebugConsole.Instance.Renderer = renderer;
        
        var key = "console";

        // ImGui.SetNextWindowSize(new Vector2(0, 500));
        if (ImGui.Begin("Console"))
        {
            ImGui.PushID(key);

            CloseButton(ref showConsole);
            ImGui.SameLine();
            ImGui.PushItemWidth(ImGui.GetWindowWidth() - 120);
            ImGui.InputText("##input", ref consoleText, 256);
            ImGui.SameLine();

            if (ImGui.Button("Send") || (ImGui.IsWindowFocused() && ImGui.IsKeyPressed(ImGuiKey.Enter)))
            {
                DebugConsole.Send(consoleText);
                consoleText = "";
                ImGui.SetKeyboardFocusHere(-1);
            }

            // if (ImGui.Button("Push Default"))
            // {
            //     DebugConsole.Log("Pushing " + DateTime.Now, "EDITOR");
            // }
            // ImGui.SameLine();
            // if (ImGui.Button("Push Warning"))
            // {
            //     DebugConsole.Warning("Pushing " + DateTime.Now, "EDITOR");
            // }
            // ImGui.SameLine();
            // if (ImGui.Button("Push Error"))
            // {
            //     DebugConsole.Error("Pushing " + DateTime.Now, "EDITOR");
            // }

            ImGui.Separator();
            // if(ImGui.BeginChild("messages", new Vector2(0, -ImGui.GetFrameHeightWithSpacing())))
            // {
            //     foreach (var line in DebugConsole.Lines)
            //     {
            //         ImGui.PushStyleColor(ImGuiCol.Text, line.color);
            //         ImGui.Text(line.ToString());
            //         ImGui.PopStyleColor();
            //     }
            // }

            // (partially) GEMINI GENERATED CODE BELOW
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0f, 0f, 0f, 1f));
            if (ImGui.BeginChild("messages", new Vector2(0, 0), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar))
            {
                unsafe
                {
                    ImGuiListClipperPtr clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
                    clipper.Begin(DebugConsole.Lines.Count);

                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.5f));
                    ImGui.SeparatorText("begin-console");
                    ImGui.PopStyleColor();

                    while (clipper.Step())
                    {
                        for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                        {
                            var line = DebugConsole.Lines[i];

                            ImGui.PushStyleColor(ImGuiCol.Text, line.color);
                            ImGui.TextUnformatted(line.ToString());
                            ImGui.PopStyleColor();
                        }
                    }
                    // ImGui.TextUnformatted("");
                    // ImGui.PushStyleVar(ImGuiStyleVar.SeparatorTextAlign, 0);
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.5f));
                    ImGui.SeparatorText("end-of-console");
                    ImGui.PopStyleColor();
                    // ImGui.PopStyleVar();
                    clipper.End();
                }

                if (MathF.Abs(ImGui.GetScrollY() - ImGui.GetScrollMaxY()) <= 0.1f)
                {
                    ImGui.SetScrollHereY(1.0f);
                }
            }
            ImGui.PopStyleColor();
            // GEMINI GENERATED CODE ABOVE
            ImGui.EndChild();

            // ImGui.Separator();


            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowSettings()
    {
        var key = "settings";

        var getName = (int i) => ThemeHandler.Themes[i].name; 
        var range = Enumerable.Range(0, ThemeHandler.Themes.Count).ToList();

        var v2c = (Vector4 v) => new Color(v.X, v.Y, v.Z, v.W);
        var c2v = (Color c) => new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);

        if(ImGui.Begin("Settings", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);

            CloseButton(ref showSettings);
            ImGui.SameLine();
            if(ImGui.Button("Reset Settings"))
            {
                ConfigHandler.ResetConfig();
                ConfigHandler.SaveConfig();
                ThemeHandler.ApplyCurrentTheme();
            }
            // ImGui.SameLine();
            // if(ImGui.Button("Save Settings"))
            // {
            //     ConfigHandler.SaveConfig(); 
            //     ThemeHandler.ApplyCurrentTheme();
            // }

            ImGui.Separator();
            ImGui.SeparatorText("UI");

            int currentThemeIndex = ConfigHandler.Config.themeIndex;

            float rand() => (float)random.Next() / (float)int.MaxValue;
            bool uselessBoolThatIndicatesNewThemeWasJustRecentlyAddedRightNow = false;
            if(ImGui.Button("New"))
            {
                ConfigHandler.Config.customThemes.Add(
                    new($"Custom Theme #{ConfigHandler.Config.customThemes.Count + 1}",
                    new Vector4(rand(), rand(), rand(), 1f),
                    new Vector4(rand(), rand(), rand(), 1f),
                    rand() > 0.5f));
                uselessBoolThatIndicatesNewThemeWasJustRecentlyAddedRightNow = true;
                // currentThemeIndex = ThemeHandler.Themes.Count - 2;
                // ConfigHandler.Config.themeIndex = ThemeHandler.Themes.Count - 1;
            }

            ImGui.SameLine();

            RedButton(() =>
            {
                if(currentThemeIndex < ThemeHandler.PresetThemes.Count)
                {
                    ImGui.BeginDisabled();
                    ImGui.Button(IconFonts.FontAwesome6.TrashCan);
                    ImGui.EndDisabled();
                    return;
                }
                if(ImGui.Button(IconFonts.FontAwesome6.TrashCan))
                {
                    ConfigHandler.Config.customThemes.RemoveAt(currentThemeIndex - ThemeHandler.PresetThemes.Count);
                    currentThemeIndex = Math.Max(0, currentThemeIndex - 1);
                }
            });

            ImGui.SameLine();

            // ConfigHandler.Config.themeIndex = currentThemeIndex;

            ConfigHandler.Config.themeIndex = Selector($"themeSelector{key}", "Theme", range, getName, currentThemeIndex);
            
            if(uselessBoolThatIndicatesNewThemeWasJustRecentlyAddedRightNow)
                ConfigHandler.Config.themeIndex = ThemeHandler.Themes.Count - 1;

            if(ConfigHandler.Config.themeIndex >= ThemeHandler.PresetThemes.Count)
            {
                var currentTheme = ThemeHandler.GetCurrentTheme();
                ImGui.InputText("Name", ref currentTheme.name, 64);
                if(ImGui.ColorEdit4("Main", ref currentTheme.main)) ThemeHandler.ApplyCurrentTheme();
                if(ImGui.ColorEdit4("Accent", ref currentTheme.accent)) ThemeHandler.ApplyCurrentTheme();
                if(ImGui.Checkbox("Use Dark Appearance", ref currentTheme.dark)) ThemeHandler.ApplyCurrentTheme();
                
            } else
            {
                var currentTheme = ThemeHandler.GetCurrentTheme();
                var main = currentTheme.main;
                var accent = currentTheme.accent;
                // var dark = currentTheme.dark;
                ImGui.BeginDisabled();
                ImGui.ColorEdit4("Main", ref main);
                ImGui.ColorEdit4("Accent", ref accent);
                // ImGui.Checkbox("Dark", ref dark);
                ImGui.EndDisabled();
            }

            ThemeHandler.ApplyCurrentTheme();

            ImGui.SeparatorText("Background");

            if(ImGui.Button("Reset to Dark")) { ConfigHandler.Config.backColor = new(20, 20, 20); ConfigHandler.Config.outlineColor = new(255, 255, 255, 20); }
            ImGui.SameLine();
            if(ImGui.Button("Reset to Light")) { ConfigHandler.Config.backColor = new(230, 230, 240); ConfigHandler.Config.outlineColor = new(10, 10, 40, 20); }

            var bc2v = c2v(ConfigHandler.Config.backColor);
            var oc2v = c2v(ConfigHandler.Config.outlineColor);

            ImGui.ColorEdit4("Back Color", ref bc2v);
            ImGui.ColorEdit4("Outline Color", ref oc2v);
            bc2v.W = 1f;

            ConfigHandler.Config.backColor = v2c(bc2v);
            ConfigHandler.Config.outlineColor = v2c(oc2v);

            ImGui.SeparatorText("Maximum FPS");

            if(ImGui.Checkbox("Limit FPS to", ref ConfigHandler.Config.limitFPS)) { Raylib.SetTargetFPS(ConfigHandler.Config.limitFPS ? ConfigHandler.Config.maxFPS : int.MaxValue); }
            ImGui.SameLine();

            if(!ConfigHandler.Config.limitFPS) {ImGui.BeginDisabled();}
            ImGui.SetNextItemWidth(100);
            if(ImGui.InputInt("FPS", ref ConfigHandler.Config.maxFPS)) { ConfigHandler.Config.maxFPS = Math.Max(10, ConfigHandler.Config.maxFPS); Raylib.SetTargetFPS(ConfigHandler.Config.maxFPS); }
            if(!ConfigHandler.Config.limitFPS) {ImGui.EndDisabled();}

            ImGui.SeparatorText("Other");

            ImGui.Checkbox("Show Welcome Screen", ref ConfigHandler.Config.showWelcome);

            ImGui.Checkbox("Pin Window Positions", ref ConfigHandler.Config.keepWindowsInPlace);
            HoverTooltip("Pin Window Manager, Info, Simulation Controls, Save & Load and Grid Controls");

            if(ImGui.Checkbox("Use Multithreading", ref ConfigHandler.Config.useParallel))
            {
                renderer.SetParallel();
            }
            HoverTooltip("Use multiple threads for computation.\nDisable if you encounter stutters often.");

            // ImGui.Checkbox("Show Info Bar", ref ConfigHandler.Config.showInfo);
            // HoverTooltip("Show the info bar on top left.");

            if(!ConfigHandler.Config.showInfo) ImGui.BeginDisabled();
            ImGui.Checkbox("Show FPS in Info Bar", ref ConfigHandler.Config.showFPSinInfo);
            if(!ConfigHandler.Config.showInfo) ImGui.EndDisabled();
            
            ImGui.SeparatorText("Simulations Path");

            WidthX(300, () => ImGui.InputText("##simulationsPath", ref ConfigHandler.Config.simulationsPath, 512));
            ImGui.SameLine();
            if (ImGui.Button($"{FontAwesome6.Folder}##sim"))
            {
                Process.Start(new ProcessStartInfo(fileName: ConfigHandler.Config.simulationsPath) {UseShellExecute = true, Verb = "open"});
            }
            HoverTooltip("Open folder");

            ImGui.PopID();
        }
        
        ImGui.End();
        
        // ImGui.ShowStyleEditor();
    }

    public void ShowWelcome()
    {
        var key = "welcome";

        var width = 2 * ImGui.CalcTextSize("Welcome to Cellular Swarm!").X + 8 + 96;

        if(ImGui.Begin("Welcome", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.PushID(key);
            ImGui.PushTextWrapPos(width);

            CloseButton(ref showWelcome);
            ImGui.SameLine();
            var dontShowAgain = !ConfigHandler.Config.showWelcome;
            ImGui.Checkbox("Don't Show Again", ref dontShowAgain);
            ConfigHandler.Config.showWelcome = !dontShowAgain;

            ImGui.Separator(); // -----------

            ImGui.Image((IntPtr)iconNoBg.Id, new Vector2(96, 96));
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);
            ImGui.BeginChild("##title", new Vector2(width - 100, 96));
            ImGui.SetWindowFontScale(2);
            ImGui.Dummy(new Vector2(0, 16));
            ImGui.Text($"Welcome to Cellular Swarm!");
            ImGui.SetWindowFontScale(1);
            ImGui.Text($"v{SimulationRenderer.VERSION} - Last Updated 10th Aug 2026");
            ImGui.EndChild();
            ImGui.PopStyleColor();

            ImGui.Separator();

            ImGui.Text(" Cellular Swarm is a life simulator that lets you create your own life forms using custom genes & molecules you define.");
            ImGui.Text(" Start playing around with the sample simulations! Open the included \"Samples\" folder and drag & drop a file here to open.");
            
            ImGui.Separator(); // -----------

            ImGui.Text("See the");
            ImGui.SameLine();
            ImGui.TextLinkOpenURL("Itch.io page", "https://aeuludag.itch.io/cellular-swarm/");
            ImGui.SameLine();
            ImGui.Text("and devlogs below for a quick introduction.");

            ImGui.SeparatorText("Devlogs");
            ImGui.TextLinkOpenURL("Devlog 0 - Release", "https://aeuludag.itch.io/cellular-swarm/devlog/1315596/release");
            ImGui.SameLine();
            ImGui.Text("- 10.08.2026");
            
            ImGui.Separator(); // -----------

            ImGui.Text("Hope you enjoy! Feel free to contact if you have any feedback.");
            ImGui.TextLinkOpenURL("@aeuludag", "https://aeuludag.github.io");

            ImGui.SameLine();
            ImGui.TextLinkOpenURL("Source Code", "https://github.com/aeuludag/CellularSwarm");

            ImGui.TextDisabled("Made with Raylib, ImGui and love <3.");

            ImGui.PopTextWrapPos();
            ImGui.PopID();
        }
        ImGui.End();
    }

    public void ShowInfo()
    {
        var key = "info";

        // ImGui.PushStyleVarY(ImGuiStyleVar.)
        
        // Raylib.DrawText($"Cellular Swarm - v{SimulationRenderer.VERSION} - {RuntimeInformation.OSArchitecture} - {Environment.OSVersion} - {DateTime.Today:d} - {Raylib.GetFPS()} FPS\n{(play ? "Playing..." : "Paused")}", 5, 5, 15, Color.White);

        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0.3f));
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 0.8f));
        ImGui.PushStyleColor(ImGuiCol.TextDisabled, new Vector4(1f, 1f, 1f, 0.4f));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(2f, 2f));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

        if(ImGui.Begin("Info", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove))
        {
            ImGui.PushID(key);

            ImGui.Text($"Cellular Swarm");
            ImGui.SameLine();
            ImGui.Text($"v{SimulationRenderer.VERSION} - {Environment.OSVersion} {RuntimeInformation.OSArchitecture} - {DateTime.Today:d}{(ConfigHandler.Config.showFPSinInfo ? $" - {Raylib.GetFPS()} FPS" : "")}");

            ImGui.PopID();
        }
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(3);
        ImGui.End();
    }

    public void SetCellIndex(int index)
    {
        renderer.cellIndex = index;
        selectorStates["cellPaletteIndex"] = index;
    }

    public static Image LoadImage(string fileName)
    // help by gpt!
    {
        DebugConsole.Info($"Loading image [{fileName}].", "EDITOR");
        byte[] imageBytes;
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"CellularSwarm.Visualizer.{fileName}"))
        {
            imageBytes = new byte[stream!.Length];
            stream.ReadExactly(imageBytes, 0, imageBytes.Length);
        }
        Image img = Raylib.LoadImageFromMemory(".png", imageBytes);
        return img;
    }
    public static Texture2D LoadTexture(string fileName)
    {
        Image img = LoadImage(fileName);
        Texture2D myTexture = Raylib.LoadTextureFromImage(img);
        Raylib.UnloadImage(img);
        LoadedTextures.Add(myTexture);
        return myTexture;
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
                WidthX(200, () => { if (ImGui.SliderFloat("##valueSlider", ref value, min, max)) dict[dictKey] = value = Math.Clamp(value, min, max); });
                // ImGui.SameLine();
                // WidthX(ImGui.CalcTextSize($"{max:F3}").X + 20, () => { if (ImGui.InputFloat("##valueInput", ref value)) { value = Math.Clamp(value, min, max); dict[dictKey] = value; } });

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
                RedButton(() => {if (ImGui.Button(IconFonts.FontAwesome6.TrashCan + " Remove")) list.Remove(item); });

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
        var isGridEmpty = simulation.Cells.Count == 0;
        var isLast = dictToRemoveFrom.Count == 1;
        var canDelete = !isReferenced && isGridEmpty && !isLast;
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);
        if (ImGui.BeginChild("removalArea", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetFrameHeight())))
        {
            if (!canDelete) ImGui.BeginDisabled();

            RedButton(() =>
            {
                if (ImGui.Button($"Delete {name}"))
                {
                    dictToRemoveFrom.Remove(id);
                }
            });
            if (!canDelete) ImGui.EndDisabled();
            // if (!canDelete)
            // {
            //     ImGui.SameLine();
            //     ImGui.TextColored(RED_WARNING, "Can't delete this now.");
            // }
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();

        if (!canDelete && ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextColored(RED_WARNING, "Can't delete this now.");
            if (isReferenced) ImGui.BulletText($"{name} is still used (see Info).");
            if (!isGridEmpty) ImGui.BulletText($"Grid must be cleared first.");
            if (isLast) ImGui.BulletText($"At least one {name} should remain.");
            ImGui.EndTooltip();
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
            GeneAction.ActionType.TransportMorphogen => "Active Transportation",
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
            3 => "Gene Activity",
            4 => "Gene Condition",
            _ => ""
        };
    }

    public static void SetTitle(string simulationName)
    {
        Raylib.SetWindowTitle($"\"{simulationName}\" - Cellular Swarm");
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
        ImGui.PushStyleColor(ImGuiCol.Text, Vector4.One);
        if (ImGui.Button($"Close"))
        {
            toFalse = false;
        }
        ImGui.PopStyleColor(4);
    }

    void RedButton(Action content)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.Text, Vector4.One);
        content();
        ImGui.PopStyleColor(4);
    }

    void WidthX(float x, Action content)
    {
        ImGui.PushItemWidth(x);
        content();
        ImGui.PopItemWidth();
    }

    public static void HoverTooltip(string text)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text(text);
            ImGui.EndTooltip();
        }
    }

    public static void HoverTooltip(string text, string subtext, bool showKeyboard = false)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text(text);
            if(showKeyboard) { ImGui.TextDisabled(FontAwesome6.Keyboard); ImGui.SameLine(); }
            ImGui.TextDisabled(subtext);
            ImGui.EndTooltip();
        }
    }

    public static void WinPos(float x, float y, float px, float py)
    {
        ImGui.SetNextWindowPos(new Vector2(x, y), ImGuiCond.Once, new Vector2(px, py));
    }
    public static void WinPosC(float x, float y, float px, float py) // c as in conditional and not communism though that would be dope
    {
        ImGui.SetNextWindowPos(new Vector2(x, y), ConfigHandler.Config.keepWindowsInPlace ? ImGuiCond.Always : ImGuiCond.Once, new Vector2(px, py));
    }

    public enum GridState
    {
        Move,
        Brush,
        Erase,
        Inspect
    }
}