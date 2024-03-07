using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //Setting Fields
    public int windowSizeIndex = 3;
    public FullScreenMode screenMode = FullScreenMode.Windowed;
    public Language selectedLanguage = Language.English;
    public float jumpSensibility = 0.25f;
    public float rollSensibility = 1.0f;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public bool mirroredTilemap = false;
    public bool showJumpGuide = false;
    public bool skipEnding = false;

    //Game Save Fields
    public Vector2 playerSavePosition = Vector2.zero;
    public LevelArea playerSaveArea = LevelArea.MainMenu;
    public bool[] playerSaveAreaActive = new bool[4] {false, false, false, false};
    public float playerSaveTimer = 0.0f;
    public int endCounter = 0;
    public bool watchedEnding = false;



    public void SaveSetting()
    {
        PlayerPrefs.SetInt(nameof(windowSizeIndex), windowSizeIndex);
        PlayerPrefs.SetInt(nameof(screenMode), (int)screenMode);
        PlayerPrefs.SetInt(nameof(selectedLanguage), (int)selectedLanguage);
        PlayerPrefs.SetFloat(nameof(jumpSensibility), jumpSensibility);
        PlayerPrefs.SetFloat(nameof(rollSensibility), rollSensibility);
        PlayerPrefs.SetFloat(nameof(masterVolume), masterVolume);
        PlayerPrefs.SetFloat(nameof(musicVolume), musicVolume);
        PlayerPrefs.SetInt(nameof(mirroredTilemap), mirroredTilemap ? 1 : 0);
        PlayerPrefs.SetInt(nameof(showJumpGuide), showJumpGuide ? 1 : 0);
        PlayerPrefs.SetInt(nameof(skipEnding), skipEnding ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SavePlayerProgress()
    {
        PlayerPrefs.SetFloat(nameof(playerSavePosition) + "X", playerSavePosition.x);
        PlayerPrefs.SetFloat(nameof(playerSavePosition) + "Y", playerSavePosition.y);
        PlayerPrefs.SetInt(nameof(playerSaveArea), (int)playerSaveArea);
        PlayerPrefs.SetInt(nameof(playerSaveAreaActive) + "0", playerSaveAreaActive[0] ? 1 : -1);
        PlayerPrefs.SetInt(nameof(playerSaveAreaActive) + "1", playerSaveAreaActive[1] ? 1 : -1);
        PlayerPrefs.SetInt(nameof(playerSaveAreaActive) + "2", playerSaveAreaActive[2] ? 1 : -1);
        PlayerPrefs.SetInt(nameof(playerSaveAreaActive) + "3", playerSaveAreaActive[3] ? 1 : -1);
        PlayerPrefs.SetFloat(nameof(playerSaveTimer), playerSaveTimer);
        PlayerPrefs.Save();
    }

    public void LoadSetting()
    {
        windowSizeIndex = PlayerPrefs.GetInt(nameof(windowSizeIndex), windowSizeIndex);
        screenMode = (FullScreenMode)PlayerPrefs.GetInt(nameof(screenMode), (int)screenMode);
        selectedLanguage = (Language)PlayerPrefs.GetInt(nameof(selectedLanguage), (int)selectedLanguage);
        jumpSensibility = PlayerPrefs.GetFloat(nameof(jumpSensibility), jumpSensibility);
        rollSensibility = PlayerPrefs.GetFloat(nameof(rollSensibility), rollSensibility);
        masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), masterVolume);
        musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), musicVolume);
        mirroredTilemap = PlayerPrefs.GetInt(nameof(mirroredTilemap), mirroredTilemap ? 1 : 0) > 0;
        showJumpGuide = PlayerPrefs.GetInt(nameof(showJumpGuide), showJumpGuide ? 1 : 0) > 0;
        endCounter = PlayerPrefs.GetInt(nameof(endCounter), endCounter);
        skipEnding = PlayerPrefs.GetInt(nameof(skipEnding), skipEnding ? 1 : 0) > 0;
        watchedEnding = PlayerPrefs.GetInt(nameof(watchedEnding), watchedEnding ? 1 : 0) > 0;
    }

    public void LoadPlayerProgress()
    {
        playerSavePosition = new Vector2(PlayerPrefs.GetFloat(nameof(playerSavePosition) + "X", playerSavePosition.x), PlayerPrefs.GetFloat(nameof(playerSavePosition) + "Y", playerSavePosition.y));
        playerSaveArea = (LevelArea)PlayerPrefs.GetInt(nameof(playerSaveArea), (int)playerSaveArea);
        playerSaveAreaActive[0] = PlayerPrefs.GetInt(nameof(playerSaveAreaActive) + "0", playerSaveAreaActive[0] ? 1 : -1) > 0;
        playerSaveAreaActive[1] = PlayerPrefs.GetInt(nameof(playerSaveAreaActive) + "1", playerSaveAreaActive[1] ? 1 : -1) > 0;
        playerSaveAreaActive[2] = PlayerPrefs.GetInt(nameof(playerSaveAreaActive) + "2", playerSaveAreaActive[2] ? 1 : -1) > 0;
        playerSaveAreaActive[3] = PlayerPrefs.GetInt(nameof(playerSaveAreaActive) + "3", playerSaveAreaActive[3] ? 1 : -1) > 0;
        playerSaveTimer = PlayerPrefs.GetFloat(nameof(playerSaveTimer), playerSaveTimer);
    }

    public void ResetPlayerProgress()
    {
        playerSavePosition = Vector2.zero;
        playerSaveArea = LevelArea.MainMenu;
        playerSaveAreaActive = new bool[4] { false, false, false, false };
        playerSaveTimer = 0.0f;
    }

    public void TrueEnding()
    {
        if (watchedEnding) { return; }
        GameManager.instance.UIMan.ActivateSkipEndToggle();
        watchedEnding = true;
        skipEnding = true;
        PlayerPrefs.SetInt(nameof(watchedEnding), watchedEnding ? 1 : 0);
        PlayerPrefs.SetInt(nameof(skipEnding), skipEnding ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void FinishGame()
    {
        endCounter++;
        PlayerPrefs.SetInt(nameof(endCounter), endCounter);
        ResetPlayerProgress();
        SavePlayerProgress();
    }

    public void ResetEndCounter()
    {
        endCounter = 0;
        PlayerPrefs.SetInt(nameof(endCounter), endCounter);
        PlayerPrefs.Save();
    }

    public void InitFromSave()
    {
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[windowSizeIndex].x, (int)GameManager.instance.GameScriptObj.WindowResolution[windowSizeIndex].y, screenMode);
        //set language
        AudioListener.volume = masterVolume;
        GameManager.instance.AudioMan.SetBGMVolume(musicVolume);
    }
}
