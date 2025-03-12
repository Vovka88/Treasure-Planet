using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class CellFactory : MonoBehaviour{

    private TableController tableController;
    [SerializeField] private RectTransform _tableRect;
    [SerializeField] private Cell _cellPrefab;
    public Cell[,] cells;

    public void InstantiateTable(TableController tableController){
        this.tableController = tableController;
        cells = new Cell[Config.maximal_table_horizontal, Config.maximal_table_vertical];
        for (int y = 0; y < Config.maximal_table_vertical; y++)
        {
            for (int x = 0; x < Config.maximal_table_horizontal; x++)
            {
                Point point = new (x, y);
                CellData cellData = GetCellAtPoint(point);
                CellData.Cell_Type cellType = cellData.cell_Type;
                // if (cellType <= 0) continue;

                var cell = InstantiateCell();
                
                cell.rect.anchoredPosition = TableController.GetBoardPositionFromPoint(point);
                
                if (cellType > 0) 
                cell.Initialize(cellData, tableController.colors[(int)(cellType-1)]);
                else
                cell.Initialize(cellData);

                cell.UpdateName();
                cells[x, y] = cell;
            }
        }
    }

    public void UpdateTable(){
        for (int y = 0; y < Config.maximal_table_vertical; y++)
        {
            for (int x = 0; x < Config.maximal_table_horizontal; x++)
            {
                if(tableController.table[x, y] == null){
                    CellData cellData = new CellData(RandomizeType(), new Point(x, y));

                    tableController.table[x, y] = cellData;
                    Cell cell = InstantiateCell();

                    cell.rect.anchoredPosition = TableController.GetBoardPositionFromPoint(cellData.point);

                    cell.Initialize(cellData, tableController.colors[(int)(cellData.cell_Type-1)]);

                    cell.UpdateName();
                    cells[x, y] = cell;
                }
            }
        }
    }

    private Cell InstantiateCell() => Instantiate(_cellPrefab, _tableRect);
    private CellData GetCellAtPoint(Point point) => tableController.table[point.x, point.y];
    private CellData.Cell_Type RandomizeType() => (CellData.Cell_Type)(Random.Range(0, 4)+1);
}