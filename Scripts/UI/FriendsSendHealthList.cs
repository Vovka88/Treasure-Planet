using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsSendHealthList : MonoBehaviour
{
    public enum ListType
    {
        Players = 0,
        Send = 1,
        Accept = 2,
    }

    public GameObject itemPrefab;  // Префаб элемента списка
    public Transform contentPanel; // Панель, где будут отображаться элементы

    public Button btn_sent;
    public int numberOfItems = 10; // Количество элементов в списке
    public event Action<int> isPlayersMenu;

    public List<int> playersSentGifts = new List<int>();

    void Start()
    {
        isPlayersMenu += WhichMenuGenerate;
        DataManager.OnGiftsListUpdate += ClearData;
        DataManager.Instance.TriggerGiftsDataUpdated();
        setActiveMenu(0);
    }

    void OnDestroy()
    {
        DataManager.OnGiftsListUpdate -= ClearData;
        DeleteAllListeners();
    }

    public void WhichMenuGenerate(int mode)
    {
        btn_sent.onClick.RemoveAllListeners();
        switch (mode)
        {
            case (int)ListType.Send:
                DataManager.OnGiftsListUpdate += CreateListOfFriendsSends;
                btn_sent.onClick.AddListener(() => SendGiftsToSelected());
                break;
            case (int)ListType.Accept:
                DataManager.OnGiftsListUpdate += CreateListOfFriendsAccepts;
                btn_sent.onClick.AddListener(() => AcceptGiftsToSelected());
                break;
            case (int)ListType.Players:
                DataManager.OnGiftsListUpdate += CreateListOfFriendsRequests;
                btn_sent.onClick.AddListener(() => SendRequestToSelected());
                break;
        }
        DataManager.Instance.TriggerGiftsDataUpdated();

    }

    public void setActiveMenu(int x)
    {
        isPlayersMenu.Invoke(x);
    }




    public void ClearData()
    {
        DataManager.Instance.friends_want_gift = null;
        DataManager.Instance.accept_friends_gift = null;
        DataManager.Instance.ask_friends_gift = null;
        DataManager.Instance.friends_array = null;
        foreach (Transform child in contentPanel)
        {
            DestroyImmediate(child.gameObject);
        }
        DeleteAllListeners();
    }

    public void DeleteAllListeners()
    {
        DataManager.OnGiftsListUpdate -= CreateListOfFriendsSends;
        DataManager.OnGiftsListUpdate -= CreateListOfFriendsAccepts;
        DataManager.OnGiftsListUpdate -= CreateListOfFriendsRequests;
    }

    public void FindSelected()
    {
        foreach (var item in contentPanel.GetComponentsInChildren<FriendGiftTemplate>())
        {
            if (item.GetComponentInChildren<Toggle>().isOn) playersSentGifts.Add(item.id);
        }
    }

    public void SelectAll()
    {
        foreach (var item in contentPanel.GetComponentsInChildren<FriendGiftTemplate>())
        {
            item.GetComponentInChildren<Toggle>().isOn = true;
        }
    }

    private void SendRequestToSelected()
    {
        FindSelected();
        if (playersSentGifts.Count == 0)
        {
            Debug.LogWarning("Никого не выбрано для отправки подарков.");
            return;
        }

        foreach (var id in playersSentGifts)
        {
            StartCoroutine(DataManager.Instance.ns.RequestGift(id));
        }
        DataManager.Instance.TriggerGiftsDataUpdated();
    }


    private void SendGiftsToSelected()
    {
        FindSelected();
        if (playersSentGifts.Count == 0)
        {
            Debug.LogWarning("Никого не выбрано для отправки подарков.");
            return;
        }

        foreach (var id in playersSentGifts)
        {
            // Debug.Log(id);
            StartCoroutine(DataManager.Instance.ns.SendGift(id));
        }
        DataManager.Instance.TriggerGiftsDataUpdated();
    }

    private void AcceptGiftsToSelected()
    {
        FindSelected();
        if (playersSentGifts.Count == 0)
        {
            Debug.LogWarning("Никого не выбрано для отправки подарков.");
            return;
        }

        foreach (var id in playersSentGifts)
        {
            StartCoroutine(DataManager.Instance.ns.AcceptGift(id));
        }
        DataManager.Instance.TriggerGiftsDataUpdated();
    }







    private async void CreateListOfFriendsRequests()
    {
        ClearData();

        await DataManager.Instance.UpdateWantsGiftsFromServer(DataManager.Instance.ns);

        var data = DataManager.Instance.friends_want_gift;
        if (data == null || data.players == null) return;


        for (int i = 0; i < data.players.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.name = "Item " + i;
            newItem.GetComponent<FriendGiftTemplate>().InstantiateFriend(data.players[i].id, data);
            newItem.GetComponent<FriendGiftTemplate>().description.text = "Попросити життя";
        }
    }

    private async void CreateListOfFriendsAccepts()
    {
        ClearData();

        await DataManager.Instance.UpdateAcceptionsofGiftsDataFromServer(DataManager.Instance.ns);

        var data = DataManager.Instance.accept_friends_gift;
        if (data == null || data.players == null) return;


        for (int i = 0; i < data.players.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.name = "Item " + i;
            newItem.GetComponent<FriendGiftTemplate>().InstantiateFriend(data.players[i].id, data);
            newItem.GetComponent<FriendGiftTemplate>().description.text = "Подарував тобі життя";
        }
    }

    private async void CreateListOfFriendsSends()
    {
        ClearData();

        DataManager.Instance.ask_friends_gift = null;

        await DataManager.Instance.UpdateFriendsGiftsDataFromServer(DataManager.Instance.ns);
        // await DataManager.Instance.UpdateFriendsDataFromServer(DataManager.Instance.ns);

        var data = DataManager.Instance.ask_friends_gift;
        // var data = DataManager.Instance.friends_array;
        if (data == null || data.players == null) return;


        for (int i = 0; i < data.players.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, contentPanel);
            newItem.name = "Item " + i;
            newItem.GetComponent<FriendGiftTemplate>().InstantiateFriend(data.players[i].id, data);
            newItem.GetComponent<FriendGiftTemplate>().description.text = "Просить вiдправити життя";
        }
    }
}
