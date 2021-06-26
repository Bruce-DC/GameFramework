using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor : Editor
{
    [MenuItem("Tools/BuildAssetBundle")]
    static void BuildAssetBundle()
    {
        string path = Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
        
        //GetMD5(Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget + "/ab_bundle1");
    }
}
