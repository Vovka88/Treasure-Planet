using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsSendHealthList : MonoBehaviour
{
    public GameObject itemPrefab;  // Префаб элемента списка
    public Transform contentPanel; // Панель, где будут отображаться элементы
    public int numberOfItems = 10; // Количество элементов в списке


    [Header("SETTINGS OF ADD FRIEND MENU")]
    [SerializeField] private GameObject spaceman_img;
    [SerializeField] private GameObject players_title;

    public event Action<bool> isPlayersMenu;

    void Start()
    {
        isPlayersMenu += WhichMenuGenerate;
        DataManager.OnFriendsListUpdate += ClearData;
        DataManager.Instance.TriggerFriendsUpdate();
        setActiveMenu(true);
    }

    void OnDestroy()
    {
        DataManager.OnFriendsListUpdate -= ClearData;
        DataManager.OnFriendsListUpdate -= CreateListOfExistingFriends;
        DataManager.OnFriendsListUpdate -= CreateListOfPlayersAndRequests;
    }

    public void WhichMenuGenerate(bool mode)
    {
        if (mode)
        {
            DataManager.OnFriendsListUpdate += CreateListOfPlayersAndRequests;
            DataManager.OnFriendsListUpdate -= CreateListOfExistingFriends;

            DataManager.Instance.TriggerFriendsUpdate();
        }
        else
        {
            DataManager.OnFriendsListUpdate += CreateListOfExistingFriends;
            DataManager.OnFriendsListUpdate -= CreateListOfPlayersAndRequests;

            DataManager.Instance.TriggerFriendsUpdate();
        }
    }

    public void setActiveMenu(bool x)
    {
        WhichMenuGenerate(x);
    }




    public void ClearData()
    {
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);
    }

    private async void CreateListOfExistingFriends()
    {
        ClearData();

        DataManager.Instance.friends_array = null;

        await DataManager.Instance.UpdateFriendsDataFromServer(DataManager.Instance.ns);

        var data = DataManager.Instance.friends_array;
        if (data == null || data.players == null) return;


        for (int i = 0; i < data.players.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.name = "Item " + i;
            newItem.GetComponent<FriendTemplate>().InstantiateFriend(i, data, FriendTemplate.FriendTemplateType.Friend);
        }
    }

    private async void CreateListOfPlayersAndRequests()
    {
        ClearData();

        DataManager.Instance.requested_friends_array = null;
        DataManager.Instance.players_no_friends_array = null;

        await DataManager.Instance.UpdateRequestsDataFromServer(DataManager.Instance.ns);
        await DataManager.Instance.UpdatePlayersDataFromServer(DataManager.Instance.ns);

        var data2 = DataManager.Instance.requested_friends_array;
        var data1 = DataManager.Instance.players_no_friends_array;
        if (data1 == null || data1.players == null) return;

        Instantiate(spaceman_img, contentPanel);
        if (data2 != null && data2.players != null && data2.players.Length > 0)
        {
            var temp1 = Instantiate(players_title, contentPanel);
            temp1.GetComponentInChildren<TMP_Text>().text = "Requests";

            for (int i = 0; i < data2.players.Length; i++)
            {
                Debug.Log(data2.players[i].player_id);
                GameObject newItem = Instantiate(itemPrefab, contentPanel);
                newItem.name = "Request " + i;
                newItem.GetComponent<FriendTemplate>().InstantiateFriend(i, data2, FriendTemplate.FriendTemplateType.Request);
            }
        }
        var temp2 = players_title;
        temp2.GetComponentInChildren<TMP_Text>().text = "Players";
        Instantiate(temp2, contentPanel);
        for (int i = 0; i < data1.players.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.name = "Item " + i;
            newItem.GetComponent<FriendTemplate>().InstantiateFriend(i, data1, FriendTemplate.FriendTemplateType.Player);
        }
    }
}
