# Sunless Skies Steam 中文补丁

适用本机 Steam Windows **x86 / IL2CPP** 构建：Build ID **20365930**，游戏资源版本 **2.0.6.454aad8**，Unity **2019.4.2f1**。不是 Epic 版补丁，两个版本的加载器和 DLL 不可混用。

[返回仓库总览](../README.md) · [译文与审核详情](../translation-source/README.md) · [对应源码](../src/steam/README.md)

## 安装

1. 完全退出游戏。
2. 将本目录内的全部内容复制到 Steam 游戏的 `Sunless Skies.exe` 所在目录，合并文件夹。
3. 从 Steam 启动游戏。首次启动会生成 IL2CPP 互操作程序集，耗时较长，并可能从 unity.bepinex.dev 下载 Unity 基础库。翻译本身使用本地译文，不调用在线翻译服务。

游戏目录可在 Steam → 管理 → 浏览本地文件中打开。安装后的 winhttp.dll、doorstop_config.ini、dotnet/、BepInEx/ 和 overTMP 均与游戏 EXE 同层。

## 内容

- BepInEx 6.0.0-be.788，Unity.IL2CPP win-x86；官方构建提交 5b766a3。
- XUnity.AutoTranslator 5.6.2 IL2CPP、ResourceRedirector 2.1.0。
- 与仓库 Epic 补丁相同的 2026-10-04 ParaTranz 译文及未覆盖旧译文补充，53,884 行，含十二个月的日期适配。
- 中文 TMP 字体 overTMP，UGUI 使用 Microsoft YaHei。
- Steam 专用 TemplateLocalization 0.2.4，用于占位符展开前匹配模板，以及动态方向显示；源码见 [src/steam/](../src/steam/README.md)。

本版不使用 Epic 的 unstripped/、Mono 加载器和 Newtonsoft.Json 游戏运行库修复。XUnity 的 IL2CPP 版本不支持 TextGetterCompatibilityMode，因此配置为 False；R 键、菜单和港口交互需要在本构建中单独验证。

## 验证状态

加载日志已确认 IL2CPP 互操作程序集生成完成，XUnity 翻译插件与中文字体成功加载。Steam 专用模板插件 0.2.4 已成功安装挂钩并载入 1,236 个模板，136 个占位符不一致的映射被跳过。实际界面、动态方向和游戏交互仍待复测，不能视为完整游玩验证。

## 停用

退出游戏，把 doorstop_config.ini 的 enabled = true 改为 enabled = false。重启即停用补丁；不需要改动游戏原始数据和存档。

译文审核统计及来源见[仓库总览](../README.md)和[译文来源](../translation-source/README.md)。许可证见 THIRD_PARTY_LICENSES/。加载器来源：https://builds.bepinex.dev/projects/bepinex_be ，翻译插件来源：https://github.com/bbepis/XUnity.AutoTranslator 。本补丁并非官方汉化。

## 动态分支描述

模板插件 0.2.4 在游戏选出 qvd 分支后翻译返回的显示文本，不修改属性数值或分支条件。分支表位于 BepInEx/Translation/zh/qvd-branches.txt，随补丁安装；它从现有译文的 JSON 字符串分支按相同键路径提取，不由 XUnity 当作普通全文译文加载。

6,174 个无歧义映射中，6,173 个通过占位符校验；73 个存在多种译文的原文未加入分支表。优先使用分支译文，无对应映射时尝试现有独立句子译文，仍未命中则保留原文。仅在已翻译模板的动态展开期间生效。

黑色箱子的继承说明、缺失译文回退和占位符保护已通过离线检查；Steam 启动日志已确认新插件挂钩及分支表加载，具体事件仍需游戏内复测。
