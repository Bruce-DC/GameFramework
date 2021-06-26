using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 主要负责在unity编辑器下上传构建好的AssetBundle
///
/// </summary>
public class FTPEditor : Editor
{
    private static string _commonUrl = "ftp://121.43.191.40:21";
    
    public static string GetUploadUrl(string fileName)
    {
        return string.Format("{0}/{1}/{2}", _commonUrl, EditorUserBuildSettings.activeBuildTarget, fileName);
    }
    
    public static string GetFilePath(string fileName)
    {
        return string.Format("{0}/{1}", _commonUrl, fileName);
    }
}