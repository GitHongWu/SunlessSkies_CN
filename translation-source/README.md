# 译文来源

当前补丁采用 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 于 **2026-10-04** 下载的官方译文，按 CC BY-NC 4.0 使用。该次快照共 23 个文件、41,865 条词条；stage 5 与 stage 9 计入已审核，共 22,482 条（53.70%）。逐文件进度见下表；[仓库总览](../README.md)提供安装入口。

仓库不再保存 ParaTranz 导出压缩包。最新译文可从[官方下载页](https://paratranz.cn/projects/5731/artifact)获取；该页面提供的最新内容不一定与上述日期的快照相同。

`game-export-2026-09-17` 的历史原文增量比较基于 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 于 **2026-09-15** 下载的译文，不是当前线上内容，也不是当前补丁的主译文源。

同原文冲突依次按已审核、UI 文件、2.0.7.0 目录、文件名与词条键选择。最新字号标签及连接词修正直接使用上游内容；仅对仍有问题的 NEW/ui.json / ui389 修正字面量 sprite 标签转义。动态日期及游戏占位符适配继续保留。

`Legacy-UI-8eb2eda.zip` 仅含旧版 ui.txt，提取自本仓库提交 8eb2eda3a6a34e61a75c957968643826d0411da6。原来源为 InstantComet/SunlessSkies（参考提交 1f0533842efd13ccfd8dd2806b2947233f62cfcd），含此前本地 UI 修订。仍有 19,778 个原文键未被新版 ParaTranz 精确覆盖，继续补充；不覆盖任何 ParaTranz 译文，审核状态未知。

安装只需要对应的 patch-epic/ 或 patch-steam/ 内全部内容；这些来源资料和源码均不需要复制到游戏目录。仓库不附带导入或调试脚本。

## 逐文件审核进度

| 文件 | 总词条 | 已审核 | 审核进度 |
| --- | ---: | ---: | ---: |
| 2.0.7.0/bargains.json | 6 | 6 | 100.00% |
| 2.0.7.0/domiciles.json | 2 | 2 | 100.00% |
| 2.0.7.0/events.json | 1752 | 1752 | 100.00% |
| 2.0.7.0/landmarks.json | 409 | 409 | 100.00% |
| 2.0.7.0/loadingHints.json | 7 | 7 | 100.00% |
| 2.0.7.0/logEntries.json | 494 | 494 | 100.00% |
| 2.0.7.0/personas.json | 4 | 4 | 100.00% |
| 2.0.7.0/prospects.json | 2 | 2 | 100.00% |
| 2.0.7.0/qualities.json | 124 | 124 | 100.00% |
| 2.0.7.0/ui.json | 726 | 726 | 100.00% |
| NEW/areas.json | 224 | 224 | 100.00% |
| NEW/bargains.json | 382 | 382 | 100.00% |
| NEW/events.json | 30051 | 14939 | 49.71% |
| NEW/exchanges.json | 172 | 172 | 100.00% |
| NEW/landmarks.json | 54 | 54 | 100.00% |
| NEW/loadingHints.json | 38 | 38 | 100.00% |
| NEW/logEntries.json | 499 | 499 | 100.00% |
| NEW/prospects.json | 392 | 392 | 100.00% |
| NEW/qualities.json | 5654 | 1383 | 24.46% |
| NEW/settings.json | 47 | 47 | 100.00% |
| NEW/ui.json | 560 | 560 | 100.00% |
| NEW/z_logbook.json | 9 | 9 | 100.00% |
| NEW/z_shipnames.json | 257 | 257 | 100.00% |

## 显示文本兼容

两版补丁的 BepInEx/Translation/zh/Text/display-compatibility.txt 仅保存 4 条去除 [ALWAYS] 显示标记后的标题映射，全部沿用现有译文，不新增本地翻译。集市说明的 delvier / deliver 原文差异，以及未覆盖的集市标题、商机列表标题和刷新提示，留待上游原文与译文维护，不在此文件中补译或兼容。
