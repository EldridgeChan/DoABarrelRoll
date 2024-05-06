using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelJump : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelCon;

    [SerializeField]
    private SpriteRenderer arrowRend;
    [SerializeField]
    private Transform arrowParentTrans;
    [SerializeField]
    private Sprite[] arrowSprites;

    private Vector2 arrowDir = Vector2.zero;

    private void Update()
    {
        if (arrowDir == Vector2.zero) { return; }
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, BarrelControl.ToRoundAngle(arrowDir));
        //arrowParentTrans.localScale = new Vector2(barrelCon.MousePosMagnitudeMultiplier(GameManager.instance.InputMan.MouseWorldPos() - barrelCon.BarrelRig.position), 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || (!collision.isTrigger && collision.CompareTag("Swamp")))
        {
            barrelCon.touchingGroundNum++;
            barrelCon.SetTouchedGround(true);
            arrowRend.sprite = arrowSprites[0];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision || collision.CompareTag("Ground") || (!collision.isTrigger && collision.CompareTag("Swamp")))
        {
            barrelCon.touchingGroundNum--;
            if (barrelCon.touchingGroundNum <= 0)
            {
                barrelCon.SetTouchedGround(false);
                arrowRend.sprite = arrowSprites[1];
            }
        }
    }

    public void SetArrowRotationAndSscale(Vector2 dir)
    {
        arrowDir = dir;
        arrowParentTrans.localScale = new Vector2(barrelCon.MousePosMagnitudeMultiplier(dir), 1.0f);
    }
}
