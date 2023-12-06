using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroller : MonoBehaviour
{
    [SerializeField]
    private RectTransform creditParentTrans;
    [SerializeField]
    private CanvasController canvasController;
    [SerializeField]
    private float scrollSpeed = 0.1f;
    [SerializeField]
    private float endYPos = 5000.0f;
    [SerializeField]
    private float endPauseTime = 3.0f;
    private bool isScrolling = true;

    private void Start()
    {
        creditParentTrans.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (creditParentTrans.anchoredPosition.y < endYPos)
        {
            creditParentTrans.anchoredPosition += scrollSpeed * Time.deltaTime * Vector2.up;
        }
        else if (isScrolling)
        {
            creditParentTrans.anchoredPosition = endYPos * Vector2.up;
            isScrolling = false;
            Invoke(nameof(BackMainMenu), endPauseTime);
        }
    }

    public void BackMainMenu()
    {
        canvasController.BackMainMenu();
    }
}
