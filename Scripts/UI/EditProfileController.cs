using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditProfileController : MonoBehaviour
{
    [SerializeField] private NetworkScript ns;
    [SerializeField] private ProfileController pc;

    [Header("Edit Profile Elements")]
    [SerializeField] private Image avatar;
    private int avatar_index = 0;

    [SerializeField] private TMP_InputField username_input;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Button submit;

    private void Awake()
    {
        username_input.onValueChanged.AddListener((fuck) => {
            counter.text = $"{username_input.text.Length}/16"; 
            if (username_input.text.Length > 0) 
                submit.gameObject.SetActive(true);
            else 
                submit.gameObject.SetActive(false);
        });
    }

    public void SaveData()
    {
        _ = SaveDataAsync();
    }

    public async Task SaveDataAsync()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(ns.UpdateUsernameAndAvatar(username_input.text, avatar_index + 1, tcs));
        await DataManager.Instance.UpdateAccountDataFromServer(ns);
        await tcs.Task;
        
        if (tcs.Task.Result == true)
        {
            gameObject.SetActive(false);
            pc.gameObject.SetActive(true);
        }
        else
        {   
            Debug.Log("Error");
            gameObject.SetActive(true);
        }

    }

    public void NextAvatar()
    {
        if (avatar_index < DataManager.Instance.avatars.Length - 1)
        {
            ++avatar_index;
            avatar.sprite = DataManager.Instance.avatars[avatar_index];
        }
    }

    public void LastAvatar()
    {
        if (avatar_index > 0)
        {
            --avatar_index;
            avatar.sprite = DataManager.Instance.avatars[avatar_index];
        }
    }

}
