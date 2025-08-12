using System;

namespace CellularSwarm.Core.Data;

public class CellData
{
    public int cellTypeID;
    public List<int> geneIDs = new();
    public Dictionary<int, float> morphogens = new();

    public static CellData FromCell(Cell cell)
    {
        return new CellData
        {
            cellTypeID = cell.cellType.id,
            geneIDs = cell.genes.Select(g => g.id).ToList(),
            morphogens = cell.Morphogens.ToDictionary(m => m.Key, m => m.Value)
        };
    }

    public static Cell ToCell(Simulation simulation, CellData data)
    {
        var cellType = simulation.CellTypes[data.cellTypeID];
        var genes = data.geneIDs.Select(id => simulation.Genes[id]).ToList();
        return new Cell(simulation, cellType, genes, data.morphogens);
    }
}

public class CellTypeData
{
    public int id;
    public string name = string.Empty;
    public static CellTypeData FromCellType(CellType cellType)
    {
        return new CellTypeData
        {
            id = cellType.id,
            name = cellType.name
        };
    }

    public static CellType ToCellType(CellTypeData data)
    {
        return new CellType(data.id, data.name);
    }
}
