# 地图悬浮提示缺失原文（2026-10-10）

用于 ParaTranz 项目 5731 的原文增量。依据 Steam 资源版本 2.0.6.454aad8 的 ChartLandmarkObject.ShowTooltip 原生代码，并与 2026-10-10 下载的 ParaTranz 快照逐字比较。Epic 同一生成流程也已核对。

文件 ui-map-tooltip-steam-2.0.6.454aad8.json 仅包含此次确认未被快照精确收录的原文；所有 translation 留空，没有 context 字段。

这是游戏拼接提示时使用的片段，不是完整句子：

- `This port sells `：后面接一种或多种可购买的物资名称。
- `This port exports `：后面接该港口出口商品的名称。
- ` and `：多种物资之间的连接词。
- `fuel`、`supplies`、`petrichor`：港口出售的物资名称，只有快照未收录的才导出；补丁已有旧词典译文时继续使用。
- `Your scout has discovered something.`：尚未识别地点时的侦察提示。游戏外围的斜体标签由补丁保留，不需要把标签写入此词条。

原文中的前后空格属于运行时片段，请不要修改 original。译文可按中文语法决定是否保留空格。颜色、斜体、加粗和句末标点由游戏负责拼接。此次插件支持按片段查找现有译文；ParaTranz 完成翻译后，重新生成补丁译文库即可使用。

地名、地标描述和商品名称在当前词典中已有映射，没有重复导出。片段识别不做通用英文词语替换，只作用于地图悬浮提示。

核对来源：https://paratranz.cn/projects/5731 。项目译文归其翻译与校对贡献者，按 CC BY-NC 4.0 使用；本增量未包含新写的中文译文。

本次导出 7 条。
