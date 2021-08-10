using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaTools
{
    public static void SetMetaTable(LuaTable tableName)
    {
        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = LuaManager.luaEnv.NewTable();
        meta.Set("__index", LuaManager.luaEnv.Global);
        tableName.SetMetaTable(meta);
        meta.Dispose();
    }
}
