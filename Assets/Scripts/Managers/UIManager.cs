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
        GameManager.instance.SaveMan.screenMode = (FullScreenMode)index;  
        SetWindowResolution(GameManager.instance.SaveMan.windowSizeIndex);
    }

    public void SetWindowResolution(int index)
    {
        GameManager.instance.SaveMan.windowSizeIndex = index;
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[index].x, (int)GameManager.instance.GameScriptObj.WindowResolution[index].y, GameManager.instance.SaveMan.screenMode);
    }

    public void SetMasterVolume(float volume)
    {
        GameManager.instance.SaveMan.masterVolume = volume;
        AudioListener.volume = volume;
        masterVolumeTxt.text = "" + (int)(volume * 100.0f);
    }

    public void SetMusicVolume(float volume)
    {
        GameManager.instance.SaveMan.musicVolume = volume;
        GameManager.instance.AudioMan.SetBGMVolume(volume);
        musicVolumeTxt.text = "" + (int)(volume * 100.0f);
    }

    public void OnFlipMapToggle(bool tf)
    {
        GameManager.instance.SaveMan.mirroredTilemap = tf;
    }

    public void SetFlipMapToggleInteractable(bool tf)
    {
        flipMapTog.interactable = tf;
    }

    public void OnShowGuideToggle(bool tf)
    {
        GameManager.instance.SaveMan.showJumpGuide = tf;
        if (GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.DisplayGuildingArrow(tf);
        }
    }

    public void InitSettingOptions()
    {
        windowModeDD.value = (int)GameManager.instance.SaveMan.screenMode;
        resolutionDD.value = GameManager.instance.SaveMan.windowSizeIndex;
        languageDD.value = (int)GameManager.instance.SaveMan.selectedLanguage;
        masterVolumeSlid.value = GameManager.instance.SaveMan.masterVolume;
        masterVolumeTxt.text = "" + (int)(GameManager.instance.SaveMan.masterVolume * 100.0f);
        musicVolumeSlid.value = GameManager.instance.SaveMan.musicVolume;
        musicVolumeTxt.text = "" + (int)(GameManager.instance.SaveMan.musicVolume * 100.0f);
        flipMapTog.isOn = GameManager.instance.SaveMan.mirroredTilemap;
        jumpGuideTog.isOn = GameManager.instance.SaveMan.showJumpGuide;
    }
}
