using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public RectTransform rect;
    [SerializeField] private Image image;
    private CellData cellData;

    public void Initialize(CellData cellData, Color color){
        this.cellData = cellData;
        if(cellData.cell_Type > 0) image.color = color;
        else if (cellData.cell_Type <= 0){
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void Initialize(CellData cellData, Sprite sprite){
        this.cellData = cellData;
        if(cellData.cell_Type > 0) image.sprite = sprite;
        else if (cellData.cell_Type <= 0){
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void Initialize(CellData cellData){
        this.cellData = cellData;
        if (cellData.cell_Type <= 0){
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void UpdateName() => transform.name = $"Cell [{cellData.point.x}; {cellData.point.y}]";
}
