using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using XLua;

public class LuaManager : UnitySingleton<LuaManager>
{
    private static string luaScriptsPath = "LocalResources/0.Lua-ab/Utility";
    private static LuaEnv _luaEnv = null;
    
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
        luaEnv.AddLoader(CustomLoader);
    }

    public static byte[] CustomLoader(ref string filePath)
    {
        string scriptPath = string.Empty;
        filePath = filePath.Replace(".", "/") + ".lua.txt";

#if UNITY_EDITOR
        scriptPath = Path.Combine(Application.dataPath, luaScriptsPath);
        scriptPath = Path.Combine(scriptPath, filePath);

        byte[] data = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(scriptPath));
        return data;
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
