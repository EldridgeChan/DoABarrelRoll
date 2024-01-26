using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardImageController : MonoBehaviour
{
    [SerializeField]
    private Material blizzardImageMat;
    [SerializeField]
    private Animator blizzardImageAnmt;
    [HideInInspector]
    public float blizzardDirection = 0;

    // Update is called once per frame
    void Update()
    {
        if (!blizzardImageAnmt) { return; }
        blizzardImageMat.SetFloat("_BlizzardDir", blizzardDirection);
    }

    public void ShowBlizzardImage(int dir)
    {
        if (!blizzardImageAnmt) { return; }
        blizzardImageAnmt.SetInteger("BlizzardDirection", dir);
        blizzardImageAnmt.SetBool("InBlizzard", true);
    }

    public void HideBlizzardImage()
    {
        if (!blizzardImageAnmt) { return; }
        blizzardImageAnmt.SetBool("InBlizzard", false);
    }
}
