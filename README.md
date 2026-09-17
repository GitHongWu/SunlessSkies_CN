# Sunless Skies Epic 中文补丁

适用于 Windows x64、Epic Games 版《Sunless Skies: Sovereign Edition》2.0.7.0（Unity 2019.4.2f1 / Mono）的离线汉化。其他渠道和版本未验证。

## 安装

1. 完全退出游戏。
2. 下载并解压本仓库，打开 **`patch/`**。
3. 将 **`patch/` 内的全部内容**复制到 **`Sunless Skies.exe` 所在目录**，合并文件夹。
4. 启动游戏。

不要只复制 `BepInEx/`，也不要把 `patch` 文件夹本身套在游戏目录里。安装后应为：

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

**`patch/` 已包含全部补丁运行文件，包括修复版 `Newtonsoft.Json.dll`，安装不依赖译文来源资料或任何额外脚本。** 不需要配置 API 密钥或在线翻译服务。传统 UGUI 文本使用系统的 Microsoft YaHei 字体。

已有其他 BepInEx 或汉化补丁时，先备份并检查兼容性。升级本补丁时覆盖 `patch/` 内全部内容；不要覆盖游戏的 `Sunless Skies_Data/Managed/`。

## 汉化范围与审核进度

译文优先采用 [ParaTranz 项目 5731：无光之空汉化 RE](https://paratranz.cn/projects/5731/files)，使用 **2026-09-15** 下载的官方导出快照，包含全部 13 个文件。既包含已审核译文，也包含尚未审核的译文。

**源项目共 38,339 条，已翻译 38,339 条（100%），已审核 16,179 条（42.20%），尚未审核 22,160 条（57.80%）。** 这里的 100% 仅表示项目词条已有译文，不代表游戏全部文本已收录，也不代表译文已全部审核。统计是本次快照，不会自动实时更新。

| 文件 | 总词条 | 已审核 | 审核进度 |
| --- | ---: | ---: | ---: |
| areas.json | 224 | 224 | 100.00% |
| bargains.json | 382 | 382 | 100.00% |
| events.json | 30051 | 12349 | 41.09% |
| exchanges.json | 172 | 172 | 100.00% |
| landmarks.json | 54 | 54 | 100.00% |
| loadingHints.json | 38 | 38 | 100.00% |
| logEntries.json | 499 | 499 | 100.00% |
| prospects.json | 392 | 392 | 100.00% |
| qualities.json | 5654 | 1207 | 21.35% |
| settings.json | 47 | 47 | 100.00% |
| ui.json | 560 | 558 | 99.64% |
| z_logbook.json | 9 | 0 | 0.00% |
| z_shipnames.json | 257 | 257 | 100.00% |

显示层插件按原文匹配，合并重复原文后得到 **31,385 个翻译键**。数量少于源词条是因为同一原文可在多个事件或属性中重复出现，不是只导入了部分文件。全部源词条均已处理，无空白条目被跳过。

同一原文出现不同译文时，优先已审核条目，其次优先 UI 文件，再按文件名和词条键排序选择。插件无法按 ParaTranz 词条 ID 区分相同原文的不同语境，因此不能同时显示这些不同译文。对于 ParaTranz 未覆盖的原文，补充旧版译文；即使 ParaTranz 条目尚未审核，也始终优先于旧版。

补丁的基础译文包含 **51,331 个翻译键 = 31,385 个 ParaTranz 键 + 19,946 个旧版补充键**，另有日期适配规则。旧版补充来自本仓库提交 `8eb2eda` 的译文快照（原来源 InstantComet/SunlessSkies，含此前本地 UI 修订），**审核状态未知，不计入上面的 ParaTranz 审核进度**。旧表有 100 行无法被当前插件有效解析，未加入。按原文精确匹配，英文措辞、空格或换行变化仍可能导致未命中。

译文做了格式转换、富文本标签转义修正、冲突选择，以及个别残留英文连接词的本地修正，没有额外全面人工审核。加载器、中文字体、交互兼容和 JSON 修复沿用此前验证的版本。通用模板适配已在用户反馈的交易事件中确认生效，但不代表游戏内所有文本均已汉化。

## 停用与卸载

退出游戏，在游戏目录 `doorstop_config.ini` 的 `[General]` 中将 `enabled = true` 改为 `enabled = false` 即可停用；改回 `true` 并重启即可启用。

卸载时移除上述补丁新增文件，不删除游戏 EXE、游戏数据目录或存档。如果还有其他 BepInEx 插件，应保留它们及共享加载器，不要直接删除整个 `BepInEx/`。

## 常见问题

| 现象 | 检查方法 |
| --- | --- |
| 完全没有中文 | 确认 `winhttp.dll` 与游戏 EXE 同层，并检查 `BepInEx/LogOutput.log` 是否生成 |
| 中文显示方块 | 完整复制 `overTMP` 和 `BepInEx/unstripped/`，然后重启游戏 |
| 按钮或 R 键交互异常 | 保持配置中 `TextGetterCompatibilityMode=True` |
| 进港后明显缓慢或 JObject / VTable 报错 | 确认 `BepInEx/unstripped/Newtonsoft.Json.dll` 来自本包，并完全重启；不要用插件 `FullNET/` 内的同名文件替代 |
| 部分仍是英文 | 可能缺少对应译文，或属于图片文字和 Epic 叠加层 |

反馈时附上游戏版本、发生场景及 `BepInEx/LogOutput.log`。游戏更新后如出现异常，先停用补丁确认原版可运行。

## 仓库目录

| 目录 | 用途 | 安装是否需要 |
| --- | --- | --- |
| `patch/` | 完整安装文件；发布包唯一来源 | **需要全部复制** |
| `translation-source/` | 译文来源快照与署名说明 | 不需要 |
| `src/` | 随包自有插件的对应源码 | 不需要 |

安装只需复制 patch/ 内全部内容，不依赖脚本或报告。translation-source/ 用于核对来源；src/ 保留随包插件的对应源码，均不需要复制到游戏目录。

## 来源与许可

| 内容 | 来源 |
| --- | --- |
| 汉化译文 | [ParaTranz 项目 5731](https://paratranz.cn/projects/5731)，InstantComet 与项目翻译贡献者，CC BY-NC 4.0；本仓库进行了格式转换、去重和冲突选择 |
| 旧版补充译文 | [InstantComet/SunlessSkies](https://github.com/InstantComet/SunlessSkies) 及此前本地 UI 修订；仅补充 ParaTranz 未覆盖的原文，审核状态未知 |
| 中文字体 `overTMP` | [InstantComet/SunlessSkies](https://github.com/InstantComet/SunlessSkies)，参考提交 `1f0533842efd13ccfd8dd2806b2947233f62cfcd` |
| 加载器 | [BepInEx 5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) |
| 翻译插件 | [XUnity.AutoTranslator 5.6.2](https://github.com/bbepis/XUnity.AutoTranslator/releases/tag/v5.6.2)，配套 ResourceRedirector 2.1.0 |
| Unity 完整托管库 | [Unity Libs](https://unity.bepinex.dev/)，2019.4.2 |
| JSON 运行库 | [Newtonsoft.Json 8.0.3](https://www.nuget.org/packages/Newtonsoft.Json/8.0.3)，net45；去除强名称以匹配游戏程序集身份，程序集版本 8.0.0.0 |
| 方案参考 | [tinygrox/SunlessSeaCN](https://github.com/tinygrox/SunlessSeaCN) |

许可证见 `patch/THIRD_PARTY_LICENSES/` 和各上游项目。各资源权利归对应作者或权利人所有，本项目不是 Failbetter Games 或 Epic Games 官方汉化。

译文采用 [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/deed.zh)：使用、改编和分发时保留来源署名及许可链接，注明修改，不得用于商业用途。此许可仅针对 ParaTranz 译文，不替代旧版补充来源或其他组件的许可。

### 运行时文本适配

补丁包含 `SunlessSkies.TemplateLocalization.dll`，在游戏显示层展开占位符之前，用现有 ui.txt 中的完整原文模板查找译文。统一 CRLF/LF 换行差异；角色名、数值、地点 ID 等占位符必须在译文中完整保留，包括重复次数，否则跳过该模板。该入口没有词条 ID，仍按原文匹配，不修改游戏数据、事件条件或存档。

该插件在游戏计算方向与距离后转换显示结果，未知名称保留原文。0.2.2 让翻译挂钩在游戏进程内持续生效。2026-09-17，用户确认交易事件正文和动态方向已显示中文；其他动态文本仍需各自验证。该事件译文中残留的连接词 `, and ` 已在本地译文表修正为 `，而`，上游来源快照保持原样。

升级时如存在旧的 `BepInEx/Translation/zh/Text/runtime-compatibility.txt`，请删除；通用插件已取代它。插件启动成功会在 BepInEx 日志写入 `Template localization ready`；遇到接口不兼容则停用自身并记录错误。可在插件生成的配置文件 `BepInEx/config/githongwu.sunlessskies.template-localization.cfg` 中设置 `Enabled = false`，重启后停用此功能。Alt+T 切回原文后新生成的模板不再提前翻译，但已经生成的文本需要重新打开界面或重启才能恢复。

日期提示包含十二个月的拆分规则，目标格式为「1905年3月19日」，正文缺少译文时保留英文。实际显示仍受文本是否命中规则影响。
