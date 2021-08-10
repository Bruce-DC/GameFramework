using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LuaToCSharp
{
    public static string GetLuaString(string luaFileName)
    {
        if (string.IsNullOrEmpty(luaFileName))
        {
            Debug.LogError("你想找一个没有文件名的Lua文件？这有点难为人吧～");
            return null;
        }
        
        string result="";

        string fullPath = "Assets/LocalResources/";
        DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.lua", SearchOption.AllDirectories);
        
        for (int i = 0; i < fileInfos.Length; i++)
        {
            
            if (fileInfos[i].Name == luaFileName+".lua")
            {
                string str = File.ReadAllText(fileInfos[i].FullName);
                return fileInfos[i].ToString();
            }
        }
        
        Debug.LogError("找不到名为\""+luaFileName+"\"的Lua文件！");
        return null;
    }
}
