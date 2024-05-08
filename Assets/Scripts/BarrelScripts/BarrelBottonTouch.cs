using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBottonTouch : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelCon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            barrelCon.BottonTouch++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Ground"))
        {
            barrelCon.BottonTouch--;
        }
    }
}
