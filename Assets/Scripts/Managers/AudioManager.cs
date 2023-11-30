using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgmSource;

    private bool musicLerping = false;
    private bool musicLerpDir = false;
    private float musicLerpT = 0.0f;

    private void Start()
    {
        StartLerpMusicVolume(true);
    }

    private void Update()
    {
        if (musicLerping)
        {
            musicLerpT = Mathf.Clamp01(musicLerpT + ((musicLerpDir ? 1.0f : -1.0f) * Time.deltaTime / GameManager.instance.GameScriptObj.MusicLerpTime));
            bgmSource.volume = Mathf.Lerp(0.0f, GameManager.instance.SaveMan.musicVolume, musicLerpT);
            musicLerping = musicLerpT > 0.0f && musicLerpT < 1.0f;
            if (musicLerpT <= 0.0f)
            {
                bgmSource.Stop();
                bgmSource.clip = null;
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SetMusicClip(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
        StartLerpMusicVolume(true);
    }

    public void StartLerpMusicVolume(bool dir)
    {
        musicLerping = true;
        musicLerpDir = dir;
    }

    public AudioClip GetCurrentMusic()
    {
        return bgmSource.clip;
    }
}
