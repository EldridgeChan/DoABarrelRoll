using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.UIMan.CanCon = this;
    }

    [SerializeField]
    private Animator mainMenuAnmt;
    [SerializeField]
    private Animator pauseMenuAnmt;

    public void GameStart()
    {
        mainMenuAnmt.SetTrigger("StartGame");
        GameManager.instance.UIMan.OnOffBlackScreen(true);
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
}
