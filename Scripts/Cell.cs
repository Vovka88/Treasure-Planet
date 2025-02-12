using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public enum Cell_Type{
        None = -1,
        Blank = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Orange = 5,
    }
    public RectTransform rect;
    [SerializeField] private Image image;
    private Cell_Type cell_Type;
    private Point point;

    public void Initialize(Cell_Type type, Point point, Color color){
    // Sprite sprite){
        cell_Type = type;
        image.color = color;
        this.point = point;
    }
}
