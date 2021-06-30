---
--- “lua框架”定义所有的常量与“枚举”（表）
---
--- Created by Administrator.
--- DateTime: 2019/5/28
---

--项目中用到的控制层脚本名称
CtrlName={
            UIRootCtrl="UIRootCtrl",
            TaskCtrl="TaskCtrl"
         }
--项目中用到的视图层脚本名称
ViewNames={
            "UIRootView","TaskView"
}

--引入Unity内置的类型
WWW=CS.UnityEngine.WWW
GameObject=CS.UnityEngine.GameObject