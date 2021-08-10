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
        get { return _instace; }
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
    public Text downloadText;

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
            Debug.LogError("玩家没有联网，不再检查更新内容，直接进入游戏。");
        }
        else
        {
            masterVersionFilePath = string.Format("{0}/SaveFiles/{1}/{2}", Application.persistentDataPath,
                Utility.GetPlatform(), masterVersionFileName);

            if (File.Exists(masterVersionFilePath))
            {
                //TODO 拿版本文件中的版本与服务器的版本进行比对，获取需要更新的文件  下载需要更新的文件
                FTPUtility.Instance.DownloadFileByWebClient(FTPUtility.Instance.testDownloadFile,DownloadComplete);
            }
            else
            {
                Debug.Log("版本文件不存在，第一次启动游戏，下载版本文件。");
                FTPUtility.Instance.DownloadFileByWebClient(masterVersionFileName);
            }
        }
    }

    /// <summary>
    /// 文件下载完成回调
    /// </summary>
    private void DownloadComplete()
    {
        Debug.LogError("下载完毕，开始解压！");
        UIRoot.Instance.ShowOrHideUI(UIRoot.CanvasType.Bottom,"DownloadingSlider",false);
    }

    /// <summary>
    /// 更新界面上的下载信息
    /// </summary>
    /// <param name="sliderValue">下载进度条</param>
    /// <param name="progressStr">下载进度值</param>
    /// <param name="speedStr">下载速度</param>
    public void UpdateDownloadInfo(float sliderValue, string progressStr, string speedStr)
    {
        if (uiSlider != null)
            uiSlider.value = sliderValue;

        if (downloadText != null)
            downloadText.text = string.Format("正在努力下载资源({0}) {1}",progressStr,speedStr);
    }

    #endregion


    #region Unity周期函数

    private void Awake()
    {
        _instace = GetComponent<GameVersion>();
    }

    #endregion
}