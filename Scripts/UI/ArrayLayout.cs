[System.Serializable]

public class ArrayLayout {
    [System.Serializable]
    public struct RowData{
        public bool[] row;
    }

    public RowData[] rows = new RowData[Config.maximal_table_vertical];
}