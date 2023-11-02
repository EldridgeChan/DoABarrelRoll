using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private bool touchedGround = false;
    private float jumpChargeT = 0.0f;
    private bool inWater = false;

    public Rigidbody2D BarrelRig { get { return barrelRig; } }

    private void Start()
    {
        if (!barrelRig) { barrelRig = GetComponent<Rigidbody2D>(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Ground") && 
            barrelRig.velocity.y < GameManager.instance.GameScriptObj.BarrelPressedMinVelocity &&
            isHitGround())
        {
            barrelParentAnmt.SetTrigger("BarrelGrounded");
        }
        if (collision.transform.CompareTag("Water"))
        {
            intoWater(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Water"))
        {
            intoWater(false);
        }
    }

    private void Update()
    {
        barrelParentAnmt.transform.rotation = Quaternion.identity;
        jumpChargeT = Mathf.Clamp01(jumpChargeT - Time.deltaTime / GameManager.instance.GameScriptObj.BarrelJumpFullChargeTime);
    }

    private void FixedUpdate()
    {
        barrelRend.flipX = barrelRig.velocity.x < 0;
        barrelRig.angularVelocity = (barrelRig.angularVelocity < 0 ? -1.0f : 1.0f) * Mathf.Clamp(Mathf.Abs(barrelRig.angularVelocity), -GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity, GameManager.instance.GameScriptObj.BarrelMaxAngularVelocity);
        barrelAnmt.speed = Mathf.Abs(barrelRig.angularVelocity) / GameManager.instance.GameScriptObj.BarrelRollAnimateSpeedDivisor;

        waterFloatAndCurrent();
    }

    private void intoWater(bool tf)
    {
        BarrelRig.gravityScale = tf ? GameManager.instance.GameScriptObj.waterFloatingGravityScale : 1.0f;
        inWater = tf;
    }

    private void waterFloatAndCurrent()
    {
        if (inWater)
        {
            BarrelRig.velocity += Vector2.right * GameManager.instance.GameScriptObj.waterCurrentAcceleration * Time.fixedDeltaTime;
            barrelRig.velocity = new Vector2(Mathf.Clamp(barrelRig.velocity.x, float.MinValue, GameManager.instance.GameScriptObj.waterCurrentMaxVelocity), Mathf.Clamp(barrelRig.velocity.y, GameManager.instance.GameScriptObj.waterSinkMaxVelocity, GameManager.instance.GameScriptObj.waterFloatingMaxVelocity));
        }
    }

    private bool isHitGround()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(barrelRig.position, Vector2.down, GameManager.instance.GameScriptObj.BarrelGroundCheckDistance);
        return hit.Length > 3 && hit[3].transform.CompareTag("Ground");
    }

    public void barrelRoll(Vector2 prePos, Vector2 nowPos)
    {
        float magnitude = Vector2.Angle(prePos - barrelRig.position, nowPos - barrelRig.position);
        int dir = rotateDir(toRoundAngle(prePos - barrelRig.position), toRoundAngle(nowPos - barrelRig.position));
        barrelRig.AddTorque(dir * magnitude * GameManager.instance.GameScriptObj.BarrelRollMultiplier * Mathf.Deg2Rad * barrelRig.inertia);
    }

    public float toRoundAngle(Vector2 v)
    {
        return (360.0f + (v.y < 0 ? -1.0f : 1.0f) * Vector2.Angle(Vector2.right, v)) % 360.0f;
    }

    private int rotateDir(float preAngle, float nowAngle)
    {
        return Mathf.Abs(nowAngle - preAngle) > 180.0f ? -1 : 1 * nowAngle > preAngle ? 1 : -1;
    }

    public void barrelJump(Vector2 dir)
    {
        if (jumpChargeT <= 0.0f && touchedGround)
        {
            barrelRig.AddForce(GameManager.instance.GameScriptObj.BarrelFullJumpForce * mousePosMagnitudeMultiplier(dir) * -(dir - (Vector2)transform.position).normalized);
            jumpChargeT = 1.0f;
            barrelParentAnmt.SetTrigger("BarrelJump");
        }
    }

    private float mousePosMagnitudeMultiplier(Vector2 mousePos)
    {
        return Mathf.Clamp01((mousePos - barrelRig.position).magnitude / GameManager.instance.GameScriptObj.BarrelJumpMaxDistance);
    }

    public void setTouchedGround(bool tF)
    {
        touchedGround = tF;
    }
}
