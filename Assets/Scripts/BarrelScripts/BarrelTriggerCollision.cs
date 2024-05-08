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
                GameManager.instance.GameCon.ActivateSwampParticle(collision.ClosestPoint(barrelCon.transform.position));
                barrelCon.IntoMud();
                barrelCon.BarrelRig.velocity = Vector2.ClampMagnitude(barrelCon.BarrelRig.velocity, GameManager.instance.GameScriptObj.BarrelSwampMaxVeclocityMagnitude);
                barrelCon.gravityDirection = (collision.ClosestPoint(barrelCon.BarrelRig.position) - barrelCon.BarrelRig.position).normalized;
            }
            barrelCon.SwampCount++;
        }
        if (collision.CompareTag("Snow"))
        {
            if (barrelCon.SnowCount <= 0)
            {
                barrelCon.BarrelGetInSnow();
            }
            barrelCon.SnowCount++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (barrelCon.InSnowLock && !collision.isTrigger && collision.CompareTag("Ground"))
        {
            GameManager.instance.GameCon.ActivateSnowParticle(collision.ClosestPoint(barrelCon.transform.position));
            barrelCon.SnowCollisionStay();
            barrelCon.gravityDirection = (collision.ClosestPoint(barrelCon.transform.position) - (Vector2)barrelCon.transform.position).normalized;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            barrelCon.GroundCount = Mathf.Clamp(barrelCon.GroundCount - 1, 0, int.MaxValue);
            if (barrelCon.GroundCount <= 0)
            {
                GameManager.instance.GameCon.DeactivateRollDust();
            }
        }
        if (collision.isTrigger && collision.CompareTag("Swamp"))
        {
            barrelCon.gravityDirection = Vector2.down;
            barrelCon.SwampCount = Mathf.Clamp(barrelCon.SwampCount - 1, 0, int.MaxValue);
        }
        if (collision.CompareTag("Snow"))
        {
            barrelCon.SnowCount = Mathf.Clamp(barrelCon.SnowCount - 1, 0, int.MaxValue);
            if (barrelCon.SnowCount <= 0)
            {
                GameManager.instance.GameCon.DeactivateSnowParticle();
                barrelCon.InSnowLock = false;
                barrelCon.gravityDirection = Vector2.down;
            }
        }
    }
}
