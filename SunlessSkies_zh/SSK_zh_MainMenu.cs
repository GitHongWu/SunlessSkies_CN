using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Skyless.Assets.Code.Skyless.UI.Controllers;
using Skyless.Assets.Code.Skyless.UI.Presenters.TitleScreen;

namespace SunlessSkiesPlugin.SunlessSkies_zh;

[HarmonyPatch(typeof(TitleScreenController))]
public static class SSK_zh_MainMenu
{
    [HarmonyTranspiler]
    [HarmonyPatch("CreateNewGameButton")]
    static IEnumerable<CodeInstruction> Patch_MainMenu(IEnumerable<CodeInstruction> instructions)
    {
        Dictionary<string, string> trans = new()
        {
            ["Exit to Desktop"] = "退出到桌面",
            ["New Game"] = "新游戏",
            ["Options"] = "设置",
            ["Load Game"] = "载入游戏",
            ["Credits"] = "制作人员",
            ["Quit to Title Screen"] = "回到标题界面",
            ["Only available while docked."] = "仅在停泊时可用。"
        };
        ILReplacer(instructions, trans);
        return instructions;
    }


    // Replaces strings in IL code
    public static IEnumerable<CodeInstruction> ILReplacer(IEnumerable<CodeInstruction> codes, Dictionary<string, string> translationDict)
    {
        var codeList = codes.ToList();
        foreach (var code in codeList.Where(code => code.opcode == OpCodes.Ldstr && translationDict.ContainsKey(code.operand.ToString())))
        {
            code.operand = translationDict[code.operand.ToString()];
        }
        return codeList.AsEnumerable();
    }

}