using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDustBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator jumpDustAnmt;

    public void DisplayJumpDust(BarrelControl barrelCon, Vector3 dir, int jumpLevel)
    {
        transform.SetPositionAndRotation(barrelCon.transform.position + (dir - barrelCon.transform.position).normalized * GameManager.instance.GameScriptObj.JumpDustYPositionOffset, Quaternion.Euler(0.0f, 0.0f, BarrelControl.ToRoundAngle(dir - barrelCon.transform.position) + 90.0f));
        jumpDustAnmt.SetTrigger("ShowDust");
        jumpDustAnmt.SetInteger("JumpLevel", jumpLevel);
        jumpDustAnmt.enabled = true;
    }
}
