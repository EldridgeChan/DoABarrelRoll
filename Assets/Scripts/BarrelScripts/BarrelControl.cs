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

    private bool touchedGround = false;
    private bool onGround = false;
    private bool inWater = false;
    private bool emojiTurning = false;
    private bool isRollSoundPlaying = false;
    private float jumpChargeT = 0.0f;
    private float pastAngularVelocity = 0.0f;
    private Vector2 pastVelocity = Vector2.zero;
    private Quaternion orgRotation = Quaternion.identity;

    [HideInInspector]
    public int groundCount = 0;
    [HideInInspector]
    public int swampCount = 0;
    [HideInInspector]
    public Vector2 gravityDirection = Vector2.down;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
        if (collision.CompareTag("Water"))
        {
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
        if (collision.CompareTag("Water"))
        {
            IntoWater(false);
            emojiTypeCon.SetWaterEmoji(false);
        }
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
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
        WaterFloatAndCurrent();
        BarrelEmojiTypeUpdate();
        BarrelRollSound();
        BarrelHitGround();
        BarrelSwampUpdate();

        pastVelocity = barrelRig.velocity;
        pastAngularVelocity = barrelRig.angularVelocity;
        if (BarrelRig.bodyType != RigidbodyType2D.Dynamic) { return; }
        BarrelRig.velocity -= Physics2D.gravity.y * (inWater ? GameManager.instance.GameScriptObj.waterFloatingGravityScale : GameManager.instance.GameScriptObj.waterOffDefaultGravityScale) * Time.fixedDeltaTime * gravityDirection;
    }

    private void BarrelUpdate()
    {
        if (GameManager.instance.GameCon.isControlLocked) { return; }
        barrelRend.flipX = barrelRig.velocity.x < 0;
        barrelRig.angularVelocity = (barrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity), -GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
        barrelAnmt.speed = Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.BarrelRollAnimateSpeedDivisor;
    }

    private void IntoWater(bool tf)
    {
        if (tf && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelSplashWaterMaxYVelocity)
        {
            waterSplashSoundPlayer.PlaySoundManual();
        }
        inWater = tf;
    }

    private void WaterFloatAndCurrent()
    {
        if (!inWater) { return; }

        BarrelRig.velocity += (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.waterCurrentAcceleration * Time.fixedDeltaTime * Vector2.right;
        barrelRig.velocity = new Vector2(Mathf.Clamp(barrelRig.velocity.x, -GameManager.instance.GameScriptObj.waterCurrentMaxVelocity, GameManager.instance.GameScriptObj.waterCurrentMaxVelocity), Mathf.Clamp(barrelRig.velocity.y, GameManager.instance.GameScriptObj.waterSinkMaxVelocity, GameManager.instance.GameScriptObj.waterFloatingMaxVelocity));
    }

    private void BarrelEmojiTypeUpdate()
    {
        emojiTypeCon.StartFastSpining(!inWater && Mathf.Abs(barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold);
        if (!inWater && groundCount <= 0 && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelEmojiFallVelocityThreshold && Mathf.Abs(barrelRig.angularVelocity) <= GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold)
        {
            emojiTypeCon.SetJumpEmoji();
            return;
        }
        if (!inWater && Mathf.Abs(pastVelocity.magnitude - barrelRig.velocity.magnitude) > GameManager.instance.GameScriptObj.BarrelEmojiHitWallVelocityThreshold && pastVelocity.magnitude > barrelRig.velocity.magnitude)
        {
            emojiTypeCon.SetSmashWall();
            return;
        }
        if (!inWater && groundCount > 0 && barrelRig.velocity.y > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinYVelocity && barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelEmojiClimbMaxYVelocity && Mathf.Abs(barrelRig.velocity.x) > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinXVelocity && Mathf.Abs(barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.BarrelEmojiClimbMinAngularVelocity && Mathf.Abs(barrelRig.angularVelocity) < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinAVThreshold && IsHitGround())
        {
            emojiTypeCon.SetClimbing();
        }
    }

    private void BarrelRollSound()
    {
        rollSoundPlayer.SetRepeatTime(Mathf.Lerp(GameManager.instance.GameScriptObj.RollSoundMaxDelay, GameManager.instance.GameScriptObj.RollSoundMinDelay, Mathf.Clamp01(Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.RollSoundMaxAngularVelocity)));
        if (groundCount > 0 && Mathf.Abs(barrelRig.angularVelocity) > GameManager.instance.GameScriptObj.RollSoundMinAngularVelocity)
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
        int dir = RotateDir(ToRoundAngle(prePos - barrelRig.position), ToRoundAngle(nowPos - barrelRig.position));
        barrelRig.AddTorque(dir * magnitude * GameManager.instance.GameScriptObj.BarrelRollMultiplier * Mathf.Deg2Rad * barrelRig.inertia);
    }

    static public float ToRoundAngle(Vector2 v)
    {
        return (360.0f + (v.y < 0 ? -1.0f : 1.0f) * Vector2.Angle(Vector2.right, v)) % 360.0f;
    }

    private int RotateDir(float preAngle, float nowAngle)
    {
        return Mathf.Abs(nowAngle - preAngle) > 180.0f ? -1 : 1 * nowAngle > preAngle ? 1 : -1;
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

    private float MousePosMagnitudeMultiplier(Vector2 mousePos)
    {
        return Mathf.Clamp01((mousePos - barrelRig.position).magnitude / GameManager.instance.GameScriptObj.BarrelJumpMaxDistance);
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
        if (!onGround)
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
        gravityDirection = (collision.GetContact(0).point - BarrelRig.position).normalized;
    }

    private void BarrelSwampUpdate()
    {
        if (swampCount <= 0) { return; }
        barrelRig.angularVelocity = (BarrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity) - GameManager.instance.GameScriptObj.BarrelSwampAVDeceleration * Time.fixedDeltaTime, 0.0f, float.MaxValue);
        BarrelRig.angularVelocity = Mathf.Clamp(BarrelRig.angularVelocity, -GameManager.instance.GameScriptObj.BarrelSwampMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelSwampMaxAngularVelocity);
    }
}
