# 通用显示模板适配

`TemplateLocalization.cs` 是随 patch-epic/ 发布的插件的最终源码。它读取合并后的 ui.txt，不保存另一份译文。ParaTranz 与旧译文的优先级沿用现有译文表。

安装时不需要此目录。已编译的 DLL 位于 patch-epic/BepInEx/plugins/SunlessSkies.TemplateLocalization/。本目录仅保留对应源码，不附带构建、测试或调试脚本。0.2.2 已由用户确认交易事件正文与动态方向显示中文，其他场景仍需验证。

插件对游戏的 BaseStoryFormatter.ReplaceAllTokens、ReplaceFormattingTokens 和 DynamicTokenReplacement.ReplaceDynamicVariableNameTokens 添加前置匹配；只对已有占位符且译文占位符集合相同的原文进行替换。GetDirectionForToken 的后置处理仅在中文模板展开期间生效。异常退出时通过 Harmony finalizer 恢复作用域。

游戏接口不提供词条 ID，所以相同原文的语境歧义仍遵循合并表的冲突选择。缺失原文、缺失译文、措辞变化以及绕过这些入口的文本不属于本插件能自动解决的范围。测试不替代 Epic 2.0.7.0 中的实际事件、交互与性能验证。

安装与验证状态见 [Epic 补丁说明](../../patch-epic/README.md)。当前插件版本 0.2.4 是自有模板插件的版本，不是游戏版本或 GitHub Release 版本。

## 动态分支描述

模板插件 0.2.4 在游戏选出 qvd 分支后翻译返回的显示文本，不修改属性数值或分支条件。分支表位于 BepInEx/Translation/zh/qvd-branches.txt，随补丁安装；它从现有译文的 JSON 字符串分支按相同键路径提取，不由 XUnity 当作普通全文译文加载。

6,174 个无歧义映射中，6,173 个通过占位符校验；73 个存在多种译文的原文未加入分支表。优先使用分支译文，无对应映射时尝试现有独立句子译文，仍未命中则保留原文。仅在已翻译模板的动态展开期间生效。

黑色箱子的继承说明、缺失译文回退和占位符保护已通过离线检查；Steam 启动日志已确认新插件挂钩及分支表加载，具体事件仍需游戏内复测。
