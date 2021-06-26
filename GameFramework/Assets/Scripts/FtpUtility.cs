using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class FtpUtility:MonoBehaviour
{
    private void Start()
    {
        ShowFtpFileAndDirectory("ftp://121.43.191.40:21/");
    }

    private FtpWebRequest CreatFtpWebRequest(string url,string methodName)
    {
        FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(url);
        ftpWebRequest.Credentials = new NetworkCredential("ftptest","boshi0513");
        ftpWebRequest.KeepAlive = true;
        ftpWebRequest.UseBinary = true;
        ftpWebRequest.Method = methodName;
        return ftpWebRequest;
    }

    private FtpWebResponse GetFtpResponse(FtpWebRequest request)
    {
        return (FtpWebResponse)request.GetResponse();
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
}
