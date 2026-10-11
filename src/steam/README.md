# Steam IL2CPP 模板适配

TemplateLocalization.Steam.cs 对应 patch-steam/BepInEx/plugins/SunlessSkies.TemplateLocalization/SunlessSkies.TemplateLocalization.Steam.dll，插件版本 0.2.9。

沿用 Epic 的模板词典、占位符校验和方向处理，入口改为 BepInEx.Unity.IL2CPP.BasePlugin.Load。使用 .NET 6 编译，引用 Steam 包中的 BepInEx.Core.dll、BepInEx.Unity.IL2CPP.dll、0Harmony.dll；不引用 Epic 的 Mono 插件。

运行时读取相邻的 BepInEx/Translation/zh/Text/ui.txt，启动前检查五个游戏方法签名。编译及游戏启动挂钩验证通过，实际游玩验证状态见 [Steam 补丁说明](../../patch-steam/README.md)。

## 动态分支描述

模板插件在游戏选出 qvd 分支后翻译返回的显示文本，不修改属性数值或分支条件。分支表位于 BepInEx/Translation/zh/qvd-branches.txt，随补丁安装；它从现有译文的 JSON 字符串分支按相同键路径提取，不由 XUnity 当作普通全文译文加载。

当前 5,501 个无歧义映射中，5,500 个通过占位符校验；63 个存在多种译文的原文未加入分支表。优先使用分支译文，无对应映射时尝试现有独立句子译文，仍未命中则保留原文。仅在已翻译模板的动态展开期间生效。

