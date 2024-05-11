using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgmAdoSrc;
    [SerializeField]
    private AudioSource clickAdoSrc;

    private bool musicLerping = false;
    private bool musicLerpDir = false;
    private float musicLerpT = 0.0f;
    private AudioClip toClip = null;

    private void Start()
    {
        StartLerpMusicVolume(true);
    }

    private void Update()
    {
        if (musicLerping)
        {
            musicLerpT = Mathf.Clamp01(musicLerpT + ((musicLerpDir ? 1.0f : -1.0f) * Time.deltaTime / GameManager.instance.GameScriptObj.MusicLerpTime));
            bgmAdoSrc.volume = Mathf.Lerp(0.0f, GameManager.instance.SaveMan.SettingSave.musicVolume, musicLerpT);
            musicLerping = musicLerpT > 0.0f && musicLerpT < 1.0f;
            if (musicLerpT <= 0.0f)
            {
                bgmAdoSrc.Stop();
                SetMusicClip();
            }
        }
    }

    public void BGMTransition(AudioClip toClip)
    {
        if (!bgmAdoSrc || toClip == bgmAdoSrc.clip) { return; }
        this.toClip = toClip;
        StartLerpMusicVolume(false);
    }

    private void SetMusicClip()
    {
        if (!bgmAdoSrc.enabled) { return; }
        bgmAdoSrc.clip = toClip;
        if (!toClip) { return; }
        bgmAdoSrc.Play();
        StartLerpMusicVolume(true);
    }

    private void StartLerpMusicVolume(bool dir)
    {
        musicLerping = true;
        musicLerpDir = dir;
    }

    public void SetBGMVolume(float volume)
    {
        bgmAdoSrc.volume = volume;
    }

    public void PlayClickSound()
    {
        if (!clickAdoSrc.enabled) { return; }
        clickAdoSrc.Play();
    }

    public void InitClickAucioSource()
    {
        clickAdoSrc.enabled = true;
    }
}
