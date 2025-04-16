using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            canvas = GetComponent<Canvas>();

            // Подпишись на событие загрузки сцены
            // SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }






    // private void OnDestroy()
    // {
    //     if (Instance == this)
    //         SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     Debug.Log($"[SceneFader] Scene Loaded: {scene.name}");
    //     StartCoroutine(AssignCameraNextFrame());
    // }

    // private IEnumerator AssignCameraNextFrame()
    // {
    //     yield return null; // ждём 1 кадр

    //     if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
    //     {
    //         Debug.Log("[SceneFader] Канвас есть!");
    //         Camera cam = Camera.main;
    //         if (cam != null)
    //         {
    //             canvas.worldCamera = cam;
    //             Debug.Log("[SceneFader] Камера успешно установлена!");
    //         }
    //         else
    //         {
    //             Debug.LogWarning("[SceneFader] Camera.main всё ещё null!");
    //         }
    //     }
    // }

    public void FadeToScene(int sceneID)
    {
        StartCoroutine(FadeSceneRoutine(sceneID));
    }

    private IEnumerator FadeSceneRoutine(int sceneID)
    {
        fadeImage.raycastTarget = true;
        yield return fadeImage.DOFade(1, fadeDuration).WaitForCompletion();
        yield return SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.2f);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        yield return fadeImage.DOFade(0, fadeDuration).WaitForCompletion();
        fadeImage.raycastTarget = false;
    }
}