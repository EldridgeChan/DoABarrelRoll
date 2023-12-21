using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelTriggerCollision : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelCon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            if (barrelCon.groundCount <= 0)
            {
                GameManager.instance.GameCon.ActivateRollDust(barrelCon.BarrelRig.angularVelocity);
                barrelCon.CheckSetEmojiNormal();
            }
            barrelCon.groundCount++;
        }
        if (collision.CompareTag("Swamp"))
        {
            barrelCon.swampCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            barrelCon.groundCount--;
            if (barrelCon.groundCount <= 0)
            {
                GameManager.instance.GameCon.DeactivateRollDust();
            }
        }
        if (collision.CompareTag("Swamp"))
        {
            barrelCon.gravityDirection = Vector2.down;
            barrelCon.swampCount--;
        }
    }
}
