using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendTemplate : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image background;
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text username;
    [SerializeField] private Button btn_add;
    [SerializeField] private Button btn_delete;

    [Header("Sprites")]
    [SerializeField] private Sprite[] bg_sprites;
    [SerializeField] private Sprite[] btn_sprites;

    public enum FriendTemplateType
    {
        Request,
        Player,
        Friend,
    }

    /// <summary>
    /// Method which instatiates friend in menu | Modes: True - Add / False - Delete
    /// </summary>
    public void InstantiateFriend(int friend_id, JsonDataList array, FriendTemplateType mode)
    {
        username.text = array.players[friend_id].username;
        avatar.sprite = DataManager.Instance.avatars[array.players[friend_id].player_avatar_id];

        background.sprite = bg_sprites[mode < FriendTemplateType.Friend ? 1 : 0];
        btn_add.GetComponent<Image>().sprite = btn_sprites[mode < FriendTemplateType.Friend ? 1 : 0];

        switch (mode)
        {
            case FriendTemplateType.Player:
                btn_add.onClick.AddListener(() => { AddToFriend(friend_id); });
                break;
            case FriendTemplateType.Friend:
                btn_add.onClick.AddListener(() => { DeleteFromFriend(friend_id); });
                break;
            case FriendTemplateType.Request:
                btn_delete.gameObject.SetActive(true);
                btn_add.onClick.AddListener(() => { AcceptFriendRequest(friend_id); });
                btn_delete.onClick.AddListener(() => { DeclineFriendRequest(friend_id); });
                break;
            
        }
    }

    public void AddToFriend(int friend_id)
    {
        _ = AddToFriendAsync(friend_id);
    }

    private async Task AddToFriendAsync(int friend_id)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.SendInviteToFriend(friend_id + 1, tcs));
        await tcs.Task;
        DataManager.Instance.TriggerFriendsUpdate();
    }
    public void DeleteFromFriend(int friend_id)
    {
        _ = DeleteFromFriendAsync(friend_id);
    }

    private async Task DeleteFromFriendAsync(int friend_id)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.DeleteFromFriends(friend_id + 3, tcs));
        await tcs.Task;
        DataManager.Instance.TriggerFriendsUpdate();
    }

    public void AcceptFriendRequest(int friend_id)
    {
        _ = AcceptFriendRequestAsync(friend_id);
    }

    private async Task AcceptFriendRequestAsync(int friend_id)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.AcceptInviteToFriend(friend_id + 3, tcs));
        await tcs.Task;
        DataManager.Instance.TriggerFriendsUpdate();
    }

    public void DeclineFriendRequest(int friend_id)
    {
        _ = DeclineFriendRequestAsync(friend_id);
    }

    private async Task DeclineFriendRequestAsync(int friend_id)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        StartCoroutine(DataManager.Instance.ns.DeclineInviteToFriend(friend_id + 2, tcs));
        await tcs.Task;
        DataManager.Instance.TriggerFriendsUpdate();
    }


}
