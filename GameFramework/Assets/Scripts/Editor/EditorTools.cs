using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorTools : Editor
{
    private static string SECRET_KEY = "lsdkangoashgasnjoghrnajkdfsbngikheiurtbga1s56tg4rae1eah4t8a423854w4e6547654fgsn/.;gh,ml;,";
    
    

    private static void GetMD5(string filePath)
    {
        string md5 = MD5File(filePath); //获取MD5值
        Debug.LogError(md5);
    }
    
    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();//创建对象
            byte[] retVal = md5.ComputeHash(fs); // 获取md5值, 是一个Hash字节数组
            fs.Close(); //关闭文件流
            StringBuilder sb = new StringBuilder(); //使用字符流节省性能
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2")); //x2:以16进制转换
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    
    [MenuItem("Tools/加密")]
    static void EncryptAssetBundle()
    {
        string path = Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget + "/ab_bundle1";
        AES.AESFileEncrypt(path, SECRET_KEY);
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Tools/解密")]
    static void DecryptAssetBunld()
    {
        string path = Application.streamingAssetsPath + "/" + EditorUserBuildSettings.activeBuildTarget + "/panel";
        // 对AB包进行解密
        byte[] bytes = AES.AESFileByteDecrypt(path, SECRET_KEY);
        // 从内存中加载AB包
        AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
        // 加载AB包中的名字为Panel预制体
        GameObject prefab = ab.LoadAsset<GameObject>("Panel");
        // 实例化预制体到Canvas下
        GameObject.Instantiate(prefab,GameObject.Find("Canvas").transform);
    }
}
