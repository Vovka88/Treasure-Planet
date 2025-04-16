using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    // [SerializeField] private Button exit_btn;
    // [SerializeField] private Button? back_btn;
    // [SerializeField] private Button next_btn;


    [SerializeField] public GameObject back_menu;
    [HideInInspector] public GameObject this_menu;
    [SerializeField] public GameObject[] next_menu;

    [HideInInspector] public GameObject beforeMenu;
    [HideInInspector] public GameObject nextMenu;

    private void Awake()
    {
        this_menu = gameObject;
    }






    public void OpenMenu(int menuId)
    {
        if (next_menu[menuId] != null)
        {
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();

            nextMenu = Instantiate(next_menu[menuId], canvas.gameObject.transform);
            nextMenu.GetComponent<SettingsMenu>().beforeMenu = this_menu;
            this_menu.SetActive(false);
        }
    }
    public void CloseMenu()
    {
        if (beforeMenu != null)
        {
            beforeMenu.SetActive(true);
            beforeMenu.GetComponent<SettingsMenu>().nextMenu = null;
        }

        Destroy(gameObject); // <--- это правильно
    }
}
