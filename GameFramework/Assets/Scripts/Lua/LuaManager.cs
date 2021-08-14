using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using XLua;

public class LuaManager : UnitySingleton<LuaManager>
{
    private static string luaScriptsPath = "/LocalResources/0.Lua-ab/";
    private static LuaEnv _luaEnv = null;
    private Dictionary<string, string> luaFileDic_Editor;
    private Dictionary<string, string> luaFileDic;
    
    public static LuaEnv luaEnv
    {
        get
        {
            if (_luaEnv==null)
            {
                _luaEnv = new LuaEnv();
            }

            return _luaEnv;
        }
    }
    
    public override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        luaFileDic_Editor = new Dictionary<string, string>();
        luaFileDic_Editor = LoadAlllLuaFile(Application.dataPath+luaScriptsPath, luaFileDic_Editor);
#else
        luaFileDic = new Dictionary<string, string>();
        luaFileDic = LoadAlllLuaFile(Application.dataPath+luaScriptsPath, luaFileDic);
#endif
        luaEnv.AddLoader(CustomLoader);
    }

    private Dictionary<string, string> LoadAlllLuaFile(string luaPath, Dictionary<string, string> dictionary)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(luaPath);
        var files = directoryInfo.GetFiles("*.lua.txt");
        var dii = directoryInfo.GetDirectories();
        for (int i = 0; i < files.Length; i++)
        {
#if UNITY_EDITOR
            if (luaFileDic_Editor.ContainsKey(files[i].Name))
            {
                Debug.LogError("存在重名的lua文件："+files[i].Name);
            }
            else
            {
                luaFileDic_Editor.Add(files[i].Name, files[i].FullName);   
            }
#else
            if (luaFileDic.ContainsKey(files[i].Name))
            {
                Debug.LogError("存在重名的lua文件："+files[i].Name);
            }
            else
            {
                luaFileDic.Add(files[i].Name, files[i].FullName);   
            }
#endif
        }

        foreach (DirectoryInfo d in dii)
        {
            LoadAlllLuaFile(d.FullName, dictionary);
        }

        return dictionary;
    }

    public byte[] CustomLoader(ref string filePath)
    {
        filePath = filePath.Replace(".", "/") + ".lua.txt";
#if UNITY_EDITOR
        if (luaFileDic_Editor.ContainsKey(filePath))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(luaFileDic_Editor[filePath]));
            return data;
        }
        else
        {
            Debug.LogError("找不到要加载的lua文件："+filePath);
            return null;
        }
#elif !UNITY_EDITOR && UNITY_IOS

#elif !UNITY_EDITOR && UNITY_ANDROID
        
#endif
        return null;
    }

    public void CallLuaFunction(string functionName)
    {
        luaEnv.DoString(functionName);
    }

    public void RequireLuaScript(string scriptName)
    {
        luaEnv.DoString("require('" + scriptName + "')");
    }

    private void OnDestroy()
    {
        luaEnv.Dispose();
        _luaEnv = null;
    }
}
