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
    public Vector2 CameraMaxOffsetBoundary = Vector2.zero;

    [Header("Barrel Behaviour Setting")]
    public float BarrelRollAcceleration = 1.0f;
    public float BarrelRollDeceleration = 55.0f;
    public float BarrelJumpFullChargeTime = 1.0f;
    public float BarrelFullJumpForce = 100.0f;
    public float BarrelMaxAngularVelocity = 1500.0f;
    public float BarrelMaxXVelocity = 50.0f;
    public float BarrelMinYVelocity = 50.0f;

    [Header("Barrel Swamp Behaviour Setting")]
    public float BarrelSwampAVDeceleration = 100.0f;
    public float BarrelSwampMaxAngularVelocity = 500.0f;
    public float BarrelSwampMaxVeclocityMagnitude = 3.0f;

    [Header("Barrel Snow Behaviour Setting")]
    public float BarrelSnowVelocityDeceleration = 2.0f;
    public float BarrelInSnowLockCoolDown = 0.05f;
    public float BarrelSnowMockAVMultiplier = 0.5f;
    public float BarrelSnowMockAVAnmtSpeedMultiplier = 0.5f;
    public float BarrelSnowMockAVRotationMultiplier = 1.0f;
    public float BarrelSnowMockAVDeleration = 1000.0f;
    public float BarrelSnowAVTranferRate = 0.5f;

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
    public float BarrelPressedMinVelocityDifferent = -5.0f;
    public float BarrelPressedMinPastVelocity = 1.0f;
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
    public float BarrelEmojiClimbMaxYVelocity = 5.0f;
    public float BarrelEmojiClimbMinXVelocity = 1.0f;
    public float BarrelEmojiClimbMinAngularVelocity = 100.0f;
    public float BarrelEmojiHitWallVelocityThreshold = 5.0f;
    public float BarrelEmojiFallVelocityThreshold = -5.0f;
    public float BarrelEmojiInjuredTime = 1.0f;
    public float BarrelEmojiBackNormalTime = 10.0f;
    public float BarrelEmojiBackNormalInWaterTime = 3.0f;
    public float BarrelEmojiOffFallCoolDownTime = 2.0f;

    [Header("Dusts Animation Setting")]
    public float JumpDustYPositionOffset = -1.3f;
    public float JumpDustDiableTimeDelay = 1.1f;
    public float JumpDustSmallJumpT = 0.3f;
    public float JumpDustLargeJumpT = 0.9f;
    public float PoundDustYPositionOffset = 0.5f;

    [Header("Cut Scene Setting")]
    public float SpeechCharacterPerSecond = 2.0f;
    public float SpeechBubbleHeight = 0.4f;
    public float SpeechBubbleWidthBuffer = 0.2f;
    public float SpeechBubbleFinishDelay = 2.0f;
    public Sprite ShipNormalBubbleSprite;
    public Sprite ShipFinishBubbleSprite;
    public Sprite OldManNormalBubbleSprite;
    public Vector3 ShipBarrelPositionOffset = new Vector2(0.0f, -5.0f);
    public float BarrelEndTargetAngularVelocity = -100.0f;
    public float BarrelEndLerpAngularVelocityTime = 1.0f;
    public float BlackScreenTransitionTime = 1.1f;
    public float EndingBlackScreenTransitionTime = 2.0f;
    public float EndingBarrelRollingTime = 2.0f;

    [Header("Screen Setting")]
    public Vector2[] WindowResolution;
    public float BackgroundTransitionPeriod = 0.8f;

    [Header("Audio Setting")]
    public float MusicLerpTime = 1.0f;
    public AudioClip[] MusicClips;
    public float RollSoundMinAngularVelocity = 100.0f;
    public float RollSoundMaxAngularVelocity = 1000.0f;
    public float RollSoundMinDelay = 0.1f;
    public float RollSoundMaxDelay = 1.0f;
    public float BarrelSplashWaterMaxYVelocity = -1.0f;

    [Header("NPCs Setting")]
    public Vector2 NPCEmojiOriginalPositionOffset = Vector2.zero;
    public float NPCEmojiMaxPositionOffset = 1.8f;
    public float NPCLookLerpTime = 1.0f;
    public float NPCCentripetalForce = 10.0f;
    public float NPCTauntHightThershold = 10.0f;
    public float NPCSpeakIdleTime = 20.0f;
    public Vector2 NPCOldManOriginalPosition = Vector2.zero;
    public Vector2 NPCOldManMovedPosition = Vector2.zero;
    public Sprite NPCOldManStandSprite;
    public Sprite NPCOldManRollSprite;

    [Header("Blizzard Setting")]
    public float BlizzardDeactivateDelay = 1.0f;
    public float BlizzardShowBuffer = 0.5f;
    public float BlizzardAcceleration = 10.0f;
    public float BlizzardSwampAccelerationOffset = 0.5f;
    public float BlizzardSwampWallAccelerationOffset = 2.0f;
    public float BlizzardSwampAngleBuffer = 10.0f;
    public float BlizzardWaterAccelerationOffset = 0.5f;

    [Header("Particle Setting")]
    public float WaterParticleMinSpeed = 5.0f;
    public float WaterParticleMaxSpeed = 20.0f;
    public float SwampParticleMinSpeed = 5.0f;
    public float SwampParticleMaxSpeed = 10.0f;
    public float ParticleTriggerMinSpeed = 1.0f;
    public float ParticleTriggerMaxSpeed = 5.0f;
    public float SnowParticleMinSpeed = 10.0f;
    public float SnowParticleMaxSpeed = 30.0f;
    public float SnowParticleMinEmissionRate = 10.0f;
    public float SnowParticleMaxEmissionRate = 50.0f;
    public float SnowTriggerMinAV = 100.0f;
    public float SnowTriggerMaxAV = 1800.0f;
    public float SnowParticleAngleOffset = 45.0f;

    [Header("Background Setting")]
    public float BackgroundLayer2Speed = 1.0f;
    public float BackgroundLayer3Speed = 1.0f;
    public float BackgroundLayer4Speed = 1.0f;
}
