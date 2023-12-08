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
    private float gameTimer = 0.0f;

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
    private AudioSource kickAdoScr;
    [SerializeField]
    private PirateShip pirateShip;
    public PirateShip PirateShip { get { return pirateShip; } }
    private SpeechScript currCutScene = SpeechScript.None;

    [Header("Testing Fields")]
    [SerializeField]
    private Transform[] testRespawns;

    [Header("UI Fields")]
    [SerializeField]
    private Animator gameCanvasAnmt;

    [Header("Old Man")]
    [SerializeField]
    private OldManBehaviour oldManBehave;
    [HideInInspector]
    public float barrelHighestY = -100.0f;

    private void Start()
    {
        GameManager.instance.GameCon = this;
        MirrorWorld(GameManager.instance.SaveMan.mirroredTilemap);
        DisplayGuildingArrow(GameManager.instance.SaveMan.showJumpGuide);
        StartLevelCutScene(GameManager.instance.SaveMan.endCounter > 0 ? SpeechScript.Start0 : SpeechScript.Start1, pirateShip.transform);
    }

    private void Update()
    {
        if (!isControlLocked)
        {
            gameTimer += Time.deltaTime;
            GameManager.instance.UIMan.CanCon.UpdateGameTimer(gameTimer);
        }
    }

    // Testing----------------------------------------------------------
    public void TeleportCheckpoint(int num)
    {
        if (num <= testRespawns.Length)
        {
            isControlLocked = false;
            BarrelCameraState(false, CameraState.CutScene);

            barrelControl.transform.position = testRespawns[num].position;
            barrelControl.BarrelRig.velocity = Vector2.zero;
            barrelControl.BarrelRig.angularVelocity = 0;
            textBbBehave.ExitSpeechBubble();
            MovePirateShipToEnd();
            GameManager.instance.AudioMan.SetMusicClip(GameManager.instance.GameScriptObj.MusicClips[(int)MusicClip.Beach]);
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
        else if (GameManager.instance.UIMan.IsSettingOpened)
        {
            GameManager.instance.UIMan.OnOffSetting(false);
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

    public void ActivateRollDust(float spinSpeed)
    {
        rollDustBehaves[rollDustIndex].ActivateRollDust(spinSpeed);
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

    public bool CurrentRollDustFlip()
    {
        return rollDustBehaves[rollDustIndex].IsRollDustFlip();
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

    public void StartOldManCutScene(SpeechScript script, Transform parentTrans)
    {
        currCutScene = script;
        textBbBehave.InitBubble(false, GameManager.instance.SpeechScripObj[(int)currCutScene], parentTrans);
    }

    public void EndLevelCutScene(int endCounter)
    {
        barrelCSBehave.enabled = true;
        isControlLocked = true;
        GameManager.instance.UIMan.CanCon.SetEndTimer(gameTimer);
        StartLevelCutScene(endCounter > 0 ? SpeechScript.End1 : SpeechScript.End0, pirateShip.transform);
    }

    public void SkipSpeech()
    {
        if (currCutScene < 0) { return; }
        textBbBehave.SkipSpeech();
    }

    public void EndSpeech()
    {
        if (currCutScene == SpeechScript.None) { return; }

        if (currCutScene < SpeechScript.End0)
        {
            EndStartCutScene();
        }
        else if (currCutScene < SpeechScript.Old0)
        {
            OnEndMenu(true);
        }
        else
        {
            oldManBehave.EndOfSpeech();
        }
        currCutScene = SpeechScript.None;
    }

    private void EndStartCutScene()
    {
        BarrelCameraState(false, CameraState.CutScene);
        kickAdoScr.Play();
        barrelControl.BarrelRig.AddForce((GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f) * GameManager.instance.GameScriptObj.BarrelKickForce * Vector2.right, ForceMode2D.Impulse);
    }

    public void StartNewGame()
    {
        pirateShip.ToPostion(true);
        barrelControl.transform.position = pirateShip.transform.position + GameManager.instance.GameScriptObj.ShipBarrelPositionOffset;
        gameTimer = 0.0f;
        StartLevelCutScene( SpeechScript.Start1, pirateShip.transform);
        OnEndMenu(false);
    }

    public void MovePirateShipToEnd()
    {
        pirateShip.ToPostion(false);
    }

    //UI
    public void OnPauseMenu(bool tf)
    {
        GameManager.instance.UIMan.CanCon.PlayMenuOnOff();
        gameCanvasAnmt.SetBool("ShowPause", tf);
    }

    public void OnEndMenu(bool tf)
    {
        GameManager.instance.UIMan.CanCon.PlayMenuOnOff();
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
