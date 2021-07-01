using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FTPUtility : MonoBehaviour
{
    private void Start()
    {
        InitpersistentDataPath();
        //ShowFtpFileAndDirectory("ftp://121.43.191.40:21/Windows/");

        //ShowFtpFileAndDirectory("ftp://121.43.191.40:21/WindowsPlayer/");
        //StartCoroutine(Download("ftp://121.43.191.40:21/Windows/MasterVersion.txt"));

        //FTPEditor.UploadFile("upload.txt");
        
        masterVersionFilePath = String.Format("{0}/SaveFiles/{1}/{2}", Application.persistentDataPath, Utility.GetPlatform(), masterVersionFileName);

        CheckVersion();
    }

    public Slider uiSlider;
    
    string masterVersionFilePath;
    private string masterVersionFileName = "MasterVersion.txt";

    private string testDownloadFile = "ab_texture";

    private long downloadFileSize = 0;   //待下载的文件大小

    void CheckVersion()
    {
        if (File.Exists(masterVersionFilePath))
        {
            Debug.Log("判断版本是否一致");
            ///获取服务端的版本号，与本地文件中的对比，如果一致则继续启动流程
            /// 如果不一致，判断是否需要强更包(一般不用)，需要强更时提示玩家进入商店下载更新
            /// 不需要强更时，直接更新不同的资源和新的资源
            ///

            
            
            downloadURL = String.Format("{0}{1}/{2}",ftpRootURL, Utility.GetPlatform(),testDownloadFile);
            
            //Debug.Log(Application.persistentDataPath + "/SaveFiles/" + downloadURL.Replace(ftpRootURL, ""));
            
            WebClient wb = new WebClient();
            wb.Credentials = new NetworkCredential("ftptest", "boshi0513");

            wb.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            wb.DownloadFileAsync(new Uri(downloadURL),Application.persistentDataPath + "/SaveFiles/" + downloadURL.Replace(ftpRootURL, ""));
            
            //wb.DownloadFile(downloadURL,Application.persistentDataPath + "/SaveFiles/" + downloadURL.Replace(ftpRootURL, ""));
            
            DownloadFile(testDownloadFile);
        }
        else
        {
            Debug.Log("版本文件不存在，第一次启动游戏，更新之。");
            DownloadFile(masterVersionFileName);
        }
    }
    
    private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
        Debug.LogError(string.Format("{0} MB / {1} MB", (e.BytesReceived / 1024d / 1024d).ToString("0.00"),  (GetDownloadFileSize(downloadURL) / 1024d / 1024d).ToString("0.00")));
        
        
        // //下载的总量
        // PrecentData preData = new PrecentData();
        // preData.total = string.Format("{0} MB / {1} MB", (e.BytesReceived / 1024d / 1024d).ToString("0.00"),  (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        // preData.precent = (float)e.BytesReceived / (float)e.TotalBytesToReceive;
        //
        //
        //
        // string value = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));
        //
        // preData.speed = value;
        //
        // Loom.QueueOnMainThread((param) =>
        // {
        //     NotificationCenter.Get().ObjDispatchEvent(KEventKey.m_evDownload, preData);
        // }, null);
        //
        //
        // NotiData data = new NotiData(NotiConst.UPDATE_PROGRESS, value);
        // if (m_SyncEvent != null) m_SyncEvent(data);
        //
        // if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive) {
        //     sw.Reset();
        //
        //     data = new NotiData(NotiConst.UPDATE_DOWNLOAD, currDownFile);
        //     if (m_SyncEvent != null) m_SyncEvent(data);
        // }
    }

    private string ftpRootURL = "ftp://121.43.191.40:21/";
    private string downloadURL;
    
    private static FTPUtility _instance;

    public static FTPUtility GetInstance()
    {
        if (_instance == null)
            _instance = new FTPUtility();

        return _instance;
    }

    public FtpWebRequest CreatFtpWebRequest(string url, string methodName)
    {
        FtpWebRequest ftpWebRequest = (FtpWebRequest) FtpWebRequest.Create(url);
        ftpWebRequest.Credentials = new NetworkCredential("ftptest", "boshi0513");
        ftpWebRequest.KeepAlive = true;
        ftpWebRequest.UseBinary = true;
        ftpWebRequest.Method = methodName;
        return ftpWebRequest;
    }

    private FtpWebResponse GetFtpResponse(FtpWebRequest request)
    {
        return (FtpWebResponse) request.GetResponse();
    }

    /// <summary>
    /// 展示FTP连接后的遍历结果
    /// </summary>
    /// <param name="url">FTP服务器地址路径</param>
    /// <returns>是否连接上FTP服务器</returns>
    private bool ShowFtpFileAndDirectory(string url)
    {
        try
        {
            FtpWebRequest request = CreatFtpWebRequest(url, WebRequestMethods.Ftp.ListDirectoryDetails);
            FtpWebResponse response = GetFtpResponse(request);

            if (response == null)
            {
                Debug.Log("Response is null");
                return false;
            }

            //读取网络流数据
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.Default);
            string s = streamReader.ReadToEnd();
            streamReader.Close();
            response.Close();

            //处理并显示文件目录列表
            string[] ftpDir = s.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int length = 0;
            for (int i = 0; i < ftpDir.Length; i++)
            {
                if (ftpDir[i].EndsWith("."))
                {
                    length = ftpDir[i].Length - 2;
                    break;
                }
            }

            for (int i = 0; i < ftpDir.Length; i++)
            {
                s = ftpDir[i];
                int index = s.LastIndexOf('\t');
                if (index == -1)
                {
                    if (length < s.Length)
                    {
                        index = length;
                    }
                    else
                    {
                        continue;
                    }
                }

                string name = s.Substring(index + 1);
                if (name == "." || name == "..")
                {
                    continue;
                }

                //判断是否为目录，在名称前加“Dirctory"来表示
                if (s[0] == 'd' || (s.ToLower()).Contains("<dir>"))
                {
                    string[] namefiled = name.Split(' ');
                    int namefiledlength = namefiled.Length;
                    string dirname;
                    dirname = namefiled[namefiledlength - 1];
                    dirname = dirname.PadRight(34, ' ');
                    name = dirname;
                    Debug.Log("<Dir>:" + name);
                }
            }

            for (int i = 0; i < ftpDir.Length; i++)
            {
                s = ftpDir[i];
                int index = s.LastIndexOf("\t");
                if (index == -1)
                {
                    if (length < s.Length)
                    {
                        index = length;
                    }
                    else
                    {
                        continue;
                    }
                }

                string name = s.Substring(index + 1);
                if (name == "." || name == "..")
                {
                    continue;
                }

                if (!((s[0] == 'd') || (s.ToLower().Contains("<dir>"))))
                {
                    string[] namefiled = name.Split(' ');
                    int namefiledlength = namefiled.Length;
                    string filename;
                    filename = namefiled[namefiledlength - 1];
                    filename = filename.PadRight(34, ' ');
                    name = filename;
                    Debug.Log("<File>:" + name);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url">待下载的文件路径，形如ftp://111.111.11.1/test.txt</param>
    private IEnumerator Download(string url)
    {
        Debug.Log("Download");
        long downloadFileAllLength = 0;
        long downloadFileAlreadyLength = 0;
        string savedFilePath = Application.persistentDataPath + "/SaveFiles/" + url.Replace(ftpRootURL, "");
        Debug.Log(savedFilePath);
        

        //开始下载
        FtpWebRequest request = CreatFtpWebRequest(url, WebRequestMethods.Ftp.DownloadFile);
        FtpWebResponse response = GetFtpResponse(request);
        Stream responseStream = response.GetResponseStream();
        FileStream fileStream = File.Create(savedFilePath);
        int buflength = 8196;
        byte[] buffer = new byte[buflength];
        int bytesRead = 1;

        while (bytesRead != 0)
        {
            bytesRead = responseStream.Read(buffer, 0, buflength);
            fileStream.Write(buffer, 0, bytesRead);
            downloadFileAlreadyLength += bytesRead;
            Debug.Log("BytesRead:" + bytesRead + "AlreadyRead:" + downloadFileAlreadyLength);
            //此处用于使用协程更新下载进度条，避免大文件的下载直接导致unity的假死
            //yield return StartCoroutine(UpdateUISlider(downloadFileAlreadyLength));
            //uiSlider.value = downloadFileAlreadyLength/downloadFileAllLength;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Download success!");
        fileStream.Close();

        yield return new WaitForSeconds(0.1f);
        AssetDatabase.Refresh();
    }

    private long GetDownloadFileSize(string fileURL)
    {
        long downloadFileAllLength = 0;
        try
        {
            //获取待下载文件的大小
            FtpWebRequest resFile = CreatFtpWebRequest(fileURL, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse res = (FtpWebResponse) resFile.GetResponse())
            {
                Debug.Log("All Length: " + res.ContentLength);
                downloadFileAllLength = res.ContentLength;
                //slider.maxValue = res.ContentLength;
                if (res == null)
                {
                    Debug.LogError("获取资源大小出错");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Download filed: " + e.Message);
        }

        return downloadFileAllLength;
    }

    IEnumerator UpdateUISlider(long length)
    {
        uiSlider.value = length;
        yield return null;
    }

    public void DownloadFile(string fileName)
    {
        downloadURL = String.Format("{0}{1}/{2}",ftpRootURL, Utility.GetPlatform(),fileName);
        Debug.Log("准备下载文件："+fileName+",文件地址为："+downloadURL);
        StartCoroutine(GetInstance().Download(downloadURL));
    }

    private void InitpersistentDataPath()
    {
        string path = string.Format("{0}{1}/{2}", Application.persistentDataPath, "/SaveFiles/", Utility.GetPlatform());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}