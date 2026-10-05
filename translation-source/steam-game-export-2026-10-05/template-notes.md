# Steam 动态模板参数说明

以下模板由 Steam 原生代码还原，既有完整提示，也有追加到其他文字中的显示片段。参数含义按生成方法及其用途说明；保留变量标记、数值格式和富文本标签。接入补丁时仍需结合运行时样本验证。

模板中的 `{value1}` 等是导出时规范化的表示，不是游戏原生占位符。翻译时可调整顺序，但不要用一个固定数值或地点名替代参数。

## 1. `steam_template_18b5233d3dccee1d5d7a_2.0.6.454aad8`

```text
<b>Fuel Consumption:</b> {value1}%

```

- `{value1}`：燃料消耗变化百分比。

## 2. `steam_template_9ea7289e75970d664960_2.0.6.454aad8`

```text
<b>Full Steam Speed:</b> {value1}%

```

- `{value1}`：全速航行速度变化百分比。

## 3. `steam_template_7a50219a12202cf8b89b_2.0.6.454aad8`

```text
<b>Location:</b> {value1}
<b>Last saved:</b> {value2}
{value3}
```

- `{value1}`：所在地点。
- `{value2}`：上次保存时间。
- `{value3}`：版本说明文字。

## 4. `steam_template_4ca03e498301099a6b11_2.0.6.454aad8`

```text
<b>Location:</b> {value1}, {value2}
<b>Last saved:</b> {value3}
<i>Game version: {value4}</i>
```

- `{value1}`：所在地点。
- `{value2}`：所在区域。
- `{value3}`：上次保存时间。
- `{value4}`：游戏版本号。

## 5. `steam_template_cbd29b67329f362c701e_2.0.6.454aad8`

```text
<b>Movement Speed:</b> {value1}%

```

- `{value1}`：移动速度变化百分比。

## 6. `steam_template_a69aa9ebcebc9f707ac3_2.0.6.454aad8`

```text
<i>Game Version: {value1}</i>
```

- `{value1}`：游戏版本号。

## 7. `steam_template_60ab0a8456f17b8aed5e_2.0.6.454aad8`

```text
<style="descriptive">To complete this prospect, supply {value1} more:</style>
```

- `{value1}`：还需交付的数量。

## 8. `steam_template_838851f1806b686164a8_2.0.6.454aad8`

```text
Blast Damage: {value1}
```

- `{value1}`：爆炸伤害数值。

## 9. `steam_template_d5509c79da9935cb72ae_2.0.6.454aad8`

```text
Chart [{value1}]
```

- `{value1}`：地图快捷键。

## 10. `steam_template_e77a86431d848ec9643a_2.0.6.454aad8`

```text
Cost: {value1} {value2}
```

- `{value1}`：经验消耗数量。
- `{value2}`：经验单位缩写。

## 11. `steam_template_4c5df708c66ab83938fc_2.0.6.454aad8`

```text
Damage: {value1}
```

- `{value1}`：伤害数值。

## 12. `steam_template_e1d2b6eae7345c280a33_2.0.6.454aad8`

```text
Facets to Take:  <#FFD245>{value1}</color>
```

- `{value1}`：可选择的特质数量。

## 13. `steam_template_4fa87ab06a0b035ec33b_2.0.6.454aad8`

```text
Facets to Take:  <#FFFFFFB2>{value1}</color>
```

- `{value1}`：可选择的特质数量。

## 14. `steam_template_1f5be24719109a595bf5_2.0.6.454aad8`

```text
Heat: {value1}
```

- `{value1}`：热量数值。

## 15. `steam_template_03e0a417b26135b35bd0_2.0.6.454aad8`

```text
Hold [{value1}]
```

- `{value1}`：货舱快捷键。

## 16. `steam_template_8a6d3cf8a5f0699a72dd_2.0.6.454aad8`

```text
Journal [{value1}]
```

- `{value1}`：日志快捷键。

## 17. `steam_template_2f0f436bfecfb611983e_2.0.6.454aad8`

```text
Level: {value1}
Experience: {value2}

Congratulations! Your captain is now at level 20, and cannot gain more facets. However, you can still increase their abilities by spending experience on the facet screen.
```

- `{value1}`：等级。
- `{value2}`：当前经验值。

## 18. `steam_template_141c6f97fa7b2fa48348_2.0.6.454aad8`

```text
Level: {value1}
Experience: {value2}/{value3}
Facets to take: {value4}

<b><i>Click to view facets</i></b>
```

- `{value1}`：等级。
- `{value2}`：当前经验值。
- `{value3}`：经验上限。
- `{value4}`：可选择的特质数量。

## 19. `steam_template_3e6888fa854d7c0e3a0f_2.0.6.454aad8`

```text
New bargains available in {value1} days.
```

- `{value1}`：距特价商品刷新的天数。

## 20. `steam_template_3cd64e3d2ee51d72062e_2.0.6.454aad8`

```text
New prospects available in {value1} days.
```

- `{value1}`：距商机刷新的天数。

## 21. `steam_template_559a984eb8037c3160f7_2.0.6.454aad8`

```text
Officers [{value1}]
```

- `{value1}`：军官界面快捷键。

## 22. `steam_template_84e16f5a3e71c12c7997_2.0.6.454aad8`

```text
Profile [{value1}]
```

- `{value1}`：角色界面快捷键。

## 23. `steam_template_6fcae8ae7fd3fa46b66c_2.0.6.454aad8`

```text
Range: {value1}
```

- `{value1}`：射程数值。

## 24. `steam_template_b039fa554b8e23921b45_2.0.6.454aad8`

```text
Terror: {value1}%
Nightmares: {value2}
```

- `{value1}`：恐惧百分比。
- `{value2}`：梦魇等级描述。

## 25. `steam_template_26bc7f41312f0e170714_2.0.6.454aad8`

```text
This locomotive has a combined hull and armour value of {value1}.
```

- `{value1}`：船体与装甲合计数值。

## 26. `steam_template_b090df81e0665e52df28_2.0.6.454aad8`

```text
Though most fixtures were reclaimed, the {value1} was unfortunately lost.
```

- `{value1}`：未能继承的装备名称。

## 27. `steam_template_c48f7d18a26e9bb751ab_2.0.6.454aad8`

```text
Weight: {value1}
```

- `{value1}`：重量数值。

## 28. `steam_template_56253b819c9dfbef3283_2.0.6.454aad8`

```text
You donate your {value1} to your chosen successor. You hope they can handle it.
```

- `{value1}`：留给继任者的机车名称。

## 29. `steam_template_6704d23757bf296a1b29_2.0.6.454aad8`

```text
Your Locomotive can carry a maximum of {value1} crew.
```

- `{value1}`：船员人数上限。

## 30. `steam_template_77913dd73d3aa4d79079_2.0.6.454aad8`

```text
Your {value1} Will change by {value2}
```

- `{value1}`：属性名称。
- `{value2}`：变动量。

## 31. `steam_template_df431bf0a2b7812863b1_2.0.6.454aad8`

```text
Your {value1} gives you a <b>{value2}% chance of success</b>
```

- `{value1}`：属性名称。
- `{value2}`：成功率百分比。

## 32. `steam_template_6124df5214947abaf93b_2.0.6.454aad8`

```text
x{value1}
```

- `{value1}`：燃料消耗倍率。

## 33. `steam_template_d96c88756df52d9a4d31_2.0.6.454aad8`

```text
x{value1} (Economical)

The more you carry, the faster your engine consumes fuel.
```

- `{value1}`：燃料消耗倍率。

## 34. `steam_template_a209137f5e9553141381_2.0.6.454aad8`

```text
x{value1} (Improvident)

The more you carry, the faster your engine consumes fuel.
```

- `{value1}`：燃料消耗倍率。

## 35. `steam_template_f973632d99b778ec0c05_2.0.6.454aad8`

```text
x{value1} (Standard)

The more you carry, the faster your engine consumes fuel.
```

- `{value1}`：燃料消耗倍率。

## 36. `steam_template_716ecb7f089983075190_2.0.6.454aad8`

```text
{value1}

 <b>Deliver {value2} more to complete this prospect.</b>
```

- `{value1}`：商机说明正文。
- `{value2}`：还需交付的数量。

## 37. `steam_template_2cbf6efb1235d8d95050_2.0.6.454aad8`

```text
{value1}

 <b>Deliver {value2} to complete this prospect.</b>
```

- `{value1}`：商机说明正文。
- `{value2}`：需要交付的数量。

## 38. `steam_template_be270da4b10389de7882_2.0.6.454aad8`

```text
{value1}

<style="descriptive">You can only have four Prospects at a time. Complete or abandon one from the Locomotive screen to free up a slot.</style>
```

- `{value1}`：集市说明正文。

## 39. `steam_template_c319245c87acaa8f2950_2.0.6.454aad8`

```text
{value1}

<style="descriptive">You can only have {value2} Smuggling Prospects at a time. Complete or abandon one from the Locomotive screen to free up a slot.</style>
```

- `{value1}`：集市说明正文。
- `{value2}`：可同时持有的走私商机数量上限。

## 40. `steam_template_abd564af67bb12299f51_2.0.6.454aad8`

```text
{value1} (completed)
```

- `{value1}`：任务名称。

## 41. `steam_template_210865f0386db7639713_2.0.6.454aad8`

```text
{value1} Bazaar
```

- `{value1}`：地点名称。

## 42. `steam_template_123195827a48ac9d18ba_2.0.6.454aad8`

```text
{value1} Exp
```

- `{value1}`：经验值。

## 43. `steam_template_bd79576a255ee582cfe1_2.0.6.454aad8`

```text
{value1} Experience Gained
```

- `{value1}`：获得的经验值。

## 44. `steam_template_c807834e051364e2f495_2.0.6.454aad8`

```text
{value1} Here, you can acquire 'prospects': opportunities to sell a good at a distant port for an excellent price. Accept a prospect to claim it, source the goods, and deliver them.
```

- `{value1}`：集市已有的说明正文。

## 45. `steam_template_38b12d6f7be1f1c9c6ae_2.0.6.454aad8`

```text
{value1} You may find bargains here, or fulfil prospects you have claimed.
```

- `{value1}`：集市已有的说明正文。

## 46. `steam_template_507c640e4b3de7f91da2_2.0.6.454aad8`

```text
{value1} barrels in hold. 
{value2}% remaining in current barrel.
```

- `{value1}`：货舱中的燃料桶数。
- `{value2}`：当前桶剩余百分比。

## 47. `steam_template_b55be694d14a47493637_2.0.6.454aad8`

```text
{value1} crates in hold. 
{value2}% remaining in current crate.
```

- `{value1}`：货舱中的补给箱数。
- `{value2}`：当前箱剩余百分比。

## 48. `steam_template_d8f943943bd8118510d6_2.0.6.454aad8`

```text
{value1} crew members on board
```

- `{value1}`：当前船员人数。

## 49. `steam_template_32ab99550d679ead3d0e_2.0.6.454aad8`

```text
{value1}/10 Improvements Left
```

- `{value1}`：剩余改进次数。

## 50. `steam_template_fc7cd4d5d9db8cf214e1_2.0.6.454aad8`

```text
{value1}<i>No stories available here at this time.</i>
```

- `{value1}`：此前已经累积的说明文字。
