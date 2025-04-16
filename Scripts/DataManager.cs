using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public int player_id;
    public int player_avatar_id;
    public int player_hp;
    public int player_money;

    public string json_levels;
    public JsonLevelDataList levels_array;
    public int current_level;
    public int current_score;


    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
