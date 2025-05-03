using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{

    public TableController tableController;

    private List<CellData> matchedTiles = new List<CellData>();
    private List<CellData> tilesToDelete = new List<CellData>();
    public CellData[,] table => tableController.table;

    public void HorizontalCheck()
    {
        for (int y = 0; y < tableController.table_height; y++)
        {
            for (int x = 0; x < tableController.table_width - 2; x++)
            {
                if (table[x, y] != null &&
                    table[x + 1, y] != null &&
                    table[x + 2, y] != null)
                {
                    if (table[x, y].cell_Type > 0 &&
                        table[x, y].cell_Type < CellData.Cell_Type.Cloud &&
                        table[x, y].cell_Type == table[x + 1, y].cell_Type &&
                        table[x, y].cell_Type == table[x + 2, y].cell_Type)
                    {

                        matchedTiles.Add(table[x, y]);
                        matchedTiles.Add(table[x + 1, y]);
                        matchedTiles.Add(table[x + 2, y]);
                        if (x + 3 < tableController.table_width &&
                                table[x + 3, y] != null &&
                                table[x, y].cell_Type == table[x + 3, y].cell_Type)
                        {
                            matchedTiles.Add(table[x + 3, y]);

                            if (x + 4 < tableController.table_width &&
                                table[x + 4, y] != null &&
                                table[x, y].cell_Type == table[x + 4, y].cell_Type)
                            {
                                matchedTiles.Add(table[x + 4, y]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void VerticalCheck()
    {
        for (int x = 0; x < tableController.table_width; x++)
        {
            for (int y = 0; y < tableController.table_height - 2; y++)
            {
                if (table[x, y] != null &&
                    table[x, y + 1] != null &&
                    table[x, y + 2] != null)
                {
                    if (table[x, y].cell_Type > 0 &&
                        table[x, y].cell_Type < CellData.Cell_Type.Cloud &&
                        table[x, y].cell_Type == table[x, y + 1].cell_Type &&
                        table[x, y].cell_Type == table[x, y + 2].cell_Type)
                    {

                        matchedTiles.Add(table[x, y]);
                        matchedTiles.Add(table[x, y + 1]);
                        matchedTiles.Add(table[x, y + 2]);

                        if (y + 3 < tableController.table_height && table[x, y].cell_Type == table[x, y + 3].cell_Type)
                        {
                            matchedTiles.Add(table[x, y + 3]);
                            if (y + 4 < tableController.table_height && table[x, y].cell_Type == table[x, y + 4].cell_Type)
                            {
                                matchedTiles.Add(table[x, y + 4]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ActivateSpecialEffects()
    {
        // Создаём копию matchedTiles, чтобы безопасно перебирать
        var matchedCopy = new List<CellData>(matchedTiles);

        foreach (var cell in matchedCopy)
        {
            if (cell == null) continue;

            switch (cell.cell_Version)
            {
                case CellData.Cell_Version.Bomb:
                    BombBreaker(cell.point.x, cell.point.y);
                    break;
                case CellData.Cell_Version.Horizontal_Spliter:
                    SpliterBreaker(cell.point.x, cell.point.y);
                    break;
                case CellData.Cell_Version.Vertical_Spliter:
                    SpliterBreaker(cell.point.x, cell.point.y, true);
                    break;
            }

            // Обнуляем, если надо
            cell.cell_Version = CellData.Cell_Version.Default;
        }
    }

    public List<CellData> MarkSpecialTiles(Point movedFrom, Point movedTo)
    {
        var allMatches = new List<(List<CellData> line, bool isHorizontal)>();

        // Горизонтальные линии
        for (int y = 0; y < tableController.table_height; y++)
        {
            int x = 0;
            while (x < tableController.table_width - 2)
            {
                var line = new List<CellData>();
                if (table[x, y] != null && table[x + 1, y] != null && table[x + 2, y] != null &&
                    table[x, y].cell_Type == table[x + 1, y].cell_Type &&
                    table[x, y].cell_Type == table[x + 2, y].cell_Type)
                {
                    int startX = x;
                    while (x < tableController.table_width && table[x, y] != null &&
                           table[startX, y].cell_Type == table[x, y].cell_Type)
                    {
                        line.Add(table[x, y]);
                        x++;
                    }

                    if (line.Count >= 3)
                        allMatches.Add((line, true)); // горизонтальная линия
                }
                else
                {
                    x++;
                }
            }
        }

        // Вертикальные линии
        for (int x = 0; x < tableController.table_width; x++)
        {
            int y = 0;
            while (y < tableController.table_height - 2)
            {
                var line = new List<CellData>();
                if (table[x, y] != null && table[x, y + 1] != null && table[x, y + 2] != null &&
                    table[x, y].cell_Type == table[x, y + 1].cell_Type &&
                    table[x, y].cell_Type == table[x, y + 2].cell_Type)
                {
                    int startY = y;
                    while (y < tableController.table_height && table[x, y] != null &&
                           table[x, startY].cell_Type == table[x, y].cell_Type)
                    {
                        line.Add(table[x, y]);
                        y++;
                    }

                    if (line.Count >= 3)
                        allMatches.Add((line, false)); // вертикальная линия
                }
                else
                {
                    y++;
                }
            }
        }

        // Обработка всех комбо
        List<CellData> specialPoint = new List<CellData>();

        foreach (var (match, isHorizontal) in allMatches)
        {
            // Ищем среди match ту, что равна movedFrom или movedTo
            CellData special = match.FirstOrDefault(cell =>
                (cell.point.x == movedTo.x && cell.point.y == movedTo.y) ||
                (cell.point.x == movedFrom.x && cell.point.y == movedFrom.y));

            // Если не нашли — просто берём последнюю
            if (special == null)
                special = match[^1];

            if (match.Count >= 5)
            {
                special.cell_Version = CellData.Cell_Version.Ultra_Bomb;
                specialPoint.Add(special);
            }
            else if (match.Count >= 4)
            {
                special.cell_Version = isHorizontal
                    ? CellData.Cell_Version.Horizontal_Spliter
                    : CellData.Cell_Version.Vertical_Spliter;

                specialPoint.Add(special);
            }
        }

        List<CellData> bombPoint = FindLShapeMatches();

        foreach (var item in bombPoint)
        {
            specialPoint.Add(item);
        }

        return specialPoint;
    }

    private List<CellData> FindLShapeMatches()
    {
        List<CellData> specialMatches = new List<CellData>();

        for (int x = 0; x < tableController.table_width; x++)
        {
            for (int y = 0; y < tableController.table_height; y++)
            {
                var center = table[x, y];
                if (center == null) continue;

                var type = center.cell_Type;

                // Проверяем L-образную форму вправо и вниз
                if (x + 2 < tableController.table_width && y + 2 < tableController.table_height)
                {
                    if (table[x + 1, y] != null && table[x + 2, y] != null &&
                        table[x, y + 1] != null && table[x, y + 2] != null &&
                        table[x + 1, y].cell_Type == type &&
                        table[x + 2, y].cell_Type == type &&
                        table[x, y + 1].cell_Type == type &&
                        table[x, y + 2].cell_Type == type)
                    {
                        specialMatches.Add(center);
                        center.cell_Version = CellData.Cell_Version.Bomb; // Назови как хочешь
                        continue;
                    }
                }

                // Проверяем другие 3 поворота L-формы:
                if (x - 2 >= 0 && y + 2 < tableController.table_height)
                {
                    if (table[x - 1, y] != null && table[x - 2, y] != null &&
                        table[x, y + 1] != null && table[x, y + 2] != null &&
                        table[x - 1, y].cell_Type == type &&
                        table[x - 2, y].cell_Type == type &&
                        table[x, y + 1].cell_Type == type &&
                        table[x, y + 2].cell_Type == type)
                    {
                        specialMatches.Add(center);
                        center.cell_Version = CellData.Cell_Version.Bomb;
                        continue;
                    }
                }

                if (x + 2 < tableController.table_width && y - 2 >= 0)
                {
                    if (table[x + 1, y] != null && table[x + 2, y] != null &&
                        table[x, y - 1] != null && table[x, y - 2] != null &&
                        table[x + 1, y].cell_Type == type &&
                        table[x + 2, y].cell_Type == type &&
                        table[x, y - 1].cell_Type == type &&
                        table[x, y - 2].cell_Type == type)
                    {
                        specialMatches.Add(center);
                        center.cell_Version = CellData.Cell_Version.Bomb;
                        continue;
                    }
                }

                if (x - 2 >= 0 && y - 2 >= 0)
                {
                    if (table[x - 1, y] != null && table[x - 2, y] != null &&
                        table[x, y - 1] != null && table[x, y - 2] != null &&
                        table[x - 1, y].cell_Type == type &&
                        table[x - 2, y].cell_Type == type &&
                        table[x, y - 1].cell_Type == type &&
                        table[x, y - 2].cell_Type == type)
                    {
                        specialMatches.Add(center);
                        center.cell_Version = CellData.Cell_Version.Bomb;
                        continue;
                    }
                }
            }
        }

        return specialMatches;
    }


    public List<CellData> FindObjectsOnField(CellData.Cell_Type type)
    {
        List<CellData> cd = new List<CellData>();

        for (int x = 0; x < tableController.table_width; x++)
        {
            for (int y = 0; y < tableController.table_height; y++)
            {
                if (table[x, y] != null)
                    if (table[x, y].cell_Type == type)
                        cd.Add(table[x, y]);
            }
        }

        Debug.LogError(cd.Count);

        return cd;
    }

    public void FindCloudsNearMatches()
    {
        List<CellData> cloudsToAdd = new List<CellData>();

        // Копируем только исходные матчевые фишки
        // List<CellData> originalMatches = new List<CellData>(matchedTiles);

        foreach (CellData tile in matchedTiles)
        {
            int x = tile.point.x;
            int y = tile.point.y;

            AddIfCloudAndNotAlready(x + 1, y, cloudsToAdd);
            AddIfCloudAndNotAlready(x - 1, y, cloudsToAdd);
            AddIfCloudAndNotAlready(x, y + 1, cloudsToAdd);
            AddIfCloudAndNotAlready(x, y - 1, cloudsToAdd);
        }

        tilesToDelete.AddRange(cloudsToAdd);
    }

    private void AddIfCloudAndNotAlready(int x, int y, List<CellData> clouds)
    {
        if (x >= 0 && x < tableController.table_width &&
            y >= 0 && y < tableController.table_height)
        {
            CellData cell = table[x, y];

            if (cell != null && cell.cell_Type == CellData.Cell_Type.Cloud)
            {
                if (!IsInListByCoords(cell, matchedTiles) &&
                    !IsInListByCoords(cell, clouds))
                    clouds.Add(cell);
            }
        }
    }

    private bool IsInListByCoords(CellData cell, List<CellData> list)
    {
        return list.Any(c => Point.Equals(c.point, cell.point));
    }


    // Метод для активування Бомби
    private void BombBreaker(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newX = x + i;
                int newY = y + j;

                // Проверяем, чтобы не выйти за границы массива
                if (newX < 0 || newX >= tableController.table_width || newY < 0 || newY >= tableController.table_height)
                    continue;

                if (table[newX, newY] == null || table[newX, newY].cell_Type == CellData.Cell_Type.None)
                    continue;

                matchedTiles.Add(table[newX, newY]);
            }
        }
    }

    // Метод активування Сплітеру
    // MODE = TRUE  - Vertical
    // MODE = FALSE - Horizontal
    private void SpliterBreaker(int x, int y, bool mode = false)
    {
        if (!mode)
        {
            for (int i = 0; i < tableController.table_width; i++)
            {
                // if (table[i, y] != null && table[i, y].cell_Type != CellData.Cell_Type.None) matchedTiles.Add(table[i, y]);
                if (table[i, y] != null && table[i, y].cell_Type != CellData.Cell_Type.None) tilesToDelete.Add(table[i, y]);
            }
        }
        else
        {
            for (int i = 0; i < tableController.table_height; i++)
            {
                // if (table[x, i] != null && table[x, i].cell_Type != CellData.Cell_Type.None) matchedTiles.Add(table[x, i]);
                if (table[x, i] != null && table[x, i].cell_Type != CellData.Cell_Type.None) tilesToDelete.Add(table[x, i]);
            }
        }
    }

    public void UltraBombBreaker(int x, int y, CellData.Cell_Type type)
    {
        // if (type > 0 && (type < CellData.Cell_Type.Cloud || type == CellData.Cell_Type.Ultra))
        // {
        // if (type == CellData.Cell_Type.Ultra)
        // {
        // foreach (var item in table)
        // {
        //     matchedTiles.Add(item);
        // }
        // }
        // else
        // {
        foreach (var item in table)
        {
            // if (item.cell_Type == type) matchedTiles.Add(item);
            if (item.cell_Type == type) tilesToDelete.Add(item);
        }
        tilesToDelete.Add(table[x, y]);
        // }
        // }
    }

    public void DoubleSplitterBreaker(int x, int y)
    {
        for (int x1 = 0; x1 < tableController.table_width; x1++)
        {
            if (IsInBounds(x1, y) && table[x1, y].cell_Type != CellData.Cell_Type.None && table[x1, y].cell_Type != CellData.Cell_Type.Cloud)
                tilesToDelete.Add(table[x1, y]);
        }

        for (int y1 = 0; y1 < tableController.table_height; y1++)
        {
            if (IsInBounds(x, y1) && table[x, y1].cell_Type != CellData.Cell_Type.None && table[x, y1].cell_Type != CellData.Cell_Type.Cloud)
                tilesToDelete.Add(table[x, y1]);
        }
    }

    public void DoubleBombBreaker(int x, int y)
    {
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                int newX = x + i;
                int newY = y + j;

                if (IsInBounds(newX, newY) && table[newX, newY].cell_Type != CellData.Cell_Type.None)
                {
                    matchedTiles.Add(table[newX, newY]);
                }
            }
        }
    }

    public void SplitterBombBreaker(int x, int y)
    {
        for (int x1 = 0; x1 < tableController.table_width; x1++)
        {
            for (int y1 = y - 1; y1 <= y + 1; y1++)
            {
                if (IsInBounds(x1, y1) && table[x1, y1].cell_Type != CellData.Cell_Type.None && table[x1, y1].cell_Type != CellData.Cell_Type.Cloud)
                    tilesToDelete.Add(table[x1, y1]);
            }
        }

        for (int y2 = 0; y2 < tableController.table_height; y2++)
        {
            for (int x2 = x - 1; x2 <= x + 1; x2++)
            {
                if (IsInBounds(x2, y2) && table[x2, y2].cell_Type != CellData.Cell_Type.None && table[x2, y2].cell_Type != CellData.Cell_Type.Cloud)
                    tilesToDelete.Add(table[x2, y2]);
            }
        }
    }

    public void UltraUltraBombBreaker()
    {
        for (int x1 = 0; x1 < tableController.table_width; x1++)
        {
            for (int y1 = 0; y1 < tableController.table_height; y1++)
            {
                if (table[x1, y1].cell_Type != CellData.Cell_Type.None && table[x1, y1].cell_Type != CellData.Cell_Type.Cloud)
                    tilesToDelete.Add(table[x1, y1]);
            }
        }
    }




    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < tableController.table_width && y >= 0 && y < tableController.table_height;
    }
    public List<CellData> GetMatchedTiles() => matchedTiles;
    public List<CellData> GetTilesToDelete()
    {
        foreach (var item in matchedTiles)
        {
            tilesToDelete.Add(item);
        }
        return tilesToDelete;
    }
    public void ClearMatchedTiles() => matchedTiles.Clear();
    public void ClearTilesToDelete() => tilesToDelete.Clear();
}