using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShip : MonoBehaviour
{
    [SerializeField]
    private bool isStart = true;
    private int endCounter = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Barrel"))
        {
            if (isStart)
            {
                StartTheLevel();
            }
            else
            {
                EndTheLevel();
                endCounter++;
            }
        }
    }

    private void StartTheLevel()
    {
        GameManager.instance.GameCon.isControlLocked = false;
        GameManager.instance.GameCon.BarrelCameraState(false, CameraState.CutScene);
    }

    private void EndTheLevel()
    {
        GameManager.instance.GameCon.isControlLocked = true;
        GameManager.instance.GameCon.BarrelCameraState(true, CameraState.CutScene);
        GameManager.instance.GameCon.EndLevelCutScene(endCounter);
        GameManager.instance.AudioMan.StartLerpMusicVolume(false);
    }
}
