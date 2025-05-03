using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions.Must;
using Unity.Collections;

[RequireComponent(typeof(CellFactory), typeof(MatchChecker))]
public class TableController : MonoBehaviour
{
    [SerializeField] public RectTransform _tableRect;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] public Sprite[] defaultColors;
    [SerializeField] public Sprite[] bombColors;
    [SerializeField] public Sprite[] spliterColors;
    [SerializeField] private int spaceBetween;
    [SerializeField] private ArrayLayout[] tableLayout;
    // [SerializeField] private SwipeController swipeController;

    [SerializeField] public LevelTarget[] targetsOnLevel;
    [HideInInspector] public CellFactory cellFactory;
    private MatchChecker matchChecker;

    [HideInInspector] public CellData[,] table;
    public int table_width;
    public int table_height;

    private bool isSwapping = false;
    public int turns = 25;
    [SerializeField] private int scoreForTile = 100;
    [SerializeField] public int[] scoreForEnd = new int[3];
    [SerializeField] public Sprite ultraBombSprite;


    [HideInInspector] public Point pointFrom;
    [HideInInspector] public Point pointTo;
    private bool swipeDetected = false;
    [SerializeField] private int level_id;

    private Item item_hammer = new Item(3, Item.ItemType.Hammer);
    private Item item_galaxy = new Item(3, Item.ItemType.Galaxy);
    private Item item_ufo = new Item(3, Item.ItemType.UFO);
    private Item item_meteorites = new Item(3, Item.ItemType.Meteorites);


    // EVENTS

    public event Action<int> onTurnsChanged;
    public event Action<int> onScoreChanged;
    public event Action<Item> activateItem;




    private void Awake()
    {
        cellFactory = GetComponent<CellFactory>();
        matchChecker = GetComponent<MatchChecker>();

        Input.simulateMouseWithTouches = false;

        activateItem += ActivateItem;

        Application.targetFrameRate = 120;
        Time.fixedDeltaTime = 1f / 120f;
        DOTween.SetTweensCapacity(500, 50);
    }

    private void Start()
    {
        InitializeTable();
        cellFactory.InstantiateTable(this);

        // var busters = DataManager.Instance.busters_on_level;

        List<CellData> bustedTiles = new List<CellData>();
        Debug.Log(DataManager.Instance.busters_on_level.Count);

        foreach (var buster in DataManager.Instance.busters_on_level)
        {
            int x, y;
            do
            {
                x = UnityEngine.Random.Range(0, table_width);
                y = UnityEngine.Random.Range(0, table_height);
            } while (table[x, y].cell_Type == CellData.Cell_Type.None ||
                     table[x, y].cell_Type >= CellData.Cell_Type.Cloud);

            bustedTiles.Add(ApplyBuster(x, y, buster.type));
            Destroy(cellFactory.cells[x, y].gameObject);
            Debug.Log("Bust Applied!");
        }
        Debug.Log("All Busters Applied");

        DataManager.Instance.busters_on_level.Clear();
        cellFactory.CreateSpecialVersionsFromCellData(bustedTiles);
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (turns > 0 && !IsAllTargetsCompleated())
        {
            Debug.Log("Is all target Compleated? = " + IsAllTargetsCompleated());
            yield return StartCoroutine(DestroyAllMatches());
            yield return StartCoroutine(WaitForSwipe());
        }
        yield return SaveLevelData();
    }

    private IEnumerator WaitForSwipe()
    {
        swipeDetected = false;

        Debug.Log("🟡 Waiting for swipe...");
        yield return new WaitUntil(() => swipeDetected);

        // Debug.Log($"✅ Swipe from {swipeStart} → {swipeDirection}");
        if (!IsInBounds(pointTo))
        {
            Debug.Log("🚫 Swipe out of bounds.");
            yield break;
        }

        yield return StartCoroutine(HandleSwap(pointFrom, pointTo));
    }

    private IEnumerator HandleSwap(Point a, Point b)
    {
        if (isSwapping) yield break;
        isSwapping = true;

        if (table[a.x, a.y].cell_Type < CellData.Cell_Type.Cloud &&
            table[b.x, b.y].cell_Type < CellData.Cell_Type.Cloud &&
            table[a.x, a.y].cell_Type > CellData.Cell_Type.None &&
            table[b.x, b.y].cell_Type > CellData.Cell_Type.None)
        {
            if (table[a.x, a.y].cell_Version != CellData.Cell_Version.Default || table[b.x, b.y].cell_Version != CellData.Cell_Version.Default)
            {
                // 🔥 Здесь происходит спецкомбинация
                Debug.Log("💥 Special combination!");

                // Просто пример обработки, можно кастомизировать
                SwapCells(a, b);
                yield return new WaitForSeconds(0.2f);

                // Пример: если одна из фишек — бомба
                if ((table[a.x, a.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Vertical_Spliter) ||
                (table[a.x, a.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter) ||
                (table[a.x, a.y].cell_Version == CellData.Cell_Version.Vertical_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Vertical_Spliter) ||
                    (table[a.x, a.y].cell_Version == CellData.Cell_Version.Vertical_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter))
                {
                    matchChecker.DoubleSplitterBreaker(a.x, a.y);
                    yield return StartCoroutine(DestroyAllMatches()); // дополнительный матч
                }
                else if ((table[a.x, a.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Bomb) ||
                (table[a.x, a.y].cell_Version == CellData.Cell_Version.Bomb && table[b.x, b.y].cell_Version == CellData.Cell_Version.Horizontal_Spliter) ||
                (table[a.x, a.y].cell_Version == CellData.Cell_Version.Bomb && table[b.x, b.y].cell_Version == CellData.Cell_Version.Vertical_Spliter) ||
                    (table[a.x, a.y].cell_Version == CellData.Cell_Version.Vertical_Spliter && table[b.x, b.y].cell_Version == CellData.Cell_Version.Bomb))
                {
                    matchChecker.SplitterBombBreaker(a.x, a.y);
                    yield return StartCoroutine(DestroyAllMatches());
                }
                else if (table[a.x, a.y].cell_Version == CellData.Cell_Version.Bomb && table[b.x, b.y].cell_Version == CellData.Cell_Version.Bomb)
                {
                    matchChecker.DoubleBombBreaker(a.x, a.y);
                    yield return StartCoroutine(DestroyAllMatches());
                }
                else if (table[a.x, a.y].cell_Version == CellData.Cell_Version.Default && table[b.x, b.y].cell_Version == CellData.Cell_Version.Ultra_Bomb)
                {
                    matchChecker.UltraBombBreaker(b.x, b.y, table[a.x, a.y].cell_Type);
                    yield return StartCoroutine(DestroyAllMatches());
                }
                else if (table[b.x, b.y].cell_Version == CellData.Cell_Version.Default && table[a.x, a.y].cell_Version == CellData.Cell_Version.Ultra_Bomb)
                {
                    matchChecker.UltraBombBreaker(a.x, a.y, table[b.x, b.y].cell_Type);
                    yield return StartCoroutine(DestroyAllMatches());
                }
                else if (table[b.x, b.y].cell_Version == CellData.Cell_Version.Ultra_Bomb && table[a.x, a.y].cell_Version == CellData.Cell_Version.Ultra_Bomb)
                {
                    matchChecker.UltraUltraBombBreaker();
                    yield return StartCoroutine(DestroyAllMatches());
                }

                isSwapping = false;
                yield break;
            }

            SwapCells(a, b);
            yield return new WaitForSeconds(0.3f); // wait animation

            if (!HasMatch())
            {
                Debug.Log("🔁 No match, reverting...");
                SwapCells(a, b);
                yield return new WaitForSeconds(0.3f); // wait revert animation
            }
            else
            {
                onTurnsChanged.Invoke(--turns);
                yield return StartCoroutine(DestroyAllMatches());
            }
        }

        isSwapping = false;
    }



    private void DropCells()
    {
        // Debug.Log("🔽 DropCells started");

        for (int y = table_height - 2; y >= 0; y--)
        {
            for (int x = 0; x < table_width; x++)
            {
                if (table[x, y] == null || table[x, y].cell_Type == CellData.Cell_Type.None || table[x, y].cell_Type >= CellData.Cell_Type.Cloud) continue;

                int dropToY = y;
                for (int k = y + 1; k < table_height && table[x, k] == null; k++)
                    dropToY = k;

                if (dropToY != y)
                {
                    // Debug.Log($"📉 Dropping {table[x, y].cell_Type} from ({x},{y}) to ({x},{dropToY})");

                    table[x, dropToY] = table[x, y];
                    table[x, y] = null;

                    table[x, dropToY].point = new Point(x, dropToY);

                    cellFactory.cells[x, dropToY] = cellFactory.cells[x, y];
                    cellFactory.cells[x, y] = null;

                    var cell = cellFactory.cells[x, dropToY];
                    if (cell != null)
                    {
                        cell.UpdateName();
                        cell.rect.DOAnchorPos(GetPositionWithPadding(new Point(x, dropToY)), 0.4f).SetEase(Ease.OutQuad);
                    }
                }
            }
        }
        // Debug.Log("✅ DropCells finished");
    }

    private void SwapCells(Point a, Point b)
    {
        (table[a.x, a.y], table[b.x, b.y]) = (table[b.x, b.y], table[a.x, a.y]);
        (table[a.x, a.y].point, table[b.x, b.y].point) = (a, b);

        (cellFactory.cells[a.x, a.y], cellFactory.cells[b.x, b.y]) = (cellFactory.cells[b.x, b.y], cellFactory.cells[a.x, a.y]);

        cellFactory.cells[a.x, a.y].rect.DOAnchorPos(GetPositionWithPadding(a), 0.3f).SetEase(Ease.OutQuad);
        cellFactory.cells[b.x, b.y].rect.DOAnchorPos(GetPositionWithPadding(b), 0.3f).SetEase(Ease.OutQuad);
    }

    public IEnumerator SwipeByCell(Point from, Point to)
    {
        if (isSwapping) yield break;

        if (!IsInBounds(from) || !IsInBounds(to))
            yield break;

        yield return StartCoroutine(HandleSwap(from, to));
    }


    /// <summary>
    /// Couroutine to destroy all matched tiles and create Special Versions of them
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyAllMatches()
    {
        yield return new WaitForSeconds(0.2f);

        while (true)
        {
            var matches = CheckForMatches();
            if (matches.Count == 0) break;

            List<CellData> protectedPoints = matchChecker.MarkSpecialTiles(pointFrom, pointTo);

            foreach (var item in matches)
            {
                if (table[item.point.x, item.point.y] != null)
                {
                    table[item.point.x, item.point.y] = null;
                }

                if (cellFactory.cells[item.point.x, item.point.y] != null)
                {
                    cellFactory.cells[item.point.x, item.point.y].InstantiateParticles();
                    Destroy(cellFactory.cells[item.point.x, item.point.y].gameObject);
                    cellFactory.cells[item.point.x, item.point.y] = null;
                    DataManager.Instance.am.PlayDestroySound();
                }
            }

            cellFactory.CreateSpecialVersionsFromCellData(protectedPoints);
            DataManager.Instance.current_score += matches.Count * scoreForTile;
            onScoreChanged.Invoke(DataManager.Instance.current_score);
            // Debug.Log("Current score: " + DataManager.Instance.current_score);
            yield return new WaitForSeconds(0.2f);
            DropCells();
            yield return new WaitForSeconds(0.5f);
            cellFactory.UpdateTable();
            matchChecker.ClearMatchedTiles();
            matchChecker.ClearTilesToDelete();
        }
    }

    private bool HasMatch()
    {
        matchChecker.HorizontalCheck();
        matchChecker.VerticalCheck();
        matchChecker.ActivateSpecialEffects();
        var array = matchChecker.GetTilesToDelete();
        // matchChecker.ClearMatchedTiles();
        return array.Count > 0;
    }

    private List<CellData> CheckForMatches()
    {
        matchChecker.HorizontalCheck();
        matchChecker.VerticalCheck();
        matchChecker.ActivateSpecialEffects();
        matchChecker.FindCloudsNearMatches();



        var matchedTiles = matchChecker.GetTilesToDelete();

        List<CellData> tilesToTargets = new List<CellData>();

        foreach (var target in targetsOnLevel)
        {
            if (target.count >= 0)
            {
                foreach (var tile in matchedTiles)
                {
                    if (tile.cell_Type == target.type)
                    {
                        tilesToTargets.Add(tile);
                    }
                }
            }
        }

        foreach (var target in targetsOnLevel)
        {
            var finded = tilesToTargets.FindAll(t => t.cell_Type == target.type);
            target.UpdateCount(target.count - finded.Count);
            // Debug.LogError($"Checked matches for target: {finded.Count / 2}");
        }

        return matchedTiles;
    }



    public void NotifySwipe(Point from, Point to)
    {
        pointFrom = from;
        pointTo = to;
        swipeDetected = true;
    }

    public bool HasAvailableMoves()
    {
        for (int y = 0; y < table_height; y++)
        {
            for (int x = 0; x < table_width; x++)
            {
                CellData current = table[x, y];
                if (current == null || current.cell_Type == CellData.Cell_Type.None) continue;

                // Попробуем поменять с соседом вправо
                if (x < table_width - 1)
                {
                    if (CanSwapCreateMatch(x, y, x + 1, y)) return true;
                }

                // Попробуем поменять с соседом вниз
                if (y < table_height - 1)
                {
                    if (CanSwapCreateMatch(x, y, x, y + 1)) return true;
                }
            }
        }

        return false;
    }

    private bool CanSwapCreateMatch(int x1, int y1, int x2, int y2)
    {
        // Поменять местами
        var temp = table[x1, y1];
        table[x1, y1] = table[x2, y2];
        table[x2, y2] = temp;

        // Проверить матч
        bool matchFound = IsPartOfMatch(x1, y1) || IsPartOfMatch(x2, y2);

        // Вернуть обратно
        temp = table[x1, y1];
        table[x1, y1] = table[x2, y2];
        table[x2, y2] = temp;

        return matchFound;
    }

    private bool IsPartOfMatch(int x, int y)
    {
        var type = table[x, y].cell_Type;

        // Горизонтально
        int countH = 1;
        int left = x - 1;
        while (left >= 0 && table[left, y] != null && table[left, y].cell_Type == type)
        {
            countH++; left--;
        }

        int right = x + 1;
        while (right < table_width && table[right, y] != null && table[right, y].cell_Type == type)
        {
            countH++; right++;
        }

        // Вертикально
        int countV = 1;
        int down = y - 1;
        while (down >= 0 && table[x, down] != null && table[x, down].cell_Type == type)
        {
            countV++; down--;
        }

        int up = y + 1;
        while (up < table_height && table[x, up] != null && table[x, up].cell_Type == type)
        {
            countV++; up++;
        }

        return countH >= 3 || countV >= 3;
    }

    public void ShuffleBoard()
    {
        List<CellData> allCells = new List<CellData>();

        // Собираем все фишки
        for (int y = 0; y < table_height; y++)
        {
            for (int x = 0; x < table_width; x++)
            {
                if (table[x, y] != null)
                    allCells.Add(table[x, y]);
            }
        }

        // Перемешиваем
        System.Random rng = new System.Random();
        do
        {
            allCells = allCells.OrderBy(a => rng.Next()).ToList();

            int i = 0;
            for (int y = 0; y < table_height; y++)
            {
                for (int x = 0; x < table_width; x++)
                {
                    if (table[x, y] != null)
                        table[x, y] = allCells[i++];
                }
            }

        } while (!HasAvailableMoves());

        Debug.LogError("BOARD SHUFFLED");
    }



    private void InitializeTable()
    {
        table = new CellData[table_width, table_height];
        for (int y = 0; y < table_height; y++)
        {
            for (int x = 0; x < table_width; x++)
            {
                table[x, y] = new CellData(WhichLayout(x, y), new Point(x, y));
            }
        }
    }

    private CellData.Cell_Type WhichLayout(int x, int y)
    {
        for (int i = 0; i < tableLayout.Length; i++)
            if (tableLayout[i].rows[y].row[x])
                return (CellData.Cell_Type)i;
        return (CellData.Cell_Type)UnityEngine.Random.Range(1, 5);
    }

    private async Task SaveLevelData()
    {
        Debug.Log($"Game ended! | Score: {DataManager.Instance.current_score}");
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        if (DataManager.Instance.ns == null)
        {
            Debug.LogError("ns is NULL! Coroutine won't start.");
            return;
        }

        if (targetsOnLevel.All(t => t.count <= 0))
        {
            Debug.Log("WIN");
            bool completed = true;
            int score = DataManager.Instance.current_score;
            List<bool> stars_completed = new List<bool>();

            foreach (var item in scoreForEnd)
            {
                if (score >= item) stars_completed.Add(true);
            }

            Debug.Log($"Отправляем запрос: score = {score}, stars = {stars_completed.Count}, completed = {completed}");
            StartCoroutine(DataManager.Instance.ns.SaveLevelData(level_id, stars_completed.Count, completed, tcs));

            await tcs.Task.ContinueWith(_ =>
            {
                Debug.Log("Coroutine finished!");
                DataManager.Instance.current_level = level_id;
                DataManager.Instance.isReturnedFromLevel = true;
                SceneManager.LoadScene(1);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        else
        {
            Debug.Log("LOSE");
            bool completed = false;
            int score = DataManager.Instance.current_score;

            Debug.Log($"Отправляем запрос: score = {score}, stars = {0}, completed = {completed}");
            StartCoroutine(DataManager.Instance.ns.SaveLevelData(level_id, 0, completed, tcs));
            StartCoroutine(DataManager.Instance.ns.SaveHp(
                DataManager.Instance.player_hp - 1,
                DataManager.Instance.player_hp == 5
            // tcs
            ));

            await tcs.Task.ContinueWith(_ =>
            {
                Debug.Log("Coroutine finished!");
                DataManager.Instance.current_level = level_id;
                DataManager.Instance.isReturnedFromLevel = true;
                SceneManager.LoadScene(1);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }



    public bool IsInBounds(Point p)
    {
        return p.x >= 0 && p.x < table_width && p.y >= 0 && p.y < table_height;
    }

    private bool IsAllTargetsCompleated()
    {
        foreach (var target in targetsOnLevel)
        {
            if (target.count > 0) return false;
        }
        return true;
    }



    public static Vector2 GetBoardPositionFromPoint(Point point)
    {
        return new Vector2(
            (Config.cell_size * point.x) + Config.cell_size / 2,
            -(Config.cell_size * point.y) - Config.cell_size / 2
        );
    }

    private Vector2 GetPositionWithPadding(Point point)
    {
        Vector2 basePosition = GetBoardPositionFromPoint(point);
        return basePosition + cellFactory._padding;
    }



    private CellData ApplyBuster(int x, int y, Buster.BusterType type)
    {
        switch (type)
        {
            case Buster.BusterType.Bomb:
                table[x, y].cell_Version = CellData.Cell_Version.Bomb;
                break;
            case Buster.BusterType.UltraBomb:
                table[x, y].cell_Version = CellData.Cell_Version.Ultra_Bomb;
                break;
            case Buster.BusterType.Splitter:
                table[x, y].cell_Version = CellData.Cell_Version.Horizontal_Spliter;
                break;
        }

        return table[x, y];
    }

    private void ActivateItem(Item item)
    {
        var matches = matchChecker.GetTilesToDelete();
        Point p1 = new Point(2, 1);
        Point p2 = new Point(3, 0);
        Point p3 = new Point(4, 1);
        foreach(var point in item.Execute(new Point[] {p1, p2, p3}))
        {
            if (table[point.x, point.y].cell_Type > CellData.Cell_Type.None && table[point.x, point.y].cell_Type < CellData.Cell_Type.Cloud) matches.Add(table[point.x, point.y]);
        }
    }
}
