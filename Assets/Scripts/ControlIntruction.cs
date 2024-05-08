using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlIntruction : MonoBehaviour
{
    [SerializeField]
    private Image intructionImg;
    [SerializeField]
    private Sprite[] intructionImages;
    [SerializeField]
    private TMP_Text[] intructionTxt;

    public void SetIntructionInputDevice(InputScheme device)
    {
        intructionImg.sprite = intructionImages[(int)device];
        for (int i = 0; i < intructionTxt.Length; i++)
        {
            intructionTxt[i].enabled = i == (int)device;
        }
    }
}
