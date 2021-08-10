using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        
        //GameVersion.Instance.CheckVersion();
        
        LuaManager.Instance.RequireLuaScript("LuaManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(NetWorkScript.Instance!=null)
            NetWorkScript.Instance.OnDestroy();
    }
}
