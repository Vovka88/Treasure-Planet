using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] private RectTransform _boardRect;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private Color[] colors;

    private void Start() {
        for (int x = 0; x < Config.maximal_table_horizontal; x++)
        {
            for (int y = 0; y < Config.maximal_table_vertical; y++)
            {
                var cell = Instantiate(_cellPrefab, _boardRect);
                var point = new Point(x, y);
                var cell_Type = RandomizeType();
                cell.rect.anchoredPosition = GetBoardPositionFromPoint(point);
                cell.Initialize(cell_Type, point, colors[(int)(cell_Type-1)]);
            }
        }
    }

    private Vector2 GetBoardPositionFromPoint(Point point) => new Vector2(Config.cell_size / 2  + Config.cell_size * point.x, -Config.cell_size / 2  + -Config.cell_size * point.y);

    private Cell.Cell_Type RandomizeType() => (Cell.Cell_Type)(Random.Range(0, colors.Length)+1);
}
