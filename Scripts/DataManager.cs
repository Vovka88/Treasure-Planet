using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public NetworkScript ns;
    public AudioManager am;
    public string token;

    public int player_id;
    public string username;
    public string player_mail;
    public int player_avatar_id;
    public int player_hp;
    public int player_money;
    public Sprite[] avatars;
    public List<Buster> players_busters;

    public string json_levels;

    public JsonDataList friends_array;
    public JsonDataList requested_friends_array;
    public JsonDataList players_no_friends_array;
    
    public JsonDataList friends_want_gift;
    public JsonDataList accept_friends_gift;
    public JsonDataList ask_friends_gift;
    
    public JsonLevelDataList levels_array;
    public List<Buster> busters_on_level;
    public int current_level;
    public int current_score;
    public DateTime first_lose_time;

    public bool isReturnedFromLevel = false;
    public bool isExitedFromLevel = false;

    // Events

    public static event Action OnHPDataUpdate;
    public static event Action OnPlayerDataUpdate;
    public static event Action OnFriendsListUpdate;
    public static event Action OnGiftsListUpdate;

    void Awake()
    {
        ns = this.GetComponent<NetworkScript>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.orientation = ScreenOrientation.Portrait;

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

    void Start()
    {
        am.LoadVolumeSettings(); // теперь всё сработает корректно
    }
    public void TriggerPlayerDataUpdated()
    {
        OnPlayerDataUpdate?.Invoke();
    }
    public void TriggerGiftsDataUpdated()
    {
        OnGiftsListUpdate?.Invoke();
    }

    public void TriggerFriendsUpdate()
    {
        OnFriendsListUpdate?.Invoke();
    }

    public void TriggerHPUpdate()
    {
        OnHPDataUpdate?.Invoke();
    }

    public async Task UpdateAccountDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.Login(token, tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;

        if (tcs.Task.Result)
        {
            OnPlayerDataUpdate?.Invoke();
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
    public async Task UpdateFriendsGiftsDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetFriendsForGifts(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }
    public async Task UpdateAcceptionsofGiftsDataFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetAcceptionsForGifts(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }
    public async Task UpdateWantsGiftsFromServer(NetworkScript ns)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        ns.StartCoroutine(ns.GetWantsForGifts(tcs)); // ⚠️ Пример: ты должен реализовать этот метод в NetworkScript
        await tcs.Task;
    }

}
