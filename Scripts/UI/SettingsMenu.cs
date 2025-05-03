using System.Data;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] public Button exit_btn;
    // [SerializeField] private Button? back_btn;
    // [SerializeField] private Button next_btn;


    [SerializeField] public GameObject back_menu;
    [HideInInspector] public GameObject this_menu;
    [SerializeField] public GameObject[] next_menu;

    [HideInInspector] public GameObject beforeMenu;
    [HideInInspector] public GameObject nextMenu;

    [HideInInspector] public GameObject instantiate_menu_position;
    [HideInInspector] public GameObject end_animation_menu_position;


    private void Start()
    {
        this_menu = gameObject;
    }






    public void OpenMenu(int menuId)
    // public void OpenMenu(int menuId, GameObject inst = null, GameObject end = null, bool withAnimation = false)
    {
        if (next_menu[menuId] != null)
        {
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();

            nextMenu = Instantiate(next_menu[menuId], canvas.gameObject.transform);
            // if (menuScale != null) nextMenu.gameObject.transform.localScale = menuScale.localScale;
            SettingsMenu nextMenuSettings = nextMenu.GetComponent<SettingsMenu>();
            nextMenuSettings.beforeMenu = this_menu;
            this_menu.SetActive(false);
        }
    }
    public void CloseMenu()
    // public void CloseMenu(bool withAnimation = false)
    {
        // if (withAnimation && 
        //     instantiate_menu_position != null && 
        //     end_animation_menu_position != null)
        // {
        //     // transform.DOJump(instantiate_menu_position.transform.position, 10, 1, 1f);
        //     transform.DOMoveY(instantiate_menu_position.transform.position.y, 0.5f);
        // }

        if (beforeMenu != null)
        {
            beforeMenu.SetActive(true);
            beforeMenu.GetComponent<SettingsMenu>().nextMenu = null;
        }

        Destroy(gameObject); // <--- это правильно
    }

    public void HideMenu()
    {
        if (beforeMenu != null)
        {
            beforeMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }



    public void ToGlobalMap()
    {
        Debug.Log("1 point");
        _ = ToGlobalMapAsync();
    }

    // public async Task ToGlobalMapAsync()
    public async Task ToGlobalMapAsync()
    {
        Debug.Log("2 point");
        if (beforeMenu != null)
        {
            beforeMenu.SetActive(true);
            beforeMenu.GetComponent<SettingsMenu>().nextMenu = null;
        }
        Destroy(gameObject); // <--- это правильно
        // TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.SaveHp(
            DataManager.Instance.player_hp - 1,
            DataManager.Instance.player_hp == 5
        // tcs
        ));
        DataManager.Instance.isReturnedFromLevel = true;
        DataManager.Instance.isExitedFromLevel = true;
        // await tcs.Task;
        SceneManager.LoadScene(1);
    }




    public void PlayClickSound(int id)
    {
        DataManager.Instance.am.PlayClickSound(id);
    }


    public void AccountDelete()
    {
        _ = AccountDeleteAsync();
    }

    public async Task AccountDeleteAsync()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.DeleteAccount(tcs));
        await tcs.Task;
    }
}
