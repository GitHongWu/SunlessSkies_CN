# 译文来源

当前补丁采用 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 的 **2026-10-11** 官方译文快照，按 CC BY-NC 4.0 使用。该次快照共 27 个文件、41,991 条词条；stage 5 与 stage 9 计入已审核，共 21,835 条（52.00%）。逐文件进度见下表；[仓库总览](../README.md)提供安装入口。

仓库不再保存 ParaTranz 导出压缩包。最新译文可从[官方下载页](https://paratranz.cn/projects/5731/artifact)获取；该页面提供的最新内容不一定与上述日期的快照相同。

`game-export-2026-09-17` 的历史原文增量比较基于 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 于 **2026-09-15** 下载的译文，不是当前线上内容，也不是当前补丁的主译文源。

先按相同 key 选择非空译文，再处理同原文冲突；两个阶段都按 2.0.7.0/、NEW/、其他目录的顺序，同一目录按文件名和词条 key 固定排序。忽略 stage 状态，也不再对 ui.json 设置额外优先级；stage 仅用于统计审核进度。同 key 而原文不同也只保留优先目录版本；被排除的原文不会由旧译文重新填回。最新字号标签及连接词修正直接使用上游内容；仅对仍有问题的 NEW/ui.json / ui389 修正字面量 sprite 标签转义。动态日期及游戏占位符适配继续保留。

`Legacy-UI-8eb2eda.zip` 仅含旧版 ui.txt，提取自本仓库提交 8eb2eda3a6a34e61a75c957968643826d0411da6。原来源为 InstantComet/SunlessSkies（参考提交 1f0533842efd13ccfd8dd2806b2947233f62cfcd），含此前本地 UI 修订。仍有 19,775 个原文键未被新版 ParaTranz 精确覆盖，继续补充；不覆盖任何 ParaTranz 译文，审核状态未知。

安装只需要对应的 patch-epic/ 或 patch-steam/ 内全部内容；这些来源资料和源码均不需要复制到游戏目录。仓库不附带导入或调试脚本。

## 原文补充导出

修正后的同名完整译文文件见 [steam-game-export-fix-2026-10-06/](steam-game-export-fix-2026-10-06/README.md)：修正四个文件共 2,446 条普通译文的换行，并修复 NEW/qualities.json 中 58 条内层 JSON 格式或分支对应问题，合计五个文件、2,504 条。qualities 中仍缺少的分支已列明，未补译。导出基于 2026-10-04 快照，上传覆盖前应核对线上后续修改；该目录是历史修复导出；当前补丁使用 2026-10-11 上游快照，未将历史修复文件再次覆盖到新译文。

- [Steam 原生代码检查（2026-10-05）](steam-game-export-2026-10-05/README.md)：基于 Steam 安装目录的 GameAssembly.dll 和 IL2CPP 元数据，扫描两个游戏程序集对应的 16,384 个独立原生函数。合计导出 68 条固定文本、50 条动态模板，并与历史 full 导出去重。此目录已替换此前 Epic 检查结果；文件名使用 Steam 资源版本 2.0.6.454aad8。译文留空，动态模板需要后续补丁接入，待核对资料与正式增量分开保存。

上述数量表示历史导出未收录的内容，原生代码静态检查仍有未完整还原的动态路径，也不替代全部 Steam 剧情资源导出；具体范围和限制见目录说明。`game-export-2026-09-17` 继续保留为历史 Epic 资源资料，本次仅将其用作去重基线。

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
| 2.0.7.0/ui-map-tooltip-steam-2.0.6.454aad8.json | 7 | 7 | 100.00% |
| 2.0.7.0/ui.json | 726 | 726 | 100.00% |
| NEW/areas.json | 224 | 224 | 100.00% |
| NEW/bargains.json | 382 | 382 | 100.00% |
| NEW/events.json | 30051 | 14287 | 47.54% |
| NEW/exchanges.json | 172 | 172 | 100.00% |
| NEW/landmarks.json | 54 | 54 | 100.00% |
| NEW/loadingHints.json | 38 | 38 | 100.00% |
| NEW/logEntries.json | 499 | 499 | 100.00% |
| NEW/prospects.json | 392 | 392 | 100.00% |
| NEW/qualities.json | 5654 | 1379 | 24.39% |
| NEW/settings.json | 47 | 47 | 100.00% |
| NEW/ui-code-steam-2.0.6.454aad8.json | 68 | 1 | 1.47% |
| NEW/ui-code-templates-steam-2.0.6.454aad8.json | 50 | 0 | 0.00% |
| NEW/ui.json | 560 | 560 | 100.00% |
| NEW/z_logbook.json | 9 | 9 | 100.00% |
| NEW/z_shipnames.json | 257 | 257 | 100.00% |
| 手动录入缺失词条.json | 1 | 1 | 100.00% |
