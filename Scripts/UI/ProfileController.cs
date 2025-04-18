using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text username;


    [Header("Editor")]
    // [SerializeField] private Button edit_btn;
    [SerializeField] private EditProfileController edp;


    private void Awake() {
        DataManager.OnPlayerDataUpdated += UpdateAvatar;
        DataManager.OnPlayerDataUpdated += UpdateUsername;
    }

    public void UpdateAvatar(){
        avatar.sprite = DataManager.Instance.avatars[DataManager.Instance.player_avatar_id - 1];
    }

    public void UpdateUsername(){
        username.text = DataManager.Instance.username;
    }

    public void Exit(){
        gameObject.SetActive(false);
    }

    public void ToEditor()
    {
        gameObject.SetActive(false);
        edp.gameObject.SetActive(true);
    }

}
