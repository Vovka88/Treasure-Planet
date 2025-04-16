using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelMenu : MonoBehaviour
{
    [SerializeField] private Text level_name;
    [SerializeField] private Button btn_play;
    [SerializeField] private Button btn_destroy;
    [SerializeField] GameObject targetsTabPosition;
    private GameObject menu;

    public void InstantiateMenu(int level_id, GameObject position)
    {
        Debug.Log($"[InstantiateMenu] Level ID: {level_id}");

        level_name.text = $"Level {level_id + 1}";

        GameObject newMenu = Instantiate(gameObject, position.transform);

        Button[] array = newMenu.GetComponentsInChildren<Button>();

        btn_destroy = array[1];
        btn_destroy.onClick.RemoveAllListeners();
        btn_destroy.onClick.AddListener(() => DestroyMenu());

        btn_play = array[0];
        btn_play.onClick.RemoveAllListeners();
        btn_play.onClick.AddListener(() =>
        {
            // SceneFader.Instance?.FadeToScene(level_id + 2);
            OnScene(level_id + 2);
        });

        menu = newMenu;
    }

    private void DestroyMenu()
    {
        DestroyImmediate(menu, true);
    }

    private void OnScene(int id)
    {
        SceneManager.LoadScene(id);
    }
}
