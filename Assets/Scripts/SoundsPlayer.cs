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
    private float minReplayTime = 1.0f;
    [SerializeField]
    private float maxReplayTime = 3.0f;

    private bool isPlayCoolDown = false;

    private void Start()
    {
        if (!adoSrc) { adoSrc = GetComponent<AudioSource>(); }

        if (onStartRepeat)
        {
            PlaySoundAuto();
        }
    }

    public void PlaySoundAuto()
    {
        CancelInvoke(nameof(OffPlayCoolDown));
        isPlayCoolDown = false;
        PlaySoundManual();
        Invoke(nameof(PlaySoundAuto), Random.Range(minReplayTime, maxReplayTime));
    }

    public void PlaySoundManual()
    {
        if (isPlayCoolDown) { return; }
        SetRandomClip();
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

    public void SetRepeatTime(float time)
    {
        minReplayTime = time;
        maxReplayTime = time;
    }

    private void SetRandomClip()
    {
        adoSrc.clip = adoClips[Random.Range(0, adoClips.Length)];
    }
}
