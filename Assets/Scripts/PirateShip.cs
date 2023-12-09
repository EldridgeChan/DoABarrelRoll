using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShip : MonoBehaviour
{
    [HideInInspector]
    public bool isStart = true;

    [SerializeField]
    private Transform pirateShipTrans;
    [SerializeField]
    private AudioSource pirateAdoSrc;

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
                GameManager.instance.SaveMan.FinishGame();
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
        GameManager.instance.GameCon.EndLevelCutScene(GameManager.instance.SaveMan.endCounter);
        GameManager.instance.AudioMan.StartLerpMusicVolume(false);
    }

    public void ToPostion(bool isStart)
    {
        pirateShipTrans.position = isStart ? GameManager.instance.GameScriptObj.PirateShipStartPosition : GameManager.instance.GameScriptObj.PirateShipEndPosition;
        if (GameManager.instance.SaveMan.mirroredTilemap)
        {
            pirateShipTrans.position = new Vector3(-transform.position.x, transform.position.y, 0.0f);
        }
        this.isStart = isStart;
    }

    public void StopAudio()
    {
        pirateAdoSrc.Stop();
    }

    public void PlayAudio()
    {
        pirateAdoSrc.Play();
    }
}
