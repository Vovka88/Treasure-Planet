using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public RectTransform rect;
    [SerializeField] public Image image;
    private CellData cellData;
    private Vector2 dragStart;
    private float minSwipeDistance = 10f;
    private bool isHorizontal;
    public TableController tableController;
    public GameObject particles;

    public void Initialize(CellData cellData, Color color)
    {
        this.cellData = cellData;
        if (cellData.cell_Type > 0) image.color = color;
        else if (cellData.cell_Type <= 0)
        {
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void Initialize(CellData cellData, Sprite sprite, GameObject particles)
    {
        this.cellData = cellData;
        if (cellData.cell_Type > 0)
        {
            image.sprite = sprite;
            this.particles = particles;
        }
        else if (cellData.cell_Type <= 0)
        {
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void Initialize(CellData cellData)
    {
        this.cellData = cellData;
        if (cellData.cell_Type <= 0)
        {
            Color c = new Color();
            c.a = 0f;
            image.color = c;
        }
    }

    public void InstantiateParticles()
    {
        if (particles != null)
        {
            GameObject instance = Instantiate(particles, transform.parent); // Спавним не как дочерний к клетке, а в том же уровне
            instance.transform.position = new Vector3(transform.position.x, transform.position.y, -1); // Помещаем на позицию клетки
            instance.transform.localScale = Vector3.one; // Убеждаемся, что масштаб нормальный

            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax); // Удаляем через время жизни
            }
            else
            {
                Destroy(instance, 2f); // Фоллбек
            }

            Debug.Log("Effect instantiated at: " + transform.position);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(cellData.cell_Type.ToString());
        dragStart = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 end = eventData.position;
        Vector2 delta = end - dragStart;

        if (delta.magnitude < minSwipeDistance) return;

        Vector2 direction;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            direction = delta.x > 0 ? Vector2.right : Vector2.left;
        else
            direction = delta.y > 0 ? Vector2.down : Vector2.up;

        // Debug.Log("Swipe Direction: " + direction);

        if (tableController != null)
        {
            Point swipeDir = VectorToPoint(direction);
            Point from = cellData.point;
            Point to = Point.Add(from, swipeDir);

            tableController.pointFrom = from;
            tableController.pointTo = to;

            if (tableController.IsInBounds(to))
            {
                tableController.NotifySwipe(from, to);
                // tableController.StartCoroutine(tableController.SwipeByCell(from, to));
            }
        }
    }

    private Point VectorToPoint(Vector2 dir)
    {
        if (dir == Vector2.up) return Point.up;
        if (dir == Vector2.down) return Point.down;
        if (dir == Vector2.left) return Point.left;
        return Point.right;
    }

    public void UpdateName() => transform.name = $"Cell [{cellData.point.x}; {cellData.point.y}]";
}
