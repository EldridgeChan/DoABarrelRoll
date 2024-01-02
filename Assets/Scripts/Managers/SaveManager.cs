using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public int windowSizeIndex = 3;
    public FullScreenMode screenMode = FullScreenMode.Windowed;
    public Language selectedLanguage = Language.English;
    public float jumpSensibility = 0.25f;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public bool mirroredTilemap = false;
    public bool showJumpGuide = false;
    public int endCounter = 0;

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(nameof(windowSizeIndex), windowSizeIndex);
        PlayerPrefs.SetInt(nameof(screenMode), (int)screenMode);
        PlayerPrefs.SetInt(nameof(selectedLanguage), (int)selectedLanguage);
        PlayerPrefs.SetFloat(nameof(jumpSensibility), jumpSensibility);
        PlayerPrefs.SetFloat(nameof(masterVolume), masterVolume);
        PlayerPrefs.SetFloat(nameof(musicVolume), musicVolume);
        PlayerPrefs.SetInt(nameof(mirroredTilemap), mirroredTilemap ? 1 : 0);
        PlayerPrefs.SetInt(nameof(showJumpGuide), showJumpGuide ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSetting()
    {
        windowSizeIndex = PlayerPrefs.GetInt(nameof(windowSizeIndex), windowSizeIndex);
        screenMode = (FullScreenMode)PlayerPrefs.GetInt(nameof(screenMode), (int)screenMode);
        selectedLanguage = (Language)PlayerPrefs.GetInt(nameof(selectedLanguage), (int)selectedLanguage);
        jumpSensibility = PlayerPrefs.GetFloat(nameof(jumpSensibility), jumpSensibility);
        masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), masterVolume);
        musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), musicVolume);
        mirroredTilemap = PlayerPrefs.GetInt(nameof(mirroredTilemap), mirroredTilemap ? 1 : 0) > 0;
        showJumpGuide = PlayerPrefs.GetInt(nameof(showJumpGuide), showJumpGuide ? 1 : 0) > 0;
        endCounter = PlayerPrefs.GetInt(nameof(endCounter), endCounter);
    }

    public void FinishGame()
    {
        endCounter++;
        PlayerPrefs.SetInt(nameof(endCounter), endCounter);
        PlayerPrefs.Save();
    }

    public void ResetProgress()
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
