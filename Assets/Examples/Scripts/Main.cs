using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main: MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        LuaManager.Instance.InitLua();

        LuaManager.Instance.DoFile("Examples/Main.lua");
        LuaManager.Instance.CallFunction("ExampleFunc");
    }
    
    private void OnDestroy()
    {
        LuaManager.TryResetInstance();
    }
}
