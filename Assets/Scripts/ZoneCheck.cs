using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneCheck : MonoBehaviour
{
    [SerializeField]
    private LevelArea upZone = LevelArea.None;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel") || upZone < LevelArea.Beach) { return; }
        //setmusicclip
        GameManager.instance.GameCon.BackgroundTransition(collision.transform.position.y > transform.position.y ? (int)upZone - 1 : Mathf.Clamp((int)upZone - 2, 0, (int)LevelArea.GlitchLand));
    }
}
