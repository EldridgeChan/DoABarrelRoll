using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCheck : MonoBehaviour
{
    [SerializeField]
    private LevelArea upZone = LevelArea.None;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision || !collision.CompareTag("Barrel") || upZone < LevelArea.Beach) { return; }
        GameManager.instance.AudioMan.BGMTransition(GameManager.instance.GameScriptObj.MusicClips[collision.transform.position.y > transform.position.y ? (int)upZone : Mathf.Clamp((int)upZone - 1, (int)LevelArea.Beach, (int)LevelArea.GlitchLand)]);
        GameManager.instance.GameCon.BackgroundTransition(collision.transform.position.y > transform.position.y ? (int)upZone : Mathf.Clamp((int)upZone - 1, 0, (int)LevelArea.GlitchLand));
        GameManager.instance.GameCon.LevelSoundTransition(collision.transform.position.y > transform.position.y ? upZone : upZone - 1);
    }
}
