# Epic 版安装与使用

适用于 Windows x64、Epic Games 版 Sunless Skies 2.0.7.0（Unity 2019.4.2f1 / Mono）。使用 BepInEx 5.4.23.5、XUnity.AutoTranslator 5.6.2 和 ResourceRedirector 2.1.0。

[返回仓库总览](../README.md) · [译文与审核详情](../translation-source/README.md) · [对应源码](../src/epic/README.md)

## 安装

1. 完全退出游戏。
2. 下载并解压本仓库，打开 **`patch-epic/`**。
3. 将 **`patch-epic/` 内的全部内容**复制到 **`Sunless Skies.exe` 所在目录**，合并文件夹。
4. 启动游戏。

不要只复制 `BepInEx/`，也不要把 `patch-epic` 文件夹本身套在游戏目录里。安装后应为：

```text
游戏目录/
├── Sunless Skies.exe          # 游戏原有文件
├── winhttp.dll
├── doorstop_config.ini
├── .doorstop_version
├── overTMP
├── THIRD_PARTY_LICENSES/
└── BepInEx/
    ├── core/
    ├── plugins/
    ├── config/AutoTranslatorConfig.ini
    ├── Translation/zh/Text/ui.txt
    └── unstripped/
        ├── Newtonsoft.Json.dll
        └── …                 # 完整保留本包内的其余运行库
```

**`patch-epic/` 已包含全部补丁运行文件，包括修复版 `Newtonsoft.Json.dll`，安装不依赖译文来源资料或任何额外脚本。** 不需要配置 API 密钥或在线翻译服务。传统 UGUI 文本使用系统的 Microsoft YaHei 字体。

已有其他 BepInEx 或汉化补丁时，先备份并检查兼容性。升级本补丁时覆盖 `patch-epic/` 内全部内容；不要覆盖游戏的 `Sunless Skies_Data/Managed/`。

## 常见问题

| 现象 | 检查方法 |
| --- | --- |
| 完全没有中文 | 确认 `winhttp.dll` 与游戏 EXE 同层，并检查 `BepInEx/LogOutput.log` 是否生成 |
| 中文显示方块 | 完整复制 `overTMP` 和 `BepInEx/unstripped/`，然后重启游戏 |
| 按钮或 R 键交互异常 | 保持配置中 `TextGetterCompatibilityMode=True` |
| 进港后明显缓慢或 JObject / VTable 报错 | 确认 `BepInEx/unstripped/Newtonsoft.Json.dll` 来自本包，并完全重启；不要用插件 `FullNET/` 内的同名文件替代 |
| 部分仍是英文 | 可能缺少对应译文，或属于图片文字和 Epic 叠加层 |

反馈时附上游戏版本、发生场景及 `BepInEx/LogOutput.log`。游戏更新后如出现异常，先停用补丁确认原版可运行。

## 运行时文本适配

补丁包含 `SunlessSkies.TemplateLocalization.dll`，在游戏显示层展开占位符之前，用现有 ui.txt 中的完整原文模板查找译文。统一 CRLF/LF 换行差异；角色名、数值、地点 ID 等占位符必须在译文中完整保留，包括重复次数，否则跳过该模板。该入口没有词条 ID，仍按原文匹配，不修改游戏数据、事件条件或存档。

该插件在游戏计算方向与距离后转换显示结果，未知名称保留原文。0.2.2 让翻译挂钩在游戏进程内持续生效。2026-09-17，用户确认交易事件正文和动态方向已显示中文；其他动态文本仍需各自验证。该事件的连接词使用最新 ParaTranz 译文，不再保留本地 `，而` 覆盖。

升级时如存在旧的 `BepInEx/Translation/zh/Text/runtime-compatibility.txt`，请删除；通用插件已取代它。插件启动成功会在 BepInEx 日志写入 `Template localization ready`；遇到接口不兼容则停用自身并记录错误。可在插件生成的配置文件 `BepInEx/config/githongwu.sunlessskies.template-localization.cfg` 中设置 `Enabled = false`，重启后停用此功能。Alt+T 切回原文后新生成的模板不再提前翻译，但已经生成的文本需要重新打开界面或重启才能恢复。

日期提示包含十二个月的拆分规则，目标格式为「1905年3月19日」，正文缺少译文时保留英文。实际显示仍受文本是否命中规则影响。

## 停用

退出游戏，将游戏目录 doorstop_config.ini 的 `[General]` 中 enabled 改为 false，再重启。卸载与来源许可见[仓库总览](../README.md)，许可证随包保存在 THIRD_PARTY_LICENSES/。

## 动态分支描述

模板插件在游戏选出 qvd 分支后翻译返回的显示文本，不修改属性数值或分支条件。分支表位于 BepInEx/Translation/zh/qvd-branches.txt，随补丁安装；它从现有译文的 JSON 字符串分支按相同键路径提取，不由 XUnity 当作普通全文译文加载。

6,174 个无歧义映射中，6,173 个通过占位符校验；73 个存在多种译文的原文未加入分支表。优先使用分支译文，无对应映射时尝试现有独立句子译文，仍未命中则保留原文。仅在已翻译模板的动态展开期间生效。

## 商店数量控件

0.2.5 对普通商店和集市买卖界面的数量输入框、库存上限文本保留游戏原始字体与布局，跳过自动翻译和字体替换；商品名称与描述照常翻译。不修改数量、价格、交易逻辑或存档。

两版编译、实际程序集接口核对及离线挂钩测试已通过；数字显示和左右箭头操作仍需游戏内复测。
