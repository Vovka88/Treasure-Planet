public class CellData {
    public enum Cell_Type{
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Orange = 5,
        Cloud = 6
    }

    public enum Cell_Version{
        Default = 0,
        Bomb = 1,
        Horizontal_Spliter = 2,
        Vertical_Spliter = 3
    }

    public Cell_Type cell_Type;
    public Cell_Version cell_Version;
    public Point point;

    public CellData(Cell_Type type, Point point){
        cell_Type = type;
        cell_Version = Cell_Version.Default;
        this.point = point;
    }

    public CellData(Cell_Type type, Cell_Version version, Point point){
        cell_Type = type;
        cell_Version = version;
        this.point = point;
    }
}