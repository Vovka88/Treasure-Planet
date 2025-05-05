using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIContoller : MonoBehaviour
{
    [SerializeField] private Sprite[] icons_for_level;

    [Header("MANAGERS")]
    [SerializeField] private HeartManager hm;
    [SerializeField] private ShipFlight shipFlight;
    private bool _heartsInitialized = false;

    [Header("MENU ELLEMENTS ON PAGE")]
    [SerializeField] public GameObject blur;
    [SerializeField] private GameObject blocking_screen;
    [SerializeField] private GameObject loading_screen;
    public bool isBlurOn = false;
    private bool isGlobalMapOn = true;


    [Header("MENU ELLEMENTS ON PAGE")]
    [SerializeField] private GameObject lvlMenu;
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private Transform menu_position_start;
    [SerializeField] private Transform menu_position_end;
    GameObject menu;


    [Header("UI ELLEMENTS ON PAGE")]
    [SerializeField] private TMP_Text hp_field;
    [SerializeField] private TMP_Text money_field;

    [SerializeField] private Image player_icon;
    [SerializeField] private Image ship;


    [Header("LEVELS BUTTONS")]
    [SerializeField] private ScriptableButton[] levels_buttons;
    private List<JsonLevelData> levels_data = new List<JsonLevelData>();



    [Header("MAIN MENUS")]
    [SerializeField] private GameObject events_menu;
    [SerializeField] private GameObject friends_menu;
    [SerializeField] private GameObject shop_menu;
    [SerializeField] private GameObject shadow;


    [Header("SECONDARY MENUS")]
    [SerializeField] private GameObject profile_menu;
    [SerializeField] private GameObject end_level_menu;
    [SerializeField] private GameObject mail_menu;

    




    async void Awake()
    {
        DataManager.OnPlayerDataUpdate += SetPlayerData;
        DataManager.OnHPDataUpdate += SetPlayerData;

        Invoke(nameof(GetLevels), 0.1f);
        await GetFriendsAsync();

        DataManager.Instance.TriggerPlayerDataUpdated();
        blur.SetActive(false);
    }

    void Start()
    {
        hm.InitFromServer(DataManager.Instance.first_lose_time, DataManager.Instance.player_hp);
        FlyShipToLevel(DataManager.Instance.current_level);
    }

    public void GetLevels()
    {
        _ = AccountLevels();
    }

    public void GetFriends()
    {
        _ = GetFriendsAsync();
    }

    public async Task AccountLevels()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        if (DataManager.Instance.ns == null)
        {
            Debug.LogError("ns is NULL! Coroutine won't start.");
            return;
        }

        loading_screen.SetActive(true);
        StartCoroutine(DataManager.Instance.ns.GetPlayerLevels(tcs));
        await tcs.Task;

        loading_screen.SetActive(false);

        var levels = JsonUtility.FromJson<JsonLevelDataList>(DataManager.Instance.json_levels);
        levels_data = new List<JsonLevelData>(); // <- Обязательно инициализируем список!

        foreach (var item in levels.levels)
        {
            Debug.Log($"level_id: {item.level_id} ; score: {item.score} ; count_of_stars: {item.count_of_stars} ; completed: {item.completed}");
            levels_data.Add(item);
        }

        // ✅ Только теперь вызываем!
        await InitializeLevels();
    }



    public async Task GetFriendsAsync()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        if (DataManager.Instance.ns == null)
        {
            Debug.LogError("ns is NULL! Coroutine won't start.");
            return;
        }

        loading_screen.SetActive(true);
        // StartCoroutine(ns.UpdateUsernameAndAvatar("new_user_test", 2, tcs));
        StartCoroutine(DataManager.Instance.ns.GetFriends(tcs));
        await tcs.Task;

        loading_screen.SetActive(false);
    }



    public void SetPlayerData()
    {
        hp_field.text = DataManager.Instance.player_hp.ToString();
        money_field.text = DataManager.Instance.player_money.ToString();
        player_icon.sprite = DataManager.Instance.avatars[DataManager.Instance.player_avatar_id];
        ship.sprite = player_icon.sprite;

        if (!_heartsInitialized)
        {
            _heartsInitialized = true;
            Debug.Log(DataManager.Instance.first_lose_time);
            Debug.Log(DataManager.Instance.player_hp);
            hm.InitFromServer(DataManager.Instance.first_lose_time, DataManager.Instance.player_hp);
        }
    }

    private async Task InitializeLevels()
    {
        var tcs = new TaskCompletionSource<bool>();

        if (levels_data == null || levels_data.Count == 0)
        {
            Debug.LogWarning("Нет данных об уровнях для инициализации!");
            tcs.SetResult(true);
            return;
        }

        // Собираем инфу о пройденных уровнях
        var completedLevels = levels_data.FindAll(l => l.completed > 0);
        int maxCompletedLevelId = completedLevels.Count > 0
            ? completedLevels.Max(l => l.level_id)
            : 0;

        // Создаём словарь для быстрого поиска информации о пройденных уровнях
        var completedLevelsDict = completedLevels.ToDictionary(l => l.level_id, l => l.count_of_stars);

        // Обрабатываем все кнопки уровней
        foreach (var btn in levels_buttons)
        {
            if (completedLevelsDict.TryGetValue(btn.level_id, out int stars))
            {
                // Пройденный уровень — проставляем полученные звёзды
                btn.InstantiateStars(stars);
                btn.GetComponent<Button>().onClick.AddListener(() => blur.SetActive(true));
            }
            else
            {
                // Непройденный уровень — ставим 0 звёзд
                btn.InstantiateStars(0);
            }

            if (btn.level_id > maxCompletedLevelId + 1)
            {
                btn.OffButton(); // Отключаем уровни, которые ещё не открыты
            }
        }

        Debug.Log("From level?: " + DataManager.Instance.isReturnedFromLevel);

        if (DataManager.Instance.isReturnedFromLevel)
        {
            blur.SetActive(true);
            StartCoroutine(DataManager.Instance.ns.GetPlayerLevels(tcs));

            await tcs.Task;
            await end_level_menu.GetComponent<EndLevelMenuController>().InstantiateEndMenuAsync();
            DataManager.Instance.TriggerHPUpdate();
        }
    }

    public void FlyShipToLevel(int id)
    {
        shipFlight.FlyToLevel(id - 1);
    }



    public void ToogleMenu(GameObject menu)
    {
        if (menu != null)
        {
            bool isIt = false;
            blocking_screen.SetActive(true);

            if (menu == events_menu)
            {
                isIt = true;
                events_menu.SetActive(isIt);
            }
            else if (menu == friends_menu)
            {
                isIt = true;
                friends_menu.SetActive(isIt);
            }
            else if (menu == shop_menu)
            {
                isIt = true;
                shop_menu.SetActive(isIt);
            }


            if (!isIt)
            {
                blocking_screen.SetActive(false);

                events_menu.SetActive(isIt);
                friends_menu.SetActive(isIt);
                shop_menu.SetActive(isIt);
            }
        }
    }



    public void TransportShadow(GameObject gameObject)
    {
        shadow.transform.DOMoveX(gameObject.transform.position.x, 0.5f);
    }



    public void OpenSettings()
    {
        blur.SetActive(true);
        menu = Instantiate(settings_menu, menu_position_start);
        // menu.transform.DOJump(menu_position_end.position, 10, 1, 1);
        menu.transform.DOMoveY(menu_position_end.transform.position.y, 0.5f);
        menu.GetComponent<SettingsMenu>().exit_btn.onClick.AddListener(() => CloseSettings());
    }

    public void CloseSettings()
    {
        blur.SetActive(false);
        // menu.transform.DOJump(menu_position_start.position, 10, 1, 1);
        menu.transform.DOMoveY(menu_position_start.transform.position.y, 0.5f);
        Invoke(nameof(DestroyMenu), 0.6f);
    }

    void DestroyMenu()
    {
        Destroy(menu);
    }

    public void OpenProfile()
    {
        DataManager.Instance.TriggerPlayerDataUpdated();
        blur.SetActive(true);
        profile_menu.SetActive(true);
        // profile_menu.transform.DOJump(menu_position_end.position, 10, 1, 1);
        profile_menu.transform.DOMoveY(menu_position_end.position.y, 0.5f);
    }

    public void CloseProfile()
    {
        DataManager.Instance.TriggerPlayerDataUpdated();
        blur.SetActive(false);
        // profile_menu.transform.DOJump(menu_position_start.position, 10, 1, 1);
        profile_menu.transform.DOMoveY(menu_position_start.position.y, 0.5f);
        Invoke(nameof(HideProfileMenu), 0.6f);
    }

    public void OpenMail()
    {
        DataManager.Instance.TriggerGiftsDataUpdated();
        blur.SetActive(true);
        mail_menu.SetActive(true);
        // profile_menu.transform.DOJump(menu_position_end.position, 10, 1, 1);
        mail_menu.transform.DOMoveY(menu_position_end.position.y, 0.5f);
    }

    public void CloseMail()
    {
        DataManager.Instance.TriggerGiftsDataUpdated();
        blur.SetActive(false);
        // profile_menu.transform.DOJump(menu_position_start.position, 10, 1, 1);
        mail_menu.transform.DOMoveY(menu_position_start.position.y, 0.5f);
        Invoke(nameof(HideProfileMenu), 0.6f);
    }
    
    void HideProfileMenu()
    {
        profile_menu.SetActive(false);
    }

    public void InstantiateMenu(int id)
    {
        if (lvlMenu.TryGetComponent(out LevelMenu menu))
        {
            // Парсим JSON с данными уровней
            JsonLevelDataList levels = JsonUtility.FromJson<JsonLevelDataList>(DataManager.Instance.json_levels);

            // Пытаемся найти данные о нужном уровне
            JsonLevelData levelData = levels.levels.FirstOrDefault(l => l.level_id == id);

            // Если данных нет — создаём новый объект с нулевыми значениями
            if (levelData == null)
            {
                levelData = new JsonLevelData
                {
                    level_id = id,
                    completed = 0,
                    count_of_stars = 0
                };
            }
            // Создаём меню с найденными или созданными данными
            menu.InstantiateMenu(levelData.level_id, menu_position_start.gameObject, menu_position_end.gameObject, icons_for_level[levelData.level_id - 1]);
            menu.btn_destroy.onClick.AddListener(() => blur.SetActive(false));
        }
    }

    public void PlayClickSound(int id)
    {
        DataManager.Instance.am.PlayClickSound(id);
    }

}
