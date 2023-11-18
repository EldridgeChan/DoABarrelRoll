using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera mainCam;
    private Camera MainCam { 
        get { if (!mainCam)
            {
                mainCam = Camera.main;
            }
            return mainCam;
} }

    private Vector2 mousePos = Vector2.zero;

    private void FixedUpdate()
    {
        if (mousePos.x != Input.mousePosition.x || mousePos.y != Input.mousePosition.y)
        {
            if (GameManager.instance.GameCon)
            {
                GameManager.instance.GameCon.BarrelRoll(MainCam.ScreenToWorldPoint(mousePos), MainCam.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        mousePos = Input.mousePosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //barrel.jumpCharge();
            if (GameManager.instance.GameCon)
            {
                GameManager.instance.GameCon.BarrelJump(MouseWorldPos());
                GameManager.instance.GameCon.SkipSpeech();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.instance.GameCon)
            {
                //GameManager.instance.LoadMan.LoadScene(0);
                GameManager.instance.GameCon.TryBarrelStand();
            }
        }

        if (Input.inputString.Length > 0)
        {
            if (int.TryParse(Input.inputString, out int num) && GameManager.instance.GameCon)
            {
                GameManager.instance.GameCon.TeleportCheckpoint(num);
            }
        }
    }

    public Vector2 MouseWorldPos()
    {
        return MainCam.ScreenToWorldPoint(mousePos);
    }
}
