using System;
using System.Threading.Tasks;
using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public NetworkScript ns;
    public string token;
    
    public int player_id;
    public string username;
    public int player_avatar_id;
    public int player_hp;
    public int player_money;
    public Sprite[] avatars;

    public string json_levels;
    public JsonDataList friends_array;
    public JsonDataList requested_friends_array;
    public JsonDataList players_no_friends_array;
    public JsonLevelDataList levels_array;
    public int current_level;
    public int current_score;

    public bool isReturnedFromLevel = false;

    // Events

    public static event Action OnPlayerDataUpdated;
    public static event Action OnFriendsListUpdate;

    void Awake()
    {
        ns = this.GetComponent<NetworkScript>();
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
    public void TriggerPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke();
    }
    
    public void TriggerFriendsUpdate()
    {
        OnFriendsListUpdate?.Invoke();
    }

    public async Task UpdateAccountDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.Login(token, tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;

        if (tcs.Task.Result)
        {
            OnPlayerDataUpdated?.Invoke();
        }
        else
        {
            Debug.LogError("Ошибка при получении данных аккаунта");
        }
    }

    public async Task UpdateFriendsDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetFriends(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }
    public async Task UpdateRequestsDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetFriendsInvites(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }
    public async Task UpdatePlayersDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetUsers(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }

}
