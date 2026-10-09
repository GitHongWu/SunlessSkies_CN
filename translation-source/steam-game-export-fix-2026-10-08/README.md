# ParaTranz 最新译文格式检查与修复（2026-10-08）

来源：[ParaTranz 项目 5731](https://paratranz.cn/projects/5731)，署名归项目翻译及校对贡献者。按 [CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/deed.zh) 使用。下载日期 2026-10-08，原压缩包名 `2026_10_08_11_00_41_c5b7a8.zip`。

检查全部 26 个文件、41,984 条词条。检查范围包括普通译文换行、VariableDescriptionText / ChangeDescriptionText / LevelDescriptionText 内层 JSON、重复分支键、分支缺少或为空、字号等富文本标签，以及数字和 valueN 格式占位符。静态检查不等于游戏全场景验证，也不判断译文内容是否准确。

## 可确定的修复

`fixed/` 下为完整同名文件，只修改 translation，不修改原文、key、stage。保留的 stage 只是来源状态，不代表新增审核。

| 文件 | 完整词条数 | 修复条数 |
| --- | ---: | ---: |
| [2.0.7.0/events.json](fixed/2.0.7.0/events.json) | 1,752 | 1 |
| [NEW/events.json](fixed/NEW/events.json) | 30,051 | 32 |
| [NEW/qualities.json](fixed/NEW/qualities.json) | 5,654 | 55 |

共修复 **88 条**：普通换行 33 条，内层 JSON 55 条。内层 JSON 中，51 条只在原文和译文均与旧快照完全一致时复用此前确认的格式/分支键修复；4 条仅重新转义字符串内的控制字符，不改变解析后的内容。没有覆盖已更新的中文措辞，也没有补译。
