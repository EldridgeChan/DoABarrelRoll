using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCheck : MonoBehaviour
{
    [SerializeField]
    private LevelArea areaLoading = LevelArea.None;
    [SerializeField]
    private bool isUpLoad = true;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Barrel") || areaLoading < LevelArea.Beach) { return; }

        GameManager.instance.GameCon.TilemapParents[(int)areaLoading - 1].SetActive(collision.transform.position.y > transform.position.y == isUpLoad);
        if (areaLoading == LevelArea.Jungle && collision.transform.position.y > transform.position.y)
        {
            GameManager.instance.GameCon.MovePirateShipToEnd();
        }
    }
}
