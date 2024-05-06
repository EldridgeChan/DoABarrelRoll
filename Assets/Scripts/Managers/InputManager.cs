using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Vector2 gamePadRollDir = Vector2.zero;
    private Vector2 gamePadLookDir = Vector2.zero;

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
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
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
            MenuButton();
        }

        //Testing Code
        TestingFeatureInput();
    }

    private void MenuButton()
    {
        if (GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.TryBarrelStand();
        }
        if (GameManager.instance.LoadMan.CurrentSceneIndex == 2)
        {
            if (GameManager.instance.UIMan.CanCon.InSceneTrasition) { return; }
            GameManager.instance.UIMan.CanCon.InSceneTrasition = true;
            GameManager.instance.AudioMan.PlayClickSound();
            GameManager.instance.UIMan.OnOffBlackScreen(true);
            GameManager.instance.LoadMan.Invoke(nameof(SceneLoadManager.LoadMainMenu), GameManager.instance.GameScriptObj.BlackScreenTransitionTime);
        }
    }

    public void GamePadJump(InputAction.CallbackContext callBack)
    {
        if (callBack.performed)
        {
            if (GameManager.instance.GameCon)
            {
                GameManager.instance.GameCon.BarrelJumpGamePad(gamePadRollDir);
                GameManager.instance.GameCon.SkipSpeech();
            }
        }
    }

    public void GamePadRoll(InputAction.CallbackContext callback)
    {
        Vector2 temp = callback.ReadValue<Vector2>();
        if (GameManager.instance.GameCon && temp != Vector2.zero && gamePadRollDir != Vector2.zero)
        {
            GameManager.instance.GameCon.BarrelRollGamePad(gamePadRollDir, temp);
        }
        gamePadRollDir = temp;
    }

    public void GamePadStand(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            MenuButton();
        }
    }

    public void GamePadLook(InputAction.CallbackContext callback)
    {
        gamePadLookDir = callback.ReadValue<Vector2>();
    }

    private void TestingFeatureInput()
    {
        if (!GameManager.instance.OnTestFeatures) { return; }
        if (Input.inputString.Length > 0 && int.TryParse(Input.inputString, out int num) && GameManager.instance.GameCon)
        {
            GameManager.instance.GameCon.TeleportCheckpoint((num + 9) % 10);
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GameManager.instance.SaveMan.ResetEndCounter();
        }
        if (Input.GetKeyDown(KeyCode.R) && SteamManager.Initialized)
        {
            SteamManager.ResetAllAchievement();
        }
    }

    public Vector2 MouseWorldPos()
    {
        return MainCam.ScreenToWorldPoint(mousePos);
    }
}
