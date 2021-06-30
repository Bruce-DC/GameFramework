using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Utility
{
    public static string GetPlatform()
    {
        string result = "";
        
        if (Application.isPlaying)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    result = "Android";
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    result = "Windows";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    result = "Iphone";
                    break;
                default:
                    Debug.LogError("当前平台不支持，请确认平台！");
                    break;
            }
        }
        return result;
    }
}
