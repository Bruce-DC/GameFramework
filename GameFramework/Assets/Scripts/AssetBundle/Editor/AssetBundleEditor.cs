using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetBundle打包
/// 流程：
/// 1.构建前先切换平台；
/// 2.构建AssetBundle存放在{StreamingAssets/AssetBundle_Normal/平台}路径下
/// 3.遍历构建后的ab包，加密后存放在{StreamingAssets/AssetBundle_AES/平台}路径下
/// 4.遍历加密后的ab包，上传到资源服务器
/// </summary>
public class AssetBundleEditor
{
    private static string AB_Path = Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget;

    private static string SECRET_KEY = "dongxiaoliBOSHI121";

    private static List<string> ab_pathList = new List<string>();
    private static List<string> ab_List = new List<string>();

    [MenuItem("Tools/BuildAssetBundle For Windows")]
    static void BuildAssetBundle4Windows()
    {
        if (!Directory.Exists(AB_Path))
        {
            Directory.CreateDirectory(AB_Path);
        }

        BuildPipeline.BuildAssetBundles(AB_Path, BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);
        //AssetDatabase.Refresh();

        //GetMD5(Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget + "/ab_bundle1");

        UploadAssetBundle();
    }

    [MenuItem("Tools/BuildAssetBundle For Android")]
    static void BuildAssetBundle4Android()
    {
        if (!Directory.Exists(AB_Path))
        {
            Directory.CreateDirectory(AB_Path);
        }

        BuildPipeline.BuildAssetBundles(AB_Path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        //AssetDatabase.Refresh();

        //GetMD5(Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget + "/ab_bundle1");

        UploadAssetBundle();
    }

    [MenuItem("Tools/BuildAssetBundle For iOS")]
    static void BuildAssetBundle4iOS()
    {
        if (!Directory.Exists(AB_Path))
        {
            Directory.CreateDirectory(AB_Path);
        }

        BuildPipeline.BuildAssetBundles(AB_Path, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
        Debug.LogError("AB包构建成功！");
        GetAllAssetBundlePath();
        EncryptNormalAssetBundle();
        Debug.LogError("AB包加密成功！");
        UploadAssetBundle();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/删除当前平台的AB包")]
    static void DeleteAssetBundle()
    {
        if (!Directory.Exists(AB_Path))
        {
            return;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(AB_Path);
        FileInfo[] fileInfos = directoryInfo.GetFiles();

        if (fileInfos.Length <= 0)
        {
            Debug.LogError("AB包不存在，无需删除！");
            return;
        }

        for (int i = 0; i < fileInfos.Length; i++)
        {
            File.Delete(fileInfos[i].FullName);
        }

        AssetDatabase.Refresh();
        Debug.LogError("AB包删除成功！");
    }

    static void GetAllAssetBundlePath()
    {
        ab_pathList.Clear();
        ab_List.Clear();

        DirectoryInfo directoryInfo = new DirectoryInfo(AB_Path);
        FileInfo[] fileInfos = directoryInfo.GetFiles();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.EndsWith(".meta"))
            {
                continue;
            }

            ab_pathList.Add(fileInfos[i].FullName);
            ab_List.Add(fileInfos[i].Name);
        }
    }

    static void EncryptNormalAssetBundle()
    {
        if (ab_pathList.Count > 1)
        {
            for (int i = 0; i < ab_pathList.Count; i++)
            {
                AES.AESFileEncrypt(ab_pathList[i], SECRET_KEY);
            }
        }
        else
        {
            Debug.LogError("没有需要加密的AB包！");
        }
    }

    static void UploadAssetBundle()
    {
        //遍历当前平台的AB包，然后上传到资源服务器
        Debug.LogError("AB包开始上传！");
        for (int i = 0; i < ab_List.Count; i++)
        {
            UploadFile(ab_List[i]);
        }
    }

    /// <summary>
    /// 上传文件功能
    /// </summary>
    /// <param name="newFileName">上传文件的文件名</param>
    /// <returns></returns>
    //[MenuItem("Assets/上传AssetBundle到FTP服务器", false, 600)]
    public static void UploadFile(string newFileName)
    {
        long uploadFileAlreadyLength = 0;

        string url = FTPEditor.GetUploadUrl(newFileName);
        string filepath = AB_Path + "/" + newFileName;
        
        FtpWebRequest request = FTPUtility.Instance.CreatFtpWebRequest(url, WebRequestMethods.Ftp.UploadFile);
        Stream responseStream = request.GetRequestStream();

        FileStream fs = File.OpenRead(filepath);
        int buflength = 1000000;
        byte[] buffer = new byte[buflength];
        int bytesRead = 1;

        while (bytesRead != 0)
        {
            bytesRead = fs.Read(buffer, 0, buflength);
            responseStream.Write(buffer, 0, bytesRead);
            uploadFileAlreadyLength += bytesRead;
            //此处用于使用协程更新下载进度条，避免大文件的上传直接导致unity的假死
            //yield return StartCoroutine(UpdateUISlider(downloadFileAlreadyLength));
        }

        responseStream.Close();
        fs.Close();
    }
}