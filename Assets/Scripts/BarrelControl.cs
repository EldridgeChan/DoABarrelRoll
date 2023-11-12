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
    private bool touchedGround = false;
    private bool onGround = false;
    private bool inWater = false;
    private bool emojiTurning = false;
    private float jumpChargeT = 0.0f;
    private Quaternion orgRotation = Quaternion.identity;

    public Rigidbody2D BarrelRig { get { return barrelRig; } }

    private void Start()
    {
        if (!barrelRig) { barrelRig = GetComponent<Rigidbody2D>(); }
        if (!barrelAnmt) { barrelAnmt = GetComponent<Animator>(); }
        BarrelRig.AddForce((GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.BarrelKickForce * Vector2.right, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            onGround = true;
            if (barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelPressedMinVelocity && IsHitGround())
            {
                barrelParentAnmt.SetTrigger("BarrelGrounded");
            }
        }
        if (collision.transform.CompareTag("Water"))
        {
            IntoWater(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Water"))
        {
            IntoWater(false);
        }
        if (collision.transform.CompareTag("Ground"))
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
    }

    private void BarrelUpdate()
    {
        if (GameManager.instance.GameCon.onMenu) { return; }
        barrelRend.flipX = barrelRig.velocity.x < 0;
        barrelRig.angularVelocity = (barrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity), -GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
        barrelAnmt.speed = Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.BarrelRollAnimateSpeedDivisor;
    }

    private void IntoWater(bool tf)
    {
        BarrelRig.gravityScale = tf ? GameManager.instance.GameScriptObj.waterFloatingGravityScale : GameManager.instance.GameScriptObj.waterOffDefaultGravityScale;
        inWater = tf;
    }

    private void WaterFloatAndCurrent()
    {
        if (!inWater) { return; }

        BarrelRig.velocity += (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.waterCurrentAcceleration * Time.fixedDeltaTime * Vector2.right;
        barrelRig.velocity = new Vector2(Mathf.Clamp(barrelRig.velocity.x, -GameManager.instance.GameScriptObj.waterCurrentMaxVelocity, GameManager.instance.GameScriptObj.waterCurrentMaxVelocity), Mathf.Clamp(barrelRig.velocity.y, GameManager.instance.GameScriptObj.waterSinkMaxVelocity, GameManager.instance.GameScriptObj.waterFloatingMaxVelocity));
    }

    private bool IsHitGround()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(barrelRig.position, Vector2.down, GameManager.instance.GameScriptObj.BarrelGroundCheckDistance);
        return hit.Length > 3 && hit[3].transform.CompareTag("Ground");
    }

    public void BarrelRoll(Vector2 prePos, Vector2 nowPos)
    {
        float magnitude = Vector2.Angle(prePos - barrelRig.position, nowPos - barrelRig.position);
        int dir = RotateDir(ToRoundAngle(prePos - barrelRig.position), ToRoundAngle(nowPos - barrelRig.position));
        barrelRig.AddTorque(dir * magnitude * GameManager.instance.GameScriptObj.BarrelRollMultiplier * Mathf.Deg2Rad * barrelRig.inertia);
    }

    public float ToRoundAngle(Vector2 v)
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
            barrelRig.AddForce(GameManager.instance.GameScriptObj.BarrelFullJumpForce * MousePosMagnitudeMultiplier(dir) * -(dir - (Vector2)transform.position).normalized);
            //Debug.Log(MousePosMagnitudeMultiplier(dir));
            jumpChargeT = 1.0f;
            barrelParentAnmt.SetTrigger("BarrelJump");
            BarrelJumpDust(dir);
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
        GameManager.instance.GameCon.OnOffLevelCanvas(true);
        jumpChargeT = 0.0f;
        orgRotation = transform.rotation;
        GameManager.instance.GameCon.onMenu = true;
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
        GameManager.instance.GameCon.OnOffLevelCanvas(false);
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
        GameManager.instance.GameCon.onMenu = false;
    }
}
