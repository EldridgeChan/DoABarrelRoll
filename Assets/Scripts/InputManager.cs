using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCam;
    private Vector2 mousePos = Vector2.zero;
    [SerializeField]
    private BarrelControl barrel;
    [SerializeField]
    private Transform[] testRespawns;
    [SerializeField]
    private SpriteRenderer arrowRend;

    private void Start()
    {
        if (!mainCam) { mainCam = Camera.main; }
    }

    private void FixedUpdate()
    {
        if (mousePos.x != Input.mousePosition.x || mousePos.y != Input.mousePosition.y)
        {
            barrel.barrelRoll(mainCam.ScreenToWorldPoint(mousePos), mainCam.ScreenToWorldPoint(Input.mousePosition));
        }
        mousePos = Input.mousePosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //barrel.jumpCharge();
            barrel.barrelJump(mouseWorldPos());
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            arrowRend.enabled = !arrowRend.enabled;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (testRespawns.Length > 0)
            {
                barrel.transform.position = testRespawns[0].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (testRespawns.Length > 1)
            {
                barrel.transform.position = testRespawns[1].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (testRespawns.Length > 2)
            {
                barrel.transform.position = testRespawns[2].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (testRespawns.Length > 3)
            {
                barrel.transform.position = testRespawns[3].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (testRespawns.Length > 4)
            {
                barrel.transform.position = testRespawns[4].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (testRespawns.Length > 5)
            {
                barrel.transform.position = testRespawns[5].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (testRespawns.Length > 6)
            {
                barrel.transform.position = testRespawns[6].position;
                barrel.BarrelRig.velocity = Vector2.zero;
                barrel.BarrelRig.angularVelocity = 0.0f;
            }
        }
    }

    public Vector2 mouseWorldPos()
    {
        return mainCam.ScreenToWorldPoint(mousePos);
    }
}
