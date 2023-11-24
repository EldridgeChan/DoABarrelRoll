using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameScriptableObject", menuName = "ScriptableObjects/Game")]
public class GameScriptableObject : ScriptableObject
{
    [Header("Camera Behaviour Setting")]
    public float CameraMaxLerpTime = 0.5f;
    public float CameraOnMenuMaxLerpT = 0.5f;
    public float CameraDefaultSize = 7.0f;
    public float CameraCutSceneSize = 5.0f;
    public float CameraCutSceneYPositionOffset = 1.0f;

    [Header("Barrel Behaviour Setting")]
    public float BarrelRollMultiplier = 1.0f;
    public float BarrelJumpFullChargeTime = 1.0f;
    public float BarrelFullJumpForce = 100.0f;
    public float BarrelJumpMaxDistance = 5.0f;
    public float BarrelMaxAngularVelocity = 1500.0f;

    [Header("Emoji Basic Setting")]
    public float BarrelEmojiMaxOffsetDistance = 1.0f;
    public float BarrelEmojiInertiaEffectivnessAVThreshold = 100.0f;
    public float BarrelEmojiInertiaMinEffectivness = 0.0f;
    public Sprite[] EmojiTypes;

    [Header("Emoji Inertia Method Setting")]
    public float BarrelEmojiCentripetalForce = 100.0f;
    public float BarrelEmojiMaxVelocity = 10.0f;
    public float BarrelEmojiCounterForceEffectiveness = 1.0f;
    public float BarrelEmojiInertiaEffectiveness = 1.0f;
    public float BarrelEmojiGravity = 1.0f;

    [Header("Emoji Centrifugal Method Setting")]
    public float BarrelEmojiCentrifugalMaxAV = 300.0f;
    public float BarrelEmojiMaxCentrifugalForce = 1.0f;
    public float BarrelEmojiMinCentrifugalForce = -1.0f;
    public float BarrelEmojiVelocityThreshold = 100.0f;

    [Header("Water Tilemap Setting")]
    public float waterFloatingGravityScale = -1.0f;
    public float waterOffDefaultGravityScale = 2.0f;
    public float waterCurrentAcceleration = 1.0f;
    public float waterFloatingMaxVelocity = 3.0f;
    public float waterCurrentMaxVelocity = 5.0f;
    public float waterSinkMaxVelocity = 10.0f;

    [Header("Barrel Animation Setting")]
    public float BarrelPressedMinVelocity = -5.0f;
    public float BarrelRollAnimateSpeedDivisor = 1000.0f;
    public float BarrelGroundCheckDistance = 1.0f;
    public float BarrelKickForce = 50.0f;
    public float BarrelStandYOffset = 0.5f;
    public float BarrelStandMaxVelocity = 1.0f;
    public float BarrelStandMaxAVelocity = 2.0f;
    public float BarrelFallGainControlTime = 1.0f;
    public float BarrelStandFallCoolDown = 1.0f;
    public string[] BarrelErrorTexts;

    [Header("Emoji Animation Setting")]
    public float BarrelEmojiDuckAcceleration = 5.0f;
    public float BarrelEmojiFallVelocity = 3.0f;
    public float BarrelEmojiTurnDuration = 0.5f;
    public float BarrelEmojiStandTimeDelay = 0.5f;
    public float BarrelEmojiStandMoveTime = 0.5f;
    public float BarrelEmojiFastSpinAVThreshold = 100.0f;
    public float BarrelEmojiFastSpinDizzyTime = 3.0f;
    public float BarrelEmojiFastSpinDisgustedTime = 5.0f;
    public float BarrelEmojiFallBackThreshold = 10.0f;
    public float BarrelEmojiFallBackDelay = 2.0f;
    public float BarrelEmojiClimbMinYVelocity = 0.1f;
    public float BarrelEmojiClimbMaxXVelocity = 5.0f;
    public float BarrelEmojiHitWallVelocityThreshold = 5.0f;
    public float BarrelEmojiFallVelocityThreshold = -5.0f;
    public float BarrelEmojiInjuredTime = 1.0f;
    public float BarrelEmojiBackNormalTime = 10.0f;

    [Header("Dusts Animation Setting")]
    public float JumpDustYPositionOffset = -1.3f;
    public float JumpDustDiableTimeDelay = 1.1f;
    public float JumpDustSmallJumpT = 0.3f;
    public float JumpDustLargeJumpT = 0.9f;
    public float PoundDustYPositionOffset = 0.5f;

    [Header("Cut Scene Setting")]
    public float SpeechCharacterPerSecond = 2.0f;
    public float SpeechBubbleHeight = 0.4f;
    public float SpeechBubbleFinishDelay = 2.0f;
    public Sprite ShipNormalBubbleSprite;
    public Sprite ShipFinishBubbleSprite;
    public Sprite OldManNormalBubbleSprite;
    public Vector3 ShipBarrelPositionOffset = new Vector2(0.0f, -5.0f);
    public float BarrelEndTargetAngularVelocity = -100.0f;
    public float BarrelEndLerpAngularVelocityTime = 1.0f;
}
