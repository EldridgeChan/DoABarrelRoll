using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanFlow : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || !collision.CompareTag("Barrel")) { return; }
        GameManager.instance.GameCon.SetBarrelInWaterCurrent(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger || !collision.CompareTag("Barrel")) { return; }
        GameManager.instance.GameCon.SetBarrelInWaterCurrent(false);
    }
}
