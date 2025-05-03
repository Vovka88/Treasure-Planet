using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // Загружаем сохранённые значения и применяем
        float musicVolume = DataManager.Instance.am.GetSavedVolume("MusicVolume", 1f);
        float sfxVolume = DataManager.Instance.am.GetSavedVolume("SFXVolume", 1f);

        musicSlider.SetValueWithoutNotify(musicVolume);
        sfxSlider.SetValueWithoutNotify(sfxVolume);

        DataManager.Instance.am.SetMusicVolume(musicVolume);
        DataManager.Instance.am.SetSFXVolume(sfxVolume);

        // Подписываемся на события
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnMusicSliderChanged(float value)
    {
        DataManager.Instance.am.SetMusicVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        DataManager.Instance.am.SetSFXVolume(value);
    }
}