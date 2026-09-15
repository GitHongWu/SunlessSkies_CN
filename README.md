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

**`patch/` 已包含全部补丁运行文件，包括修复版 `Newtonsoft.Json.dll`，安装不依赖 `debug/`、`outputs/json-fix`、译文来源资料或任何额外脚本。** 不需要配置 API 密钥或在线翻译服务。传统 UGUI 文本使用系统的 Microsoft YaHei 字体。

已有其他 BepInEx 或汉化补丁时，先备份并检查兼容性。升级本补丁时覆盖 `patch/` 内全部内容；如果曾安装临时诊断插件，退出游戏后移除游戏内的 `BepInEx/plugins/SunlessSkies.PerformanceProbe/`。不要覆盖游戏的 `Sunless Skies_Data/Managed/`。

## 汉化范围

包含 **42,661 个去重后的翻译键**，覆盖界面、剧情事件、物品、地区、交易、日志和船名。来源为 InstantComet 项目的全部可解析分类译文和翻译缓存，并保留本地 UI 修订。另有 505 行格式有歧义的源文本未启用。

这不代表游戏内容已 100% 汉化或逐条校对。仍可能出现英文、排版问题和不自然的译文。已实际确认菜单中文显示、交互键恢复及进港后明显缓慢问题的修复；未逐场景验证全部内容。

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
| `tools/` | 校验并生成安装 ZIP | 不需要 |
| `translation-source/` | 上游原始译文及导入报告 | 不需要 |
| `debug/` | 性能探针源码；默认不提交 Git | 不需要 |
| `dist/` | 按需打包生成的 ZIP；默认不保留、不提交 Git | 可直接解压 ZIP 安装 |

维护者在仓库根目录执行：

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\Build-Release.ps1
```

脚本校验运行文件、JSON 修复版、离线配置及诊断文件隔离后，仅将 `patch/` 内容打包到 `dist/SunlessSkies-Epic-CN.zip`。安装该 ZIP 时直接复制解压内容到游戏目录，无需再找其他文件。只做校验可加 `-VerifyOnly`。

修改译文请编辑 `patch/BepInEx/Translation/zh/Text/ui.txt`，使用 UTF-8，并保留变量、富文本标签和转义符。修改后重启游戏。

## 来源与许可

| 内容 | 来源 |
| --- | --- |
| 译文和中文字体 `overTMP` | [InstantComet/SunlessSkies](https://github.com/InstantComet/SunlessSkies)，参考提交 `1f0533842efd13ccfd8dd2806b2947233f62cfcd` |
| 加载器 | [BepInEx 5.4.23.5](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.5) |
| 翻译插件 | [XUnity.AutoTranslator 5.6.2](https://github.com/bbepis/XUnity.AutoTranslator/releases/tag/v5.6.2)，配套 ResourceRedirector 2.1.0 |
| Unity 完整托管库 | [Unity Libs](https://unity.bepinex.dev/)，2019.4.2 |
| JSON 运行库 | [Newtonsoft.Json 8.0.3](https://www.nuget.org/packages/Newtonsoft.Json/8.0.3)，net45；去除强名称以匹配游戏程序集身份，程序集版本 8.0.0.0 |
| 方案参考 | [tinygrox/SunlessSeaCN](https://github.com/tinygrox/SunlessSeaCN) |

许可证见 `patch/THIRD_PARTY_LICENSES/` 和各上游项目。各资源权利归对应作者或权利人所有，本项目不是 Failbetter Games 或 Epic Games 官方汉化。
