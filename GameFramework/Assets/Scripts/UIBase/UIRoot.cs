using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public enum CanvasType
    {
        Top,
        Mid,
        Bottom,
    }

    private static UIRoot _instance;

    public static UIRoot Instance
    {
        get { return _instance; }
    }

    private GameObject canvas_Top;
    private GameObject canvas_Mid;
    private GameObject canvas_Bottom;

    private void Awake()
    {
        _instance = this;

        InitUI();
    }

    private void InitUI()
    {
        canvas_Top = transform.Find("TopCanvas").gameObject;
        canvas_Mid = transform.Find("MidCanvas").gameObject;
        canvas_Bottom = transform.Find("BottomCanvas").gameObject;
    }

    public void ShowOrHideUI(CanvasType ct, string uiName, bool isShow)
    {
        switch (ct)
        {
            case CanvasType.Top:
                if (canvas_Top.transform.Find(uiName))
                {
                    canvas_Top.transform.Find(uiName).gameObject.SetActive(isShow);
                }
                else
                {
                    if (isShow)
                    {
                        
                    }
                }
                    
                break;
            case CanvasType.Mid:
                canvas_Mid.transform.Find(uiName).gameObject.SetActive(isShow);
                break;
            case CanvasType.Bottom:
                canvas_Bottom.transform.Find(uiName).gameObject.SetActive(isShow);
                break;
        }
    }
}