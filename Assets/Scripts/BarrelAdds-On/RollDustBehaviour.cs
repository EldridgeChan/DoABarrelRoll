using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDustBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator rollDustAnmt;
    [SerializeField]
    private SpriteRenderer rollDustRend;

    public void ActivateRollDust(ContactPoint2D contact, float spinSpeed)
    {
        rollDustAnmt.SetTrigger("ShowDust");
        RollDustActive(contact,Mathf.Abs(spinSpeed));
        rollDustRend.flipX = spinSpeed > 0;
    }

    public void RollDustActive(ContactPoint2D contact, float spinSpeed)
    {
        transform.SetPositionAndRotation(contact.point, Quaternion.Euler(0.0f, 0.0f, BarrelControl.ToRoundAngle(contact.normal) - 90.0f));
        rollDustAnmt.SetFloat("SpinSpeed", Mathf.Abs(spinSpeed));
    }

    public void DeactivateRollDust()
    {
        rollDustAnmt.SetTrigger("DustEnd");
    }
}
