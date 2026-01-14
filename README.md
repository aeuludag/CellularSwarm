# Cellular Swarm

Cellular Swarm is a life simulator that lets you create your own life forms using custom genes & molecules you define. It aims to give an abstract idea on how cells that originated from a single cell in multicellular bodies differentiate over time in a process called *morphogenesis*.

## Project Structure

Cells lay on a 2D hexagonal grid. Each cell has the exact same *genes*. *Genes* are responsible for doing *actions* that are multiplying, apoptosis, *morphogen* addition-deletion and active transportation. *Genes* activate or deactivate based on given *conditions*, such as a certain *morphogen* concentration being enough or the cell being on the edge of the organism. Over time *morphogens* diffuse and decay, which may affect which *genes* are active and which are not.

## Simulation

Each simulation contains morphogens, cell types, genes, gene conditions, gene actions and the current cells. Every cell in the simulation shares the same defined genes (as explained in project structure).

In each simulation step these happen in the order:

1. All cells go through the genome and activate and inhibit genes based on the cellular conditions.
2. Morphogen addition-deletion and cell type differing actions of the activated genes are performed.
3. Cells that need to multiply are multiplied.
4. Diffusion happens (*diffusion step* many times).
5. Cells that need to go through apoptosis, go through apoptosis.
6. Active transportation actions of the remaining cells are performed.

Properties for diffusion:

**Maximum Concentration**: The maximum morphogen concentration that is allowed to exist. The overflown amount is ignored.

**Diffusion Step**: Controls how many times does the diffusion step of *morphogens* occur in each simulation step.

**Diffusion Threshold**: The minimum morphogen concentration above zero that is allowed to exist. Diffusion does not occur if the concentration difference of a morphogen between two cells are less than the threshold.

## Morphogen

Morphogens are chemicals that naturally diffuse between cells and decay. Their concentrations play a key role in differentiation.

**Diffusion Rate**: Controls how fast & easy the morphogen spreads between cells in each diffusion step (a simulation step can have more than one diffusion steps). It is proportional to the diffusion amount. Higher diffusion rate makes it easier for the morphogen to spread across the organism and lowering it makes the morphogen more concentrated in the area it is produced.

**Decay Rate:** Controls how much the morphogen will decay in a simulation step. Higher decay rate makes it harder for the morphogen to reach further cells, and lowering it may cause the morphogen to accumulate in the organism over time.

## Cell

Each cell occupies one unit on a hexagonal grid. Cells have their cellular content, consisting of morphogens and their concentrations. Cells are given a cell type which is useful for gene regulation and visualization.

## Cell Types

Cell types exist as a label and are used in gene conditions and visualizing.

## Genes

Genes form the logic for the entire simulation. Genes hold activator conditions, inhibitor conditions and gene actions. When activator conditions are met and inhibitor conditions are not, the gene gets activated and its actions are performed in the simulation step.

## Gene Conditions

Gene conditions are used to activate & inhibit genes. There are several gene condition types.

**Concentration Condition**: Has a morphogen to check, comparison type (greater than, less than, equal to), and a concentration threshold. Checks for the concentration of a given morphogen and compares it to the threshold. Returns true if the comparison is true.

**Neighbour Count Condition**: Has a comparison type (greater than, less than, equal to) and a count threshold. Checks the neighbour count of the cell (min. 0 and max. 6) and returns true if the comparison between count and threshold is true.

**Cell Type Condition**: Has a cell type. Returns true if the cell type of the cell is the given cell type.

The conditions can also be negated (”Cell type is not x”, “neighbour count is not y” etc.).

## Gene Actions

Gene actions are the actions that cells perform when their respective gene is active. These actions are morphogen addition-deletion, active transportation, cell type differentiation, multiplying and apoptosis.

**Morphogen Addition-Deletion**: Synthesises or removes a given amount of morphogen from the cell.

**Active Transportation**: The cell can create a pressure to suck in or push away given morphogens.

**Cell Type Differentiation**: Changes the cell type of the cell.

**Multiply**: The cell divides and creates a copy in a random available space. Asymmetric division can be implemented by setting a share ratio for a morphogen. Share ratio of `0.0` meaning no amount of that morphogen will be shared with the daughter cell. `0.5` is the default behaviour (shared equally) and `1.0` means all of that morphogen goes to the daughter cell.

**Apoptosis**: The cell dies with all of its cellular content.

## Technical Details

This project was made in C#. The C# solution has two separate projects: `CellularSwarm.Core` (core simulation logic) and `CellularSwarm.Visualizer` (visualization & rendering). This means that simulation logic doesn't require a visualizer to function, and you can write your own visualization engine if you don’t like mine.

This version uses Raylib to render the main window and cells. ImGui is used for UI elements (editor windows). I initially planned to use Unity for visualization, but it wasn’t working with the newer C# versions (and would be too clunky anyways). The lightness of Raylib + ImGui just felt right for some reason lol.

## Issues & Future Plans

I plan to optimize the project further by either rewriting it in a language like C or using compute shaders somehow. My current logical structure implemented in C# may not serve a pleasant view.

I think a 3D simulation might be interesting as well with similar a setup.

## Credits & References

I wanted to work on this project after seeing [this video](https://youtu.be/nLu4n7yNGdk) by *Simulife Hub*. The concept of creating your own life with your own genes and watching them grow and differentiate was really miraculous and I wanted to test it on my own too. Since their version was not open source I wanted to create my own life simulator.

Hope you like it!!! I would greatly appreciate feedbacks on how to improve.

-Emir