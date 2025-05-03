using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelUIController : MonoBehaviour
{

    [SerializeField] public TableController tc;
    [SerializeField] private ScoreProgressBar spb;
    private NetworkScript ns;

    [Header("UPPER BAR SETTINGS")]
    [SerializeField] private TMP_Text hp_field;
    [SerializeField] private TMP_Text turns_field;
    [SerializeField] private GameObject task_bar;

    [Header("SETTINGS MENU")]
    [SerializeField] public GameObject blur;
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private Transform menu_position_start;
    [SerializeField] private Transform menu_position_end;
    GameObject menu;

    private List<GameObject> instansTarget = new List<GameObject>();

    private void Awake()
    {
        tc.onTurnsChanged += UIChangeTurns;

        hp_field.text = DataManager.Instance.player_hp.ToString();
        foreach (var target in tc.targetsOnLevel)
        {
            instansTarget.Add(target.InstantiateTarget(task_bar));
        }
    }

    private void UIChangeTurns(int turn)
    {
        turns_field.text = turn.ToString();
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

    // public void CloseBlur()
    // {
    //     blur.SetActive(false);
    // }

}