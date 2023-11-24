using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Camera Fields")]
    [SerializeField]
    private CameraMovement camMove;

    [Header("Barrel Fields")]
    [SerializeField]
    private BarrelControl barrelControl;
    [SerializeField]
    private BarrelCutSceneBehaviour barrelCSBehave;
    [SerializeField]
    private SpriteRenderer arrowRend;

    [Header("Mirror Level Fields")]
    [SerializeField]
    private Transform[] flipPosTrans;
    [SerializeField]
    private Transform[] flipScaleTrans;
    [SerializeField]
    private Collider2D[] tilemapColids;

    [Header("Menu Function Fields")]
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

    [Header("Cutscene Fields")]
    [SerializeField]
    private TextBubbleBehaviour textBbBehave;
    [SerializeField]
    private PirateShip startPirateShip;
    public PirateShip StartPirateShip { get { return startPirateShip; } }
    [SerializeField]
    private PirateShip endPirateShip;
    public PirateShip EndPirateShip { get { return endPirateShip ; } }
    private SpeechScript currCutScene = SpeechScript.None;

    [Header("Testing Fields")]
    [SerializeField]
    private Transform[] testRespawns;

    [Header("UI Fields")]
    [SerializeField]
    private Animator gameCanvasAnmt;

    void Start()
    {
        GameManager.instance.GameCon = this;
        MirrorWorld(GameManager.instance.SaveMan.mirroredTilemap);
        DisplayGuildingArrow(GameManager.instance.SaveMan.showJumpGuide);
        StartLevelCutScene(SpeechScript.Start0, startPirateShip.transform);
    }

    // Testing----------------------------------------------------------
    public void TeleportCheckpoint(int num)
    {
        if (num <= testRespawns.Length && num > 0)
        {
            barrelControl.transform.position = testRespawns[num - 1].position;
            barrelControl.BarrelRig.velocity = Vector2.zero;
            barrelControl.BarrelRig.angularVelocity = 0;
        }
    }


    // Barrel Behaviour
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

    public Vector3 GetBarrelPosition()
    {
        return barrelControl.transform.position;
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

    //Barrel Adds on
    public void DisplayGuildingArrow(bool tf)
    {
        arrowRend.enabled = tf;
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

    //CutScene
    public void BarrelCameraState(bool dir, CameraState state)
    {
        camMove.SetCurretCamState(state);
        camMove.SetCamLerpDir(dir);
    }

    public void StartLevelCutScene(SpeechScript script, Transform parentTrans)
    {
        currCutScene = script;
        textBbBehave.InitBubble(true, GameManager.instance.SpeechScripObj[(int)currCutScene], parentTrans);
    }

    public void EndLevelCutScene()
    {
        barrelCSBehave.enabled = true;
        StartLevelCutScene(SpeechScript.End0, endPirateShip.transform);
    }

    public void SkipSpeech()
    {
        if (currCutScene < 0) { return; }
        textBbBehave.SkipSpeech();
    }

    public void EndSpeech()
    {
        switch (currCutScene)
        {
            case SpeechScript.Start0:
                EndStartCutScene();
                break;
            case SpeechScript.End0:
                OnEndMenu(true);
                break;
            default:
                Debug.Log("ERROR: Undefined CutSceneIndex");
                break;
        }
        currCutScene = SpeechScript.None;
    }

    private void EndStartCutScene()
    {
        BarrelCameraState(false, CameraState.CutScene);
        isControlLocked = false;
        barrelControl.BarrelRig.AddForce((GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.BarrelKickForce * Vector2.right, ForceMode2D.Impulse);
    }

    public void StartNewGame()
    {
        barrelControl.transform.position = startPirateShip.transform.position + GameManager.instance.GameScriptObj.ShipBarrelPositionOffset;
        StartLevelCutScene(0, startPirateShip.transform);
        OnEndMenu(false);
    }

    //UI
    public void OnPauseMenu(bool tf)
    {
        gameCanvasAnmt.SetBool("ShowPause", tf);
    }

    public void OnEndMenu(bool tf)
    {
        gameCanvasAnmt.SetBool("ShowEnd", tf);
    }

    //Setting
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
}
