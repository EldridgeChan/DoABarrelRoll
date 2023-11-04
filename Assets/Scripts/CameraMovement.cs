using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    void Update()
    {
        if (GameManager.instance.GameCon)
        {
            transform.position = GameManager.instance.GameCon.BarrelControl.transform.position + Vector3.back * 10.0f;
        }
    }
}
