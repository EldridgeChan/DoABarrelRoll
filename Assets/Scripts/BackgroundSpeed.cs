using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpeed : MonoBehaviour
{
    [SerializeField]
    private int layerNum = 0;
    [SerializeField]
    private Animator layerScrollAnmt;

    private void Start()
    {
        if (layerScrollAnmt == null) { layerScrollAnmt = GetComponent<Animator>(); }
        SetScrollSpeed();
        enabled = false;
    }

    private void SetScrollSpeed()
    {
        switch (layerNum)
        {
            case 2:
                layerScrollAnmt.speed = GameManager.instance.GameScriptObj.BackgroundLayer2Speed;
                break;
            case 3:
                layerScrollAnmt.speed = GameManager.instance.GameScriptObj.BackgroundLayer3Speed;
                break;
            case 4:
                layerScrollAnmt.speed = GameManager.instance.GameScriptObj.BackgroundLayer4Speed;
                break;
            default:
                Debug.Log("ERROR: Unreconized layerNum. Did you forgot to set the layernum on " + gameObject.name + "?");
                break;
        }
        layerScrollAnmt.SetInteger("Dir", GameManager.instance.SaveMan.SettingSave.mirroredTilemap ? -1 : 1);
    }
}
