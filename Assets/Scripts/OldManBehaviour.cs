using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform oldManEmojiTrans;

    private bool isTalking = false;
    private float lookT = 0.0f;
    private Vector2 lerpFromPos = Vector2.zero;
    private int talkCounter = 0;
    private Transform barrelTrans;
    private Vector2 mockPosition = Vector2.zero;
    private Vector2 mockVelocity = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrel"))
        {
            barrelTrans = collision.transform;
            StartLooking();
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
        }
    }

    private void LookingBehaviour()
    {
        if (barrelTrans == null) { return; }
        lookT = Mathf.Clamp01(lookT + Time.deltaTime / GameManager.instance.GameScriptObj.OldManLookLerpTime);
        oldManEmojiTrans.position = Vector2.Lerp(lerpFromPos, GameManager.instance.GameScriptObj.OldManEmojiMaxPositionOffset * (Vector2)(barrelTrans.position - transform.position).normalized + (Vector2)transform.position + GameManager.instance.GameScriptObj.OldManEmojiOriginalPositionOffset, lookT);
    }

    private void FreeEmojiBehaviour()
    {
        mockVelocity = mockVelocity + GameManager.instance.GameScriptObj.OldManCentripetalForce * Time.deltaTime * ((Vector2)(transform.position + (Vector3)GameManager.instance.GameScriptObj.OldManEmojiOriginalPositionOffset - oldManEmojiTrans.position)).normalized;
        Vector2 localExpectedPos = (mockPosition + mockVelocity * Time.deltaTime) - ((Vector2)transform.position + GameManager.instance.GameScriptObj.OldManEmojiOriginalPositionOffset);
        mockPosition = (localExpectedPos.magnitude > GameManager.instance.GameScriptObj.OldManEmojiMaxPositionOffset ? GameManager.instance.GameScriptObj.OldManEmojiMaxPositionOffset * localExpectedPos.normalized : localExpectedPos) + (Vector2)transform.position + GameManager.instance.GameScriptObj.OldManEmojiOriginalPositionOffset;
        oldManEmojiTrans.position = mockPosition;
    }

    private void StartLooking()
    {
        isTalking = true;
        lookT = 0.0f;
        lerpFromPos = oldManEmojiTrans.position;
        if (talkCounter <= 0)
        {
            //testing code
            //GameManager.instance.SaveMan.endCounter = 3;
            //GameManager.instance.GameCon.StartCutScene((SpeechScript)Mathf.Clamp((int)SpeechScript.Taunt0 + GameManager.instance.SaveMan.endCounter, (int)SpeechScript.Taunt0, (int)SpeechScript.EndOfEnum - 1), transform, false);

            GameManager.instance.GameCon.StartCutScene((SpeechScript)Mathf.Clamp((int)SpeechScript.Old0 + GameManager.instance.SaveMan.endCounter, (int)SpeechScript.Old0, (int)SpeechScript.Taunt0 - 1), transform, false);
            talkCounter++;
        }
        else if (GameManager.instance.GameCon.barrelHighestY - transform.position.y > GameManager.instance.GameScriptObj.OldManTauntHightThershold)
        {
            GameManager.instance.GameCon.StartCutScene((SpeechScript)Random.Range((int)SpeechScript.Taunt0, (int)SpeechScript.EndOfEnum), transform, false);
        }
    }

    public void EndOfSpeech()
    {
        isTalking = false;
        mockPosition = oldManEmojiTrans.position;
        mockVelocity = Vector2.zero;
    }

    public void resetOldMan()
    {
        talkCounter = 0;
    }
}
