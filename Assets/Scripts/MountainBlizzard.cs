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
        blizzardActive = true;
        PauseBlizzard();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel")) { return; }
        blizzardActive = false;
        CancelInvoke();
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

    private void PauseBlizzard()
    {
        inBlizzard = false;
        if (doubleDirection)
        {
            blizzardDirRight = !blizzardDirRight;
        }
        Invoke(nameof(StartBlizzard), blizzardInacticeTime);
    }
}
