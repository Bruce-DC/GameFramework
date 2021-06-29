using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
    string masterVersionFilePath;
    private string masterVersionFileName = "MasterVersion.txt";

    // Start is called before the first frame update
    void Start()
    {
        masterVersionFilePath = String.Format("{0}/SaveFiles/{1}", Application.persistentDataPath, masterVersionFileName);

        CheckVersion();
    }

    void CheckVersion()
    {
        if (File.Exists(masterVersionFilePath))
        {
            Debug.Log("判断版本是否一致");
        }
        else
        {
            Debug.Log("版本文件不存在，第一次启动游戏，更新之。");
            FTPUtility.GetInstance().DownloadFile(masterVersionFileName);
        }
    }
}