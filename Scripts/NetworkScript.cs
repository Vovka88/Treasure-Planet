using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkScript : MonoBehaviour
{
    //     public static NetworkScript Instance { get; private set; }
    private string apiUrlBase = "http://127.0.0.1:8000/api"; // Используем API-роут

    // void Awake()
    // {
    //     Screen.sleepTimeout = SleepTimeout.NeverSleep;

    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }


    ///----------------------------
    /// PLAYER CODE PART
    ///----------------------------

    public IEnumerator GetUsers(TaskCompletionSource<bool> tcs)
    {
        string apiUrl = $"{apiUrlBase}/getplayers";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                var data = JsonUtility.FromJson<JsonDataList>(json);

                // Сохранение токена
                DataManager.Instance.players_no_friends_array = data;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Логін "Написано"
    public IEnumerator Login(string email, string password, TaskCompletionSource<bool> tcs)
    {
        string loginUrl = $"{apiUrlBase}/login";

        string jsonData = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                // Debug.Log("Получен json: " + json);
                var temp = JsonUtility.FromJson<JsonData>(json);

                // Сохранение токена

                yield return SafeWriteToFile(temp.token);
                DataManager.Instance.player_id = temp.player_id;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Логін 2 "Написано"
    public IEnumerator Login(string token, TaskCompletionSource<bool> tcs)
    {
        string loginUrl = $"{apiUrlBase}/loginByToken";

        string jsonData = "{\"token\": \"" + token + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                var data = JsonUtility.FromJson<JsonData>(json);

                // Сохранение токена
                DataManager.Instance.token = token;
                DataManager.Instance.player_id = data.player_id;
                DataManager.Instance.username = data.username;
                DataManager.Instance.player_avatar_id = data.player_avatar_id;
                DataManager.Instance.player_id = data.player_id;
                DataManager.Instance.player_hp = data.player_hp;
                DataManager.Instance.player_money = data.player_money;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Оновлення аккаунту "написано"
    public IEnumerator UpdateUsernameAndAvatar(string username, int avatar_id, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/updateusername";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                  ", \"username\": \"" + username +
                  "\", \"avatar_id\": " + avatar_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка обновления " + request.error);
                tcs.SetResult(false);
            }
        }
    }



    // Реєстрація "написано"
    public IEnumerator Registration(string email, string password, TaskCompletionSource<bool> tcs)
    {
        Debug.Log("NS start Registration");
        string regUrl = $"{apiUrlBase}/registration";

        string jsonData = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(regUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Debug.Log("Аккаунт успешно зарегестрирован");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Выход из аккаунта (написано)
    public IEnumerator Logout(TaskCompletionSource<bool> tcs)
    {
        string logoutUrl = $"{apiUrlBase}/logout";

        string token = File.ReadLines(Application.persistentDataPath + "/token.txt").ToString();

        string jsonData = "{\"token\": \"" + token + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(logoutUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                yield return SafeWriteToFile(string.Empty);
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка выхода: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Удаление аккаунта (написано)
    public IEnumerator DeleteAccount(TaskCompletionSource<bool> tcs)
    {
        string logoutUrl = $"{apiUrlBase}/deleteUser"; ;

        string jsonData = "{\"player_id\": \"" + DataManager.Instance.player_id + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(logoutUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Debug.Log("Успешное удаление");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка удаление: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    public bool SafeWriteToFile(string content)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        using (FileStream fs = new FileStream(Application.persistentDataPath + "/token.txt", FileMode.Create, FileAccess.Write, FileShare.None))
        using (StreamWriter writer = new StreamWriter(fs))
        {
            writer.Write(content);
        }

        return true;
    }

    ///----------------------------
    /// LEVELS CODE PART
    ///----------------------------

    // Получение данных левела (написано) 
    public IEnumerator GetLevelScore(int levelId, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/getstats";

        string jsonData = "{\"player_id\": \"" + DataManager.Instance.player_id + "\", \"level_id\": \"" + levelId + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string level_score = request.downloadHandler.text;
                tcs.SetResult(true);
                DataManager.Instance.current_score = Int32.Parse(level_score);
            }
            else
            {
                Debug.LogError("Ошибка выхода: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Получение уровней игрока (написано) 
    public IEnumerator GetPlayerLevels(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/getlevels";

        Debug.Log("Player ID: " + DataManager.Instance.player_id);

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string levels = request.downloadHandler.text;
                DataManager.Instance.json_levels = levels;
                Debug.Log(levels);
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Сохранение данных уровня (написано)
    public IEnumerator SaveLevelData(int level_id, int count_of_stars, bool isCompleted, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/savelevelstats";

        JsonLevelData data = new JsonLevelData
        {
            player_id = DataManager.Instance.player_id,
            level_id = level_id,
            score = DataManager.Instance.current_score,
            count_of_stars = count_of_stars,
            completed = isCompleted ? 1 : 0
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;   
                Debug.Log("Данные Сохранены: " + json);
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка сохранения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    ///----------------------------
    /// FRIENDS CODE PART
    ///----------------------------


    // Получение друзей (написано)
    public IEnumerator GetFriends(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/getfriends";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                var data = JsonUtility.FromJson<JsonDataList>(json);

                DataManager.Instance.friends_array = data;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Получение инвайтов в друзья (написано)
    public IEnumerator GetFriendsInvites(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/getfriendsinvites";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                var data = JsonUtility.FromJson<JsonDataList>(json);

                DataManager.Instance.requested_friends_array = data;

                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Отправка инвайтов в друзей (написано)
    public IEnumerator SendInviteToFriend(int friend_id, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/sendfriendinvite";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                            ", \"friend_id\": " + friend_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // string json = request.downloadHandler.text;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка удаления " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Принятие в друзья (написано)
    public IEnumerator AcceptInviteToFriend(int friend_id, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/acceptfriendinvite";

        string jsonData = "{\"player_id\": " + friend_id +
                            ", \"friend_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        Debug.LogError(DataManager.Instance.player_id);
        Debug.LogError(friend_id);


        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // string json = request.downloadHandler.text;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка удаления " + request.error);
                tcs.SetResult(false);
            }
        }
    }


    // Отмена приглашения в друзья (написано)
    public IEnumerator DeclineInviteToFriend(int friend_id, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/declinefriendinvite";

        string jsonData = "{\"player_id\": " + friend_id +
                            ", \"friend_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);


        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // string json = request.downloadHandler.text;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка удаления " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Удаление из друзей ( написано)
    public IEnumerator DeleteFromFriends(int friend_id, TaskCompletionSource<bool> tcs)
    {
        Debug.Log("Starting friend deleting");
        string scoreUrl = $"{apiUrlBase}/deletefriend";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                            ", \"friend_id\": " + friend_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        Debug.LogError(DataManager.Instance.player_id);
        Debug.LogError(friend_id);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // string json = request.downloadHandler.text;
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка удаления " + request.error);
                tcs.SetResult(false);
            }
        }
    }

}