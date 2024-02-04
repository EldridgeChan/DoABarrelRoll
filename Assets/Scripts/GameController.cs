using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Camera Fields")]
    [SerializeField]
    private CameraMovement camMove;
    [SerializeField]
    private Animator backgroundAnmt;

    [Header("Barrel Fields")]
    [SerializeField]
    private BarrelControl barrelControl;
    [SerializeField]
    private BarrelCutSceneBehaviour barrelCSBehave;
    [SerializeField]
    private SpriteRenderer arrowRend;

    [Header("Level Fields")]
    [SerializeField]
    private Transform[] flipPosTrans;
    [SerializeField]
    private Transform[] flipScaleTrans;
    [SerializeField]
    private Collider2D[] tilemapColids;
    [SerializeField]
    private GameObject[] tilemapParents;
    public GameObject[] TilemapParents { get { return tilemapParents; } }
    [HideInInspector]
    public LevelArea CurrentArea = LevelArea.Beach;

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
    private PirateShip startPirateShip;
    public PirateShip StartPirateShip { get { return startPirateShip; } }
    [SerializeField]
    private PirateShip endPirateShip;
    public PirateShip EndPirateShip { get { return endPirateShip; } }
    private SpeechScript currCutScene = SpeechScript.None;

    [Header("Testing Fields")]
    [SerializeField]
    private Transform[] testRespawns;

    [Header("UI Fields")]
    [SerializeField]
    private Animator gameCanvasAnmt;
    [SerializeField]
    private BlizzardImageController blizzardImageCon;
    public BlizzardImageController BlizzardImageCon { get { return blizzardImageCon; } }

    [Header("Old Man")]
    [SerializeField]
    private OldManBehaviour oldManBehave;
    [HideInInspector]
    public float barrelHighestY = -100.0f;

    [Header ("Particle System")]
    [SerializeField]
    private Animator SnowFlakeParentAnmt;
    [SerializeField]
    private ParticlesController waterParticle;
    [SerializeField]
    private ParticlesController swampParticle;
    [SerializeField]
    private ParticlesController snowParticle;

    [Header("Sound")]
    [SerializeField]
    private GameObject beachSoundParent;
    [SerializeField]
    private GameObject jungleSoundParent;
    [SerializeField]
    private GameObject snowSoundParent;
    [SerializeField]
    private Animator blizzardSoundAnmt;

    private void Start()
    {
        GameManager.instance.GameCon = this;
        MirrorWorld(GameManager.instance.SaveMan.mirroredTilemap);
        DisplayGuildingArrow(GameManager.instance.SaveMan.showJumpGuide);
        backgroundAnmt.speed = 1.0f / GameManager.instance.GameScriptObj.BackgroundTransitionPeriod;
        if (GameManager.instance.SaveMan.playerSaveArea == LevelArea.MainMenu)
        {
            StartCutScene(GameManager.instance.SaveMan.endCounter <= 0 ? SpeechScript.Start0 : SpeechScript.Start1, startPirateShip.transform);
        }
        else
        {
            LoadingPlayerProgress();
            barrelControl.Invoke(nameof(barrelControl.BarrelGainControl), GameManager.instance.GameScriptObj.BarrelFallGainControlTime);
            BarrelCameraState(false, CameraState.CutScene);
        }
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
            barrelControl.teleportReset();
            isControlLocked = false;
            BarrelCameraState(false, CameraState.CutScene);
            barrelControl.BarrelRig.position = testRespawns[num].position;
            textBbBehave.ExitSpeechBubble();
            TeleportLevelArea(num);
            GameManager.instance.AudioMan.BGMTransition(GameManager.instance.GameScriptObj.MusicClips[(int)CurrentArea]);
            CancelInvoke(nameof(DeactivateSnowParent));
            BackgroundTransition((int)CurrentArea);
            LevelSoundTransition(CurrentArea);
            TilemapParents[0].SetActive(num < 2);
            TilemapParents[1].SetActive(num >= 2 && num <= 3);
            TilemapParents[2].SetActive(num >= 3 && num <= 9);
            TilemapParents[3].SetActive(true);
        }
    }

    private void TeleportLevelArea(int num)
    {
        if (num < 2)
        {
            CurrentArea = LevelArea.Beach;
        }
        else if (num < 3)
        {
            CurrentArea = LevelArea.Jungle;
        }
        else if (num <= 9)
        {
            CurrentArea = LevelArea.SnowMountain;
        }
        else
        {
            CurrentArea = LevelArea.GlitchLand;
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

    public void BarrelBlizzard(int dir)
    {
        barrelControl.BarrelBlizzard(dir);
    }

    public void SetBarrelInWaterCurrent(bool tf)
    {
        barrelControl.SetInWaterCurrent(tf);
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

    public void StartCutScene(SpeechScript script, Transform parentTrans, bool isShip = true)
    {
        currCutScene = script;
        textBbBehave.InitBubble(isShip, GameManager.instance.SpeechScripObj[(int)currCutScene], parentTrans);
    }

    public void EndLevelCutScene(int endCounter)
    {
        barrelCSBehave.enabled = true;
        barrelControl.GravityDirection = Vector2.down;
        isControlLocked = true;
        GameManager.instance.UIMan.CanCon.SetEndTimer(gameTimer);
        StartCutScene(endCounter > 0 ? SpeechScript.End1 : SpeechScript.End0, endPirateShip.transform);
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
        OnlyActivateBeach();
        BackgroundTransition((int)LevelArea.Beach);
        GameManager.instance.AudioMan.BGMTransition(GameManager.instance.GameScriptObj.MusicClips[(int)LevelArea.Beach]);
        barrelControl.transform.position = startPirateShip.transform.position + GameManager.instance.GameScriptObj.ShipBarrelPositionOffset;
        gameTimer = 0.0f;
        oldManBehave.resetOldMan();
        barrelHighestY = -100.0f;
        StartCutScene(SpeechScript.Start1, startPirateShip.transform);
        OnEndMenu(false);

        backgroundAnmt.SetInteger("LevelArea", 1);
        for (int i = 0; i < TilemapParents.Length; i++)
        {
            TilemapParents[i].SetActive(i == 0);
        }
    }

    private void OnlyActivateBeach()
    {
        for (int i = 0; i < TilemapParents.Length; i++)
        {
            TilemapParents[i].SetActive(i == 0);
        }
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

    public void BackgroundTransition(int level)
    {
        if (!backgroundAnmt) { return; }
        backgroundAnmt.SetInteger("LevelArea", level);
        CurrentArea = (LevelArea)level;

        if (level == (int)LevelArea.SnowMountain)
        {
            StartSnowing();
        }
        else
        {
            StopSnowing();
        }
    }

    //Snowing
    public void StartSnowing()
    {
        if (!SnowFlakeParentAnmt) { return; }
        SnowFlakeParentAnmt.gameObject.SetActive(true);
        SnowFlakeParentAnmt.SetBool("IsSnowing", true);
    }

    public void StopSnowing()
    {
        if (!SnowFlakeParentAnmt) { return; }
        SnowFlakeParentAnmt.SetBool("IsSnowing", false);
        Invoke(nameof(DeactivateSnowParent), GameManager.instance.GameScriptObj.BlizzardDeactivateDelay);
    }

    private void DeactivateSnowParent()
    {
        if (!SnowFlakeParentAnmt) { return; }
        SnowFlakeParentAnmt.gameObject.SetActive(false);
    }

    public void SetBlizzardDirection(int dir)
    {
        if (!SnowFlakeParentAnmt) { return; }
        SnowFlakeParentAnmt.SetInteger("BlizzardDir", dir);
    }

    //Particles
    public void ActivateWaterParticle(Vector2 pos)
    {
        waterParticle.transform.position = pos;
        waterParticle.SetParticleStartSpeed(Mathf.Lerp(GameManager.instance.GameScriptObj.WaterParticleMinSpeed, GameManager.instance.GameScriptObj.WaterParticleMaxSpeed, BarrelParticleTriggerT()));
        waterParticle.SetParticleAngle(ParticleRotation(pos));
        waterParticle.StartParticle();
    }

    public void ActivateSwampParticle(Vector2 pos)
    {
        swampParticle.transform.position = pos;
        swampParticle.SetParticleStartSpeed(Mathf.Lerp(GameManager.instance.GameScriptObj.SwampParticleMinSpeed, GameManager.instance.GameScriptObj.SwampParticleMaxSpeed, BarrelParticleTriggerT()));
        swampParticle.SetParticleAngle(ParticleRotation(pos));
        swampParticle.StartParticle();
    }

    private float BarrelParticleTriggerT()
    {
        return (Mathf.Clamp(barrelControl.BarrelRig.velocity.magnitude, GameManager.instance.GameScriptObj.ParticleTriggerMinSpeed, GameManager.instance.GameScriptObj.ParticleTriggerMaxSpeed) - GameManager.instance.GameScriptObj.ParticleTriggerMinSpeed) / (GameManager.instance.GameScriptObj.ParticleTriggerMaxSpeed - GameManager.instance.GameScriptObj.ParticleTriggerMinSpeed);
    }

    private float ParticleRotation(Vector2 pos)
    {
        
        return (barrelControl.transform.position.x > pos.x ? 1 : -1) * Vector2.Angle(Vector2.up, (Vector2)barrelControl.transform.position - pos);
    }

    public void ActivateSnowParticle(Vector2 pos)
    {
        snowParticle.transform.position = pos;
        snowParticle.SetParticleEmissionRate(Mathf.Lerp(GameManager.instance.GameScriptObj.SnowParticleMinEmissionRate, GameManager.instance.GameScriptObj.SnowParticleMaxEmissionRate, (Mathf.Clamp(Mathf.Abs(barrelControl.MockAngularVelocity), GameManager.instance.GameScriptObj.SnowTriggerMinAV, GameManager.instance.GameScriptObj.SnowTriggerMaxAV) - GameManager.instance.GameScriptObj.SnowTriggerMinAV) / (GameManager.instance.GameScriptObj.SnowTriggerMaxAV - GameManager.instance.GameScriptObj.SnowTriggerMinAV)));
        snowParticle.SetParticleStartSpeed(Mathf.Lerp(GameManager.instance.GameScriptObj.SnowParticleMinSpeed, GameManager.instance.GameScriptObj.SnowParticleMaxSpeed, (Mathf.Clamp(Mathf.Abs(barrelControl.MockAngularVelocity), GameManager.instance.GameScriptObj.SnowTriggerMinAV, GameManager.instance.GameScriptObj.SnowTriggerMaxAV) - GameManager.instance.GameScriptObj.SnowTriggerMinAV) / (GameManager.instance.GameScriptObj.SnowTriggerMaxAV - GameManager.instance.GameScriptObj.SnowTriggerMinAV)));
        snowParticle.SetParticleAngle(ParticleRotation(pos) + (barrelControl.MockAngularVelocity < 0 ? -1 : 1) * GameManager.instance.GameScriptObj.SnowParticleAngleOffset);
        if (!snowParticle.isPartSysPlaying())
        {
            snowParticle.StartParticle();
        }
    }

    public void DeactivateSnowParticle()
    {
        snowParticle.StopParticle();
    }

    //Sound
    public void LevelSoundTransition(LevelArea level)
    {
        beachSoundParent.SetActive(level == LevelArea.Beach);
        jungleSoundParent.SetActive(level == LevelArea.Jungle);
        snowSoundParent.SetActive(level == LevelArea.SnowMountain);
    }

    public void SetBlizzardSoundActive(bool tf)
    {
        blizzardSoundAnmt.SetBool("InBlizzard", tf);
    }

    //Player Progress
    public void SavingPlayerProgress()
    {
        GameManager.instance.SaveMan.playerSavePosition = barrelControl.transform.position;
        GameManager.instance.SaveMan.playerSaveArea = CurrentArea;
        GameManager.instance.SaveMan.playerSaveAreaActive = new bool[4] { TilemapParents[0].activeSelf, TilemapParents[1].activeSelf, TilemapParents[2].activeSelf, TilemapParents[3].activeSelf };
        GameManager.instance.SaveMan.playerSaveTimer = gameTimer;
    }

    public void LoadingPlayerProgress()
    {
        barrelControl.transform.position = GameManager.instance.SaveMan.playerSavePosition;
        CurrentArea = GameManager.instance.SaveMan.playerSaveArea;
        GameManager.instance.AudioMan.BGMTransition(GameManager.instance.GameScriptObj.MusicClips[(int)CurrentArea]);
        BackgroundTransition((int)CurrentArea);
        LevelSoundTransition(CurrentArea);
        TilemapParents[0].SetActive(GameManager.instance.SaveMan.playerSaveAreaActive[0]);
        TilemapParents[1].SetActive(GameManager.instance.SaveMan.playerSaveAreaActive[1]);
        TilemapParents[2].SetActive(GameManager.instance.SaveMan.playerSaveAreaActive[2]);
        TilemapParents[3].SetActive(GameManager.instance.SaveMan.playerSaveAreaActive[3]);
        gameTimer = GameManager.instance.SaveMan.playerSaveTimer;
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
