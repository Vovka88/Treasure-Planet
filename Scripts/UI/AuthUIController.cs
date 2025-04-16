using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading.Tasks;
using TMPro;
public class AuthUIController : MonoBehaviour
{
    public NetworkScript ns;

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
    }


    public async void AutoLogin()
    {
        string tokenPath = Application.persistentDataPath + "/token.txt";
        if (File.ReadAllText(tokenPath).ToString().Length > 0)
        {
            string token = File.ReadAllText(tokenPath);
            Debug.Log("Токен загружен: " + token);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            StartCoroutine(ns.Login(token, tcs));

            await tcs.Task;
            if (tcs.Task.Result == true)
            {
                ToggleMenu(new GameObject());
                SceneManager.LoadScene(1);
            }
            else
            {

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
            StartCoroutine(ns.Registration(registrationMail.text, registrationPassword.text, tcs));
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
            StartCoroutine(ns.Login(loginMail.text, loginPassword.text, tcs));
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
        StartCoroutine(ns.Logout(tcs));
        await tcs.Task; // Ожидаем завершения корутины
    }

    public async Task AccountDelete()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(ns.DeleteAccount(tcs));
        await tcs.Task;
    }
}
