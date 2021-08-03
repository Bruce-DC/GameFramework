using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameVersion : MonoBehaviour
{
    #region 成员变量

    private static GameVersion _instace;

    public static GameVersion Instance
    {
        get
        {
            return _instace;
        }
    }

    /// <summary>
    /// 客户端本地存储主版本的文件的路径
    /// </summary>
    string masterVersionFilePath;

    /// <summary>
    /// 客户端本地存储主版本的文件的名称
    /// </summary>
    private string masterVersionFileName = "MasterVersion.txt";

    #endregion
    
    #region UI组件
    /// <summary>
    /// 展示进度的进度条
    /// </summary>
    public Slider uiSlider;
    /// <summary>
    /// 展示下载速度的文本组件
    /// </summary>
    public Text speedText;
    /// <summary>
    /// 展示下载进度的文本组件
    /// </summary>
    public Text progressText;
    #endregion

    #region 成员方法

    /// <summary>
    /// 检查版本
    /// 本地版本文件不存在，则先从服务器下载版本文件
    /// 拿版本文件中的版本与服务器的版本进行比对，获取需要更新的文件
    /// 下载需要更新的文件
    /// </summary>
    public void CheckVersion()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogError("玩家没有联网，不再检查更新内容。");
        }
        else
        {
            masterVersionFilePath = string.Format("{0}/SaveFiles/{1}/{2}", Application.persistentDataPath,
                Utility.GetPlatform(), masterVersionFileName);

            if (File.Exists(masterVersionFilePath))
            {
                //TODO 拿版本文件中的版本与服务器的版本进行比对，获取需要更新的文件  下载需要更新的文件
                FTPUtility.Instance.DownloadFileByWebClient(FTPUtility.Instance.testDownloadFile);
            }
            else
            {
                Debug.Log("版本文件不存在，第一次启动游戏，下载版本文件。");
                FTPUtility.Instance.DownloadFileByWebClient(masterVersionFileName);
            }
        }
    }

    public void UpdateDownloadInfo(float sliderValue, string progressStr)
    {
        uiSlider.value = sliderValue;
        progressText.text = progressStr;
    }

    public void UpdateDownloadSpeed(string speedStr)
    {
        speedText.text = speedStr;
    }

    #endregion


    #region Unity周期函数

    private void Awake()
    {
        _instace = GetComponent<GameVersion>();
    }

    private void Start()
    {
        CheckVersion();
    }

    #endregion
}