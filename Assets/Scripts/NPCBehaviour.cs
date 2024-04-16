using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    [SerializeField]
    private SpeechScript firstEncounterSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript newEncounterSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript firstIdleSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript newIdleSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript firstTauntSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript newTauntSpeech = SpeechScript.None;
    [SerializeField]
    private SpeechScript npcEndSpeech = SpeechScript.None;
    [SerializeField]
    private LevelArea npcIndicator = LevelArea.None;

    [SerializeField]
    private Transform NPCEmojiTrans;
    [SerializeField]
    private Animator devilAnmt;
    [SerializeField]
    private SpriteRenderer npcBarrelRend;

    private bool isTalking = false;
    private bool barrelInRange = false;
    private float lookT = 0.0f;
    private Vector2 lerpFromPos = Vector2.zero;
    private Vector2 emojiOrigin = Vector2.zero;
    private int talkCounter = 0;
    private Transform barrelTrans;
    private Vector2 mockPosition = Vector2.zero;
    private Vector2 mockVelocity = Vector2.zero;
    private float idleTimer = 0.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrel"))
        {
            barrelInRange = true;
            barrelTrans = collision.transform;
            StartTalking();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrel"))
        {
            barrelInRange = false;
            EndOfSpeech();
        }
    }

    private void Start()
    {
        EndOfSpeech();
        NPCsCheckInit();
    }

    private void Update()
    {
        if (barrelInRange)
        {
            
            if (isTalking)
            {
                LookingBehaviour();
            }
            else
            {
                idleTimer = Mathf.Clamp(idleTimer + Time.deltaTime, 0.0f, GameManager.instance.GameScriptObj.NPCSpeakIdleTime);
                if (idleTimer >= GameManager.instance.GameScriptObj.NPCSpeakIdleTime)
                {
                    StartTalking();
                }
            }
        } 
        else
        {
            FreeEmojiBehaviour();
        }
    }

    private void NPCsCheckInit()
    {
        emojiOrigin = (Vector2)transform.position + GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset;
        if (npcIndicator == LevelArea.Beach)
        {
            transform.position = GameManager.instance.SaveMan.endCounter != 3 ? GameManager.instance.GameScriptObj.NPCOldManOriginalPosition : GameManager.instance.GameScriptObj.NPCOldManMovedPosition;
            transform.position = new Vector2((GameManager.instance.SaveMan.mirroredTilemap ? -1 : 1) * transform.position.x, transform.position.y);
            emojiOrigin = (Vector2)transform.position + (GameManager.instance.SaveMan.endCounter == 3 ? Vector2.zero : GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset);
            npcBarrelRend.sprite = GameManager.instance.SaveMan.endCounter == 3 ? GameManager.instance.GameScriptObj.NPCOldManRollSprite : GameManager.instance.GameScriptObj.NPCOldManStandSprite;
        }
        else if (npcIndicator == LevelArea.Jungle)
        {
            gameObject.SetActive(GameManager.instance.SaveMan.endCounter < 3);
        }
        else if (npcIndicator == LevelArea.GlitchLand)
        {
            gameObject.SetActive(GameManager.instance.SaveMan.endCounter < 2);
        }
    }

    private void LookingBehaviour()
    {
        if (barrelTrans == null) { return; }
        lookT = Mathf.Clamp01(lookT + Time.deltaTime / GameManager.instance.GameScriptObj.NPCLookLerpTime);
        NPCEmojiTrans.position = Vector2.Lerp(lerpFromPos, GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset * (Vector2)(barrelTrans.position - transform.position).normalized + emojiOrigin, lookT);
    }

    private void FreeEmojiBehaviour()
    {
        mockVelocity = mockVelocity + GameManager.instance.GameScriptObj.NPCCentripetalForce * Time.deltaTime * ((Vector2)((Vector3)emojiOrigin - NPCEmojiTrans.position)).normalized;
        Vector2 localExpectedPos = (mockPosition + mockVelocity * Time.deltaTime) - emojiOrigin;
        mockPosition = (localExpectedPos.magnitude > GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset ? GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset * localExpectedPos.normalized : localExpectedPos) + emojiOrigin;
        NPCEmojiTrans.position = mockPosition;
    }

    private void StartTalking()
    {
        if (isTalking) { return; }
        isTalking = true;
        lookT = 0.0f;
        lerpFromPos = NPCEmojiTrans.position;
        GameManager.instance.GameCon.SpeakingNPCIndex = (int)npcIndicator - 1;

        //testing code
        //talkCounter = 1;

        if (talkCounter <= 0)
        {
            if (devilAnmt && npcIndicator == LevelArea.GlitchLand && GameManager.instance.SaveMan.endCounter == 1)
            {
                //animation fade away
                if (SteamManager.Initialized)
                {
                    SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDDEVIL + "");
                }
                devilAnmt.SetTrigger("DevilDisappear");
                isTalking = true;
                return;
            }
            if (npcIndicator == LevelArea.Beach && GameManager.instance.SaveMan.endCounter == 4 && SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDOLD + "");
            }
            if (npcIndicator == LevelArea.Jungle && GameManager.instance.SaveMan.endCounter == 2 && SteamManager.Initialized)
            {
                SteamManager.UnlockAchievement(AchievementType.ACHIEVEMENT_ENDCLOWN + "");
            }

            GameManager.instance.GameCon.StartCutScene((SpeechScript)Mathf.Clamp((int)firstEncounterSpeech + GameManager.instance.SaveMan.endCounter, (int)firstEncounterSpeech, (int)firstIdleSpeech - 1), transform, false);
            talkCounter++;
        }
        else if (idleTimer >= GameManager.instance.GameScriptObj.NPCSpeakIdleTime)
        {
            if (npcIndicator == LevelArea.Beach && GameManager.instance.SaveMan.endCounter < 3 || npcIndicator == LevelArea.GlitchLand && GameManager.instance.SaveMan.endCounter < 1)
            {
                //Stand Lock
                GameManager.instance.GameCon.EndingCutSceneStand();
            }

            bool isNew = (int)firstEncounterSpeech + GameManager.instance.SaveMan.endCounter >= (int)newEncounterSpeech;
            GameManager.instance.GameCon.StartCutScene((SpeechScript)Random.Range((int)(isNew ? newIdleSpeech : firstIdleSpeech), (int)(isNew ? firstTauntSpeech : newIdleSpeech)), transform, false);
        }
        else if (GameManager.instance.GameCon.barrelHighestY - transform.position.y > GameManager.instance.GameScriptObj.NPCTauntHightThershold)
        {
            bool isNew = (int)firstEncounterSpeech + GameManager.instance.SaveMan.endCounter >= (int)newEncounterSpeech;
            GameManager.instance.GameCon.StartCutScene((SpeechScript)Random.Range((int)(isNew ? newTauntSpeech : firstTauntSpeech), (int)(isNew ? npcEndSpeech : newTauntSpeech)), transform, false);
        }
    }

    public void EndOfSpeech()
    {
        isTalking = false;
        idleTimer = 0.0f;
        mockPosition = NPCEmojiTrans.position;
        mockVelocity = Vector2.zero;
    }

    public void resetNPC()
    {
        talkCounter = 0;
        NPCsCheckInit();
    }
}
