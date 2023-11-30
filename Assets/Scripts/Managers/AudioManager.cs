using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgmAdoSrc;

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
            bgmAdoSrc.volume = Mathf.Lerp(0.0f, GameManager.instance.SaveMan.musicVolume, musicLerpT);
            musicLerping = musicLerpT > 0.0f && musicLerpT < 1.0f;
            if (musicLerpT <= 0.0f)
            {
                bgmAdoSrc.Stop();
                bgmAdoSrc.clip = null;
            }
        }
    }

    public void SetBGMVolume(float volume)
    {
        bgmAdoSrc.volume = volume;
    }

    public void SetMusicClip(AudioClip clip)
    {
        bgmAdoSrc.clip = clip;
        bgmAdoSrc.Play();
        StartLerpMusicVolume(true);
    }

    public void StartLerpMusicVolume(bool dir)
    {
        musicLerping = true;
        musicLerpDir = dir;
    }

    public AudioClip GetCurrentMusic()
    {
        return bgmAdoSrc.clip;
    }
}
