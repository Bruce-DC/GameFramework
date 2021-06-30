---
---   任务面板 控制层脚本
---
---   功能：
---       主要就是加载“游戏UI主窗体预设”
---
--- Created by Administrator.
--- DateTime: 2019/5/28
---

TaskCtrl={}
local this=TaskCtrl

--定义项目参数
local string scenesNameByTask="mainscenes"  --场景名称参数
local string packageNameByTask="mainscenes/mainscenes.ab" --AB包名称参数
local string assetNameByTask="TaskView.prefab"   --资源名称参数

--调用C#的ABFW框架
local abMgrClass=CS.ABFW.AssetBundleMgr
local abMgrObj=abMgrClass.GetInstance()

--得到实例
function TaskCtrl.GetInstance()
    return this
end

--开始处理核心逻辑(开始处理)
function TaskCtrl.StartProcess()
    --print("@@@@@@  TaskCtrl.StartProcess 启动成功！！！ ")
    --调用AB包中的资源
    local tmpObj=abMgrObj:LoadAsset(scenesNameByTask,packageNameByTask,assetNameByTask,false)
    if (tmpObj~=nil) then
        local cloneObj=CS.UnityEngine.Object.Instantiate(tmpObj)
        --对于克隆出来的uI预设，动态添加 "BaseLuaUIForm"
        CS.LuaFramework.LuaHelper.GetInstance():AddBaseLuaUIForm(cloneObj)

    end

end
