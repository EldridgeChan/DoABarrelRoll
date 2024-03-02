using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextBubbleBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform bubbleTrans;
    [SerializeField]
    private Image bubbleImg;
    [SerializeField]
    private TMP_Text bubbleTxt;
    [SerializeField]
    private AudioSource bubbleAdoScr;

    private SpeechesScripableObject speechScript = null;
    private int textIndex = 0;
    private int charIndex = 0;
    private string currFullText;
    private bool isShip = true;
    
    public void ShowAllChar()
    {
        CancelInvoke();
        bubbleTxt.text = currFullText;
        charIndex = currFullText.Length;
        if (isShip)
        {
            bubbleImg.sprite = GameManager.instance.GameScriptObj.ShipFinishBubbleSprite;
        }
        Invoke(nameof(ShowNextText), GameManager.instance.GameScriptObj.SpeechBubbleFinishDelay);
    }

    public void SkipSpeech()
    {
        if (!isShip) { return; }
        PlayBubbleSound();
        bubbleImg.enabled = true;
        if (charIndex >= currFullText.Length)
        {
            ShowNextText();
        }
        else
        {
            ShowAllChar();
        }        
    }

    public void ShowNextChar()
    {
        bubbleImg.enabled = true;
        bubbleTxt.text += currFullText[charIndex];
        charIndex++;
        if (currFullText[charIndex - 1].Equals(" "))
        {
            ShowNextChar();
            return;
        }

        PlayBubbleSound();

        if (charIndex >= currFullText.Length)
        {
            ShowAllChar();
        }
        else
        {
            Invoke(nameof(ShowNextChar), 1.0f / GameManager.instance.GameScriptObj.SpeechCharacterPerSecond);
        }
    }

    public void ShowNextText()
    {
        bubbleImg.enabled = true;
        CancelInvoke();
        textIndex++;
        if (textIndex >= speechScript.AllSpeech[(int)GameManager.instance.SaveMan.selectedLanguage].bubbleSpeeches.Length)
        {
            ExitSpeechBubble();
        }
        else
        {
            ResetBubbleSpeech();
            ShowNextChar();
        }
    }

    public void ExitSpeechBubble()
    {
        CancelInvoke();
        GameManager.instance.GameCon.EndSpeech();
        bubbleImg.enabled = false;
        bubbleTxt.text = "";
    }

    public void ResetBubbleSpeech()
    {
        charIndex = 0;
        bubbleTrans.localPosition = speechScript.AllSpeech[(int)GameManager.instance.SaveMan.selectedLanguage].bubbleSpeeches[textIndex].position;
        currFullText = speechScript.AllSpeech[(int)GameManager.instance.SaveMan.selectedLanguage].bubbleSpeeches[textIndex].bubbleText.TrimEnd();
        SetBubbleSize();
        bubbleImg.sprite = isShip ? GameManager.instance.GameScriptObj.ShipNormalBubbleSprite : GameManager.instance.GameScriptObj.OldManNormalBubbleSprite;
    }

    private void SetBubbleSize()
    {
        bubbleTxt.text = currFullText;
        bubbleTxt.ForceMeshUpdate();
        bubbleTrans.sizeDelta = new Vector2(bubbleTxt.textBounds.size.x + GameManager.instance.GameScriptObj.SpeechBubbleWidthBuffer, GameManager.instance.GameScriptObj.SpeechBubbleHeight);
        bubbleTxt.text = "";
    }

    public void InitBubble(bool isShip, SpeechesScripableObject speechScript, Transform parentTrans)
    {
        CancelInvoke();
        bubbleTxt.enableWordWrapping = false;
        transform.SetParent(parentTrans);
        this.speechScript = speechScript;
        this.isShip = isShip;
        textIndex = 0;
        ResetBubbleSpeech();
        Invoke(nameof(ShowNextChar), speechScript.StartDelay);
    }

    public void PlayBubbleSound()
    {
        bubbleAdoScr.Play();
    }
}
