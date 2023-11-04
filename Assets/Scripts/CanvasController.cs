using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private Toggle mapFilpToggle;
    [SerializeField]
    private Toggle arrowShowToggle;

    private void Start()
    {
        mapFilpToggle.isOn = GameManager.instance.SaveMan.mirroredTilemap;
        arrowShowToggle.isOn = GameManager.instance.SaveMan.showJumpGuide;
    }

    public void GameStart()
    {
        GameManager.instance.LoadMan.LoadScene(1);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void MapFlipToggled(bool tf)
    {
        GameManager.instance.SaveMan.mirroredTilemap = tf;
    }

    public void ArrowShowToggled(bool tf)
    {
        GameManager.instance.SaveMan.showJumpGuide = tf;
    }
}
