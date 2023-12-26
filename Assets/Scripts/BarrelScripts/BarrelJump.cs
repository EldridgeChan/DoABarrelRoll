using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelJump : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelCon;
    private int touchingGroundNum = 0;

    [SerializeField]
    private SpriteRenderer arrowRend;
    [SerializeField]
    private Transform arrowParentTrans;
    [SerializeField]
    private Sprite[] arrowSprites;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, BarrelControl.ToRoundAngle(GameManager.instance.InputMan.MouseWorldPos() - (Vector2)transform.position));
        arrowParentTrans.localScale = new Vector2(barrelCon.MousePosMagnitudeMultiplier(GameManager.instance.InputMan.MouseWorldPos()), 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || (!collision.isTrigger && collision.CompareTag("Swamp")))
        {
            touchingGroundNum++;
            barrelCon.SetTouchedGround(true);
            arrowRend.sprite = arrowSprites[0];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Swamp"))
        {
            touchingGroundNum--;
            if (touchingGroundNum <= 0)
            {
                barrelCon.SetTouchedGround(false);
                arrowRend.sprite = arrowSprites[1];
            }
        }
    }
}
