using UnityEngine;

public class Point : MonoBehaviour
{
    public int x;
    public int y;

    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    public void Add(Point point){
        x+=point.x;
        y+=point.y;
    }

    public void Multiply(int value){
        x*=value;
        y*=value;
    }

    public Vector2 ToVector() => new Vector2(x, y);
    public bool Equals(Point point) => x == point.x && y == point.y;

    public static Point Clone(Point point) => new Point(point.x, point.y);
    public static Vector2 ToVector(Point point) => new Vector2(point.x, point.y);
    public static Point FromVector(Vector2 vector) => new Point((int)vector.x, (int)vector.y);
    public static Point FromVector(Vector3 vector) => new Point((int)vector.x, (int)vector.y);
    public static Point Multiply(Point point, int value) => new Point(point.x * value, point.y * value);
    public static Point Add(Point point1, Point point2) => new Point(point1.x + point2.x, point1.y + point2.y);
    public static bool Equals(Point point1, Point point2) => point1.x == point2.x && point1.y == point2.y;

    public static Point zero => new (0, 0);
    public static Point up => new (0, 1);
    public static Point down => new (0, -1);
    public static Point right => new (1, 0);
    public static Point left => new (-1, 0);
}
