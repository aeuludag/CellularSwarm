using System;
using Newtonsoft.Json;

namespace CellularSwarm.Core.Data;

public class CellData
{
    // [JsonProperty("ct")]
    public int cellTypeID;
    // [JsonProperty("m")]
    public Dictionary<int, float> morphogens = new();

    public static CellData FromCell(Cell cell)
    {
        return new CellData
        {
            cellTypeID = cell.cellType.id,
            morphogens = cell.Morphogens.ToDictionary(m => m.Key, m => m.Value)
        };
    }

    public static Cell ToCell(Simulation simulation, CellData data)
    {
        var cellType = simulation.CellTypes[data.cellTypeID];
        return new Cell(simulation, cellType, data.morphogens);
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
