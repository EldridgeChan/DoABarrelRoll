using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private Animator mainMenuAnmt;
    [SerializeField]
    private TMP_Text gameTimerTxt;
    [SerializeField]
    private TMP_Text endBoardTimerTxt;
    [SerializeField]
    private AudioSource mainMenuAdoSrc;
    [SerializeField]
    private GameObject continueButtonGameObj;
    [HideInInspector]
    public bool InSceneTrasition = false;

    private void Start()
    {
        GameManager.instance.UIMan.CanCon = this;
        if (continueButtonGameObj)
        {
            continueButtonGameObj.SetActive(GameManager.instance.SaveMan.playerSaveArea != LevelArea.MainMenu);
        }
    }

    public void GameStart()
    {
        mainMenuAnmt.SetTrigger("StartGame");
        GameManager.instance.AudioMan.InitClickAucioSource();
        GameManager.instance.AudioMan.PlayClickSound();
        PlayMenuOnOff();
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.AudioMan.BGMTransition(null);
        GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadPlayScene), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void GameStartBeginning()
    {
        GameManager.instance.SaveMan.ResetPlayerProgress();
        GameStart();
    }

    public void GameExit()
    {
        mainMenuAnmt.SetTrigger("StartGame");
        GameManager.instance.AudioMan.InitClickAucioSource();
        GameManager.instance.AudioMan.PlayClickSound();
        PlayMenuOnOff();
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.AudioMan.BGMTransition(null);
        Invoke(nameof(DummyQuit), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void DummyQuit()
    {
        Application.Quit();
    }

    public void BackMainMenu(bool isEnd)
    {
        if (isEnd)
        {
            GameManager.instance.GameCon.OnEndMenu(false);
        }
        else
        {
            GameManager.instance.GameCon.OnPauseMenu(false);
            GameManager.instance.GameCon.SavingPlayerProgress();
            GameManager.instance.SaveMan.SavePlayerProgress();
        }
        BackMainMenu();
    }

    public void BackMainMenu()
    {
        GameManager.instance.CurrentEnding = EndingType.None;
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadMainMenu), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void ContinueGame()
    {
        GameManager.instance.GameCon.TryBarrelStand();
        GameManager.instance.AudioMan.PlayClickSound();
        PlayMenuOnOff();
    }

    public void StartNewGame()
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.GameCon.StartNewGame();
    }

    public void OnSetting()
    {
        GameManager.instance.UIMan.OnOffSetting(true);
        PlayMenuOnOff();
        GameManager.instance.AudioMan.InitClickAucioSource();
        GameManager.instance.AudioMan.PlayClickSound();
        if (mainMenuAnmt) 
        { 
            mainMenuAnmt.SetTrigger("StartGame");
        }
        else if (GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.OnPauseMenu(false);
        }
    }

    public void OffSetting()
    {
        PlayMenuOnOff();
        GameManager.instance.AudioMan.PlayClickSound();
        if (mainMenuAnmt)
        {
            mainMenuAnmt.SetTrigger("StartGame");
        }
        else if (GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.OnPauseMenu(true);
        }

    }

    public void CreditScene()
    {
        CreditScene(EndingType.None);
    }

    public void CreditScene(EndingType ending)
    {
        GameManager.instance.CurrentEnding = ending;
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadCredit), ending == EndingType.NoFuckGiven ? GameManager.instance.GameScriptObj.EndingBlackScreenTransitionTime : GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void UpdateGameTimer(float time)
    {
        if (!gameTimerTxt) { return; }
        gameTimerTxt.text = string.Format("{0:D2}:{1:00.000}", (int)(time / 60), (time % 60));
    }

    public void SetEndTimer(float time)
    {
        if (!endBoardTimerTxt) { return; }
        endBoardTimerTxt.text = string.Format("{0:D2}:{1:00.000}", (int)(time / 60), (time % 60));
    }

    public void PlayMenuOnOff()
    {
        if (!mainMenuAdoSrc.enabled) { return; }
        mainMenuAdoSrc.Play();
    }
}
