using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FriendGiftTemplate : MonoBehaviour
{
    public int id;

    [Header("UI Elements")]
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text username;

    [Header("Sprites")]
    [SerializeField] public TMP_Text description;

    /// <summary>
    /// Method which instatiates friend in menu | Modes: True - Add / False - Delete
    /// </summary>

    public void InstantiateFriend(int friend_id, JsonDataList array)
    {
        var temp = array.players.ToList().Find(f => f.id == friend_id);

        username.text = temp.username;
        avatar.sprite = DataManager.Instance.avatars[temp.avatar_id - 1];
        id = friend_id;
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
        StartCoroutine(DataManager.Instance.ns.DeleteFromFriends(friend_id + 2, tcs));
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