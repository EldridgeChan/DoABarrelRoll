using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelCon;
    [SerializeField]
    private Rigidbody2D barrelRig;
    [SerializeField]
    private Animator emojiAnmt;
    private Vector2 mockVelocity = Vector2.zero;
    private Vector2 barrelPreVelocity = Vector2.zero;
    private float radiusT = 0.0f;

    [SerializeField]
    private bool lockRotation = false;
    private void Start()
    {
        barrelPreVelocity = barrelRig.velocity;
    }

    private void FixedUpdate()
    {
        if (lockRotation)
        {
            transform.rotation = Quaternion.identity;
        }
        EmojiBehave();
    }

    private void EmojiBehave()
    {
        InertiaMethod();
        CentrifugalMethod();
    }

    private void InertiaMethod()
    {
        EmojiMove();
        EmojiCentralAccelerate();
    }

    //Inertia Method
    private void EmojiMove()
    {
        transform.position = barrelRig.position + ClampVector(((Vector2)transform.position - barrelRig.position) + Time.fixedDeltaTime * EmojiInertiaEffectivenessMultiplier() * mockVelocity, GameManager.instance.GameScriptObj.BarrelEmojiMaxOffsetDistance);
    }

    private float EmojiInertiaEffectivenessMultiplier()
    {
        return Mathf.Lerp(1.0f, GameManager.instance.GameScriptObj.BarrelEmojiInertiaMinEffectivness, Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity), 0.0f, GameManager.instance.GameScriptObj.BarrelEmojiInertiaEffectivnessAVThreshold) / GameManager.instance.GameScriptObj.BarrelEmojiInertiaEffectivnessAVThreshold);
    }

    //Inertia Method
    private void EmojiCentralAccelerate()
    {
        mockVelocity = ClampVector(mockVelocity
            + GameManager.instance.GameScriptObj.BarrelEmojiCentripetalForce * (barrelRig.position - (Vector2)transform.position) //Centripetal
            + GameManager.instance.GameScriptObj.BarrelEmojiCounterForceEffectiveness * -barrelRig.velocity //Counter Force
            + GameManager.instance.GameScriptObj.BarrelEmojiInertiaEffectiveness * (barrelPreVelocity - barrelRig.velocity) //Inertia
            + GameManager.instance.GameScriptObj.BarrelEmojiGravity * Time.fixedDeltaTime * Vector2.down
            , GameManager.instance.GameScriptObj.BarrelEmojiMaxVelocity);
        barrelPreVelocity = barrelRig.velocity;
    }

    private void CentrifugalMethod()
    {
        if (barrelRig.velocity.magnitude < GameManager.instance.GameScriptObj.BarrelEmojiVelocityThreshold)
        {
            return;
        }
        EmojiCentrifugalForce();
    }

    //Centripetal Method
    private void EmojiCentrifugalForce()
    {
        radiusT = transform.localPosition.magnitude / GameManager.instance.GameScriptObj.BarrelEmojiMaxOffsetDistance;
        radiusT = Mathf.Clamp(radiusT + CentrifugalDeltaT() * Time.fixedDeltaTime, 0.0001f, 1.0f);
        transform.localPosition = Mathf.Lerp(0.0f, GameManager.instance.GameScriptObj.BarrelEmojiMaxOffsetDistance, radiusT) * transform.localPosition.normalized;
    }

    private float CentrifugalDeltaT()
    {
        return Mathf.Lerp(GameManager.instance.GameScriptObj.BarrelEmojiMinCentrifugalForce, GameManager.instance.GameScriptObj.BarrelEmojiMaxCentrifugalForce, Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.BarrelEmojiCentrifugalMaxAV);
    }

    private Vector2 ClampVector(Vector2 vector, float max)
    {
        return vector.magnitude > max ? vector.normalized * max : vector;
    }

    private void EmojiFacesTree()
    {
        //emojiAnmt.SetFloat();
    }
}
