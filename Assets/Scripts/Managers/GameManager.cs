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
    [SerializeField]
    private UIManager uiMan;
    public UIManager UIMan { get { return uiMan; } }
    [SerializeField]
    private AudioManager audioMan;
    public AudioManager AudioMan { get { return audioMan; } }

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

    [SerializeField]
    private bool onTestFeatures = true;
    public bool OnTestFeatures { get { return onTestFeatures; } }
    [HideInInspector]
    public EndingType CurrentEnding = EndingType.None;

    private List<PromptLanguage> languageListeners;

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
        if (!uiMan) { uiMan = GetComponent<UIManager>(); }
        if (!audioMan) { audioMan = GetComponent<AudioManager>(); }
        Cursor.lockState = CursorLockMode.Confined;

        languageListeners = new List<PromptLanguage>();
        saveMan.LoadSetting();
        SaveMan.LoadPlayerProgress();
        saveMan.InitFromSave();
        uiMan.InitSettingOptions();
    }

    public void AddLangListener(PromptLanguage pl)
    {
        languageListeners.Add(pl);
    }

    public void RemoveLangListener(PromptLanguage pl)
    {
        languageListeners.Remove(pl);
    }

    public void UpdateLangListeners()
    {
        for (int i = 0; i < languageListeners.Count; i++)
        {
            if (languageListeners[i])
            {
                languageListeners[i].SetLanguageText();
            }
        }
    }
}
