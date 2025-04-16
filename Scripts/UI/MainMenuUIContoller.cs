using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIContoller : MonoBehaviour
{
    public NetworkScript ns;



    [Header("MENU ELLEMENTS ON PAGE")]
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject loading_screen;
    private bool isBlurOn = false;


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
    [SerializeField] private Sprite[] avatars;
    [SerializeField] private Image ship;

    [Header("LEVELS BUTTONS")]
    [SerializeField] private ScriptableButton[] levels_buttons;
    private List<JsonLevelData> levels_data = new List<JsonLevelData>();


    [Header("LEVEL ENDING SETTINGS")]
    [SerializeField] private TMP_Text header_field;
    [SerializeField] private TMP_Text score_field;
    [SerializeField] private GameObject ending_menu;
    [SerializeField] private Transform[] positions_for_star;
    [SerializeField] private Image full_star;
    [SerializeField] private Image empty_star;

    void Awake()
    {
        Invoke(nameof(GetLevels), 0.1f);
        SetPlayerData();
        // await InitializeLevels();
    }

    public void GetLevels()
    {
        _ = AccountLevels();
    }

    public async Task AccountLevels()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        if (ns == null)
        {
            Debug.LogError("ns is NULL! Coroutine won't start.");
            return;
        }

        loading_screen.SetActive(true);
        StartCoroutine(ns.GetPlayerLevels(tcs));
        await tcs.Task;

        loading_screen.SetActive(false);

        var levels = JsonUtility.FromJson<JsonLevelDataList>(DataManager.Instance.json_levels);
        levels_data = new List<JsonLevelData>(); // <- Обязательно инициализируем список!

        foreach (var item in levels.levels)
        {
            Debug.Log($"level_id: {item.level_id}");
            levels_data.Add(item);
        }

        // ✅ Только теперь вызываем!
        await InitializeLevels();
    }


    public void SetPlayerData()
    {
        hp_field.text = DataManager.Instance.player_hp.ToString();
        money_field.text = DataManager.Instance.player_money.ToString();
        player_icon.sprite = avatars[DataManager.Instance.player_avatar_id];
        ship.sprite = player_icon.sprite;
    }

    private async Task InitializeLevels()
    {
        Debug.Log("Интициализация уровней вызвана");

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        if (levels_data == null || levels_data.Count == 0)
        {
            Debug.LogWarning("Нет данных об уровнях для инициализации!");
            tcs.SetResult(true);
            return;
        }

        // Найдём максимальный level_id, который прошёл игрок
        int last_completed_level = levels_data.Max(l => l.level_id);

        // Выключим уровни выше последнего пройденного
        for (int i = 0; i < levels_buttons.Length; i++)
        {
            if (i + 1 > last_completed_level)
            {
                levels_buttons[i].OffButton();
                Debug.Log($"OffButton вызван для кнопки {i + 1}");
            }
        }

        // Проставим звёзды для пройденных уровней
        foreach (var level in levels_data)
        {
            int index = level.level_id - 1; // Индекс кнопки (уровень 1 → индекс 0)

            if (index >= 0 && index < levels_buttons.Length)
            {
                levels_buttons[index].InstantiateStars(level.count_of_stars);
                Debug.Log($"InstantiateStars вызван для кнопки {level.level_id}, звёзд: {level.count_of_stars}");
            }
            else
            {
                Debug.LogWarning($"Нет кнопки для level_id {level.level_id} (index {index})");
            }
        }

        tcs.SetResult(true);
        await tcs.Task;
    }






    void Update()
    {
        if (menu == null && isBlurOn)
        {
            // blur.GetComponent<Image>().DOFade(0f, 1f).SetEase(Ease.InOutQuad);
            isBlurOn = false;
            blur.SetActive(isBlurOn);
        }
    }




    public void OpenSettings()
    {
        // blur.GetComponent<Image>().DOFade(1f, 1f).SetEase(Ease.InOutQuad);
        isBlurOn = true;
        blur.SetActive(isBlurOn);


        menu = Instantiate(settings_menu, menu_position_start);
        menu.transform.DOJump(menu_position_end.position, 10, 1, 1);
    }

    public void InstantiateMenu(int id)
    {
        if (lvlMenu.TryGetComponent(out LevelMenu menu))
        {
            JsonLevelData levels = JsonUtility.FromJson<JsonLevelData>(DataManager.Instance.json_levels);

            menu.InstantiateMenu(levels.level_id, gameObject);
        }
    }


    public void InstantiateEndMenu()
    {
        ending_menu.SetActive(true);
        JsonLevelData levelData = DataManager.Instance.levels_array.levels.ToList().Find(t => t.level_id == DataManager.Instance.current_level);
        if (levelData.is_completed)
        {
            header_field.text = "РІВЕНЬ ПРОЙДЕНИЙ!";
            for (int i = 0; i < positions_for_star.Length; i++)
            {
                if (levelData.count_of_stars >= i + 1) Instantiate(full_star, positions_for_star[i]);
                else Instantiate(empty_star, positions_for_star[i]);
            }
        }
        else
        {
            header_field.text = "РІВЕНЬ НЕ ПРОЙДЕНО(";
        }

        score_field.text = "Рахунок: " + levelData.score.ToString();
    }

    public void CloseEndMenu(){
        ending_menu.SetActive(false);
    }





}
