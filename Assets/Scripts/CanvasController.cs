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
    private Animator pauseMenuAnmt;
    [SerializeField]
    private TMP_Text gameTimerTxt;
    [SerializeField]
    private TMP_Text endBoardTimerTxt;

    private void Start()
    {
        GameManager.instance.UIMan.CanCon = this;
    }

    public void GameStart()
    {
        mainMenuAnmt.SetTrigger("StartGame");
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.AudioMan.StartLerpMusicVolume(false);
        GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadPlayScene), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void BackMainMenu()
    {
        pauseMenuAnmt.SetBool("ShowPause", false);
        GameManager.instance.UIMan.OnOffBlackScreen(true);
        GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadMainMenu), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
    }

    public void ContinueGame()
    {
        GameManager.instance.GameCon.TryBarrelStand();
    }

    public void StartNewGame()
    {
        GameManager.instance.GameCon.StartNewGame();
    }

    public void OnSetting()
    {
        GameManager.instance.UIMan.OnOffSetting(true);
        if (mainMenuAnmt) 
        { 
            mainMenuAnmt.SetTrigger("StartGame"); 
        }
        else if (pauseMenuAnmt)
        {
            pauseMenuAnmt.SetBool("ShowPause", false);
        }
    }

    public void OffSetting()
    {
        if (mainMenuAnmt)
        {
            mainMenuAnmt.SetTrigger("StartGame");
        }
        else if (pauseMenuAnmt)
        {
            pauseMenuAnmt.SetBool("ShowPause", true);
        }

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
}
