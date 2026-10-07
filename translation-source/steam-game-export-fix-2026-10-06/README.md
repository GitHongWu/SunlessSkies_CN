# ParaTranz 译文换行与内层 JSON 修复（2026-10-06）

来源为 [ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 及项目翻译贡献者，使用 **2026-10-04 下载的快照**。按 [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/deed.zh) 保留署名；本次修正普通译文中的换行转义，以及 qualities 中的内层 JSON 格式和分支对应错误。

## 文件与修改范围

以下为保留全部词条的完整同名文件，不是只含修改项的增量。请按完整相对路径对应 ParaTranz 文件，两个 events.json 不要互相覆盖。

| 文件 | 完整词条数 | 修正词条数 |
| --- | ---: | ---: |
| [NEW/events.json](NEW/events.json) | 30,051 | 2,440 |
| [2.0.7.0/events.json](2.0.7.0/events.json) | 1,752 | 1 |
| [NEW/bargains.json](NEW/bargains.json) | 382 | 1 |
| [NEW/ui.json](NEW/ui.json) | 560 | 4 |
| [NEW/qualities.json](NEW/qualities.json) | 5,654 | 58 |

合计修正 **2,504 条**。其中普通译文 2,446 条，仅将 translation 中字面量 `\n`、`\r`、`\r\n` 转为实际换行，保留已有真实换行。qualities 的 58 条修复另见下文。所有文件均保留原文、词条 key、审核阶段 stage 和已有 context。保留的 stage 是来源快照的状态，不代表新增人工审核。

`NEW/qualities.json` 已按后续确认加入：修复 55 条不可解析的内层 JSON，以及 3 条误译成中文的分支键。保留中文措辞，纠正引号、括号、分隔符和转义；将一处译文移回对应分支，删除一处重复粘贴的对象。357 条 VariableDescriptionText 均通过解析和分支对照检查。4 条仍缺少共 5 个分支，未补译，详见 [qualities 修复说明](qualities-repair-notes.md)。

`2.0.7.0/qualities.json` 中已确认的内层 JSON 换行是正常格式，未改动。没有对所有反斜杠做通用解码。

JSON 文件中实际换行仍必须序列化为 `\n`；qualities 的 translation 本身又是一层 JSON，需再解析一层才能得到显示文本。因此不能只数编辑器中的斜杠。本次已按各自层级重新解析验证，最终文本不再包含误作文字的换行转义。
