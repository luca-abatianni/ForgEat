using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

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
}
