using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text versionTxt;

    private void Start()
    {
        versionTxt.text = Application.version;
    }
}
