# 提示补充、复数名称增量、以及部分遗漏（2026-10-21）

## 航行与状态提示补充

`2.0.7.0/logEntries.json`：79 条，作为增量导入。

- 2 条来自 SceneData：靠近新温彻斯特时的航行提示。
- 77 条来自 PlayerController：燃料／补给告急、恐惧／士气、船员不足、食物转换、受击、全速航行、地图边界与星光区域提示，包括车务官及吉祥物的反应。
- 来源为旧 `game-export-2026-09-17/review/asset-candidates.json`；曾因组件解析不完整而仅保留为待核对候选。这次在游戏内确认了其中两个实例；所以把本批全部 79 条已在本地 Steam 和 Epic 的 `Sunless Skies_Data/resources.assets` 中验证的原文导入。
- 不包含内部 GUID、AI 调试文字、角色创建资源标识、难度设置，以及 Reach RnD 测试场景中的描述；已有地点名称及说明不重复导出。

## 剩余派生名称增量

追加 95 条到已有 `2.0.7.0-Addenda/ui-runtime.json`，文件合计 96 条。译文留空，原有单数词条不修改。

这些原文来自两平台属性数据与游戏 Pluralize 方法的实际计算结果，包括复数和去冠词形式。部分输出英语形式不自然，但仍原样保留。

下表为导出原文与原始单数词条的对应关系，便于参照已有译文审核。

| 派生原文 | 属性 ID | 原始名称 |
| --- | --- | --- |
| Mistake, Forged into a Triumphs | 138917 | A Mistake, Forged into a Triumph |
| Adamant-Reinforced Windshields | 135538 | Adamant-Reinforced Windshield |
| Additional Cargo Bays | 137235 | Additional Cargo Bay |
| Adjustable Infirmaries | 137244 | Adjustable Infirmary |
| Agravain-Class Juggernaughts | 134946 | Agravain-Class Juggernaught |
| Altani-Class Outriders | 134944 | Altani-Class Outrider |
| Amniotic Crew Containments | 135534 | Amniotic Crew Containment |
| Elaborate But Fragile Artifices | 138927 | An Elaborate But Fragile Artifice |
| Exchange of Burdens | 138923 | An Exchange of Burdens |
| Old Orbit, Fondly Remembereds | 138928 | An Old Orbit, Fondly Remembered |
| Auxiliaries | 131179 | Auxiliary |
| Bedevilled Didacts | 139315 | Bedevilled Didact |
| Bedivere-Class Escorts | 134930 | Bedivere-Class Escort |
| Bridges | 131178 | Bridge |
| Caminus Yards 'Blackjape's | 137211 | Caminus Yards 'Blackjape' |
| Caminus Yards 'Brassraven's | 133107 | Caminus Yards 'Brassraven' |
| Caminus Yards 'Grimalkin's | 137210 | Caminus Yards 'Grimalkin' |
| Caminus Yards 'Saintfire's | 137213 | Caminus Yards 'Saintfire' |
| 'Cantankerous' Boring-Rigs | 135539 | 'Cantankerous' Boring-Rig |
| Chief Engineers | 131169 | Chief Engineer |
| Cotterell & Hathersage 'Beulah's | 137205 | Cotterell & Hathersage 'Beulah' |
| Cotterell & Hathersage 'Emanation's | 133108 | Cotterell & Hathersage 'Emanation' |
| Cotterell & Hathersage 'Vala's | 137212 | Cotterell & Hathersage 'Vala' |
| Crown & Misery Pneumatic Mining Arrays | 134834 | Crown & Misery Pneumatic Mining Array |
| Current Locomotives | 132796 | Current Locomotive |
| Cyclopean Owls | 136735 | Cyclopean Owl |
| Date of Fuzzs | 138157 | Date of Fuzz |
| 'Durendal' Pressure-Canning Systems | 135082 | 'Durendal' Pressure-Canning System |
| Engines | 140435 | Engine |
| First Officers | 131168 | First Officer |
| Gates of Ivories | 137246 | Gates of Ivory |
| Gleaming Galleys | 137239 | Gleaming Galley |
| 'Heathcliffe' Durable Platings | 135542 | 'Heathcliffe' Durable Plating |
| Hidden Holds | 139181 | Hidden Hold |
| Lamellared Sky-Steel Platings | 137240 | Lamellared Sky-Steel Plating |
| Mascots | 131172 | Mascot |
| Masked Citizens | 139316 | Masked Citizen |
| Medea-Class Destroyers | 134947 | Medea-Class Destroyer |
| 'Mighty Pen' Defensive Library Systems | 135537 | 'Mighty Pen' Defensive Library System |
| Minimum Safe Manning Numbers | 138302 | Minimum Safe Manning Number |
| 'Montresor' Chambers | 135532 | 'Montresor' Chamber |
| Murgatroyd's 'Stain-away' Canning Stations | 135080 | Murgatroyd's 'Stain-away' Canning Station |
| Officer ScuzzleFuzzs | 132939 | Officer ScuzzleFuzz |
| Old Friends | 139450 | Old Friend |
| Parsival-Class Coursers | 134928 | Parsival-Class Courser |
| Pellinore-Class Traders | 134929 | Pellinore-Class Trader |
| Platings | 131177 | Plating |
| Plucky Baronesses | 139313 | Plucky Baroness |
| 'Pluto' Miniature Law-Furnaces | 135543 | 'Pluto' Miniature Law-Furnace |
| Portsmouth House 'Albertine Candle's | 139069 | Portsmouth House 'Albertine Candle' |
| Portsmouth House 'Jubilee's | 139070 | Portsmouth House 'Jubilee' |
| Primary Weapons | 131180 | Primary Weapon |
| 'Proserpina' Superior Mining Arrays | 135541 | 'Proserpina' Superior Mining Array |
| Quartermasters | 131171 | Quartermaster |
| Reclaimed Marauder Mangonels | 137196 | Reclaimed Marauder Mangonel |
| Rose & Adamant Platings | 137245 | Rose & Adamant Plating |
| Secondary Weapons | 131181 | Secondary Weapon |
| Sensible Plumbings | 137237 | Sensible Plumbing |
| Signallers | 131170 | Signaller |
| 'Signora Zenobia' Prestige Scythes | 135535 | 'Signora Zenobia' Prestige Scythe |
| 'Spatchcock' Reclaimed Locomotives | 132785 | 'Spatchcock' Reclaimed Locomotive |
| 'Spatchcocker' Megs | 139314 | 'Spatchcocker' Meg |
| The Blue Kingdom's Gratitudes | 139091 | The Blue Kingdom's Gratitude |
| The Britomarts | 140437 | The Britomart |
| The Cloak of the Lions | 139354 | The Cloak of the Lion |
| The Conquest of an All-Too-Familiar Rivals | 138920 | The Conquest of an All-Too-Familiar Rival |
| The Erroneous Assumption That There Will Be a Tomorrows | 138926 | The Erroneous Assumption That There Will Be a Tomorrow |
| The Final Moment of Accelerations | 138921 | The Final Moment of Acceleration |
| The Imagined Contents of a Span of Time Between When an Event was Supposed to Occur, and When it Finally Dids | 138915 | The Imagined Contents of a Span of Time Between When an Event was Supposed to Occur, and When it Finally Did |
| The Insistent Surveyor and her Teams | 140315 | The Insistent Surveyor and her Team |
| The Lost Love's Voices | 138969 | The Lost Love's Voice |
| The Mesmeric Orators | 138975 | The Mesmeric Orator |
| The 'Osiris' Patented Dividers | 135545 | The 'Osiris' Patented Divider |
| The Pain Experienced at Apogees | 138912 | The Pain Experienced at Apogee |
| The Revelation That Uncertainty is Itself an Answers | 138916 | The Revelation That Uncertainty is Itself an Answer |
| The Rite of Ravings | 139656 | The Rite of Raving |
| The Shield of the Stars | 139355 | The Shield of the Star |
| The Shivering Jemmies | 140436 | The Shivering Jemmy |
| The 'Speciometer' Assaying Devices | 135705 | The 'Speciometer' Assaying Device |
| 'The Tears of Astolat's | 137198 | 'The Tears of Astolat' |
| The 'Uninvited's | 137197 | The 'Uninvited' |
| The Unseen Queen, depicted with the Martyr-King's Cups | 139332 | The Unseen Queen, depicted with the Martyr-King's Cup |
| The Watchers in Azures | 137247 | The Watchers in Azure |
| The Wound That Saves Your Lives | 138925 | The Wound That Saves Your Life |
| 'The Wrath of Heaven's | 137199 | 'The Wrath of Heaven' |
| To Become Fire Rather Than Be Burneds | 138911 | To Become Fire Rather Than Be Burned |
| Two Futures, Endlessly Circlings | 138914 | Two Futures, Endlessly Circling |
| Upgraded Chiropterous Hoarders | 132943 | Upgraded Chiropterous Hoarder |
| Upgraded Clay Conductors | 132922 | Upgraded Clay Conductor |
| Wit & Vinegar Sawing Devices | 135712 | Wit & Vinegar Sawing Device |
| Wit & Vinegar 'Sneeze-lurker's | 139072 | Wit & Vinegar 'Sneeze-lurker' |
| Wit & Vinegar Winch & Pulleys | 137238 | Wit & Vinegar Winch & Pulley |
| Wit & Vinegar 'Zounderkite's | 139073 | Wit & Vinegar 'Zounderkite' |
| Your Undercover Aunts | 136292 | Your Undercover Aunt |
| Sunless Sea: A Whisper-Locked Puzzle-Boxes | 116130 | Sunless Sea: A Whisper-Locked Puzzle-Box |

## 任务完成标记修正

`NEW/qualities.json` 为现有 ParaTranz 文件的修正，包含 5 个词条中的 7 个内层分支。来源为 2026-10-21 快照；修复为仅在遗漏标记的译文开头添加 `[COMPLETE]`以匹配原文，每条导出均保留完整内层 JSON。

| Key | 修正分支 |
| --- | --- |
| `136056$LevelDescriptionText` | `70`、`80`、`90` |
| `139080$LevelDescriptionText` | `200` |
| `137200$LevelDescriptionText` | `100` |
| `139968$LevelDescriptionText` | `20` |
| `139338$LevelDescriptionText` | `2` |

已验证外层和内层 JSON 均可解析，且仅以上 7 个分支值增加前缀。**补丁已兼容其他已有 `[完成]` 前缀的译文**。

## 继承与胜利赠礼遗漏

向现有 `2.0.7.0-Addenda/ui-runtime.json` 增补 8 条原文，译文留空。来源为 resources.assets 中 `AffiliationQualitiesLibrary` 的四组 `AffiliationInheritenceQuote` 与 `AffiliationVictoryGiftQuote`，覆盖学术、波希米亚、权贵和恶徒关系的继承／胜利赠礼。原文标点保持不变。
