using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptLanguage : MonoBehaviour
{
    [SerializeField]
    private TMP_Text promptTxt;
    [SerializeField]
    private bool isTitleFont = false;
    [SerializeField]
    private string[] languageTexts;

    private void Start()
    {
        if (!promptTxt)
        {
            Debug.Log("ERROR: TMP_Text component not found on gameobject name " + gameObject.name);
            return;
        }
        SetLanguageText();
        GameManager.instance.AddLangListener(this);
    }

    private void OnDisable()
    {
        GameManager.instance.RemoveLangListener(this);
    }

    public void SetLanguageText()
    {
        promptTxt.text = languageTexts[(int)GameManager.instance.SaveMan.selectedLanguage];
        if (GameManager.instance.SaveMan.selectedLanguage == Language.English)
        {
            promptTxt.font = isTitleFont ? GameManager.instance.GameScriptObj.TitleENFont : GameManager.instance.GameScriptObj.LanguageFonts[0];
        }
        else
        {
            promptTxt.font = GameManager.instance.GameScriptObj.LanguageFonts[(int)GameManager.instance.SaveMan.selectedLanguage];
        }
    }

}
