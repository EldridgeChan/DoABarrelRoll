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
    [SerializeField]
    private GameObject[] swampDecorativeTilemaps;
    [SerializeField]
    private Animator glitchTilemapParentAnmt;
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

    [Header("NPCs")]
    [SerializeField]
    private NPCBehaviour[] NPCBehaves;
    [HideInInspector]
    public int SpeakingNPCIndex = -1;
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
            StartStartGameCutScene();
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
            barrelControl.TeleportReset();
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
            TilemapParents[1].SetActive(num >= 1 && num <= 4);
            TilemapParents[2].SetActive(num >= 4 && num <= 7);
            TilemapParents[3].SetActive(num >= 7);

        }
    }

    private void TeleportLevelArea(int num)
    {
        if (num <= 1)
        {
            CurrentArea = LevelArea.Beach;
        }
        else if (num <= 4)
        {
            CurrentArea = LevelArea.Jungle;
        }
        else if (num <= 7)
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

    private void StartStartGameCutScene()
    {
        if (GameManager.instance.SaveMan.endCounter >= 5)
        {
            // --> No Fuck Given Animation
            StartPirateShip.disableCollider();
            barrelControl.BarrelCutsceneAnmt.SetInteger("Direction", GameManager.instance.SaveMan.mirroredTilemap ? -1 : 1);
            barrelControl.BarrelCutsceneAnmt.enabled = true;
        }

        StartCutScene((SpeechScript)Mathf.Clamp((int)SpeechScript.Start0 + GameManager.instance.SaveMan.endCounter, (int)SpeechScript.Start0, (int)SpeechScript.Start5), startPirateShip.transform);
    }

    public void EndLevelCutScene()
    {
        barrelCSBehave.enabled = true;
        barrelControl.GravityDirection = Vector2.down;
        isControlLocked = true;
        GameManager.instance.UIMan.CanCon.SetEndTimer(gameTimer);
        StartCutScene((SpeechScript)Mathf.Clamp((int)SpeechScript.End0 + GameManager.instance.SaveMan.endCounter, (int)SpeechScript.End0, (int)SpeechScript.End4), endPirateShip.transform);
    }

    public void SkipSpeech()
    {
        if (currCutScene < 0) { return; }
        textBbBehave.SkipSpeech();
    }

    public void EndSpeech()
    {
        if (currCutScene == SpeechScript.None) { return; }

        if (currCutScene < SpeechScript.Start5)
        {
            if (currCutScene == SpeechScript.Start4 && SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDFRIEND + "");
            }
            EndStartCutScene();
        }
        else if (currCutScene == SpeechScript.Start5)
        {
            //End --> No Fuck Given
            if (SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDING1 + "");
            }
            GameManager.instance.UIMan.CanCon.CreditScene(EndingType.NoFuckGiven);
            GameManager.instance.SaveMan.endCounter = 0;
            
        }
        else if (currCutScene < SpeechScript.Old0)
        {
            if (GameManager.instance.SaveMan.endCounter == 1 && SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_FIRSTFINISH + "");
            }
            if (gameTimer <= 1400.888f && SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_BEATME + "");
            }
            OnEndMenu(true);
        }
        else if (currCutScene == SpeechScript.OldIdle0)
        {
            //End --> Live to Die
            if (SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDING2 + "");
            }
            GameManager.instance.UIMan.CanCon.CreditScene(EndingType.LiveToDie);   
        }
        else if (currCutScene == SpeechScript.DevilIdle0)
        {
            //End --> Strangled By Finish Line
            if (SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDING3 + "");
            }
            GameManager.instance.UIMan.CanCon.CreditScene(EndingType.StrangledByFinishLine);
        }

        if (currCutScene >= SpeechScript.Old0)
        {
            NPCBehaves[SpeakingNPCIndex].EndOfSpeech();
            SpeakingNPCIndex = -1;
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
        barrelControl.BarrelReset();
        gameTimer = 0.0f;
        ResetAllNPC();
        barrelHighestY = -100.0f;
        StartStartGameCutScene();
        OnEndMenu(false);

        backgroundAnmt.SetInteger("LevelArea", 1);
        for (int i = 0; i < TilemapParents.Length; i++)
        {
            TilemapParents[i].SetActive(i == 0);
        }
    }

    public void EndingCutSceneStand()
    {
        canOnOff = false;
        if (isControlLocked)
        {
            OnPauseMenu(false);
            barrelControl.CancelInvoke(nameof(barrelControl.BarrelGainControl));

        }
        else
        {
            barrelControl.BarrelStandReady();
        }
    }

    public void EndingCutSceneTrigger(int index)
    {
        switch (index)
        {
            case 0:
                barrelControl.BarrelCutsceneAnmt.SetTrigger("StartRolling");
                barrelControl.Invoke(nameof(barrelControl.BarrelCutsceneOrder), GameManager.instance.GameScriptObj.EndingBarrelRollingTime);
                BarrelCameraState(false, CameraState.CutScene);
                break;
            case 1:
                barrelControl.BarrelCutsceneAnmt.SetTrigger("StandStill");
                barrelControl.BarrelCutsceneOrder();
                break;
            case 2:
                barrelControl.BarrelCutsceneAnmt.SetTrigger("Jump");
                break;
            default:
                Debug.Log("ERROR: Undefined Index For ending cutscene trigger!!");
                break;
        }
    }



    private void ResetAllNPC()
    {
        for (int i = 0; i < NPCBehaves.Length; i++)
        {
            NPCBehaves[i].resetNPC();
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

    public void SetSwampDecorationActive(bool tf)
    {
        for (int i = 0; i < swampDecorativeTilemaps.Length; i++)
        {
            swampDecorativeTilemaps[i].SetActive(tf);
        }
    }

    public void SetGlitchLandDark(bool tf)
    {
        glitchTilemapParentAnmt.SetBool("InGlitch", tf);
    }

    //Snowing
    public void StartSnowing()
    {
        if (!SnowFlakeParentAnmt) { return; }
        SnowFlakeParentAnmt.gameObject.SetActive(true);
        CancelInvoke(nameof(DeactivateSnowParent));
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
        if (!beachSoundParent || !jungleSoundParent || !snowSoundParent) { return; }
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
        GameManager.instance.SaveMan.playerSavePosition = new Vector2(barrelControl.transform.position.x * (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f), barrelControl.transform.position.y);
        GameManager.instance.SaveMan.playerSaveArea = CurrentArea;
        GameManager.instance.SaveMan.playerSaveAreaActive = new bool[4] { TilemapParents[0].activeSelf, TilemapParents[1].activeSelf, TilemapParents[2].activeSelf, TilemapParents[3].activeSelf };
        GameManager.instance.SaveMan.playerSaveTimer = gameTimer;
    }

    public void LoadingPlayerProgress()
    {
        barrelControl.transform.position = new Vector2(GameManager.instance.SaveMan.playerSavePosition.x * (GameManager.instance.SaveMan.mirroredTilemap ? -1.0f : 1.0f), GameManager.instance.SaveMan.playerSavePosition.y);
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
