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
        if (mapFilpToggle) { mapFilpToggle.isOn = GameManager.instance.SaveMan.mirroredTilemap; }
        if (arrowShowToggle) { arrowShowToggle.isOn = GameManager.instance.SaveMan.showJumpGuide; }
    }

    public void GameStart()
    {
        GameManager.instance.LoadMan.LoadScene(1);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void BackMainMenu()
    {
        GameManager.instance.LoadMan.LoadScene(0);
    }

    public void ContinueGame()
    {
        GameManager.instance.GameCon.TryBarrelStand();
    }

    public void MapFlipToggled(bool tf)
    {
        GameManager.instance.SaveMan.mirroredTilemap = tf;
    }

    public void ArrowShowToggled(bool tf)
    {
        GameManager.instance.SaveMan.showJumpGuide = tf;
    }

    public void StartNewGame()
    {
        GameManager.instance.GameCon.StartNewGame();
    }
}
