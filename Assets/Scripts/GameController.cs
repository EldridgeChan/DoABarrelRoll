using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private BarrelControl barrelControl;
    public BarrelControl BarrelControl { get { return barrelControl; } }
    [SerializeField]
    private SpriteRenderer arrowRend;
    [SerializeField]
    private Transform[] testRespawns;

    [SerializeField]
    private Transform[] flipPosTrans;
    [SerializeField]
    private Transform[] flipScaleTrans;
    [SerializeField]
    private Collider2D[] tilemapColids;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.GameCon = this;
        MirrorWorld(GameManager.instance.SaveMan.mirroredTilemap);
        DisplayGuildingArrow(GameManager.instance.SaveMan.showJumpGuide);

    }

    public void DisplayGuildingArrow(bool tf)
    {
        arrowRend.enabled = tf;
    }

    public void TeleportCheckpoint(int num)
    {
        if (num <= testRespawns.Length && num > 0)
        {
            barrelControl.transform.position = testRespawns[num - 1].position;
            barrelControl.BarrelRig.velocity = Vector2.zero;
            barrelControl.BarrelRig.angularVelocity = 0;
        }
    }

    private void MirrorWorld(bool tf)
    {
        foreach (Transform trans in flipPosTrans)
        {
            if (trans)
            {
                trans.position = new Vector3(trans.position.x * (tf ? -1.0f : 1.0f), trans.position.y, trans.position.z);
            }
        }
        foreach (Transform trans in flipScaleTrans)
        {
            if (trans)
            {
                trans.localScale = new Vector3(trans.localScale.x * (tf ? -1.0f : 1.0f) ,trans.localScale.y, trans.localScale.z);
            }
        }
        foreach (Collider2D colid in tilemapColids)
        {
            colid.enabled = true;
        }
    }
}
