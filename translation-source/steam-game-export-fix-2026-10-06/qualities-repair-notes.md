# NEW/qualities.json 修复说明

这是基于 2026-10-04 ParaTranz 快照的完整同名文件，共 5,654 条。本次仅修改其中 58 条的 translation，其他 5,596 条及所有原文、词条 key、stage、context 保持不变。未新增中文译文，也未将修复视为人工审核。

## 已修复

- 修复 55 条无法解析的内层 JSON：包括引号、反斜杠、实际换行的序列化、逗号、冒号、括号等问题。
- 还原 134377、135954、138889 中被误译的分支键；132752 的结构修复也包含还原分支键。分支键供游戏选择文本，不能翻译。
- 135177：将已有“需要签证”的译文从 Navigator/1 移到 Navigator/0；不编写“已有签证”的译文。
- 134430：保留第一份完整对象，删除后面重复粘贴的对象；保留对象的内容和分支与原文对应。
- 131798、133438、136053：按原文核对对话边界，修复混乱引号和多余反斜杠，保留现有中文措辞，包括重复的“他说”。

已验证全部 357 条 VariableDescriptionText 的内层 JSON 均可解析、无重复键；除下列已知缺失外，分支路径与原文一致。修复后的最终显示文本没有残留字面量换行转义。此检查不代表对全部译文准确性进行了校对。

## 译文缺失的部分

以下词条 key 均带 `$VariableDescriptionText` 后缀。在 translation 内补充对应分支即可，不要改动外层词条 key 或 original。

| 词条 key | 缺失分支 | 对应原文 |
| --- | --- | --- |
| `133938$VariableDescriptionText` | `TimeRemaining/1` | `You have four days left to pass this law.` |
| `135177$VariableDescriptionText` | `Navigator/1` | `You've already got a visa, so you'll be able to come with me."` |
| `138480$VariableDescriptionText` | `failed_dead/1` | `A particularly delicious-looking specimen is being dragged toward the edge now. ` |
| `138458$VariableDescriptionText` | `trial_desc3/2` | 原文为空字符串；这是缺失的空分支，无需编写译文。 |
| `138458$VariableDescriptionText` | `trial_desc3/3` | `"T-ten thousand?" The Deferred spirit panics and runs blindly from the dock, charging straight into you. "You! Will you forgive me?"` |

按确认，本次保留上述缺失状态，没有写入空译文占位或本地补译；138458 整条未改动。因此格式问题已修复，但这些分支的翻译覆盖仍不完整。
