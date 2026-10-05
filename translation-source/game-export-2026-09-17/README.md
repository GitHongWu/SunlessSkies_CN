# Sunless Skies 当前安装数据 → ParaTranz 原文导出

提取日期：2026-09-17。来源：本机 Epic Sunless Skies (2.0.7.0) 安装目录。对比基准：[ParaTranz 项目 5731](https://paratranz.cn/projects/5731) 于 **2026-09-15** 下载的导出快照，不是实时线上项目；仓库不再保存该压缩包。游戏文件指纹及译文来源日期见 review/source-fingerprints.json。

共导出 **39,983 条**可定位文本，保留原文换行、格式标签及 `[dir:...]` 等未展开占位符。快照未收录的键 **2,523 条**；同键差异 **1,003 条**，其中 **930 条仅空白/换行转义差异**，另 **73 条内容变化**。这些数量不能直接解释为“游戏更新新增了这么多台词”：可能包括旧版漏收、重复显示文本、开发测试内容及键方案变化。

## 如何使用

- `translator-delta/`：供 ParaTranz 维护者检查的精简版，包含 9 个 JSON 文件、3,524 条词条（2,521 条新键、1,003 条同键差异）。内容来自 `delta/`，排除了 `domiciles.json` 的 2 条词条。每条仅保留 `key`、`original`、`translation`；**整个 `context` 字段已删除，而非留空**。所有译文留空，原文和键与 `delta/` 一致。
- `full/`：本次已覆盖范围的全量 JSON；同键同原文沿用快照译文。新原文及有实质或转义差异的原文均留空译文。未导出 stage，避免批量导入时意外赋予审核状态；原有阶段放在 context 中供参考。
- `delta/`：仅新增与同键变化条目，共 3,526 条，适合先给项目维护者审核。格式和 full 相同：`key / original / translation / context`。
- `review/changed.json`：逐项保存旧原文、当前原文、旧译文及差异分类。旧译文仅参考，未自动套用到变化后的原文。
- `review/old-not-located.json`：879 个旧条目未在本次对应文件中定位，或是已排除的内部字段/人工正则。**不代表游戏已删除，不建议批量删除上游词条。**
- `review/code-candidates.json` 与 `asset-candidates.json`：尚需确认用途的代码/不完整结构资源候选，不在 full 和 delta 的统计中，不应直接整批导入。
- 其余 review 文件记录数据边界验证、内部字段排除、资源解析限制和统计，均为本次原文提取资料，不是汉化运行依赖。

使用 `translator-delta/` 时，只需映射 `key`（词条键）、`original`（原文）、`translation`（译文），没有上下文字段。需要核对旧原文、旧译文或具体差异时，请查阅 `review/changed.json`；需要提取位置等信息时，可查看仍保留 `context` 的 `delta/` 或 `full/`。

`translator-delta/` 的全部 9 个文件已采用带游戏版本的文件名，例如 `bargains-2.0.7.0.json`、`events-2.0.7.0.json`。本次仅修改文件名，词条 `key` 保持原样，尚未添加版本后缀。维护者要求原文变化条目作为新条目录入，不能直接用当前同键条目覆盖线上内容；上传前还需按双方确认的规则处理词条键。这里记录的是当前文件状态，不表示已完成全部上传格式调整。

先在 ParaTranz 测试文件或备份后的项目中检查导入映射，注意不要用空译文覆盖线上已经补完的翻译。本次整理没有操作线上 ParaTranz，也没有修改游戏或现有汉化补丁。

## 覆盖范围与键

九个 BuildInStorage/data 二进制数据文件均由游戏自带反序列化器读取到文件末尾。按上游各表已采用的字段方案导出；内部效果表达式、时间元数据等独立列出，不当作可翻译台词。二进制结构中仍可能保留未启用的开发条目，不保证每条都能在正常游玩中出现。

Unity `.assets` 与 `level*` 场景读取了航行消息、地标、UI 文本及教学等指定显示字段，并检查程序集字面量。并非所有资源都能完整解析：184 个组件有结构读取限制（主要是控制器映射，也包含少量游戏/UI 组件），详见 resource-parse-limitations.json。加载提示采用补充解析，已验证整个 40 项字符串数组的数量与长度边界。图片文字、运行时拼接结果等不保证覆盖。

数据库类词条尽量沿用 `ID$字段名`；商店沿用上游的 ShopName/ShopDescription。上游手工编号的日志、UI 等只在原文完全一致时继承旧键；新资源原文采用 `asset_内容哈希`，`full/` 和 `delta/` 的 context 给出资源文件、PathID、对象/类型和字段路径；`translator-delta/` 不包含这些信息。**内容哈希不是稳定的游戏实体 ID**，其原文变化会形成新键，不做无依据的改动配对。`z_logbook.json` 的 sr: 规则是人工翻译匹配模板，不是可从游戏提取的台词，因此不伪造这个文件。

下表为 `full/` 的提取统计，包含 `domiciles.json`；不代表 `translator-delta/` 的文件清单。

| 文件 | 本次全量 | 新键 | 内容变化 | 格式/转义差异 |
| --- | ---: | ---: | ---: | ---: |
| areas.json | 224 | 0 | 0 | 0 |
| bargains.json | 382 | 0 | 5 | 1 |
| domiciles.json | 2 | 2 | 0 | 0 |
| events.json | 30528 | 834 | 54 | 864 |
| exchanges.json | 172 | 0 | 0 | 0 |
| landmarks.json | 452 | 409 | 0 | 0 |
| loadingHints.json | 40 | 7 | 0 | 0 |
| logEntries.json | 927 | 494 | 0 | 0 |
| personas.json | 4 | 4 | 0 | 0 |
| prospects.json | 392 | 0 | 2 | 0 |
| qualities.json | 5640 | 47 | 12 | 65 |
| settings.json | 47 | 0 | 0 | 0 |
| ui.json | 1034 | 726 | 0 | 0 |
| z_shipnames.json | 139 | 0 | 0 | 0 |

## 来源与许可

游戏英文原文权利归 Failbetter Games 等相应权利人；此导出不宣称对游戏原文拥有再许可权。沿用的中文译文来自 ParaTranz 项目 5731（InstantComet 与项目贡献者），CC BY-NC 4.0：https://paratranz.cn/projects/5731 。本次进行了提取、分类、键映射与差异比较，未新增人工译文。旧译文参考及相关说明保留在 `full/`、`delta/` 和 `review/` 中，`translator-delta/` 不包含旧译文或 context。

这是一份翻译维护资料，**不是游戏补丁，不能复制进 BepInEx 后直接生效**。本次使用的临时提取工具位于被 Git 忽略的 work/，不放入补丁或此导出包。
