using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource adoSrc;
    [SerializeField]
    private AudioClip[] adoClips;
    [SerializeField]
    private bool onStartRepeat = false;
    [SerializeField]
    private bool notRandom = false;
    [SerializeField]
    private float minReplayTime = 1.0f;
    [SerializeField]
    private float maxReplayTime = 3.0f;

    private bool isPlayCoolDown = false;
    private int clipIndex = 0;

    private void OnEnable()
    {
        if (!adoSrc) { adoSrc = GetComponent<AudioSource>(); }

        if (onStartRepeat)
        {
            PlaySoundAuto();
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
        adoSrc.Stop();
        adoSrc.clip = null;
    }

    public void PlaySoundAuto()
    {
        PlaySoundManual();
        Invoke(nameof(PlaySoundAuto), Random.Range(minReplayTime, maxReplayTime));
    }

    public void PlaySoundManual()
    {
        if (isPlayCoolDown) { return; }
        SetClip();
        adoSrc.Play();
        isPlayCoolDown = true;
        Invoke(nameof(OffPlayCoolDown), minReplayTime);
    }

    public void OffPlayCoolDown()
    {
        isPlayCoolDown = false;
    }

    public void StopRepeat()
    {
        CancelInvoke(nameof(PlaySoundAuto));
    }

    public void ChangeClips(AudioClip[] clips)
    {
        adoClips = clips;
        clipIndex = 0;
    }

    public void SetRepeatTime(float time)
    {
        minReplayTime = time;
        maxReplayTime = time;
    }

    public void SetRandom(bool tf)
    {
        notRandom = !tf;
    }

    private void SetClip()
    {
        if (notRandom)
        {
            adoSrc.clip = adoClips[clipIndex];
            clipIndex = (clipIndex + 1) % adoClips.Length;
        }
        else
        {
            adoSrc.clip = adoClips[Random.Range(0, adoClips.Length)];
        }
    }
}
