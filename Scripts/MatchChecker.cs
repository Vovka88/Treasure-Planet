using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour{

    public TableController tableController;

    private List<CellData> matchedTiles = new List<CellData>();
    public CellData[ , ] table => tableController.table;

    public void HorizontalCheck(){
        for (int y = 0; y < Config.maximal_table_vertical; y++)
        {
            for (int x = 0; x < Config.maximal_table_horizontal - 2; x++)
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
                        switch (table[x, y].cell_Version)
                        {
                            case CellData.Cell_Version.Bomb: 
                                BombBreaker(x, y);
                                break;
                            case CellData.Cell_Version.Horizontal_Spliter: 
                                BombBreaker(x, y);
                                break;
                            case CellData.Cell_Version.Vertical_Spliter: 
                                BombBreaker(x, y);
                                break;
                            default: break;
                        }

                        matchedTiles.Add(table[x, y]);
                        matchedTiles.Add(table[x + 1, y]);
                        matchedTiles.Add(table[x + 2, y]);

                        if (x + 3 < Config.maximal_table_horizontal && table[x, y].cell_Type == table[x + 3, y].cell_Type)
                        {
                            matchedTiles.Add(table[x + 3, y]);
                            if (x + 4 < Config.maximal_table_horizontal && table[x, y].cell_Type == table[x + 4, y].cell_Type)
                            {
                                matchedTiles.Add(table[x + 4, y]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void VerticalCheck(){
        for (int x = 0; x < Config.maximal_table_horizontal; x++)
        {
            for (int y = 0; y < Config.maximal_table_vertical - 2; y++)
            {
                if (table[x, y] != null &&
                    table[x, y + 1] != null &&
                    table[x, y + 2] != null)
                {
                    if( table[x, y].cell_Type > 0 && 
                        table[x, y].cell_Type < CellData.Cell_Type.Cloud &&
                        table[x, y].cell_Type == table[x, y + 1].cell_Type &&
                        table[x, y].cell_Type == table[x, y + 2].cell_Type )
                    {
                        switch (table[x, y].cell_Version)
                        {
                            case CellData.Cell_Version.Bomb: 
                                BombBreaker(x, y);
                                break;
                            case CellData.Cell_Version.Horizontal_Spliter: 
                                BombBreaker(x, y);
                                break;
                            case CellData.Cell_Version.Vertical_Spliter: 
                                BombBreaker(x, y);
                                break;
                            default: break;
                        }

                        matchedTiles.Add(table[x, y]);
                        matchedTiles.Add(table[x, y + 1]);
                        matchedTiles.Add(table[x, y + 2]);

                        if (y + 3 < Config.maximal_table_vertical && table[x, y].cell_Type == table[x, y + 3].cell_Type)
                        {
                            matchedTiles.Add(table[x, y + 3]);
                            if (y + 4 < Config.maximal_table_vertical && table[x, y].cell_Type == table[x, y + 4].cell_Type)
                            {
                                matchedTiles.Add(table[x, y + 4]);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void FindCloudsNearMatches()
    {
        List<CellData> cloudsToAdd = new List<CellData>();

        foreach (CellData tile in matchedTiles)
        {
            int x = tile.point.x;
            int y = tile.point.y;

            CheckAndAddCloud(x + 1, y, cloudsToAdd);
            CheckAndAddCloud(x - 1, y, cloudsToAdd);
            CheckAndAddCloud(x, y + 1, cloudsToAdd);
            CheckAndAddCloud(x, y - 1, cloudsToAdd);
        }

        matchedTiles.AddRange(cloudsToAdd);
    }

    private void CheckAndAddCloud(int x, int y, List<CellData> clouds)
    {
        if (x >= 0 && x < Config.maximal_table_horizontal && 
            y >= 0 && y < Config.maximal_table_vertical && 
            table[x, y] != null && 
            table[x, y].cell_Type == CellData.Cell_Type.Cloud)
        {
            clouds.Add(table[x, y]);
        }
    }

    // Метод для знаходження Бомби
    private void BombChecker(int x, int y){

    }
    // Метод для активування Бомби
    private void BombBreaker(int x, int y){
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (table[x + i, y + j] == null || table[x + i, y + j].cell_Type == CellData.Cell_Type.None) continue;
                matchedTiles.Add(table[x + i, y + j]);
            }
        }
    }

    public List<CellData> GetMatchedTiles() => matchedTiles;
}