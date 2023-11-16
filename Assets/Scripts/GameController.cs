using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField]
    private BarrelControl barrelControl;
    [SerializeField]
    private SpriteRenderer arrowRend;
    [SerializeField]
    private Canvas LevelCanvas;
    [SerializeField]
    private CameraMovement camMove;

    [Header("Mirror Level References")]
    [SerializeField]
    private Transform[] flipPosTrans;
    [SerializeField]
    private Transform[] flipScaleTrans;
    [SerializeField]
    private Collider2D[] tilemapColids;

    [Header("Menu Function Fields")]
    [HideInInspector]
    public bool isControlLocked = false;
    private bool canOnOff = true;

    [Header("Jump Dust Fields")]
    [SerializeField]
    private JumpDustBehaviour[] jumpDustBehaves;
    private int jumpDustIndex = 0;

    [Header("Roll Dust Fields")]
    [SerializeField]
    private RollDustBehaviour[] rollDustBehaves;
    private int rollDustIndex = 0;

    [Header("Pound Dust Fields")]
    [SerializeField]
    private Animator poundDustAnmt;

    [Header("Testing Fields")]
    [SerializeField]
    private Transform[] testRespawns;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.GameCon = this;
        MirrorWorld(GameManager.instance.SaveMan.mirroredTilemap);
        DisplayGuildingArrow(GameManager.instance.SaveMan.showJumpGuide);

    }

    public void DisplayGuildingArrow(bool tf)
    {
        arrowRend.enabled = tf;
    }

    public void TeleportCheckpoint(int num)
    {
        if (num <= testRespawns.Length && num > 0)
        {
            barrelControl.transform.position = testRespawns[num - 1].position;
            barrelControl.BarrelRig.velocity = Vector2.zero;
            barrelControl.BarrelRig.angularVelocity = 0;
        }
    }

    public void TryBarrelStand()
    {
        if (!canOnOff) { return; }
        canOnOff = false;
        Invoke(nameof(EnableStandFall), GameManager.instance.GameScriptObj.BarrelStandFallCoolDown);

        if (!isControlLocked)
        {
            barrelControl.ControlBarrelStand();
        }
        else
        {
            barrelControl.BarrelFall();
        }
    }

    public void EnableStandFall()
    {
        canOnOff = true;
    }


    public void BarrelJump(Vector2 dir)
    {
        if (isControlLocked) { return; }
        barrelControl.BarrelJump(dir);
    }

    public void BarrelRoll(Vector2 prePos, Vector2 nowPos)
    {
        if (isControlLocked) { return; }
        barrelControl.BarrelRoll(prePos, nowPos);
    }

    public Vector3 getBarrelPosition()
    {
        return barrelControl.transform.position;
    }

    private void MirrorWorld(bool tf)
    {
        foreach (Transform trans in flipPosTrans)
        {
            if (trans)
            {
                trans.position = new Vector3(trans.position.x * (tf ? -1.0f : 1.0f), trans.position.y, trans.position.z);
            }
        }
        foreach (Transform trans in flipScaleTrans)
        {
            if (trans)
            {
                trans.localScale = new Vector3(trans.localScale.x * (tf ? -1.0f : 1.0f) ,trans.localScale.y, trans.localScale.z);
            }
        }
        foreach (Collider2D colid in tilemapColids)
        {
            colid.enabled = true;
        }
    }

    public void OnOffLevelCanvas(bool tf)
    {
        LevelCanvas.gameObject.SetActive(tf);
    }

    public void DisplayJumpDust(int jumpLevel)
    {
        jumpDustBehaves[jumpDustIndex].DisplayJumpDust(barrelControl, GameManager.instance.InputMan.MouseWorldPos(), jumpLevel);
        jumpDustIndex = (jumpDustIndex + 1) % jumpDustBehaves.Length;
    }

    public void ActivateRollDust(ContactPoint2D contact, float spinSpeed)
    {
        rollDustBehaves[rollDustIndex].ActivateRollDust(contact, spinSpeed);
    }

    public void RollDustActive(ContactPoint2D contact, float spinSpeed)
    {
        rollDustBehaves[rollDustIndex].RollDustActive(contact, spinSpeed);
    }

    public void DeactivateRollDust()
    {
        rollDustBehaves[rollDustIndex].DeactivateRollDust();
        rollDustIndex = (rollDustIndex + 1) % rollDustBehaves.Length;
    }

    public void GroundPoundDust(Vector2 pos)
    {
        poundDustAnmt.transform.position = pos + Vector2.down * GameManager.instance.GameScriptObj.PoundDustYPositionOffset;
        poundDustAnmt.SetTrigger("PoundDust");
    }

    public void BarrelCameraState(bool dir, CameraState state)
    {
        camMove.SetCurretCamState(state);
        camMove.SetCamLerpDir(dir);
    }
}
