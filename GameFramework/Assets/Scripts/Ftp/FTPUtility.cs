using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FTPUtility : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// FTPUtility对象，用于维护单例
    /// </summary>
    private static FTPUtility _instance;

    /// <summary>
    /// FTPUtility对象(get属性)，用于维护单例
    /// </summary>
    public static FTPUtility Instance
    {
        get
        {
            if (_instance == null)
                _instance = new FTPUtility();

            return _instance;
        }
    }
    
    /// <summary>
    /// FTP服务器根目录IP地址
    /// </summary>
    private string ftpRootURL = "ftp://121.43.191.40:21/";
    
    /// <summary>
    /// 待下载文件的url地址
    /// </summary>
    private string downloadURL;
    
    /// <summary>
    /// 待下载的文件大小
    /// </summary>
    private long downloadFileSize = 0;
    
    /// <summary>
    /// 是否正在下载中
    /// </summary>
    private bool isDownloading = true;
    
    /// <summary>
    /// 每0.2秒更新一次界面上的下载速度
    /// </summary>
    private float updateDownloadSpeedTime = 0.2f;

    /// <summary>
    /// 更新下载速度前，文件下载的大小
    /// </summary>
    private long preDownloadSize = 0;

    /// <summary>
    /// 更新下载速度的时候，文件当前已经下载的大小
    /// </summary>
    private long curDownloadSize = 0;
    
    /// <summary>
    /// 客户端本地存储主版本的文件的路径
    /// </summary>
    string masterVersionFilePath;

    /// <summary>
    /// 客户端本地存储主版本的文件的名称
    /// </summary>
    private string masterVersionFileName = "MasterVersion.txt";
    
    /// <summary>
    /// 供测试下载使用的AssetBundle包名
    /// </summary>
    public string testDownloadFile = "ab_texture";

    /// <summary>
    /// WebClient对象，用于方便获取下载过程中的数据
    /// </summary>
    private WebClient wb;

    /// <summary>
    /// WebClient对象(get属性)，用于方便获取下载过程中的数据
    /// </summary>
    private WebClient WB
    {
        get
        {
            if (wb == null)
            {
                wb = new WebClient();
                wb.Proxy = null;
                wb.Credentials = new NetworkCredential("ftptest", "boshi0513");
                wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                wb.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadComplete);
            }

            return wb;
        }
    }
    #endregion

    #region 成员方法
    /// <summary>
    /// 下载进度发生变化时调用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        curDownloadSize = e.BytesReceived;
        
        float sliderValue = (float) e.BytesReceived / (float) downloadFileSize;
        string progressStr = string.Format("{0} MB / {1} MB", (e.BytesReceived / 1024d / 1024d).ToString("0.00"), (downloadFileSize / 1024d / 1024d).ToString("0.00"));
        GameVersion.Instance.UpdateDownloadInfo(sliderValue,progressStr);
    }

    /// <summary>
    /// 下载文件完成时调用
    /// 正常下载完成和下载被中断时都会调用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DownloadComplete(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Debug.LogError("玩家取消下载");
        }
    }
    
    /// <summary>
    /// 创建FTP请求
    /// </summary>
    /// <param name="url">FTP请求的URL</param>
    /// <param name="methodName">FTP请求要做的操作，如：获取文件大小、下载、上传等</param>
    /// <returns></returns>
    public FtpWebRequest CreatFtpWebRequest(string url, string methodName)
    {
        FtpWebRequest ftpWebRequest = (FtpWebRequest) FtpWebRequest.Create(url);
        ftpWebRequest.Credentials = new NetworkCredential("ftptest", "boshi0513");
        ftpWebRequest.KeepAlive = true;
        ftpWebRequest.UseBinary = true;
        ftpWebRequest.Method = methodName;
        return ftpWebRequest;
    }
    
    /// <summary>
    /// 通过WebClient下载文件
    /// </summary>
    /// <param name="fileName">需要下载文件的文件名</param>
    public void DownloadFileByWebClient(string fileName)
    {
        downloadURL = String.Format("{0}{1}/{2}",ftpRootURL, Utility.GetPlatform(),fileName);
        
        try
        {
            //获取待下载文件的大小
            FtpWebRequest resFile = CreatFtpWebRequest(downloadURL, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse res = (FtpWebResponse) resFile.GetResponse())
            {
                downloadFileSize = res.ContentLength;
                if (res == null)
                {
                    Debug.LogError("获取资源大小出错");
                    return;
                }

                isDownloading = true;
                WB.DownloadFileAsync(new Uri(downloadURL),Application.persistentDataPath + "/SaveFiles/" + downloadURL.Replace(ftpRootURL, ""));
            }
        }
        catch (Exception e)
        {
            Debug.Log("Download filed: " + e.Message);
        }
    }

    /// <summary>
    /// 初始化PersistentDataPath
    /// 第一次启动时可能会不存在该路径，所以在下载文件之前要先把路径创建好
    /// </summary>
    private void InitPersistentDataPath()
    {
        string path = string.Format("{0}{1}/{2}", Application.persistentDataPath, "/SaveFiles/", Utility.GetPlatform());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    #endregion
    
    
    #region Unity生命周期函数
    private void Awake()
    {
        InitPersistentDataPath();
    }

    private void Update()
    {
        if (isDownloading)
        {
            updateDownloadSpeedTime -= Time.deltaTime;
            if (updateDownloadSpeedTime <= 0.0f)
            {
                updateDownloadSpeedTime += 0.2f;

                if (curDownloadSize > preDownloadSize)
                {
                    GameVersion.Instance.UpdateDownloadSpeed(string.Format("下载速度为：{0}kb/s",
                        ((curDownloadSize - preDownloadSize) / 1024 / 0.2f).ToString("0.00")));
                    preDownloadSize = curDownloadSize;
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        if (WB != null)
        {
            //被销毁时关闭WebClient，否则即使销毁，下载的线程依然在跑
            WB.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(ProgressChanged);
            WB.CancelAsync();
        }

        isDownloading = false;
    }
    #endregion
}