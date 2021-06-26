using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FTPUtility : MonoBehaviour
{
    private void Start()
    {
        //ShowFtpFileAndDirectory("ftp://121.43.191.40:21/Windows/");

        //ShowFtpFileAndDirectory("ftp://121.43.191.40:21/WindowsPlayer/");
        //StartCoroutine(DownloadFile("ftp://121.43.191.40:21/Windows/问号.png"));
        
        FTPEditor.UploadFile("upload.txt");
    }

    private static string _commonUrl = "ftp://121.43.191.40:21";

    public static string GetFilePath(string fileName)
    {
        return string.Format("{0}/{1}", _commonUrl, fileName);
    }

    public static string GetUploadUrl(string fileName)
    {
        return string.Format("{0}/{1}/{2}", _commonUrl, GetUploadPlatform(),fileName);
    }

    private static string GetUploadPlatform()
    {
        string result = "";

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                result = "Android";
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                result = "WindowsPlayer";
                break;
            case RuntimePlatform.IPhonePlayer:
                result = "IPhonePlayer";
                break;
            default:
                Debug.LogError("当前平台不支持，请确认平台！");
                break;
        }

        return result;
    }

    public static FtpWebRequest CreatFtpWebRequest(string url, string methodName)
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
    public IEnumerator DownloadFile(string url)
    {
        long downloadFileAllLength = 0;
        long downloadFileAlreadyLength = 0;
        string savedFilePath = Application.dataPath + "/SaveFiles/" + url.Replace("ftp://121.43.191.40:21/", "");
        try
        {
            //获取待下载文件的大小
            FtpWebRequest resFile = CreatFtpWebRequest(url, WebRequestMethods.Ftp.GetFileSize);
            using (FtpWebResponse res = (FtpWebResponse) resFile.GetResponse())
            {
                Debug.Log("All Length: " + res.ContentLength);
                downloadFileAllLength = res.ContentLength;
                //slider.maxValue = res.ContentLength;
                if (res == null)
                {
                    yield break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Download filed: " + e.Message);
        }

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
            //Debug.Log("BytesRead:" + bytesRead + "AlreadyRead:" + downloadFileAlreadyLength);
            //此处用于使用协程更新下载进度条，避免大文件的下载直接导致unity的假死
            //yield return StartCoroutine(UpdateUISlider(downloadFileAlreadyLength));
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Download success!");
        fileStream.Close();

        //yield return new WaitForSeconds(0.1f);
        //AssetDatabase.Refresh();
    }
}