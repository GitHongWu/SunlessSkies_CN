# 通用显示模板适配

`TemplateLocalization.cs` 是随 patch/ 发布的插件的最终源码。它读取合并后的 ui.txt，不保存另一份译文。ParaTranz 与旧译文的优先级沿用现有译文表。

安装时不需要此目录。已编译的 DLL 位于 patch/BepInEx/plugins/SunlessSkies.TemplateLocalization/。本目录仅保留对应源码，不附带构建、测试或调试脚本。0.2.2 已由用户确认交易事件正文与动态方向显示中文，其他场景仍需验证。

插件对游戏的 BaseStoryFormatter.ReplaceAllTokens、ReplaceFormattingTokens 和 DynamicTokenReplacement.ReplaceDynamicVariableNameTokens 添加前置匹配；只对已有占位符且译文占位符集合相同的原文进行替换。GetDirectionForToken 的后置处理仅在中文模板展开期间生效。异常退出时通过 Harmony finalizer 恢复作用域。

游戏接口不提供词条 ID，所以相同原文的语境歧义仍遵循合并表的冲突选择。缺失原文、缺失译文、措辞变化以及绕过这些入口的文本不属于本插件能自动解决的范围。测试不替代 Epic 2.0.7.0 中的实际事件、交互与性能验证。
