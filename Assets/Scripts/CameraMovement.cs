using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float camLerpT = 0.0f;
    void Update()
    {
        camLerpT = Mathf.Clamp01(camLerpT + (GameManager.instance.GameCon.onMenu ? 1.0f : -1.0f) * Time.deltaTime / GameManager.instance.GameScriptObj.CameraMaxLerpTime);
        if (GameManager.instance.GameCon)
        {
            transform.position = (Vector3)Vector2.Lerp(GameManager.instance.GameCon.getBarrelPosition(), GameManager.instance.InputMan.MouseWorldPos(), GameManager.instance.GameScriptObj.CameraOnMenuMaxLerpT * camLerpT) + Vector3.back * 10.0f;
        }
    }
}
