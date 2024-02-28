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

    private bool isTalking = false;
    private bool barrelInRange = false;
    private float lookT = 0.0f;
    private Vector2 lerpFromPos = Vector2.zero;
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
            StartLooking();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrel"))
        {
            barrelInRange = false;
            idleTimer = 0.0f;
        }
    }

    private void Start()
    {
        EndOfSpeech();
    }

    private void Update()
    {
        if (isTalking)
        {
            LookingBehaviour();
        } 
        else
        {
            FreeEmojiBehaviour();
            if (barrelInRange)
            {
                idleTimer = Mathf.Clamp(idleTimer + Time.deltaTime, 0.0f, GameManager.instance.GameScriptObj.NPCSpeakIdleTime);
                if (idleTimer >= GameManager.instance.GameScriptObj.NPCSpeakIdleTime)
                {
                    StartLooking();
                }
            }
        }
    }

    private void LookingBehaviour()
    {
        if (barrelTrans == null) { return; }
        lookT = Mathf.Clamp01(lookT + Time.deltaTime / GameManager.instance.GameScriptObj.NPCLookLerpTime);
        NPCEmojiTrans.position = Vector2.Lerp(lerpFromPos, GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset * (Vector2)(barrelTrans.position - transform.position).normalized + (Vector2)transform.position + GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset, lookT);
    }

    private void FreeEmojiBehaviour()
    {
        mockVelocity = mockVelocity + GameManager.instance.GameScriptObj.NPCCentripetalForce * Time.deltaTime * ((Vector2)(transform.position + (Vector3)GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset - NPCEmojiTrans.position)).normalized;
        Vector2 localExpectedPos = (mockPosition + mockVelocity * Time.deltaTime) - ((Vector2)transform.position + GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset);
        mockPosition = (localExpectedPos.magnitude > GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset ? GameManager.instance.GameScriptObj.NPCEmojiMaxPositionOffset * localExpectedPos.normalized : localExpectedPos) + (Vector2)transform.position + GameManager.instance.GameScriptObj.NPCEmojiOriginalPositionOffset;
        NPCEmojiTrans.position = mockPosition;
    }

    private void StartLooking()
    {
        if (isTalking) { return; }
        isTalking = true;
        lookT = 0.0f;
        lerpFromPos = NPCEmojiTrans.position;
        GameManager.instance.GameCon.SpeakingNPCIndex = (int)npcIndicator - 1;
        if (talkCounter <= 0)
        {
            //testing code
            //GameManager.instance.SaveMan.endCounter = 2;
            //GameManager.instance.GameCon.StartCutScene((SpeechScript)Mathf.Clamp((int)SpeechScript.Taunt0 + GameManager.instance.SaveMan.endCounter, (int)SpeechScript.Taunt0, (int)SpeechScript.EndOfEnum - 1), transform, false);

            GameManager.instance.GameCon.StartCutScene((SpeechScript)Mathf.Clamp((int)firstEncounterSpeech + GameManager.instance.SaveMan.endCounter, (int)firstEncounterSpeech, (int)firstIdleSpeech - 1), transform, false);
            talkCounter++;
        }
        else if (idleTimer >= GameManager.instance.GameScriptObj.NPCSpeakIdleTime)
        {
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
        mockPosition = NPCEmojiTrans.position;
        mockVelocity = Vector2.zero;
    }

    public void resetNPC()
    {
        talkCounter = 0;
    }
}
