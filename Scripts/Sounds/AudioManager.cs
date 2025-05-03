using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;


    [SerializeField] private AudioSource sfxSource;

    public AudioClip[] destroySounds;
    public AudioClip[] clickSounds;


    private void Awake()
    {
        LoadVolumeSettings();
    }
    public void PlayDestroySound()
    {
        if (destroySounds.Length > 0 && sfxSource != null)
        {
            AudioClip clip = destroySounds[Random.Range(0, destroySounds.Length)];
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayRandomClickSound()
    {
        if (clickSounds.Length > 0 && sfxSource != null)
        {
            AudioClip clip = clickSounds[Random.Range(0, clickSounds.Length)];
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayClickSound(int index)
    {
        if (clickSounds.Length > 0 && sfxSource != null && index <= clickSounds.Length)
        {
            AudioClip clip = clickSounds[index];
            sfxSource.PlayOneShot(clip);
        }
    }

    public void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float value)
    {
        float volume = Mathf.Approximately(value, 0f) ? -80f : Mathf.Log10(value) * 20;
        mixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float volume = Mathf.Approximately(value, 0f) ? -80f : Mathf.Log10(value) * 20;
        mixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public float GetSavedVolume(string key, float defaultValue = 1f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }



}