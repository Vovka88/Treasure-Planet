using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // <--- обязательно!

public class BlurEffectController : MonoBehaviour
{
    public Image image;

    private Material materialInstance;

    // Дефолтные значения
    private float defaultAlpha;
    private float defaultSize;
    private float defaultBrightness;

    private void Start()
    {
        materialInstance = Instantiate(image.material);
        image.material = materialInstance;

        // Сохраняем дефолт
        if (materialInstance.HasProperty("_Alpha"))
            defaultAlpha = materialInstance.GetFloat("_Alpha");

        if (materialInstance.HasProperty("_Size"))
            defaultSize = materialInstance.GetFloat("_Size");

        if (materialInstance.HasProperty("_Brightness"))
            defaultBrightness = materialInstance.GetFloat("_Brightness");
    }

    // Плавное включение блюра
    public void EnableBlur(float duration = 0.5f)
    {
        if (materialInstance == null) return;

        if (materialInstance.HasProperty("_Alpha"))
            DOTween.To(() => materialInstance.GetFloat("_Alpha"),
                       x => materialInstance.SetFloat("_Alpha", x),
                       defaultAlpha, duration);

        if (materialInstance.HasProperty("_Size"))
            DOTween.To(() => materialInstance.GetFloat("_Size"),
                       x => materialInstance.SetFloat("_Size", x),
                       defaultSize, duration);

        if (materialInstance.HasProperty("_Brightness"))
            DOTween.To(() => materialInstance.GetFloat("_Brightness"),
                       x => materialInstance.SetFloat("_Brightness", x),
                       defaultBrightness, duration);
    }

    // Плавное выключение блюра
    public void DisableBlur(float duration = 0.5f)
    {
        if (materialInstance == null) return;

        if (materialInstance.HasProperty("_Alpha"))
            DOTween.To(() => materialInstance.GetFloat("_Alpha"),
                       x => materialInstance.SetFloat("_Alpha", x),
                       1f, duration);

        if (materialInstance.HasProperty("_Size"))
            DOTween.To(() => materialInstance.GetFloat("_Size"),
                       x => materialInstance.SetFloat("_Size", x),
                       1f, duration);

        if (materialInstance.HasProperty("_Brightness"))
            DOTween.To(() => materialInstance.GetFloat("_Brightness"),
                       x => materialInstance.SetFloat("_Brightness", x),
                       1f, duration);
    }

    // Настройка только альфы (ручная)
    public void SetAlpha(float alpha, float duration = 0.3f)
    {
        if (materialInstance != null && materialInstance.HasProperty("_Alpha"))
        {
            DOTween.To(() => materialInstance.GetFloat("_Alpha"),
                       x => materialInstance.SetFloat("_Alpha", x),
                       Mathf.Clamp01(alpha), duration);
        }
    }
}