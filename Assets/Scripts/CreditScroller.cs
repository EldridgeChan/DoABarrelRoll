using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditScroller : MonoBehaviour
{
    [SerializeField]
    private RectTransform creditParentTran;
    [SerializeField]
    private RectTransform gameTitleTran;
    [SerializeField]
    private TMP_Text endingTxt;
    [SerializeField]
    private float scrollSpeed = 0.1f;
    [SerializeField]
    private float endYPos = 5000.0f;
    [SerializeField]
    private float StartWaitTime = 3.0f;
    [SerializeField]
    private float endPauseTime = 3.0f;
    private bool isScrolling = false;

    private void Start()
    {
        switch (GameManager.instance.CurrentEnding)
        {
            case EndingType.StrangledByFinishLine:
                endingTxt.text = "Strangled By Finish Line";
                break;
            case EndingType.LiveToDie:
                endingTxt.text = "Live To Die";
                break;
            case EndingType.NoFuckGiven:
                endingTxt.text = "No Fuck Given";
                break;
            default:
                endingTxt.text = "";
                break;
        }
        creditParentTran.anchoredPosition = GameManager.instance.CurrentEnding != EndingType.None ? Vector2.zero : -gameTitleTran.anchoredPosition;
        Invoke(nameof(StartScrolling), StartWaitTime);

    }

    private void Update()
    {
        if (isScrolling)
        {
            creditParentTran.anchoredPosition += scrollSpeed * Time.deltaTime * Vector2.up;
            if (creditParentTran.anchoredPosition.y >= endYPos)
            {
                creditParentTran.anchoredPosition = endYPos * Vector2.up;
                isScrolling = false;
                Invoke(nameof(BackMainMenu), endPauseTime);
            }
        }
    }

    public void StartScrolling()
    {
        isScrolling = true;
    }

    public void BackMainMenu()
    {
        GameManager.instance.UIMan.CanCon.BackMainMenu();
    }
}
