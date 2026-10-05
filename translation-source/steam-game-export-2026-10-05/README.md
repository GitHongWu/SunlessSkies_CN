# Steam 游戏代码文本检查（2026-10-05）

本目录已用 **Steam 原生代码检查结果**：Windows x86 / IL2CPP，`PlayerSettings.bundleVersion` **2.0.6.454aad8**，Unity 2019.4.2f1。

## 翻译增量

| 文件 | 条数 | 内容 |
| --- | ---: | --- |
| [ui-code-steam-2.0.6.454aad8.json](translator-delta/ui-code-steam-2.0.6.454aad8.json) | 68 | 固定界面文字、确认提示及集市默认说明 |
| [ui-code-templates-steam-2.0.6.454aad8.json](translator-delta/ui-code-templates-steam-2.0.6.454aad8.json) | 50 | 按键、地点、数量、属性、说明正文等动态模板及显示片段 |

合计 **118 条**，只含 `key`、`original`、`translation`，译文留空。文件名与词条键均标识 Steam 资源版本。

已与 `game-export-2026-09-17/full/` 精确去重。该目录仍是历史 Epic 资源导出，只作为已有原文的比较基线。

## 模板如何使用

参数含义见 [template-notes.md](template-notes.md)。`{value1}` 等是本次为可变部分设置的标记，不是游戏原生占位符。原有空格、标点和标签保留；部分模板是工具提示或长段落中的一个片段。

**模板翻译后仍需补丁端接入并在游戏中验证，不能仅复制这些 JSON 就生效。** 接入时保留数值格式和动态变量；地点名、说明正文等文字参数应走已有翻译流程。固定词条中的条件提示、旧存档迁移提示也未必在普通游玩中触发。

## 实际检查范围

以 Steam 的 `GameAssembly.dll` 和 `global-metadata.dat` 为输入，通过 Il2CppDumper 6.7.46 建立方法和字符串地址映射，再用 Capstone 5.0.9 反汇编并追踪部分 x86 字符串生成路径。**未将 BepInEx 生成的 interop 代理 DLL 当成游戏实现，也未使用 Epic IL 方法体生成本次条目。**

- 枚举游戏的 `Assembly-CSharp.dll`、`Assembly-CSharp-firstpass.dll` 元数据方法 **29,999 个**，其中 **26,623 个**带原生地址；抽象方法、共享实现和泛型等使这些数量不等同于独立函数数。
- 扫描 **16,384 个独立原生函数地址**，其中 **1,826 个**直接引用字符串，记录 **7,911 次**字符串引用。
- 对其中 **1,195 个 Skyless / Failbetter 命名空间内含字符串的函数**进行进一步参数、拼接、格式化和显示路径追踪。其他库和辅助代码的字符串仍列入待核对资料，不自动当作翻译词条。
- 正式增量来自直接文字赋值、界面文字参数、工具提示返回值，以及已筛选的 UI 生成方法中的常量或拼接结果。来源文件保留各项采用的证据类型，便于区别“追踪到显示赋值”与“在 UI 生成方法中确认”。

这是覆盖上述游戏代码范围的**静态扫描与部分生成路径还原**，不是全部运行时显示文字已完整导出的保证。**491 个方法**存在反汇编边界、分支预算或未支持运算等限制；其中有些仍能提供有效的局部结果。循环、多次跨方法拼接、虚函数/接口间接调用、反射、字符串构建器和运行时状态没有全部还原。条件分支按可能路径分析，不保证每种组合都能在游戏中触发。共享原生地址只扫描一次，可能缺少部分别名上下文。

本次不是 Steam 全部剧情资源的重新导出，没有覆盖所有资源包和运行时数据，也没有逐条在游戏中触发验证；这些不能由原生代码扫描替代。历史资源导出保持原样。

## 核对资料

- `review/confirmed-origins.json`：每条正式增量的 Steam 方法、RVA、显示或拼接位置、参数来源及筛选依据。RVA 为模块相对地址，不是运行时绝对地址。
- `review/source-fingerprints.json`：原生 DLL、元数据与版本资源文件的 SHA-256。

工具依据：[Il2CppDumper](https://github.com/Perfare/Il2CppDumper)、[Capstone](https://github.com/capstone-engine/capstone)。游戏原文权利归相应权利人；本目录不授予游戏原文再许可。
