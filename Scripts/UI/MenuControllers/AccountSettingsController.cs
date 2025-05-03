using System.Data;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountSettingsController : MonoBehaviour
{
    [SerializeField] public Button exit_btn;
    // [SerializeField] private Button? back_btn;
    // [SerializeField] private Button next_btn;


    [SerializeField] public GameObject back_menu;
    [HideInInspector] public GameObject this_menu;
    [SerializeField] public GameObject[] next_menu;

    [HideInInspector] public GameObject beforeMenu;
    [SerializeField] public GameObject parent;
    // [HideInInspector] public GameObject nextMenu;

    private void Awake()
    {
        this_menu = gameObject;
    }






    public void OpenMenu(int menuId)
    {
        next_menu[menuId].SetActive(true);
        gameObject.SetActive(false);

        next_menu[menuId].GetComponent<AccountSettingsController>().beforeMenu = gameObject;
    }
    public void CloseMenu()
    {
        Destroy(parent); // <--- это правильно
    }

    public void HideMenu()
    {
        beforeMenu.SetActive(true);
        gameObject.SetActive(false);
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


    public void AccountLogout()
    {
        _ = AccountLogoutAsync();
    }

    public async Task AccountDeleteAsync()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.DeleteAccount(tcs));
        await tcs.Task;

        CloseMenu();
        SceneManager.LoadScene(0);
    }


    public async Task AccountLogoutAsync()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.Logout(tcs));
        await tcs.Task;
        
        CloseMenu();
        SceneManager.LoadScene(0);
    }
}
