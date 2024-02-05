using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BarrelControl : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D barrelRig;
    [SerializeField]
    private Animator barrelAnmt;
    [SerializeField]
    private Animator barrelParentAnmt;
    [SerializeField]
    private SpriteRenderer barrelRend;
    [SerializeField]
    private EmojiBehaviour emojiBehave;
    [SerializeField]
    private Animator barrelErrorTextAnmt;
    [SerializeField]
    private Transform barrelErrorTextCanvasTrans;
    [SerializeField]
    private TMP_Text barrelErrorText;
    [SerializeField]
    private EmojiTypeController emojiTypeCon;

    [SerializeField]
    private SoundsPlayer waterSplashSoundPlayer;
    [SerializeField]
    private SoundsPlayer rollSoundPlayer;
    [SerializeField]
    private SoundsPlayer jumpSoundPlayer;
    [SerializeField]
    private SoundsPlayer smashSoundPlayer;
    [SerializeField]
    private SoundsPlayer mudSplashSoundPlayer;
    [SerializeField]
    private SoundsPlayer snowSoundPlayer;

    private bool touchedGround = false;
    private bool inWater = false;
    private bool inWaterFlow = false;
    private bool emojiTurning = false;
    private bool isRollSoundPlaying = false;
    private float jumpChargeT = 0.0f;
    private float pastAngularVelocity = 0.0f;
    private int pastRotateDir = 0;
    private Vector2 pastVelocity = Vector2.zero;
    private Quaternion orgRotation = Quaternion.identity;
    private float mockAngularVelocity = 0.0f;
    public float MockAngularVelocity { get { return mockAngularVelocity; } }

    [HideInInspector]
    public int touchingGroundNum = 0;
    [HideInInspector]
    public int GroundCount = 0;
    [HideInInspector]
    public int SwampCount = 0;
    [HideInInspector]
    public int SnowCount = 0;
    [HideInInspector]
    public Vector2 GravityDirection = Vector2.down;
    [HideInInspector]
    public bool InSnowLock = false;

    public Rigidbody2D BarrelRig { get { return barrelRig; } }
    public EmojiTypeController EmojiTypeCon { get { return emojiTypeCon; } }

    private void Start()
    {
        if (!barrelRig) { barrelRig = GetComponent<Rigidbody2D>(); }
        if (!barrelAnmt) { barrelAnmt = GetComponent<Animator>(); }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GroundCollisionStay(collision);
        SwampCollisionStay(collision);
        SnowCollisionStay(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            GameManager.instance.GameCon.ActivateWaterParticle(collision.ClosestPoint(transform.position));
            IntoWater(true);
        }
        if (collision.CompareTag("FallCheck"))
        {
            emojiTypeCon.FallBackDown();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            emojiTypeCon.SetWaterEmoji(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision) { return; }
        if (collision.CompareTag("Water"))
        {
            IntoWater(false);
            emojiTypeCon.SetWaterEmoji(false);
        }

    }

    private void Update()
    {
        barrelParentAnmt.transform.rotation = Quaternion.identity;
        barrelErrorTextCanvasTrans.SetPositionAndRotation(transform.position + Vector3.up * 1.2f, Quaternion.identity);
        if (!emojiTurning)
        {
            jumpChargeT = Mathf.Clamp01(jumpChargeT - Time.deltaTime / GameManager.instance.GameScriptObj.BarrelJumpFullChargeTime);
        }
        else
        {
            jumpChargeT = Mathf.Clamp01(jumpChargeT + Time.deltaTime / GameManager.instance.GameScriptObj.BarrelEmojiTurnDuration);
            transform.rotation = Quaternion.Lerp(orgRotation, Quaternion.identity, jumpChargeT);
            if (jumpChargeT >= 1.0f)
            {
                emojiTurning = false;
            }
        }
    }

    private void FixedUpdate()
    {
        BarrelUpdate();
        WaterFloat();
        WaterCurrent();
        BarrelEmojiTypeUpdate();
        BarrelRollSound();
        BarrelHitGround();
        BarrelSwampUpdate();
        BarrelSnowUpdate();

        pastVelocity = barrelRig.velocity;
        pastAngularVelocity = barrelRig.angularVelocity;
    }

    private void BarrelUpdate()
    {
        if (GameManager.instance.GameCon.isControlLocked) { return; }
        barrelRend.flipX = barrelRig.velocity.x < 0;
        barrelRig.angularVelocity = (barrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity), -GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
        barrelAnmt.speed = Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.BarrelRollAnimateSpeedDivisor;
        barrelRig.velocity = new Vector2(barrelRig.velocity.x, Mathf.Clamp(barrelRig.velocity.y, GameManager.instance.GameScriptObj.BarrelMinYVelocity, float.MaxValue));
        BarrelRig.velocity -= Physics2D.gravity.y * (inWater ? GameManager.instance.GameScriptObj.waterFloatingGravityScale : GameManager.instance.GameScriptObj.waterOffDefaultGravityScale) * Time.fixedDeltaTime * GravityDirection;
    }

    public void IntoWater(bool tf)
    {
        if (tf && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelSplashWaterMaxYVelocity)
        {
            waterSplashSoundPlayer.PlaySoundManual();
        }
        inWater = tf;
    }

    public void IntoMud()
    {
        mudSplashSoundPlayer.PlaySoundManual();
    }

    private void WaterFloat()
    {
        if (!inWater) { return; }
        barrelRig.velocity = new Vector2(Mathf.Clamp(barrelRig.velocity.x, -GameManager.instance.GameScriptObj.waterCurrentMaxVelocity, GameManager.instance.GameScriptObj.waterCurrentMaxVelocity), Mathf.Clamp(barrelRig.velocity.y, GameManager.instance.GameScriptObj.waterSinkMaxVelocity, GameManager.instance.GameScriptObj.waterFloatingMaxVelocity));
    }

    public void SetInWaterCurrent(bool tf)
    {
        inWaterFlow = tf;
    }

    private void WaterCurrent()
    {
        if (!inWaterFlow) { return; }
        BarrelRig.velocity += (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.waterCurrentAcceleration * Time.fixedDeltaTime * Vector2.right;
    }

    private void BarrelEmojiTypeUpdate()
    {
        emojiTypeCon.StartFastSpining(!inWater && Mathf.Abs(barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold);
        if (!inWater && GroundCount <= 0 && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelEmojiFallVelocityThreshold && Mathf.Abs(barrelRig.angularVelocity) <= GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold)
        {
            emojiTypeCon.SetJumpEmoji();
            return;
        }
        if (!inWater && Mathf.Abs(pastVelocity.magnitude - barrelRig.velocity.magnitude) > GameManager.instance.GameScriptObj.BarrelEmojiHitWallVelocityThreshold && pastVelocity.magnitude > barrelRig.velocity.magnitude)
        {
            emojiTypeCon.SetSmashWall();
            return;
        }
        if (!inWater && GroundCount > 0 && barrelRig.velocity.y > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinYVelocity && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelEmojiClimbMaxYVelocity && Mathf.Abs(barrelRig.velocity.x) > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinXVelocity && Mathf.Abs(barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinAngularVelocity && Mathf.Abs(barrelRig.angularVelocity) < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold && IsHitGround())
        {
            emojiTypeCon.SetClimbing();
        }
    }

    private void BarrelRollSound()
    {
        rollSoundPlayer.SetRepeatTime(Mathf.Lerp(GameManager.instance.GameScriptObj.RollSoundMaxDelay, GameManager.instance.GameScriptObj.RollSoundMinDelay, Mathf.Clamp01(Mathf.Abs(InSnowLock ? mockAngularVelocity : barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.RollSoundMaxAngularVelocity)));
        if (GroundCount > 0 && Mathf.Abs(InSnowLock ? mockAngularVelocity : barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.RollSoundMinAngularVelocity)
        {            
            if (!isRollSoundPlaying)
            {
                isRollSoundPlaying = true;
                rollSoundPlayer.PlaySoundAuto();
            }
        }
        else
        {
            if (isRollSoundPlaying)
            {
                isRollSoundPlaying = false;
                rollSoundPlayer.StopRepeat();
            }
        }
    }

    private void BarrelHitGround()
    {
        if (pastVelocity.y < GameManager.instance.GameScriptObj.BarrelPressedMinPastVelocity && pastVelocity.y - barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelPressedMinVelocityDifferent && IsHitGround())
        {
            barrelParentAnmt.SetTrigger("BarrelGrounded");
            smashSoundPlayer.PlaySoundManual();
            GameManager.instance.GameCon.GroundPoundDust(transform.position);
        }
    }

    private bool IsHitGround()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(barrelRig.position, Vector2.down, GameManager.instance.GameScriptObj.BarrelGroundCheckDistance);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }

    public void BarrelRoll(Vector2 prePos, Vector2 nowPos)
    {
        float magnitude = Vector2.Angle(prePos - barrelRig.position, nowPos - barrelRig.position);
        pastRotateDir = RotateDir(ToRoundAngle(prePos - barrelRig.position), ToRoundAngle(nowPos - barrelRig.position));
        if (!InSnowLock)
        {
            barrelRig.AddTorque(pastRotateDir * magnitude * (BarrelRig.angularVelocity * pastRotateDir > 0.0f ? GameManager.instance.GameScriptObj.BarrelRollAcceleration : GameManager.instance.GameScriptObj.BarrelRollDeceleration) * Mathf.Deg2Rad * barrelRig.inertia);
        }
        else
        {
            mockAngularVelocity += pastRotateDir * magnitude * GameManager.instance.GameScriptObj.BarrelSnowMockAVMultiplier;
        }
    }

    static public float ToRoundAngle(Vector2 v)
    {
        return (360.0f + (v.y < 0 ? -1.0f : 1.0f) * Vector2.Angle(Vector2.right, v)) % 360.0f;
    }

    private int RotateDir(float preAngle, float nowAngle)
    {
        return (Mathf.Abs(nowAngle - preAngle) > 180.0f ? -1 : 1) * (nowAngle > preAngle ? 1 : -1);
    }

    public void BarrelJump(Vector2 dir)
    {
        if (jumpChargeT <= 0.0f && touchedGround)
        {
            jumpSoundPlayer.PlaySoundManual();
            barrelRig.AddForce(GameManager.instance.GameScriptObj.BarrelFullJumpForce * MousePosMagnitudeMultiplier(dir) * -(dir - (Vector2)transform.position).normalized);
            jumpChargeT = 1.0f;
            barrelParentAnmt.SetTrigger("BarrelJump");
            BarrelJumpDust(dir);
            emojiTypeCon.SetJumpEmoji();
            SnowJump();
        }
    }

    private void BarrelJumpDust(Vector2 dir)
    {
        if (!GameManager.instance.GameCon) 
        {
            Debug.Log("ERROR: Game Controller Null Exception");
            return; 
        }
        GameManager.instance.GameCon.DisplayJumpDust(BarrelJumpLevel(MousePosMagnitudeMultiplier(dir)));
    }

    private int BarrelJumpLevel(float jumpT)
    {
        if (jumpT < GameManager.instance.GameScriptObj.JumpDustSmallJumpT)
        {
            return 0;
        }
        else if (jumpT < GameManager.instance.GameScriptObj.JumpDustLargeJumpT)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public float MousePosMagnitudeMultiplier(Vector2 mousePos)
    {
        return Mathf.Clamp01((mousePos - barrelRig.position).magnitude / (1.0f / GameManager.instance.SaveMan.jumpSensibility));
    }

    public void SetTouchedGround(bool tF)
    {
        touchedGround = tF;
    }

    public void ControlBarrelStand()
    {
        int errorCode = BarrelStandErrorCode();
        if (errorCode != -1)
        {
            barrelErrorText.text = GameManager.instance.GameScriptObj.BarrelErrorTexts[errorCode];
            barrelErrorTextAnmt.SetTrigger("ShowText");
            return;
        }
        GameManager.instance.GameCon.SetSwampDecorationActive(false);
        GameManager.instance.GameCon.OnPauseMenu(true);
        jumpChargeT = 0.0f;
        orgRotation = transform.rotation;
        GameManager.instance.GameCon.isControlLocked = true;
        GameManager.instance.GameCon.BarrelCameraState(true, CameraState.Menu);
        BarrelRig.bodyType = RigidbodyType2D.Kinematic;
        BarrelRig.velocity = Vector2.zero;
        BarrelRig.angularVelocity = 0.0f;
        emojiTurning = true;
        Invoke(nameof(BarrelStand), GameManager.instance.GameScriptObj.BarrelEmojiTurnDuration);
    }

    public void BarrelStand()
    {
        barrelAnmt.speed = 1.0f;
        emojiBehave.BarrelEmojiStand();
        barrelAnmt.SetTrigger("BarrelStand");
    }

    private int BarrelStandErrorCode()
    {
        if (!IsOnSolidGround(Physics2D.RaycastAll(transform.position, Vector2.down, GameManager.instance.GameScriptObj.BarrelGroundCheckDistance)))
        {
            return 0;
        }
        if (BarrelRig.velocity.magnitude > GameManager.instance.GameScriptObj.BarrelStandMaxVelocity)
        {
            return 1;
        }
        if (Mathf.Abs(BarrelRig.angularVelocity) > GameManager.instance.GameScriptObj.BarrelStandMaxAVelocity)
        {
            return 2;
        }
        return -1;
    }

    private bool IsOnSolidGround(RaycastHit2D[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.isTrigger && hits[i].transform.CompareTag("Ground"))
            {
                return true;
            }
        }
        return false;
    }

    public void BarrelFall()
    {
        if (BarrelRig.bodyType == RigidbodyType2D.Dynamic) { return; }
        GameManager.instance.GameCon.OnPauseMenu(false);
        barrelAnmt.SetTrigger("BarrelFall");
        emojiBehave.BarrelEmojiFall();
        Invoke(nameof(BarrelGainControl), GameManager.instance.GameScriptObj.BarrelFallGainControlTime);
    }

    public void BarrelGainControl()
    {
        BarrelRig.bodyType = RigidbodyType2D.Dynamic;
        BarrelRig.velocity = Vector2.zero;
        BarrelRig.angularVelocity = 0.0f;
        barrelAnmt.speed = 0.0f;
        GameManager.instance.GameCon.isControlLocked = false;
        GameManager.instance.GameCon.BarrelCameraState(false, CameraState.Menu);
        GameManager.instance.GameCon.SetSwampDecorationActive(true);
    }

    public void CheckSetEmojiNormal()
    {
        if (!inWater && barrelRig.angularVelocity <= GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold)
        {
            emojiTypeCon.SetNormal();
        }
    }

    private void GroundCollisionStay(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Ground")) { return; }
        GameManager.instance.GameCon.RollDustActive(collision.GetContact(0), barrelRig.angularVelocity);
        if (GameManager.instance.GameCon.CurrentRollDustFlip() != barrelRig.angularVelocity > 0)
        {
            GameManager.instance.GameCon.DeactivateRollDust();
            GameManager.instance.GameCon.ActivateRollDust(barrelRig.angularVelocity);
        }
    }

    private void SwampCollisionStay(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Swamp")) { return; }
        GravityDirection = (collision.GetContact(collision.contactCount <= 1 ? 0 : pastRotateDir * RotateDir(ToRoundAngle(collision.GetContact(0).point - barrelRig.position), ToRoundAngle(collision.GetContact(1).point - barrelRig.position)) < 0 ? 1 : 0).point - BarrelRig.position).normalized;
    }

    private void BarrelSwampUpdate()
    {
        if (SwampCount <= 0) { return; }
        barrelRig.angularVelocity = (BarrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity) - GameManager.instance.GameScriptObj.BarrelSwampAVDeceleration * Time.fixedDeltaTime, 0.0f, float.MaxValue);
        BarrelRig.angularVelocity = Mathf.Clamp(BarrelRig.angularVelocity, -GameManager.instance.GameScriptObj.BarrelSwampMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelSwampMaxAngularVelocity);
    }

    private void SnowCollisionStay(Collision2D collision)
    {
        if (SnowCount <= 0 || !InSnowLock || !collision.transform.CompareTag("Ground")) { return; }
        //GravityDirection = (collision.GetContact(0).point - BarrelRig.position).normalized;
        BarrelRig.velocity = Mathf.Clamp(BarrelRig.velocity.magnitude - GameManager.instance.GameScriptObj.BarrelSnowVelocityDeceleration * Time.fixedDeltaTime, 0.0f, float.MaxValue) * BarrelRig.velocity.normalized;
    }

    public void IntoSnowLock()
    {
        if (SnowCount <= 0) { return; }
        InSnowLock = true;
    }

    private void SnowJump()
    {
        if (!InSnowLock) { return; }
        GameManager.instance.GameCon.DeactivateSnowParticle();
        GravityDirection = Vector2.down;
        InSnowLock = false;
        BarrelRig.angularVelocity = mockAngularVelocity;
        mockAngularVelocity = 0.0f;
        Invoke(nameof(IntoSnowLock), GameManager.instance.GameScriptObj.BarrelInSnowLockCoolDown);
    }

    private void BarrelSnowUpdate()
    {
        if (!InSnowLock || GameManager.instance.GameCon.isControlLocked) { return; }  
        barrelRend.flipX = mockAngularVelocity < 0;
        transform.Rotate(0.0f, 0.0f, mockAngularVelocity * GameManager.instance.GameScriptObj.BarrelSnowMockAVRotationMultiplier * Time.fixedDeltaTime);
        mockAngularVelocity = Mathf.Clamp(mockAngularVelocity, -GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
        barrelAnmt.speed = Mathf.Abs(mockAngularVelocity) * GameManager.instance.GameScriptObj.BarrelSnowMockAVAnmtSpeedMultiplier;
        mockAngularVelocity = (mockAngularVelocity < 0 ? -1.0f : 1.0f) *  Mathf.Clamp(Mathf.Abs(mockAngularVelocity) - GameManager.instance.GameScriptObj.BarrelSnowMockAVDeleration * Time.fixedDeltaTime, 0.0f, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
    }

    public void BarrelBlizzard(int dir)
    {
        if (InSnowLock || BarrelRig.bodyType != RigidbodyType2D.Dynamic) { return; }
        BarrelRig.velocity += dir * GameManager.instance.GameScriptObj.BlizzardAcceleration * (Vector2.Angle(Vector2.right * dir, GravityDirection) < (180.0f - GameManager.instance.GameScriptObj.BlizzardSwampAngleBuffer) ? GameManager.instance.GameScriptObj.BlizzardSwampAccelerationOffset : (inWater ? GameManager.instance.GameScriptObj.BlizzardWaterAccelerationOffset : 1.0f)) * Time.fixedDeltaTime * Vector2.right;
    }

    public void PlaySnowSound()
    {
        snowSoundPlayer.PlaySoundManual();
    }

    public void TeleportReset()
    {
        BarrelRig.velocity = Vector2.zero;
        BarrelRig.angularVelocity = 0;
        IntoWater(false);
        SwampCount = 0;
        GroundCount = 0;
        touchingGroundNum = 0;
        SnowCount = 0;
        InSnowLock = false;
        inWaterFlow = false;
        GravityDirection = Vector2.down;
    }
}
