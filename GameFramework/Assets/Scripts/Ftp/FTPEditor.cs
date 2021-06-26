using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 主要负责在unity编辑器下上传构建好的AssetBundle
///
/// </summary>
public class FTPEditor : Editor
{
    /// <summary>
    /// 上传文件功能
    /// </summary>
    /// <param name="newFileName">上传文件的文件名</param>
    /// <returns></returns>
    //[MenuItem("Assets/上传AssetBundle到FTP服务器", false, 600)]
    public static void UploadFile(string newFileName)
    {
        long uploadFileAlreadyLength = 0;

        string url = FTPUtility.GetUploadUrl(newFileName);
        string filepath = Application.dataPath + "/StreamingAssets/" + newFileName;
        Debug.LogError(url);
        Debug.LogError(filepath);
        //return;

        FtpWebRequest request = FTPUtility.CreatFtpWebRequest(url, WebRequestMethods.Ftp.UploadFile);
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
    }
}
