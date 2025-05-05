using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkScript : MonoBehaviour
{
    private string apiUrlBase = "http://127.0.0.1:8000/api"; // Используем API-роут

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
                DataManager.Instance.player_avatar_id = data.avatar_id;
                DataManager.Instance.player_hp = data.player_hp;
                DataManager.Instance.player_money = data.player_money;

                Debug.Log($"Lose time string: {data.lose_time}");
                DateTime parsedLoseTime = new DateTime();
                if (data.lose_time.Length > 0)
                {
                    parsedLoseTime = DateTime.ParseExact(
                        data.lose_time,
                        "yyyy-MM-dd HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                }
                else
                {
                    parsedLoseTime = DateTime.Now;
                }

                // Сохраняем
                DataManager.Instance.first_lose_time = parsedLoseTime;

                // Debug.Log(json);
                // Debug.Log(data.lose_time);
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

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
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

    // Сохранение хп игрока (не написано)
    public IEnumerator SaveHp(int hp, bool isItFirst)
    {
        Debug.Log("SaveHp method entered");
        string scoreUrl = $"{apiUrlBase}/updateplayerhp";
        string jsonData = "";
        if (isItFirst)
        {
            jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                                ", \"hp\": " + hp +
                                 ", \"lose_time\": \"" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
        }
        else
        {
            jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                                ", \"hp\": " + hp + "}";
        }

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        Debug.Log("Starting Save");
        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            // request.timeout = 10;

            yield return request.SendWebRequest();
            DataManager.Instance.TriggerHPUpdate();

            // Debug.Log("Ending Save");
            // if (request.result == UnityWebRequest.Result.Success)
            // {
            //     Debug.Log("Сохранение прошло успешно");
            //     tcs.SetResult(true);
            // }
            // else
            // {
            //     Debug.LogError("Ошибка сохранения " + request.error);
            //     tcs.SetResult(false);
            // }
        }
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
                DataManager.Instance.friends_array = null;
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
                            ", \"friend_id\": " + (friend_id + 1) + "}";
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

        string jsonData = "{\"player_id\": " + (friend_id - 2) +
                            ", \"friend_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        Debug.LogError(DataManager.Instance.player_id);
        Debug.LogError(friend_id - 2);


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

        Debug.Log(DataManager.Instance.player_id);
        Debug.Log(friend_id + 1);

        string jsonData = "{\"player_id\": " + (friend_id + 1) +
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
        string scoreUrl = $"{apiUrlBase}/deletefriend";

        Debug.Log(DataManager.Instance.player_id);
        Debug.Log(friend_id);

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                            ", \"friend_id\": " + (friend_id) + "}";
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


    ///----------------------------
    /// GIFTS CODE PART
    ///----------------------------

    // Получение друзей которые просят сердца (написано)
    public IEnumerator GetWantsForGifts(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/heartsentrequests";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // Debug.LogError(DataManager.Instance.player_id);
        // Debug.LogError(friend_id);


        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log(json);

                JsonDataList data = new JsonDataList();

                DataManager.Instance.friends_want_gift = null;

                try
                {
                    data = JsonUtility.FromJson<JsonDataList>(json);
                }
                catch (System.Exception)
                {
                    
                }

                // Сохранение токена
                DataManager.Instance.friends_want_gift = data;

                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Получение сердец друзей (написано)
    public IEnumerator GetAcceptionsForGifts(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/heartincoming-hearts";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        // Debug.LogError(DataManager.Instance.player_id);
        // Debug.LogError(friend_id);


        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DataManager.Instance.accept_friends_gift = null;
                string json = request.downloadHandler.text;
                Debug.Log(json);

                JsonDataList data = new JsonDataList();

                DataManager.Instance.accept_friends_gift = null;

                try
                {
                    data = JsonUtility.FromJson<JsonDataList>(json);
                }
                catch (System.Exception)
                {
                    
                }

                // Сохранение токена
                DataManager.Instance.accept_friends_gift = data;

                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Получение друзей которые просят сердца (написано)
    public IEnumerator GetFriendsForGifts(TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/heartincomingrequests";

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
                DataManager.Instance.ask_friends_gift = null;

                string json = request.downloadHandler.text;
                Debug.Log(json);
                JsonDataList data = new JsonDataList();

                DataManager.Instance.ask_friends_gift = null;

                try
                {
                    data = JsonUtility.FromJson<JsonDataList>(json);
                }
                catch (System.Exception)
                {
                    
                }

                // Сохранение токена
                DataManager.Instance.ask_friends_gift = data;

                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Принятие подарка (не написано)
    public IEnumerator AcceptGift(int sender_id)
    {
        string scoreUrl = $"{apiUrlBase}/heartreceive";

        Debug.Log(DataManager.Instance.player_id - 3);
        Debug.Log(sender_id);

        string jsonData = "{\"player_id\": " + (DataManager.Instance.player_id - 3) + ", \"sender_id\": " + sender_id + "}";
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
                Debug.Log(json);
                StartCoroutine(SaveHp(DataManager.Instance.player_hp + 1, false));
            }
            else
            {
                Debug.LogError("Ошибка получения " + request.error);
            }
        }
    }

    // Отправка подарка (не написано)
    public IEnumerator SendGift(int receiver_id)
    {
        string scoreUrl = $"{apiUrlBase}/heartsend";

        Debug.Log(receiver_id);

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + ", \"receiver_id\": " + receiver_id + "}";
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
                Debug.Log(json);
            }
            else
            {
                Debug.LogError("Ошибка отправки " + request.error);
            }
        }
    }

    // Попросить подарок (не написано)
    public IEnumerator RequestGift(int receiver_id)
    {
        string scoreUrl = $"{apiUrlBase}/heartrequest";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id + ", \"receiver_id\": " + receiver_id + "}";
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
                Debug.Log(json);
            }
            else
            {
                Debug.LogError("Ошибка отправки " + request.error);
            }
        }
    }


}