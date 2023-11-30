using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCheck : MonoBehaviour
{
    [SerializeField]
    MusicClip toMusic = MusicClip.None;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel") || toMusic == MusicClip.None) { return; }
        if (GameManager.instance.AudioMan.GetCurrentMusic() != GameManager.instance.GameScriptObj.MusicClips[(int)toMusic])
        {
            GameManager.instance.AudioMan.SetMusicClip(GameManager.instance.GameScriptObj.MusicClips[(int)toMusic]);
        }
    }
}
