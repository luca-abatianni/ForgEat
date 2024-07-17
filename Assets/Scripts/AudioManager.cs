using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioMixer mixer;

    [Header("---Audio Clip---")]
    public AudioClip bgMusic;
    public AudioClip button;
    public AudioClip back;
    public AudioClip confirm;

    private void Start()
    {
        musicSource.clip = bgMusic;
        musicSource.Play();
    }

    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(button);
    }

    public void PlayBackButtonClick()
    {
        sfxSource.PlayOneShot(back);
    }

    public void PlayConfirmButtonClick()
    {
        sfxSource.PlayOneShot(confirm);
    }

    public void ChangeMusicVolume()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(MenuChoices.musicVolume)*20);
    }

    public void ChangeSFXVolume()
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(MenuChoices.sfxVolume)*20);
    }

    //commento
}
