/*
Copyright (c) 2015-2016 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using LuaInterface;
using Cos.Common;

public class LuaManager : TSingletonMono<LuaManager>
{

    #region Singleton

    protected override void OnCreateInstance()
    {
        IsInited = false;
    }

    protected override void OnResetInstance()
    {
        if(!IsInited)
            return;

        Destroy();
        IsInited = false;
    }

    #endregion

    public bool IsInited { get; private set; }

    public LuaState CurrentLuaState
    {
        get
        {
            return luaState;
        }
    }

    protected LuaState luaState = null;
    protected LuaLooper loop = null;

    public void InitLua()
    {
        if(IsInited)
            return;

        luaState = new LuaState();

        OpenLibs();
        luaState.LuaSetTop(0);

        LuaBinder.Bind(luaState);
        LuaCoroutine.Register(luaState, this);

        luaState.Start();

        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = luaState;

        IsInited = true;
    }
    
    protected virtual void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
        luaState.OpenLibs(LuaDLL.luaopen_bit);
    }
        
    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        luaState.OpenLibs(LuaDLL.luaopen_cjson);
        luaState.LuaSetField(-2, "cjson");

        luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
        luaState.LuaSetField(-2, "cjson.safe");                               
    }
        
    public virtual void Destroy()
    {
        if (luaState != null)
        {
            LuaState state = luaState;
            luaState = null;
            
            if (loop != null)
            {
                loop.Destroy();
                loop = null;
            }

            state.Dispose();
        }
    }


    #region Interface

    public object[] DoFile(string fileName)
    {
        return luaState.DoFile(fileName);
    }

    public object[] CallFunction(string funcName, params object[] args)
    {
        var func = luaState.GetFunction(funcName);
        object[] result = null;
        if(func != null)
        {
            result = func.Call(args);
            func.Dispose();
            func = null;
        }
        return result;
    }

    #endregion
}
