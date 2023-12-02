using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCutSceneBehaviour : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D barrelRig;

    private float oriAngularVelocity = 0.0f;
    private float lerpT = 0.0f;

    private void OnEnable()
    {
        oriAngularVelocity = barrelRig.angularVelocity;
        lerpT = 0.0f;
    }

    private void FixedUpdate()
    {
        lerpT = Mathf.Clamp01(lerpT + Time.fixedDeltaTime / GameManager.instance.GameScriptObj.BarrelEndLerpAngularVelocityTime);
        barrelRig.angularVelocity = Mathf.Lerp(oriAngularVelocity, (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.BarrelEndTargetAngularVelocity, lerpT);

        if (CheckBarrelEndPosition())
        {
            barrelRig.angularVelocity = 0.0f;
            barrelRig.velocity = Vector2.zero;
            transform.position = GameManager.instance.GameCon.PirateShip.transform.position + GameManager.instance.GameScriptObj.ShipBarrelPositionOffset;
            enabled = false;
        }
    }

    private bool CheckBarrelEndPosition()
    {
        if (GameManager.instance.SaveMan.mirroredTilemap)
        {
            return transform.position.x <= GameManager.instance.GameCon.PirateShip.transform.position.x;
        }
        else
        {
            return transform.position.x >= GameManager.instance.GameCon.PirateShip.transform.position.x;
        }
    }
}
