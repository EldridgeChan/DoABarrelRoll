using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public bool mirroredTilemap = false;
    public bool showJumpGuide = false;
    public Language selectedLanguage = Language.English;
    public int windowSizeIndex = 3;
    public FullScreenMode screenMode = FullScreenMode.Windowed;
}
