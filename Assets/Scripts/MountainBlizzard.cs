using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainBlizzard : MonoBehaviour
{
    [SerializeField]
    private bool doubleDirection = false;
    [SerializeField]
    private bool blizzardDirRight = true;
    [SerializeField]
    private float blizzardStartDelay = 1.0f;
    [SerializeField]
    private float blizzardActiveTime = 1.0f;
    [SerializeField]
    private float blizzardInacticeTime = 1.0f;

    private bool blizzardActive = false;
    private bool inBlizzard = false;
    private int barrelCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || !collision.CompareTag("Barrel")) { return; }
        barrelCount++;
        if (barrelCount > 1) { return; }
        GameManager.instance.GameCon.StartSnowing();
        blizzardActive = true;
        GameManager.instance.GameCon.BlizzardImageCon.ShowBlizzardImage(0);
        Invoke(nameof(ShowBlizzard), blizzardStartDelay - GameManager.instance.GameScriptObj.BlizzardShowBuffer);
        Invoke(nameof(StartBlizzard), blizzardStartDelay);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger || !collision.CompareTag("Barrel")) { return; }
        barrelCount = Mathf.Clamp(barrelCount - 1, 0, int.MaxValue);
        if (barrelCount > 0) { return; }
        blizzardActive = false;
        PauseBlizzard();
        CancelInvoke();
        GameManager.instance.GameCon.BlizzardImageCon.HideBlizzardImage();
        if (GameManager.instance.GameCon.CurrentArea != LevelArea.SnowMountain)
        {
            GameManager.instance.GameCon.StopSnowing();
        }
    }

    private void FixedUpdate()
    {
        if (blizzardActive && inBlizzard)
        {
            GameManager.instance.GameCon.BarrelBlizzard((GameManager.instance.SaveMan.mirroredTilemap ? -1 : 1) * (blizzardDirRight ? 1 : -1));
        }
    }

    private void StartBlizzard()
    {
        inBlizzard = true;
        CancelInvoke();
        Invoke(nameof(PauseBlizzard), blizzardActiveTime);
    }

    private void ShowBlizzard()
    {
        if (!GameManager.instance.GameCon) { return; }
        GameManager.instance.GameCon.SetBlizzardSoundActive(true);
        GameManager.instance.GameCon.SetBlizzardDirection((GameManager.instance.SaveMan.mirroredTilemap ? -1 : 1) * (blizzardDirRight ? 1 : -1));
        GameManager.instance.GameCon.BlizzardImageCon.ShowBlizzardImage((GameManager.instance.SaveMan.mirroredTilemap ? -1 : 1) * (blizzardDirRight ? 1 : -1));
    }

    private void PauseBlizzard()
    {
        if (!GameManager.instance.GameCon) { return; }
        inBlizzard = false;
        GameManager.instance.GameCon.SetBlizzardSoundActive(false);
        GameManager.instance.GameCon.SetBlizzardDirection(0);
        GameManager.instance.GameCon.BlizzardImageCon.ShowBlizzardImage(0);
        if (doubleDirection)
        {
            blizzardDirRight = !blizzardDirRight;
        }
        CancelInvoke();
        Invoke(nameof(ShowBlizzard), blizzardInacticeTime - GameManager.instance.GameScriptObj.BlizzardShowBuffer);
        Invoke(nameof(StartBlizzard), blizzardInacticeTime);
    }
}
