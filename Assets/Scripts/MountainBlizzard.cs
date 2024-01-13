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
    private float blizzardActiveTime = 1.0f;
    [SerializeField]
    private float blizzardInacticeTime = 1.0f;

    private bool blizzardActive = false;
    private bool inBlizzard = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel")) { return; }
        GameManager.instance.GameCon.StartSnowing();
        blizzardActive = true;
        PauseBlizzard();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel")) { return; }
        blizzardActive = false;
        CancelInvoke();
        if (GameManager.instance.GameCon.CurrentArea != LevelArea.SnowMountain)
        {
            GameManager.instance.GameCon.StopSnowing();
        }
    }

    private void FixedUpdate()
    {
        if (blizzardActive && inBlizzard)
        {
            GameManager.instance.GameCon.BarrelBlizzard(blizzardDirRight ? 1 : -1);
        }
    }

    private void StartBlizzard()
    {
        inBlizzard = true;
        Invoke(nameof(PauseBlizzard), blizzardActiveTime);
    }

    private void ShowBlizzard()
    {
        if (!GameManager.instance.GameCon) { return; }
        GameManager.instance.GameCon.SetBlizzardDirection(blizzardDirRight ? 1 : -1);
    }

    private void PauseBlizzard()
    {
        if (!GameManager.instance.GameCon) { return; }
        inBlizzard = false;
        GameManager.instance.GameCon.SetBlizzardDirection(0);
        if (doubleDirection)
        {
            blizzardDirRight = !blizzardDirRight;
        }
        Invoke(nameof(ShowBlizzard), blizzardInacticeTime - GameManager.instance.GameScriptObj.BlizzardShowBuffer);
        Invoke(nameof(StartBlizzard), blizzardInacticeTime);
    }
}
