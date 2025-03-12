using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ModestTree;
using UnityEngine;

[RequireComponent(typeof(MatchChecker))]
[RequireComponent(typeof(CellFactory))]
public class TableController : MonoBehaviour
{
    public ArrayLayout[] tableLayout;
    [SerializeField] private RectTransform _tableRect;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] public Sprite[] defaultColors;
    [SerializeField] public Sprite[] bombColors;
    // [SerializeField] public Color[] colors;

    public CellData[,] table;
    private CellFactory cellFactory;
    private MatchChecker matchChecker;
    private SwipeController swipeController;

    public event Action<int> onValueChanged; 
    private int turns = 25;

    private void Awake() {
        cellFactory = GetComponent<CellFactory>();
        matchChecker = GetComponent<MatchChecker>();
        swipeController = GetComponent<SwipeController>();
    }

    private void Start() {
        InitializeTable();
        cellFactory.InstantiateTable(this);

        onValueChanged?.Invoke(turns);
        StartCoroutine(LoopDestroy());
    }

    private void HandleSwipe(Point start, Point direction) {
        Point target = new Point(start.x + direction.x, start.y + -direction.y);
        
        if (target.x < 0 || target.x >= Config.maximal_table_horizontal ||
            target.y < 0 || target.y >= Config.maximal_table_vertical) return;

        SwapCells(start, target);
        
        if (!CheckMatchAfterSwap()) {
            StartCoroutine(RevertSwap(start, target));
        }
        else {
            StartCoroutine(Destroy());
        }
    }

    private bool CheckMatchAfterSwap() {
        matchChecker.HorizontalCheck();
        matchChecker.VerticalCheck();
        return matchChecker.GetMatchedTiles().Count > 0;
    }

    private void SwapCells(Point p1, Point p2) {
        if (table[p1.x, p1.y] == null || table[p2.x, p2.y] == null) return;
        
        CellData tempData = table[p1.x, p1.y];
        table[p1.x, p1.y] = table[p2.x, p2.y];
        table[p2.x, p2.y] = tempData;
        
        table[p1.x, p1.y].point = new Point(p1.x, p1.y);
        table[p2.x, p2.y].point = new Point(p2.x, p2.y);

        Cell tempCell = cellFactory.cells[p1.x, p1.y];
        cellFactory.cells[p1.x, p1.y] = cellFactory.cells[p2.x, p2.y];
        cellFactory.cells[p2.x, p2.y] = tempCell;

        cellFactory.cells[p1.x, p1.y].rect.anchoredPosition = GetBoardPositionFromPoint(p1);
        cellFactory.cells[p2.x, p2.y].rect.anchoredPosition = GetBoardPositionFromPoint(p2);
    }

    private IEnumerator RevertSwap(Point p1, Point p2) {
        yield return new WaitForSeconds(0.3f);
        SwapCells(p1, p2);
    }

    private void ClearMatches(){
        var list = CheckForMatches();
        foreach (CellData item in list)
        {
            table[item.point.x, item.point.y] = null;
            Destroy(cellFactory.cells[item.point.x, item.point.y].gameObject);
        }
        list.Clear();
    }

    // Ініціалізує поле
    private void InitializeTable(){
        table = new CellData[Config.maximal_table_horizontal, Config.maximal_table_vertical];
        for (int y = 0; y < Config.maximal_table_vertical; y++)
        {
            for (int x = 0; x < Config.maximal_table_horizontal; x++)
            {
                table[x, y] = new CellData(
                    WhichTableLayoutUse(x, y),
                    new Point(x, y)
                );
            }
        }
    }

    // Метод повертаючий тип який буде стояти на початку гри у певній точці поля
    // Номера массиву: 0 - нічого; 1 - червоний; 2 - синій; 3 - зелений; 4 - жовтий
    // 5 - помаранчевий; 6 - облако;
    // Якщо не буде нічого - пoверне випадковий тип
    private CellData.Cell_Type WhichTableLayoutUse(int x, int y){
        for (int i = 0; i < tableLayout.Length; i++)
        {
            if ( tableLayout[i].rows[y].row[x] ) return (CellData.Cell_Type)i;
        }
        return RandomizeType();
    }

    private List<CellData> CheckForMatches(){
        matchChecker.HorizontalCheck();
        matchChecker.VerticalCheck();
        matchChecker.FindCloudsNearMatches();
        return matchChecker.GetMatchedTiles();
    }

    private void DropCells(){
        for (int y = Config.maximal_table_vertical - 1; y >= 0; y--)
        {
            for (int x = 0; x < Config.maximal_table_horizontal; x++)
            {
                if (table[x, y] == null) continue;
                if (table[x, y].cell_Type <= 0) continue;

                int targetY = y;
                for (int k = y + 1; k < Config.maximal_table_vertical; k++)
                {
                    if (table[x, k] == null) targetY = k;
                    else break;
                }

                if (targetY != y)
                {
                    if (cellFactory.cells[x, y] == null) continue;
                    table[x, targetY] = table[x, y];
                    table[x, y] = null;
                    if(table[x, targetY] != null) table[x, targetY].point = new Point(x, targetY);

                    cellFactory.cells[x, targetY] = cellFactory.cells[x, y];
                    cellFactory.cells[x, y] = null;

                    cellFactory.cells[x, targetY].UpdateName();
                    StartCoroutine(DropCellAnimation(x, targetY));
                }
            }
        }
    }

    IEnumerator DropCellAnimation(int x, int targetY){
        yield return cellFactory.cells[x, targetY].rect.DOMove(GetBoardPositionFromPoint(new Point(x, targetY)), 1f);
    }
    IEnumerator Destroy(){
        yield return new WaitForSeconds(0.3f);

        ClearMatches();
        yield return new WaitForSeconds(0.3f);
        
        DropCells();
        yield return new WaitForSeconds(0.3f);
        
        ClearMatches();
        yield return new WaitForSeconds(0.3f);
        
        cellFactory.UpdateTable();
    }

    IEnumerator LoopDestroy(){      
        while(turns > 0){
            do{
            
                StartCoroutine(Destroy());
                yield return new WaitForSeconds(3f);
                Debug.Log("Ended step");

            } while(!CheckForMatches().IsEmpty());

            swipeController.OnSwipe += HandleSwipe;
            yield return StartCoroutine(WaitForSwipe());
            Debug.Log("Player turn ended");
            swipeController.OnSwipe -= HandleSwipe;

            turns--;
        }

        Debug.Log("Ended loop");
    }

    private IEnumerator WaitForSwipe() {
        bool swipeDetected = false;
        Point start = new Point(0, 0);
        Point direction = new Point(0, 0);

        // Удаляем прошлые подписки, если они были
        swipeController.OnSwipe -= HandleSwipeEvent;
        swipeController.OnSwipe += HandleSwipeEvent;

        yield return new WaitUntil(() => swipeDetected);
        // HandleSwipe(start, direction);

        void HandleSwipeEvent(Point s, Point d) {
            start = s;
            direction = d;
            swipeDetected = true;
     }
}

    public static Vector2 GetBoardPositionFromPoint(Point point) => new Vector2(Config.cell_size / 2  + Config.cell_size * point.x, -Config.cell_size / 2  + -Config.cell_size * point.y);
    public CellData.Cell_Type RandomizeType() => (CellData.Cell_Type)(UnityEngine.Random.Range(0, 4)+1);
}