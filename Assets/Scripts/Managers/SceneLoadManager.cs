using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private int currentSceneIndex = 0;
    public int CurrentSceneIndex { get { return currentSceneIndex; } }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadPlayScene()
    {
        LoadScene(1);
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    public void LoadCredit()
    {
        LoadScene(2);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.instance.UIMan.OnOffBlackScreen(false);
        GameManager.instance.UIMan.SetFlipMapToggleInteractable(scene.buildIndex == 0);
        currentSceneIndex = scene.buildIndex;
        if (scene.buildIndex == 0)
        {
            GameManager.instance.AudioMan.BGMTransition(GameManager.instance.GameScriptObj.MusicClips[(int)LevelArea.MainMenu]);
        }
    }
}
