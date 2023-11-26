using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Animator inGameCanvasAnmt;
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
        if (!tf)
        {
            CanCon.OffSetting();
            //saveManager
        }
    }

    public void SetWindowMode(int index)
    {
        GameManager.instance.SaveMan.screenMode = index < 2 ? (FullScreenMode)index : FullScreenMode.Windowed;  
        SetWindowResolution(GameManager.instance.SaveMan.windowSizeIndex);
    }

    public void SetWindowResolution(int index)
    {
        GameManager.instance.SaveMan.windowSizeIndex = index;
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[index].x, (int)GameManager.instance.GameScriptObj.WindowResolution[index].y, GameManager.instance.SaveMan.screenMode);
    }
}
