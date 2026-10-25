# 游戏原文增量

本目录收录当前补丁仍缺少可用译文的 356 条原文，译文全部留空。能利用现有词典或补丁派生规则翻译的条目不重复导出。

## 文件与导入方式

| 文件 | 条目数 | 内容 |
| --- | ---: | --- |
| `NEW/z_shipnames.json` | 350 | 随机残骸名称，以及对应活动机车名称的翻译来源 |
| `2.0.7.0-Addenda/ui-runtime.json` | 6 | 银行完整说明 1 条、装备失败提示 5 条 |

按 ParaTranz 已有同路径文件增量导入，不用本批文件整体替换原文件。保留游戏原文的大小写、拼写、标点、空格和换行。

## 随机残骸与活动机车名称

原文来自游戏 `VesselNames` 名称池及 `WreckVesselNameBehaviour.AssignName` 的显示规则：将 UCE、HML 前缀替换为 The。574 个不重复原始船名生成 573 个残骸显示名，全部原名已在 Steam 原生元数据中核对。

残骸显示名中，130 个已有译文，93 个可复用原始船名译文并去掉既有阵营前缀，均已由补丁覆盖；仅导出剩余 350 个缺译名称。

活动机车通过 `VesselNameBehaviour.SetName` 分别显示类型与随机船名。574 个原始船名中，135 个已有译文，89 个复用同名 The 残骸译文并补回“联合贸易机车”或“英国皇家机车”前缀，已由补丁覆盖；剩余 350 个对应本批待翻译名称，不另行重复导出。例如 `HML Wintersound` 对应 `The Wintersound`。

后续导入本批 The 名称译文时，应同步生成名称池中对应的 UCE／HML 显示映射；完整船名已有正式译文时保留其优先级。映射仅用于显示，不修改内部名称、名称占用与回收逻辑或存档。

## 银行与装备提示

银行说明来自 `BankController.Show()`，已确认实际显示的是完整正文，而非资源中残缺的 `Hallidges prides itslef ` 占位文字。导出保留正文末尾的空格及两次换行（` \n \n`）。

装备失败提示包括：

- `One of your skills is too low to equip this item.`
- `You dont have this`
- `That item does not fit in this equipment slot.`
- `You already have all these equipped`
- `You cannot change equipment unless you're docked`

技能不足提示已由截图确认，来自 `GridLocoPresenter.BeginEquip`，同文也由 `SkylessCharacter.TryEquip` 返回。其余提示来自 `SkylessCharacter.TryEquip` 和 `EquipmentService.TryEquip`；已核对 `HoldController.EquipItem` 会将失败结果的 Message 交给警告弹窗。Steam 原生候选与 Epic IL 原文一致，保留原文中的 `dont` 拼写；不包含无 CurrentCharacter 等内部异常诊断文本。
