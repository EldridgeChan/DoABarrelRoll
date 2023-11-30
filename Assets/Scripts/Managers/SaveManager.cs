using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public int windowSizeIndex = 3;
    public FullScreenMode screenMode = FullScreenMode.Windowed;
    public Language selectedLanguage = Language.English;
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public bool mirroredTilemap = false;
    public bool showJumpGuide = false;

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(nameof(windowSizeIndex), windowSizeIndex);
        PlayerPrefs.SetInt(nameof(screenMode), (int)screenMode);
        PlayerPrefs.SetInt(nameof(selectedLanguage), (int)selectedLanguage);
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
        masterVolume = PlayerPrefs.GetFloat(nameof(masterVolume), masterVolume);
        musicVolume = PlayerPrefs.GetFloat(nameof(musicVolume), musicVolume);
        mirroredTilemap = PlayerPrefs.GetInt(nameof(mirroredTilemap), mirroredTilemap ? 1 : 0) > 0;
        showJumpGuide = PlayerPrefs.GetInt(nameof(showJumpGuide), showJumpGuide ? 1 : 0) > 0;
    }

    public void InitFromSave()
    {
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[windowSizeIndex].x, (int)GameManager.instance.GameScriptObj.WindowResolution[windowSizeIndex].y, screenMode);
        //set language
        AudioListener.volume = masterVolume;
        GameManager.instance.AudioMan.SetBGMVolume(musicVolume);
    }
}
