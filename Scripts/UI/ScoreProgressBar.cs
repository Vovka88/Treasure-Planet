using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreProgressBar : MonoBehaviour
{
    [SerializeField] private LevelUIController lc;

    public Slider progressSlider;
    public RectTransform fillArea; // Drag сюда Fill Area из иерархии слайдера
    public GameObject starPrefab;  // Префаб звезды
    public Sprite starOn;
    public Sprite starOff;


    private GameObject[] stars;

    void Start()
    {
        progressSlider.minValue = 0;
        progressSlider.maxValue = lc.tc.scoreForEnd[lc.tc.scoreForEnd.Length - 1];
        progressSlider.value = 0;

        stars = new GameObject[lc.tc.scoreForEnd.Length];

        for (int i = 0; i < lc.tc.scoreForEnd.Length; i++)
        {
            GameObject star = Instantiate(starPrefab, fillArea);
            stars[i] = star;

            // Важно: после следующего кадра fillArea.rect.width будет валиден
            LayoutRebuilder.ForceRebuildLayoutImmediate(fillArea);

            float normalized = (float)lc.tc.scoreForEnd[i] / progressSlider.maxValue;
            float x = normalized * fillArea.rect.width;

            RectTransform rt = star.GetComponent<RectTransform>();
            rt.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, 0);

            star.GetComponent<Image>().sprite = starOff;
        }

        lc.tc.onScoreChanged += UpdateProgress;
    }
    
    public void UpdateProgress(int currentScore)
    {
        // progressSlider.value = Mathf.Clamp(currentScore, 0, progressSlider.maxValue);
        progressSlider.DOValue(Mathf.Clamp(currentScore, 0, progressSlider.maxValue), 0.5f);

        for (int i = 0; i < lc.tc.scoreForEnd.Length; i++)
        {
            Image starImage = stars[i].GetComponent<Image>();
            starImage.sprite = currentScore >= lc.tc.scoreForEnd[i] ? starOn : starOff;
        }
    }
}
