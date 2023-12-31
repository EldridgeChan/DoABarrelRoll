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
            if (barrelCon.GroundCount <= 0)
            {
                GameManager.instance.GameCon.ActivateRollDust(barrelCon.BarrelRig.angularVelocity);
                barrelCon.CheckSetEmojiNormal();
            }
            barrelCon.GroundCount++;
        }
        if (collision.isTrigger && collision.CompareTag("Swamp"))
        {
            if (barrelCon.SwampCount <= 0)
            {
                barrelCon.IntoMud();
                barrelCon.BarrelRig.velocity = Vector2.ClampMagnitude(barrelCon.BarrelRig.velocity, GameManager.instance.GameScriptObj.BarrelSwampMaxVeclocityMagnitude);
                barrelCon.GravityDirection = (collision.ClosestPoint(barrelCon.BarrelRig.position) - barrelCon.BarrelRig.position).normalized;
            }
            barrelCon.SwampCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            barrelCon.GroundCount--;
            if (barrelCon.GroundCount <= 0)
            {
                GameManager.instance.GameCon.DeactivateRollDust();
            }
        }
        if (collision.isTrigger && collision.CompareTag("Swamp"))
        {
            barrelCon.GravityDirection = Vector2.down;
            barrelCon.SwampCount--;
        }
    }
}
