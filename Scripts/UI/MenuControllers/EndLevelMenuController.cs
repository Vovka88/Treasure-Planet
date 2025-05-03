using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelMenuController : MonoBehaviour
{
    [SerializeField] private GameObject blur;

    [Header("LEVEL ENDING SETTINGS")]
    [SerializeField] private TMP_Text header_field;
    [SerializeField] private TMP_Text score_field;
    [SerializeField] private GameObject[] positions_for_star;
    // [SerializeField] private GameObject star;
    [SerializeField] private Sprite full_star;
    [SerializeField] private Sprite empty_star;

    // public void InstantiateEndMenu(){
    //     InstantiateEndMenuAsync();
    // }

    public async Task InstantiateEndMenuAsync()
    {
        blur.SetActive(true);
        gameObject.SetActive(true);

        DataManager.Instance.levels_array = JsonUtility.FromJson<JsonLevelDataList>(DataManager.Instance.json_levels);
        JsonLevelData levelData = DataManager.Instance.levels_array.levels.ToList().Find(t => t.level_id == DataManager.Instance.current_level);

        if (levelData == null || DataManager.Instance.isExitedFromLevel)
        {
            Debug.LogWarning("Уровень не найден, создаю заглушку.");
            levelData = new JsonLevelData()
            {
                level_id = DataManager.Instance.current_level,
                completed = 0,
                score = 0,
                count_of_stars = 0
            };
        }


        Debug.Log("Is completed?: " + levelData.completed);
        Debug.Log("Score of level: " + levelData.score);
        Debug.Log("Level id: " + DataManager.Instance.current_level);

        foreach (var item in DataManager.Instance.levels_array.levels.ToList())
        {
            Debug.Log("DM Level_id: " + item.level_id);
        }


        Debug.Log("Is completed?: " + levelData.completed);
        Debug.Log("Score of level: " + levelData.score);
        Debug.Log("Level id: " + DataManager.Instance.current_level);

        if (levelData.completed == 1)
        {
            header_field.text = "РІВЕНЬ ПРОЙДЕНИЙ!";
        }
        else
        {
            header_field.text = "РІВЕНЬ НЕ ПРОЙДЕНО(";
        }
        
        for (int i = 0; i < positions_for_star.Length; i++)
        {
            if (levelData.count_of_stars >= i + 1) positions_for_star[i].GetComponent<Image>().sprite = full_star;
            else positions_for_star[i].GetComponent<Image>().sprite = empty_star;
        }

        score_field.text = "Рахунок: " + levelData.score.ToString();
    }

    public void CloseEndMenu()
    {
        gameObject.SetActive(false);
        blur.SetActive(false);
    }
}
