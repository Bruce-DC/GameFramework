using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Utility
{
    public static string GetPlatform()
    {
        string result = "";
        
        Debug.Log(Application.platform);
        
        // if (Application.isPlaying)
        // {
        //     switch (Application.platform)
        //     {
        //         case RuntimePlatform.Android:
        //             result = "Android";
        //             break;
        //         case RuntimePlatform.WindowsEditor:
        //         case RuntimePlatform.WindowsPlayer:
        //             result = "Windows";
        //             break;
        //         case RuntimePlatform.IPhonePlayer:
        //             result = "";
        //             break;
        //         default:
        //             Debug.LogError("当前平台不支持，请确认平台！");
        //             break;
        //     }
        // }
        // else
        // {
        //     switch (EditorUserBuildSettings.activeBuildTarget)
        //     {
        //         case BuildTarget.Android:
        //             result = "Android";
        //             break;
        //         case BuildTarget.StandaloneWindows:
        //             result = "Windows";
        //             break;
        //         case BuildTarget.iOS:
        //             result = "IPhonePlayer";
        //             break;
        //         default:
        //             Debug.LogError("当前平台不支持，请确认平台！");
        //             break;
        //     }
        // }
        
        
        return result;
    }
}
