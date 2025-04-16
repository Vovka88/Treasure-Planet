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

    IEnumerator GetUsers()
    {
        string apiUrl = $"{apiUrlBase}/players";
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
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
                // Debug.Log("Успешная авторизация");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

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
                Debug.Log("Получен json: " + json);
                var data = JsonUtility.FromJson<JsonData>(json);

                // Сохранение токена

                DataManager.Instance.player_id = data.player_id;
                DataManager.Instance.player_avatar_id = data.player_avatar_id;
                DataManager.Instance.player_id = data.player_id;
                DataManager.Instance.player_hp = data.player_hp;
                DataManager.Instance.player_money = data.player_money;
                Debug.Log("Успешная авторизация");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
                tcs.SetResult(false);
            }
        }
    }

    // Оновлення аккаунту "не написано"
    public IEnumerator UpdateData(string token, TaskCompletionSource<bool> tcs)
    {
        string loginUrl = $"{apiUrlBase}/loginByToken";

        string jsonData = "{\"token\": \"" + token + "\", \"name\": \"" + "\"}";
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
                Debug.Log("Получен json: " + json);
                var data = JsonUtility.FromJson<JsonData>(json);

                // Сохранение токена
                Debug.Log("Успешная авторизация");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка входа: " + request.error);
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

    public IEnumerator SaveLevelData(int level_id, int count_of_stars, bool isCompleted, TaskCompletionSource<bool> tcs)
    {
        string scoreUrl = $"{apiUrlBase}/savelevelstats";

        string jsonData = "{\"player_id\": " + DataManager.Instance.player_id +
                  ", \"level_id\": " + level_id +
                  ", \"score\": " + DataManager.Instance.current_score +
                  ", \"count_of_stars\": " + count_of_stars +
                  ", \"completed\": " + isCompleted + "}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(scoreUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Данные Сохранены");
                tcs.SetResult(true);
            }
            else
            {
                Debug.LogError("Ошибка сохранения " + request.error);
                tcs.SetResult(false);
            }
        }
    }



}