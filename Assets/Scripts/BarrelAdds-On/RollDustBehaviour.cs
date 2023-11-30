using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDustBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator rollDustAnmt;
    [SerializeField]
    private SpriteRenderer rollDustRend;
    private bool dustActive = false;

    public void ActivateRollDust(float spinSpeed)
    {
        if (dustActive) { return; }
        rollDustAnmt.SetTrigger("ShowDust");
        rollDustRend.flipX = spinSpeed > 0;
        dustActive = true;
    }

    public void RollDustActive(ContactPoint2D contact, float spinSpeed)
    {
        transform.SetPositionAndRotation(contact.point, Quaternion.Euler(0.0f, 0.0f, BarrelControl.ToRoundAngle(contact.normal) - 90.0f));
        rollDustAnmt.SetFloat("SpinSpeed", Mathf.Abs(spinSpeed));
    }

    public void DeactivateRollDust()
    {
        if (rollDustAnmt)
        {
            rollDustAnmt.SetTrigger("DustEnd");
        }
        dustActive = false;
    }

    public bool IsRollDustFlip()
    {
        return rollDustRend.flipX;
    }
}
