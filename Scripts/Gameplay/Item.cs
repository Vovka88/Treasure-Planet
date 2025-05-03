using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        UFO = 1,
        Galaxy = 2,
        Hammer = 3,
        Meteorites = 4,
    }

    public ItemType type;
    public int count;

    public Item(int count, ItemType type)
    {
        this.type = type;
        this.count = count;
    }


    public List<Point> Execute(Point[] point)
    {
        List<Point> points_to_destroy = new List<Point>();
        switch (type)
        {
            case ItemType.UFO:
                ExecuteUFO(points_to_destroy, point);
                break;
            case ItemType.Galaxy:
                ExecuteGalaxy(points_to_destroy, point[0]);
                break;
            case ItemType.Hammer:
                ExecuteHammer(points_to_destroy, point[1]);
                break;
            case ItemType.Meteorites:
                ExecuteMeteorites(points_to_destroy, point);
                break;
        }

        return points_to_destroy;
    }

    private static void ExecuteUFO(List<Point> points_to_destroy, Point[] point)
    {

    }

    private static void ExecuteGalaxy(List<Point> points_to_destroy, Point point)
    {
        for (int x = point.x - 1; x <= point.x + 1; x++)
        {
            for (int y = point.y - 1; y <= point.y + 1; y++)
            {
                points_to_destroy.Add(new Point(x, y));
            }
        }
    }

    private static void ExecuteHammer(List<Point> points_to_destroy, Point point)
    {
        points_to_destroy.Add(point);
    }

    private static void ExecuteMeteorites(List<Point> points_to_destroy, Point[] point)
    {
        foreach (var p in point)
        {
            points_to_destroy.Add(p);
        }
    }
}
