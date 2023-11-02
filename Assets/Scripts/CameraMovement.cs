using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform barrelTrans;

    void Update()
    {
        transform.position = barrelTrans.position + Vector3.back * 10.0f;
    }
}
