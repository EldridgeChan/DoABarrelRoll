using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShip : MonoBehaviour
{
    public bool isStart = true;

    [SerializeField]
    private Transform pirateShipTrans;
    [SerializeField]
    private AudioSource pirateAdoSrc;
    [SerializeField]
    private Collider2D startEndColid;

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
        GameManager.instance.GameCon.BarrelCameraState(true, CameraState.CutScene);
        GameManager.instance.GameCon.EndLevelCutScene();
        GameManager.instance.AudioMan.BGMTransition(null);
    }

    public void StopAudio()
    {
        pirateAdoSrc.Stop();
    }

    public void PlayAudio()
    {
        pirateAdoSrc.Play();
    }

    public void disableCollider()
    {
        startEndColid.enabled = false;
    }
}
