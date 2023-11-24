using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;

    //Managers
    [SerializeField]
    private InputManager inputMan;
    public InputManager InputMan { get { return inputMan; } }
    [SerializeField]
    private SceneLoadManager loadMan;
    public SceneLoadManager LoadMan { get { return loadMan; } }
    [SerializeField]
    private SaveManager saveMan;
    public SaveManager SaveMan { get { return saveMan; } }  

    //ScriptObjs
    [SerializeField]
    private GameScriptableObject gameScriptObj;
    public GameScriptableObject GameScriptObj { get { return gameScriptObj; } }
    [SerializeField]
    private SpeechesScripableObject[] speechScripObj;
    public SpeechesScripableObject[] SpeechScripObj { get { return speechScripObj; } }    

    //GameController
    private GameController gameCon;
    public GameController GameCon
    {
        get { return gameCon; }
        set
        {
            if (gameCon)
            {
                Debug.Log("ERROR: Tried to Init Game Controller when there is one");
            }
            else
            {
                gameCon = value;
            }
        }
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (!inputMan) { inputMan = GetComponent<InputManager>(); }
        if (!loadMan) { loadMan = GetComponent<SceneLoadManager>(); }
        if (!saveMan) { saveMan = GetComponent<SaveManager>(); }
        Cursor.lockState = CursorLockMode.Confined;
    }
}
