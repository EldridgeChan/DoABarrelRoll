using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameScriptableObject", menuName = "ScriptableObjects/Game")]
public class GameScriptableObject : ScriptableObject
{
    [Header("Barrel Behaviour Setting")]
    public float BarrelRollMultiplier = 1.0f;
    public float BarrelJumpFullChargeTime = 1.0f;
    public float BarrelFullJumpForce = 100.0f;
    public float BarrelJumpMaxDistance = 5.0f;
    public float BarrelRollAnimateSpeedDivisor = 1000.0f;
    public float BarrelPressedMinVelocity = -5.0f;
    public float BarrelGroundCheckDistance = 1.0f;
    public float BarrelMaxAngularVelocity = 1500.0f;

    [Header("Barrel Emoji Setting")]
    [Header("Emoji Basic Setting")]
    public float BarrelEmojiMaxOffsetDistance = 1.0f;
    public float BarrelEmojiInertiaEffectivnessAVThreshold = 100.0f;
    public float BarrelEmojiInertiaMinEffectivness = 0.0f;

    [Header("Barrel Inertia Method Setting")]
    public float BarrelEmojiCentripetalForce = 100.0f;
    public float BarrelEmojiMaxVelocity = 10.0f;
    public float BarrelEmojiCounterForceEffectiveness = 1.0f;
    public float BarrelEmojiInertiaEffectiveness = 1.0f;
    public float BarrelEmojiGravity = 1.0f;

    [Header("Barrel Centrifugal Method Setting")]
    public float BarrelEmojiCentrifugalMaxAV = 300.0f;
    public float BarrelEmojiMaxCentrifugalForce = 1.0f;
    public float BarrelEmojiMinCentrifugalForce = -1.0f;
    public float BarrelEmojiVelocityThreshold = 100.0f;

    [Header("Water Tilemap Setting")]
    public float waterFloatingGravityScale = -1.0f;
    public float waterCurrentAcceleration = 1.0f;
    public float waterFloatingMaxVelocity = 3.0f;
    public float waterCurrentMaxVelocity = 5.0f;
    public float waterSinkMaxVelocity = 10.0f;

}
