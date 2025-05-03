using System.Collections.Generic;
using UnityEngine;

public class CellFactory : MonoBehaviour
{
    private TableController tableController;
    [SerializeField] private RectTransform _tableRect;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] public Vector2 _padding;
    public Cell[,] cells;
    [SerializeField] private GameObject particle;

    public void InstantiateTable(TableController tableController)
    {
        this.tableController = tableController;
        cells = new Cell[tableController.table_width, tableController.table_height];

        for (int y = 0; y < tableController.table_height; y++)
        {
            for (int x = 0; x < tableController.table_width; x++)
            {
                Point point = new Point(x, y);
                CellData cellData = GetCellAtPoint(point);
                // Debug.Log($"Cell ({point.x};{point.y}): {cellData.cell_Type}");
                CellData.Cell_Type cellType = cellData.cell_Type;

                var cell = InstantiateCell();
                cell.tableController = tableController;
                cell.rect.anchoredPosition = GetPositionWithPadding(point);

                if (cellType > CellData.Cell_Type.None && cellType < CellData.Cell_Type.Cloud)
                {
                    int cellIndex = (int)(cellType - 1);
                    cell.Initialize(cellData, tableController.defaultColors[cellIndex], particle);
                }
                else if (cellType >= CellData.Cell_Type.Cloud)
                {
                    cell.Initialize(cellData, tableController.defaultColors[(int)(cellType - 1)], particle);
                }
                else
                    cell.Initialize(cellData);

                cell.UpdateName();
                cells[x, y] = cell;
            }
        }
    }

    public void UpdateTable()
    {
        for (int y = 0; y < tableController.table_height; y++)
        {
            for (int x = 0; x < tableController.table_width; x++)
            {
                if (tableController.table[x, y] == null)
                {
                    CellData cellData = new CellData(RandomizeType(), new Point(x, y));
                    tableController.table[x, y] = cellData;

                    Cell cell = InstantiateCell();
                    cell.tableController = tableController;
                    cell.rect.anchoredPosition = GetPositionWithPadding(cellData.point);
                    int cellIndex = (int)(cellData.cell_Type - 1);
                    cell.Initialize(cellData, tableController.defaultColors[cellIndex], particle);
                    cell.UpdateName();

                    cells[x, y] = cell;
                }
            }
        }
    }
    public void CreateSpecialVersionsFromCellData(List<CellData> protectedPoints)
    {
        foreach (var point in protectedPoints)
        {
            if(point.cell_Type > CellData.Cell_Type.None && 
                point.cell_Type < CellData.Cell_Type.Cloud)
            { 
                CreateSpecialVersionByCellData(point);
                tableController.table[point.point.x, point.point.y] = point;
            }
        }
    }

    private void CreateSpecialVersionByCellData(CellData cellData){
            CellData cd = cellData;
            Cell cell = InstantiateCell();
            cell.tableController = tableController;
            switch (cd.cell_Version)
            {
                case CellData.Cell_Version.Bomb:
                    cell.image.sprite = tableController.bombColors[(int)cd.cell_Type - 1];
                    cell.image.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case CellData.Cell_Version.Vertical_Spliter:
                    cell.image.sprite = tableController.spliterColors[(int)cd.cell_Type - 1];
                    cell.image.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case CellData.Cell_Version.Horizontal_Spliter:
                    cell.image.sprite = tableController.spliterColors[(int)cd.cell_Type - 1];
                    cell.image.rectTransform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case CellData.Cell_Version.Ultra_Bomb:
                    cell.image.sprite = tableController.ultraBombSprite;
                    cell.image.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                default:
                    cell.image.sprite = tableController.defaultColors[(int)cd.cell_Type - 1];
                    cell.image.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
            cell.rect.anchoredPosition = GetPositionWithPadding(cd.point);
            cell.Initialize(cd);
            cell.UpdateName();
            cells[cd.point.x, cd.point.y] = cell;
    }

    private Cell InstantiateCell() => Instantiate(_cellPrefab, _tableRect);
    private CellData GetCellAtPoint(Point point) => tableController.table[point.x, point.y];
    private CellData.Cell_Type RandomizeType() => (CellData.Cell_Type)(Random.Range(0, 4) + 1);

    private Vector2 GetPositionWithPadding(Point point)
    {
        Vector2 basePosition = TableController.GetBoardPositionFromPoint(point);
        return basePosition + _padding;
    }

}
