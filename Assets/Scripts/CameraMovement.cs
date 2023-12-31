using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Camera mainCam;

    private float camLerpT = 0.0f;
    private bool lerpPositiveDir = true;
    private CameraState CurrCamState = CameraState.CutScene;

    private void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        camLerpT = Mathf.Clamp01(camLerpT + (lerpPositiveDir ? 1.0f : -1.0f) * Time.deltaTime / GameManager.instance.GameScriptObj.CameraMaxLerpTime);

        switch (CurrCamState)
        {
            case CameraState.CutScene:
                lerpCutscene();
                break;
            case CameraState.Menu:
                lerpMenu();
                break;
            default:
                Debug.Log("ERROR: Unexpected Camera State");
                break;
        }
    }

    private void lerpMenu()
    {
        Vector2 clampedMousePos = new Vector2(Mathf.Clamp(GameManager.instance.InputMan.MouseWorldPos().x, GameManager.instance.GameCon.GetBarrelPosition().x - GameManager.instance.GameScriptObj.CameraMaxOffsetBoundary.x, GameManager.instance.GameCon.GetBarrelPosition().x + GameManager.instance.GameScriptObj.CameraMaxOffsetBoundary.x), Mathf.Clamp(GameManager.instance.InputMan.MouseWorldPos().y, GameManager.instance.GameCon.GetBarrelPosition().y - GameManager.instance.GameScriptObj.CameraMaxOffsetBoundary.y, GameManager.instance.GameCon.GetBarrelPosition().y + GameManager.instance.GameScriptObj.CameraMaxOffsetBoundary.y));
        transform.position = (Vector3)Vector2.Lerp(GameManager.instance.GameCon.GetBarrelPosition(), clampedMousePos, GameManager.instance.GameScriptObj.CameraOnMenuMaxLerpT * camLerpT) + Vector3.back * 10.0f;
    }

    private void lerpCutscene()
    {
        transform.position = (Vector3)Vector2.Lerp(Vector2.zero, Vector2.up * GameManager.instance.GameScriptObj.CameraCutSceneYPositionOffset, camLerpT) + GameManager.instance.GameCon.GetBarrelPosition() + Vector3.back * 10.0f;
        mainCam.orthographicSize = Mathf.Lerp(GameManager.instance.GameScriptObj.CameraDefaultSize, GameManager.instance.GameScriptObj.CameraCutSceneSize, camLerpT);
    }

    public void SetCamLerpDir(bool tf)
    {
        lerpPositiveDir = tf;
    }

    public void SetCurretCamState(CameraState state)
    {
        CurrCamState = state;
    }
}
