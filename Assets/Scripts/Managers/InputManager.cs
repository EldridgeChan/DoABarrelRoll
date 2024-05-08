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
    public Vector2 GamePadLookDir { get { return gamePadLookDir; } }

    private InputScheme lastInputDevice = InputScheme.KeyMouse;

    private void FixedUpdate()
    {
        if (mousePos.x != Input.mousePosition.x || mousePos.y != Input.mousePosition.y)
        {
            if (GameManager.instance.GameCon)
            {
                GameManager.instance.GameCon.BarrelRoll(MainCam.ScreenToWorldPoint(mousePos), MainCam.ScreenToWorldPoint(Input.mousePosition));
            }
            SetLastInputDevice(InputScheme.KeyMouse);
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
            SetLastInputDevice(InputScheme.KeyMouse);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuButton();
            SetLastInputDevice(InputScheme.KeyMouse);
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
            SetLastInputDevice(InputScheme.GamePad);
        }
    }

    public void GamePadRoll(InputAction.CallbackContext callback)
    {
        Vector2 temp = callback.ReadValue<Vector2>();
        if (temp != Vector2.zero && gamePadRollDir != Vector2.zero)
        {
            SetLastInputDevice(InputScheme.GamePad);
            if (!GameManager.instance.GameCon) { return; }
            GameManager.instance.GameCon.BarrelRollGamePad(gamePadRollDir, temp);
        }
        gamePadRollDir = temp;
    }

    public void GamePadStand(InputAction.CallbackContext callback)
    {
        if (callback.performed)
        {
            SetLastInputDevice(InputScheme.GamePad);
            MenuButton();
        }
    }

    public void GamePadLook(InputAction.CallbackContext callback)
    {
        gamePadLookDir = callback.ReadValue<Vector2>();
        if (callback.ReadValue<Vector2>() != Vector2.zero)
        {
            SetLastInputDevice(InputScheme.GamePad);
        }
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

    private void SetLastInputDevice(InputScheme device)
    {
        if (GameManager.instance.GameCon && lastInputDevice != device)
        {
            GameManager.instance.GameCon.UpdateIntructionInputDevice(device);
        }
        lastInputDevice = device;
    }

    public Vector2 MouseWorldPos()
    {
        return MainCam.ScreenToWorldPoint(mousePos);
    }
}
