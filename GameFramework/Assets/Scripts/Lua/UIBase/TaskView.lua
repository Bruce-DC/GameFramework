---
---  任务面板 显示层脚本
---
--- Created by Administrator.
--- DateTime: 2019/5/28 14:23
---

TaskView={}
local this=TaskView

--说明:
--输入参数： obj 表示UI窗体对象。
function TaskView.Awake(obj)
    print("TaskView.Awake")
end

function TaskView.Start(obj)
    print("TaskView.Start")
end

function TaskView.Update(obj)
    print("TaskView.Update")
end