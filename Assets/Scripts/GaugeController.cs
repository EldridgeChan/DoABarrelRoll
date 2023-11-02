using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeController : MonoBehaviour
{
    [SerializeField]
    private Image gaugeImg;

    public void setGauge(float t)
    {
        gaugeImg.fillAmount = t;
    }
}
