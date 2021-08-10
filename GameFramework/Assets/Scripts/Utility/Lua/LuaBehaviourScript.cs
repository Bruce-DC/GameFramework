using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviourScript : MonoBehaviour
{
    public TextAsset luaScript;
    
    private LuaTable _luaTable;

    private Action start;
    private Action update;
    private Action destory;
    private void Awake()
    {
        _luaTable = LuaManager.luaEnv.NewTable();
        
        LuaTools.SetMetaTable(_luaTable);
        
        // LuaManager.luaEnv.DoString(LuaToCSharp.GetLuaString("Test"), "Test", _luaTable);
        LuaManager.luaEnv.DoString(luaScript.text, "Test", _luaTable);
        
        _luaTable.Get("start",out start);
        _luaTable.Get("update",out update);
        _luaTable.Get("destory",out destory);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (start!=null)
        {
            start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (update!=null)
        {
            update();
        }
    }

    private void OnDestroy()
    {
        if (destory!=null)
        {
            destory();
        }
    }
}
