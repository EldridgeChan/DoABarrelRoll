using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Animator inGameCanvasAnmt;

    [SerializeField]
    private TMP_Dropdown windowModeDD;
    [SerializeField]
    private TMP_Dropdown resolutionDD;
    [SerializeField]
    private TMP_Dropdown languageDD;
    [SerializeField]
    private Slider jumpSensibilitySlid;
    [SerializeField]
    private TMP_Text jumpSensibilityTxt;
    [SerializeField]
    private Slider rollSensibilitySlid;
    [SerializeField]
    private TMP_Text rollSensibilityTxt;
    [SerializeField]
    private Slider masterVolumeSlid;
    [SerializeField]
    private TMP_Text masterVolumeTxt;
    [SerializeField]
    private Slider musicVolumeSlid;
    [SerializeField]
    private TMP_Text musicVolumeTxt;
    [SerializeField]
    private Toggle flipMapTog;
    [SerializeField]
    private Toggle jumpGuideTog;
    [SerializeField]
    private Toggle skipEndTog;
    [SerializeField]
    private GameObject skipEndTogObj;

    private bool isSettingOpened = false;
    public bool IsSettingOpened { get { return isSettingOpened; } }
    

    private CanvasController canCon;
    public CanvasController CanCon 
    { 
        get { return canCon; }
        set
        {
            if (canCon) 
            {
                Debug.Log("ERROR: Canvas Controller Reinit");
                return;
            }
            canCon = value;
        }
    }

    public void OnOffBlackScreen(bool tf)
    {
        inGameCanvasAnmt.SetBool("BlackScreen", tf);
    }

    public void OnOffSetting(bool tf)
    {
        inGameCanvasAnmt.SetBool("Setting", tf);
        isSettingOpened = tf;
        if (!tf)
        {
            CanCon.OffSetting();
            GameManager.instance.SaveMan.SaveSetting();
        }
    }

    public void SetWindowMode(int index)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.screenMode = (FullScreenMode)index;  
        SetWindowResolution(GameManager.instance.SaveMan.windowSizeIndex);
    }

    public void SetWindowResolution(int index)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.windowSizeIndex = index;
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[index].x, (int)GameManager.instance.GameScriptObj.WindowResolution[index].y, GameManager.instance.SaveMan.screenMode);
    }

    public void SetLanguage(int index)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.selectedLanguage = (Language)index;
        GameManager.instance.UpdateLangListeners();
    }
    public void SetJumpSensibility(float sensibility)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.jumpSensibility = sensibility;
        jumpSensibilityTxt.text = sensibility.ToString("0.00");
    }

    public void SetRollSensibility(float sensibility)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.rollSensibility = sensibility;
        rollSensibilityTxt.text = sensibility.ToString("0.00");
    }

    public void SetMasterVolume(float volume)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.masterVolume = volume;
        AudioListener.volume = volume;
        masterVolumeTxt.text = "" + (int)(volume * 100.0f);
    }

    public void SetMusicVolume(float volume)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.musicVolume = volume;
        GameManager.instance.AudioMan.SetBGMVolume(volume);
        musicVolumeTxt.text = "" + (int)(volume * 100.0f);
    }

    public void OnFlipMapToggle(bool tf)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.mirroredTilemap = tf;
    }

    public void SetSettingInteractable(bool tf)
    {
        flipMapTog.interactable = tf;
        languageDD.interactable = tf;
    }

    public void OnShowGuideToggle(bool tf)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.showJumpGuide = tf;
        if (GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.DisplayGuildingArrow(tf);
        }
    }

    public void OnSkipEndToggle(bool tf)
    {
        GameManager.instance.AudioMan.PlayClickSound();
        GameManager.instance.SaveMan.skipEnding = tf;
    }

    public void ActivateSkipEndToggle()
    {
        skipEndTogObj.SetActive(true);
        skipEndTog.isOn = true;
    }

    public void InitSettingOptions()
    {
        windowModeDD.value = (int)GameManager.instance.SaveMan.screenMode;
        resolutionDD.value = GameManager.instance.SaveMan.windowSizeIndex;
        languageDD.value = (int)GameManager.instance.SaveMan.selectedLanguage;
        jumpSensibilitySlid.value = GameManager.instance.SaveMan.jumpSensibility;
        jumpSensibilityTxt.text = GameManager.instance.SaveMan.jumpSensibility.ToString("0.00");
        rollSensibilitySlid.value = GameManager.instance.SaveMan.rollSensibility;
        rollSensibilityTxt.text = GameManager.instance.SaveMan.rollSensibility.ToString("0.00");
        masterVolumeSlid.value = GameManager.instance.SaveMan.masterVolume;
        masterVolumeTxt.text = "" + (int)(GameManager.instance.SaveMan.masterVolume * 100.0f);
        musicVolumeSlid.value = GameManager.instance.SaveMan.musicVolume;
        musicVolumeTxt.text = "" + (int)(GameManager.instance.SaveMan.musicVolume * 100.0f);
        flipMapTog.isOn = GameManager.instance.SaveMan.mirroredTilemap;
        jumpGuideTog.isOn = GameManager.instance.SaveMan.showJumpGuide;
        skipEndTog.isOn = GameManager.instance.SaveMan.skipEnding;
        skipEndTogObj.SetActive(GameManager.instance.SaveMan.watchedEnding);
    }
}
