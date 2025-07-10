namespace CellularSwarm.Core.Data;

public class CellData
{
    public int cellTypeID;
    public int DNAId;
    public List<CellularContentPair> cellularContent;
}

public class CellularContentPair
{
    public int morphogenID;
    public int concentration;
}