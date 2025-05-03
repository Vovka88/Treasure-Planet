using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text level_name;
    [SerializeField] private Button btn_play;
    [SerializeField] public Button btn_destroy;
    [SerializeField] private Image targets_icon;
    private GameObject menu;



    // [SerializeField] private Button add_super_bomb;
    // [SerializeField] private Button add_bomb;
    // [SerializeField] private Button add_splitter;

    [SerializeField] private Image[] add_busters_buttons;
    [SerializeField] private Sprite locked_buster;
    // [SerializeField] private Sprite target_sprite;


    private GameObject start_position;
    private GameObject end_position;

    public void InstantiateMenu(int level_id, GameObject start_position, GameObject end_position, Sprite target_sprite)
    {
        Debug.Log($"[InstantiateMenu] Level ID: {level_id}");

        level_name.text = $"Рівень {level_id}";
        targets_icon.sprite = target_sprite;

        // Инстанцируем копию меню
        GameObject newMenu = Instantiate(gameObject, start_position.transform);

        Button[] array = newMenu.GetComponentsInChildren<Button>();

        btn_destroy = array[1];
        btn_destroy.onClick.RemoveAllListeners();
        btn_destroy.onClick.AddListener(() => DestroyMenu());

        btn_play = array[0];
        btn_play.onClick.RemoveAllListeners();
        btn_play.onClick.AddListener(() =>
        {
            // SceneFader.Instance?.FadeToScene(level_id + 2);
            OnScene(level_id + 1);
        });

        // Копируем кнопки усилителей из нового меню
        LevelMenu newMenuManager = newMenu.GetComponent<LevelMenu>();
        List<Image> local_buster_buttons = add_busters_buttons.ToList();

        // Проставляем замки там, где усилители закончились
        foreach (var buster in DataManager.Instance.players_busters)
        {
            if (buster.count <= 0)
            {
                if (buster.id >= 0 && buster.id < local_buster_buttons.Count)
                {
                    local_buster_buttons[buster.id].sprite = locked_buster;
                }
                else
                {
                    Debug.LogError($"[InstantiateMenu] Buster ID {buster.id} вне диапазона (Всего кнопок: {local_buster_buttons.Count})");
                }
            }
        }

        // Сохраняем новое меню и позиции
        menu = newMenu;
        this.start_position = start_position;
        this.end_position = end_position;

        // Плавное появление меню
        menu.transform.DOMoveY(this.end_position.transform.position.y, 0.5f);
    }

    private void DestroyMenu()
    {
        menu.transform.DOMoveY(start_position.transform.position.y, 0.5f);
        Invoke(nameof(DestroyIm), 0.6f);
        DataManager.Instance.am.PlayClickSound(0);
    }

    void DestroyIm()
    {
        DestroyImmediate(menu, true);
    }

    public void PlayClickSound(int id)
    {
        DataManager.Instance.am.PlayClickSound(id);
    }

    private void OnScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
