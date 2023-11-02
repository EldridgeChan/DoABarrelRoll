using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;

    [SerializeField]
    private InputManager inputMan;
    public InputManager InputMan { get { return inputMan; } }

    [SerializeField]
    private GameScriptableObject gameScriptObj;
    public GameScriptableObject GameScriptObj { get { return gameScriptObj; } }

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
