using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "SpeechesScripableObject", menuName = "ScriptableObjects/Speeches")]
public class SpeechesScripableObject : ScriptableObject
{
    public float StartDelay = 1.0f;
    public SpeechLanguages[] AllSpeech;

    [System.Serializable]
    public class SpeechLanguages
    {
        public Language language;
        public BubbleSpeech[] bubbleSpeeches;
    }

    [System.Serializable]
    public class BubbleSpeech
    {
        public Vector2 position = Vector2.zero;
        public string bubbleText = "";
    }
}
