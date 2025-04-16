using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelUIController : MonoBehaviour
{

    [SerializeField] private TableController tc;
    private NetworkScript ns;

    [Header("UPPER BAR SETTINGS")]
    [SerializeField] private TMP_Text hp_field;
    [SerializeField] private TMP_Text turns_field;
    [SerializeField] private GameObject task_bar;

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

}