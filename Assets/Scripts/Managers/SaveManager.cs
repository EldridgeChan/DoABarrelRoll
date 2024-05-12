using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string dirPath = "";
    public string DirPath { 
        get
        {
            if (dirPath.Length <= 0)
            {
                dirPath = Application.persistentDataPath;
            }
            return dirPath;
        } 
    }

    //Setting Fields
    private SettingSavefile settingSave;
    public SettingSavefile SettingSave { get { return settingSave; } }

    //Game Save Fields
    private ProgressSavefile progressSave;
    public ProgressSavefile ProgressSave { get { return progressSave; } }

    public class SettingSavefile 
    {
        public int windowSizeIndex = 3;
        public FullScreenMode screenMode = FullScreenMode.Windowed;
        public Language selectedLanguage = Language.English;
        public float jumpSensibility = 0.25f;
        public float rollSensibility = 1.0f;
        public float masterVolume = 0.5f;
        public float musicVolume = 1.0f;
        public bool mirroredTilemap = false;
        public bool showJumpGuide = true;
        public bool skipEnding = false;
    }

    public class ProgressSavefile
    {
        public ProgressSavefile(GameVersion gameVersion)
        {
            this.gameVersion = gameVersion;
        }

        public GameVersion gameVersion = GameVersion.None;
        public Vector2 playerSavePosition = Vector2.zero;
        public LevelArea playerSaveArea = LevelArea.MainMenu;
        public bool[] playerSaveAreaActive = new bool[4] { false, false, false, false };
        public float playerSaveTimer = 0.0f;
        public int endCounter = 0;
        public bool watchedEnding = false;

        public void ResetProgress()
        {
            playerSavePosition = Vector2.zero;
            playerSaveArea = LevelArea.MainMenu;
            playerSaveAreaActive = new bool[4] { false, false, false, false };
            playerSaveTimer = 0.0f;
        }

        public void UpdateProgress(Vector2 pos, LevelArea area, bool[] activeAreas, float timer)
        {
            playerSavePosition = pos;
            playerSaveArea = area;
            playerSaveAreaActive = activeAreas;
            playerSaveTimer = timer;
        }

        public void FinishGame()
        {
            endCounter++;
            ResetProgress();
        }

        public void ResetEndCounter()
        {
            endCounter = 0;
        }

        public void TrueEnding()
        {
            watchedEnding = true;
        }
    }

    public void SaveSetting()
    {
        string filePath = Path.Combine(DirPath, GameManager.instance.GameScriptObj.SettingFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            string dataToStore = JsonUtility.ToJson(settingSave);
            using FileStream stream = new (filePath, FileMode.Create);
            using StreamWriter writer = new (stream);
            writer.Write(dataToStore);
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR occured when trying to save setting to file: " + filePath + "\n" + ex);
        }
    }

    public void SavePlayerProgress()
    {
        ProgressSave.gameVersion = (GameVersion)GameVersionConverter();

        string filePath = Path.Combine(DirPath, GameManager.instance.GameScriptObj.ProgressFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            string dataToStore = JsonUtility.ToJson(progressSave);
            using FileStream stream = new (filePath, FileMode.Create);
            using StreamWriter writer = new (stream);
            writer.Write(dataToStore);
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR occured when trying to save setting to file: " + filePath + "\n" + ex);
        }
    }

    private int GameVersionConverter()
    {
        string version = Application.version;
        if (version.EndsWith('o'))
        {
            return 0;
        }
        version = version.Replace(".", "_");
        for (int i = 1; i < (int)GameVersion.EndOfEnum; i++)
        {
            if (version.Equals((GameVersion)i + ""))
            {
                return i;
            }
        }
        return -1;
    }

    public void LoadSetting()
    {
        if (settingSave != null) { return; }
        string filePath = Path.Combine(DirPath, GameManager.instance.GameScriptObj.SettingFileName);

        if (File.Exists(filePath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(filePath, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }
                settingSave = JsonUtility.FromJson<SettingSavefile>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR occured when trying to load Setting data from file: " + filePath + "\n" + ex);
            }
        }
        else
        {
            settingSave = new SettingSavefile();
        }
    }

    public void LoadPlayerProgress()
    {
        if (progressSave != null) { return; }
        string filePath = Path.Combine(DirPath, GameManager.instance.GameScriptObj.ProgressFileName);

        if (File.Exists(filePath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new(filePath, FileMode.Open))
                {
                    using StreamReader reader = new(stream);
                    dataToLoad = reader.ReadToEnd();
                }
                progressSave = JsonUtility.FromJson<ProgressSavefile>(dataToLoad);
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR occured when trying to load Setting data from file: " + filePath + "\n" + ex);
            }
        }
        else
        {
            progressSave = new ProgressSavefile((GameVersion)GameVersionConverter());
        }
    }

    public void ResetPlayerProgress()
    {
        progressSave.ResetProgress();
    }

    public void TrueEnding()
    {
        if (ProgressSave.watchedEnding) { return; }
        GameManager.instance.UIMan.ActivateSkipEndToggle();
        ProgressSave.TrueEnding();
        SettingSave.skipEnding = true;
        SavePlayerProgress();
        SaveSetting();
    }

    public void FinishGame()
    {
        progressSave.FinishGame();
        SavePlayerProgress();
    }

    public void ResetEndCounter()
    {
        progressSave.ResetEndCounter();
        SavePlayerProgress();
    }

    public void InitFromSave()
    {
        Screen.SetResolution((int)GameManager.instance.GameScriptObj.WindowResolution[SettingSave.windowSizeIndex].x, (int)GameManager.instance.GameScriptObj.WindowResolution[SettingSave.windowSizeIndex].y, SettingSave.screenMode);
        AudioListener.volume = SettingSave.masterVolume;
        GameManager.instance.AudioMan.SetBGMVolume(SettingSave.musicVolume);
    }

    public void ConvertNewSave()
    {
        if (PlayerPrefs.HasKey("windowSizeIndex"))
        {
            settingSave = new SettingSavefile();
            settingSave.windowSizeIndex = PlayerPrefs.GetInt("windowSizeIndex", 3);
            settingSave.screenMode = (FullScreenMode)PlayerPrefs.GetInt("screenMode", (int)FullScreenMode.Windowed);
            settingSave.selectedLanguage = (Language)PlayerPrefs.GetInt("selectedLanguage", (int)Language.English);
            settingSave.jumpSensibility = PlayerPrefs.GetFloat("jumpSensibility", 0.25f);
            settingSave.rollSensibility = PlayerPrefs.GetFloat("rollSensibility", 1.0f);
            settingSave.masterVolume = PlayerPrefs.GetFloat("masterVolume", 0.5f);
            settingSave.musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
            settingSave.mirroredTilemap = PlayerPrefs.GetInt("mirroredTilemap", 0) > 0;
            settingSave.showJumpGuide = PlayerPrefs.GetInt("showJumpGuide", 1) > 0;
            settingSave.skipEnding = PlayerPrefs.GetInt("skipEnding", 0) > 0;
            SaveSetting();
        }
        if (PlayerPrefs.HasKey("playerSaveArea") || PlayerPrefs.HasKey("endCounter") || PlayerPrefs.HasKey("watchedEnding"))
        {
            progressSave = new ProgressSavefile((GameVersion)GameVersionConverter());
            ProgressSave.playerSavePosition = new Vector2(PlayerPrefs.GetFloat("playerSavePositionX", 0.0f), PlayerPrefs.GetFloat("playerSavePositionY", 0.0f));
            ProgressSave.playerSaveArea = (LevelArea)PlayerPrefs.GetInt("playerSaveArea", (int)LevelArea.MainMenu);
            ProgressSave.playerSaveAreaActive[0] = PlayerPrefs.GetInt("playerSaveAreaActive0", 0) > 0;
            ProgressSave.playerSaveAreaActive[1] = PlayerPrefs.GetInt("playerSaveAreaActive1", 0) > 0;
            ProgressSave.playerSaveAreaActive[2] = PlayerPrefs.GetInt("playerSaveAreaActive2", 0) > 0;
            ProgressSave.playerSaveAreaActive[3] = PlayerPrefs.GetInt("playerSaveAreaActive3", 0) > 0;
            ProgressSave.playerSaveTimer = PlayerPrefs.GetFloat("playerSaveTimer", 0.0f);
            ProgressSave.endCounter = PlayerPrefs.GetInt("endCounter", 0);
            ProgressSave.watchedEnding = PlayerPrefs.GetInt("watchedEnding", 0) > 0;
            SavePlayerProgress();
        }
        PlayerPrefs.DeleteAll();
    }
}
