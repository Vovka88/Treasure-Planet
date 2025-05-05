using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountSettingsData : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text mail;

    private void Start() {
        UpdateData();
    }

    public async void UpdateData()
    {
        await DataManager.Instance.UpdateAccountDataFromServer(DataManager.Instance.ns);

        Debug.Log(DataManager.Instance.avatars[DataManager.Instance.player_avatar_id]);
        Debug.Log(DataManager.Instance.username);

        avatar.sprite = DataManager.Instance.avatars[DataManager.Instance.player_avatar_id];
        username.text = DataManager.Instance.username;
        // mail.text = DataManager.Instance.username;
    }
}
