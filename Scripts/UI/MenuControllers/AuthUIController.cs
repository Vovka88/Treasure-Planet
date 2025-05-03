using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using DG.Tweening;
public class AuthUIController : MonoBehaviour
{

    [Header("AUTH SETTINGS")]
    [SerializeField] private GameObject authMenu;

    [Header("LOGIN SETTINGS")]
    [SerializeField] private GameObject loginMenu;
    [SerializeField] private TMP_InputField loginMail;
    [SerializeField] private TMP_InputField loginPassword;

    [Header("REGISTRATION SETTINGS")]
    [SerializeField] private GameObject regMenu;
    [SerializeField] private TMP_InputField registrationMail;
    [SerializeField] private TMP_InputField registrationPassword;

    [Header("LOADING SETTINGS")]
    [SerializeField] private GameObject loadingScreen;

    [Header("ERROR SETTINGS")]
    [SerializeField] private GameObject errorMenu;

    [Header("Settings")]
    [SerializeField] private GameObject blur;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings_menu;
    [SerializeField] private Transform menu_position_start;
    [SerializeField] private Transform menu_position_end;


    public void ToGame()
    {
        AutoLogin();
    }

    public void ToMenu()
    {
        _ = ToMenuAsync();
        // _ = AccountDelete();
    }

    public void ToLogin()
    {
        _ = ToLoginAsync();
    }

    public void ToRegister()
    {
        Debug.Log("To register pressed");
        _ = ToRegisterAsync();
    }

    ///


    private async Task ToMenuAsync()
    {


        await AccountLogout();
        // await AccountDelete();
    }

    private async Task ToLoginAsync()
    {
        await AccountLogin();
        // await AccountDelete();
    }

    private async Task ToRegisterAsync()
    {
        Debug.Log("To registerAsync entered");
        await AccountRegistration();
        // await AccountDelete();
    }


    public void ToggleMenu(GameObject menu)
    {
        authMenu.SetActive(menu == authMenu);
        loginMenu.SetActive(menu == loginMenu);
        regMenu.SetActive(menu == regMenu);
        errorMenu.SetActive(menu == errorMenu);
    }


    public async void AutoLogin()
    {
        string tokenPath = Application.persistentDataPath + "/token.txt";
        if (File.ReadAllText(tokenPath).ToString().Length > 0)
        {
            string token = File.ReadAllText(tokenPath);
            Debug.Log("Токен загружен: " + token);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(DataManager.Instance.ns.Login(token, tcs));
            loadingScreen.SetActive(true);
            await tcs.Task;
            if (tcs.Task.Result == true)
            {
                loadingScreen.SetActive(false);
                ToggleMenu(new GameObject());
                DataManager.Instance.TriggerPlayerDataUpdated();
                SceneManager.LoadScene(1);
            }
            else
            {
                loadingScreen.SetActive(false);
                BlurSwitch(true);
                ToggleMenu(errorMenu);
            }
        }
        else
        {
            Debug.Log("Токен не найден, нужно авторизоваться!");
            ToggleMenu(authMenu);
        }
    }


    public async Task AccountRegistration()
    {
        Debug.Log("AccountRegistration started");

        if (registrationMail.text.Length > 3 || registrationPassword.text.Length > 4)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(DataManager.Instance.ns.Registration(registrationMail.text, registrationPassword.text, tcs));
            loadingScreen.SetActive(true);

            await tcs.Task;
            loadingScreen.SetActive(false);

            ToggleMenu(new GameObject());
        }
        else
        {
            Debug.LogError("!-SOMETHING HAPPENED WITH REGISTRATION-!");
        }
        Debug.Log("AccountRegistration ended");
    }

    public async Task AccountLogin()
    {
        if (loginMail.text.Length > 3 || loginPassword.text.Length > 4)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(DataManager.Instance.ns.Login(loginMail.text, loginPassword.text, tcs));
            await tcs.Task;
            ToggleMenu(new GameObject());
        }
        else
        {
            Debug.LogError("!-SOMETHING HAPPENED WITH LOGIN-!");
        }
    }

    public async Task AccountLogout()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.Logout(tcs));
        await tcs.Task; // Ожидаем завершения корутины
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

    public void BlurSwitch(bool active)
    {
        blur.SetActive(active);
    }

    public void PlayClickSound(int id)
    {
        DataManager.Instance.am.PlayClickSound(id);
    }
}
