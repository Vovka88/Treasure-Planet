using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    public enum AudioType { Music, SFX }
    public AudioType audioType;

    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private Button button;
    private Image icon;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicLastVolumeKey = "LastMusicVolume";
    private const string SFXLastVolumeKey = "LastSFXVolume";

    private bool IsMuted => GetVolume() <= 0.001f;

    void Start()
    {
        button = GetComponent<Button>();
        icon = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
        UpdateIcon();
    }

    void OnClick()
    {
        if (IsMuted)
        {
            float lastVolume = PlayerPrefs.GetFloat(GetLastKey(), 0.5f);
            SetVolume(lastVolume);
        }
        else
        {
            float currentVolume = GetVolume();
            PlayerPrefs.SetFloat(GetLastKey(), currentVolume);
            SetVolume(0f);
        }

        UpdateIcon();
    }

    private float GetVolume()
    {
        string key = audioType == AudioType.Music ? MusicVolumeKey : SFXVolumeKey;
        return PlayerPrefs.GetFloat(key, 1f);
    }

    private void SetVolume(float value)
    {
        if (audioType == AudioType.Music)
            DataManager.Instance.am.SetMusicVolume(value);
        else
            DataManager.Instance.am.SetSFXVolume(value);
    }

    private string GetLastKey()
    {
        return audioType == AudioType.Music ? MusicLastVolumeKey : SFXLastVolumeKey;
    }

    private void UpdateIcon()
    {
        if (icon != null)
            icon.sprite = IsMuted ? offSprite : onSprite;
    }
}
