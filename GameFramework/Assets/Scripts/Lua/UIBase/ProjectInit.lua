---
---  “lua框架”项目初始化
---
---   功能：
---      1： 引入项目中所有的视图层脚本
---      2： 通过CtrlMgr.lua （控制层）脚本，来缓存系统中所有其他控制层脚本。
---      3： 提供访问其他控制层脚本的入口函数。
---      4： 调用项目中第一个UI预设控制层脚本。
---
--- Created by Administrator.
--- DateTime: 2019/5/28
---

--引入控制层管理器脚本
require("CtrlMgr")

ProjectInit={}
local this=ProjectInit

function ProjectInit.Init()
    --导入引入项目中所有的视图层脚本
    this.ImportAllViews()
    --lua控制器初始化
    CtrlMgr.Init()
    --加载UI‘根窗体’控制脚本
    CtrlMgr.StartProcess(CtrlName.UIRootCtrl)
end

--导入引入项目中所有的视图层脚本
function ProjectInit.ImportAllViews()
    for i = 1, #ViewNames do
        require(tostring(ViewNames[i]))
    end
end
















