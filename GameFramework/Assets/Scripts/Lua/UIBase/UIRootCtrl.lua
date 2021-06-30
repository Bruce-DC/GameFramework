---
---  UI根预设_控制脚本
---
---  功能:
---      1: 加载UIRootView UI面板（预设）。
---      2： 给UI面板（预设），动态添加 "BaseLuaUIForm" 的lua脚本。
---      3： "BaseLuaUIForm" 脚本，通过xlua的 C#--->lua映射，使得
---          与“UI预设”相同名称的lua脚本，获取Unity的生命周期。
---
---
---
--- Created by Administrator.
--- DateTime: 2019/5/28
---

UIRootCtrl={}
local this=UIRootCtrl
--定义项目参数
local string scenesNameByUIRoot="mainscenes"  --场景名称参数
local string packageNameByUIRoot="mainscenes/mainscenes.ab" --AB包名称参数
local string assetNameByUIRoot="UIRootView.prefab"   --资源名称参数

--调用C#的ABFW框架
local abMgrClass=CS.ABFW.AssetBundleMgr
local abMgrObj=abMgrClass.GetInstance()

--得到实例
function UIRootCtrl.GetInstance()
    return this
end

--开始处理核心逻辑(开始处理)
function UIRootCtrl.StartProcess()
    --加载AB包
    abMgrObj:LoadAssetBundlePackage(scenesNameByUIRoot,packageNameByUIRoot,this.ProcessComplete)

end

--处理核心逻辑完毕的回调函数
function UIRootCtrl.ProcessComplete()
    --print("回调函数 正确显示！")
    --调用AB包中的资源
    local tmpObj=abMgrObj:LoadAsset(scenesNameByUIRoot,packageNameByUIRoot,assetNameByUIRoot,false)
    if (tmpObj~=nil) then
        local cloneObj=CS.UnityEngine.Object.Instantiate(tmpObj)
        --对于克隆出来的uI预设，动态添加 "BaseLuaUIForm"
        CS.LuaFramework.LuaHelper.GetInstance():AddBaseLuaUIForm(cloneObj)

        --查找UI预设上button 组件。
        local btnStart=UIRootView.BtnStart
        --增加Button 的监听。
        btnStart.onClick:AddListener(this.ProcessBtnStart)

    end
end


--定义Button 的监听事件（回调方法）
function UIRootCtrl.ProcessBtnStart()
    --print("@@@@@@@  定义Button 的监听事件（回调方法）")
    --加载游戏主窗体
    CtrlMgr.StartProcess(CtrlName.TaskCtrl)
end












