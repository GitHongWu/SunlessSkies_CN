# Sunless Skies 中文补丁

Windows 版《Sunless Skies》的离线汉化，分别提供 Epic 和 Steam 补丁。译文共享，加载器及运行时插件按游戏构建分别适配。

## 选择版本

| 游戏版本 | 对应补丁与安装说明 | 运行环境 | 验证状态 |
| --- | --- | --- | --- |
| Epic 2.0.7.0 | [patch-epic/](patch-epic/README.md) | x64 / Mono，Unity 2019.4.2f1 | 已有游戏内使用反馈，交易事件正文与动态方向已确认；不代表全部内容经过测试 |
| Steam 资源版本 2.0.6.454aad8，Build ID 20365930 | [patch-steam/](patch-steam/README.md) | x86 / IL2CPP，Unity 2019.4.2f1 | 加载器、翻译插件及字体已初始化；专用模板插件已成功加载 1,236 个模板，实际界面与交互待复测 |

**只安装对应版本，不要混用两个补丁目录中的 DLL、配置和运行库。** 其他游戏构建尚未验证。

## 安装

1. 完全退出游戏。
2. 按上表选择 `patch-epic/` 或 `patch-steam/`。
3. 将所选目录内的**全部内容**复制到 `Sunless Skies.exe` 所在目录，合并文件夹。
4. 启动游戏；Steam 版从 Steam 启动。

不要只复制 BepInEx/，也不要把 patch-epic 或 patch-steam 文件夹本身套进游戏目录。安装不需要 src/、translation-source/ 或任何调试脚本。已有其他补丁时，先备份并检查兼容性。

两个版本均使用本地译文，不需要翻译服务或 API 密钥。Steam 首次启动会生成 IL2CPP 互操作程序集，可能需要联网下载 Unity 基础库，详见其安装说明。

## 译文与审核进度

优先采用 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 的 **2026-10-04 官方快照**，包含全部 23 个文件。

| 源项目统计 | 数量 | 比例 |
| --- | ---: | ---: |
| 总词条／已翻译 | 41,865 | 100% |
| 已审核（stage 5、9） | 22,482 | 53.70% |
| 尚未审核 | 19,383 | 46.30% |

100% 指已收录词条都有译文，不代表游戏文本全部收录或全部审核。统计不会实时更新；[逐文件进度与来源说明](translation-source/README.md)提供详细信息。

补丁共有 **53,872 个基础翻译键：34,094 个 ParaTranz 键 + 19,778 个旧版补充键**，另有 12 条动态日期规则。ParaTranz 始终优先；旧译文仅补充未精确覆盖的原文，审核状态未知。

## 停用与反馈

退出游戏，把游戏目录 doorstop_config.ini 中 `[General]` 的 `enabled = true` 改为 `enabled = false`，重启即可停用。恢复为 true 后重启即可启用。

卸载只移除补丁新增文件，不删除游戏 EXE、游戏数据和存档。若共用 BepInEx 加载器运行其他插件，不要直接删除整个 BepInEx/。

反馈请注明渠道、游戏版本、发生场景，并附 BepInEx/LogOutput.log。游戏更新后出现异常时，可先停用补丁检查原版运行情况。

## 仓库目录

| 目录 | 用途 |
| --- | --- |
| [patch-epic/](patch-epic/README.md) | Epic 安装文件 |
| [patch-steam/](patch-steam/README.md) | Steam 安装文件 |
| [src/epic/](src/epic/README.md) | Epic 自有插件源码 |
| [src/steam/](src/steam/README.md) | Steam 自有插件源码 |
| [translation-source/](translation-source/README.md) | 译文来源地址与日期、审核统计及旧版补充资料 |
| [game-export-2026-09-17/](translation-source/game-export-2026-09-17/README.md) | 原文提取与历史增量资料，不是运行补丁 |

## 来源与许可

| 内容 | 来源 |
| --- | --- |
| 汉化译文 | [ParaTranz 项目 5731](https://paratranz.cn/projects/5731)，InstantComet 与项目翻译贡献者，CC BY-NC 4.0；本仓库进行了格式转换、去重和冲突选择 |
| 旧版补充译文 | [InstantComet/SunlessSkies](https://github.com/InstantComet/SunlessSkies) 及此前本地 UI 修订；仅补充 ParaTranz 未覆盖的原文，审核状态未知 |
| 中文字体 `overTMP` | [InstantComet/SunlessSkies](https://github.com/InstantComet/SunlessSkies)，参考提交 `1f0533842efd13ccfd8dd2806b2947233f62cfcd` |
| Epic 加载器 | [BepInEx 5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) |
| Steam 加载器 | [BepInEx 6.0.0-be.788](https://builds.bepinex.dev/projects/bepinex_be)，IL2CPP win-x86 |
| 翻译插件 | [XUnity.AutoTranslator 5.6.2](https://github.com/bbepis/XUnity.AutoTranslator/releases/tag/v5.6.2)，配套 ResourceRedirector 2.1.0 |
| Epic Unity 完整托管库 | [Unity Libs](https://unity.bepinex.dev/)，2019.4.2 |
| Epic JSON 运行库修复 | [Newtonsoft.Json 8.0.3](https://www.nuget.org/packages/Newtonsoft.Json/8.0.3)，net45；去除强名称以匹配游戏程序集身份，程序集版本 8.0.0.0 |
| 方案参考 | [tinygrox/SunlessSeaCN](https://github.com/tinygrox/SunlessSeaCN) |

许可证见所选补丁目录中的 `THIRD_PARTY_LICENSES/` 和各上游项目。各资源权利归对应作者或权利人所有，本项目不是 Failbetter Games 或 Epic Games 官方汉化。

译文采用 [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/deed.zh)：使用、改编和分发时保留来源署名及许可链接，注明修改，不得用于商业用途。此许可仅针对 ParaTranz 译文，不替代旧版补充来源或其他组件的许可。
