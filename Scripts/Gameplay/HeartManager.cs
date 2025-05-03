using System;
using TMPro;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    [SerializeField] private GameObject time_bar;
    [SerializeField] private TMP_Text timer;
    private bool initialized = false;

    public int maxHearts = 5;
    public int currentHearts = 0;
    public float heartCooldownMinutes = 21f;

    private DateTime lastHeartTime;
    private float timeLeftToNextHeart = 0f;

    // Эти данные ты получаешь из БД
    private bool needsInitFromServer = false;

    public void InitFromServer(DateTime serverLastHeartTime, int serverHeartCount)
    {
        // if (serverHeartCount != null)
        // {
            lastHeartTime = serverLastHeartTime;
        // }
        currentHearts = serverHeartCount;

        initialized = true;
        needsInitFromServer = true;
    }


    private void CalculateInitialHearts()
    {
        if (currentHearts >= maxHearts)
        {
            time_bar.SetActive(false);
            timeLeftToNextHeart = 0f;
            return;
        }

        DateTime now = DateTime.UtcNow;
        TimeSpan timePassed = now - lastHeartTime;

        int heartsToAdd = Mathf.FloorToInt((float)timePassed.TotalMinutes / heartCooldownMinutes);

        if (heartsToAdd > 0)
        {
            currentHearts += heartsToAdd;

            if (currentHearts >= maxHearts)
            {
                currentHearts = maxHearts;
                timeLeftToNextHeart = 0f;
                time_bar.SetActive(false);
            }
            else
            {
                double leftover = timePassed.TotalMinutes % heartCooldownMinutes;
                timeLeftToNextHeart = (float)(heartCooldownMinutes - leftover) * 60f;
                time_bar.SetActive(true);
            }

            lastHeartTime = now - TimeSpan.FromMinutes(timePassed.TotalMinutes % heartCooldownMinutes);
        }
        else
        {
            timeLeftToNextHeart = (float)(heartCooldownMinutes - timePassed.TotalMinutes) * 60f;
            time_bar.SetActive(true);
        }

        DataManager.Instance.player_hp = currentHearts; // Ставим новое значение
    }


    private void Update()
    {
        if (needsInitFromServer)
        {
            needsInitFromServer = false;
            CalculateInitialHearts(); // ✅ Только один раз
        }

        if (!initialized || currentHearts >= maxHearts) return;

        timeLeftToNextHeart -= Time.deltaTime;

        int min = Mathf.FloorToInt(timeLeftToNextHeart / 60f);
        int sec = Mathf.FloorToInt(timeLeftToNextHeart % 60f);
        timer.text = $"{min:00}:{sec:00}";

        if (timeLeftToNextHeart <= 0f)
        {
            currentHearts++;

            if (currentHearts >= maxHearts)
            {
                currentHearts = maxHearts;
                timeLeftToNextHeart = 0f;
                time_bar.SetActive(false);
            }
            else
            {
                timeLeftToNextHeart = heartCooldownMinutes * 60f;
                time_bar.SetActive(true);
            }

            lastHeartTime = DateTime.UtcNow;

            // Сохраняем результат
            DataManager.Instance.player_hp = currentHearts;
            DataManager.Instance.first_lose_time = lastHeartTime;

            StartCoroutine(DataManager.Instance.ns.SaveHp(currentHearts, currentHearts == maxHearts));
            DataManager.Instance.TriggerHPUpdate();
        }
    }



    public DateTime GetLastHeartTimeToSave()
    {
        return lastHeartTime;
    }
}