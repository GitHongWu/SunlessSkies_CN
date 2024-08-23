using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

namespace SunlessSkiesPlugin;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]  // PLUGIN_GUID:csproj.AssemblyName PLUGIN_NAME:proj.Product
[BepInProcess("Sunless Skies.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    Harmony HarmonyInstance;
    public static Font TranslateFont;
    public static TMP_FontAsset TMPTranslateFont;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

        // LoadCustomFont();
        LoadFont("noto_sans_sc_variable_font_wght");
        Logger.LogInfo($"Font is loaded");

        HarmonyInstance = new Harmony($"{MyPluginInfo.PLUGIN_GUID} Harmony is loaded");
        HarmonyInstance.PatchAll();
        Logger.LogInfo($"Harmony Pathes Applied For {MyPluginInfo.PLUGIN_NAME}!");
    }

    private void Start()
    {
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} called Start!!!");
    }

    /// <summary>
    /// 加载字体
    /// </summary>
    /// <param name="fontName">字体名称</param>
    public void LoadFont(string fontName)
    {
        try
        {
            string path = $"{Paths.PluginPath}\\Resources\\{fontName}";
            Logger.LogInfo($"PluginPath: {path}");
            if (File.Exists(path))
            {
                var ab = AssetBundle.LoadFromFile(path);
                TranslateFont = ab.LoadAsset<Font>("NotoSansSC-VariableFont_wght");
                TMPTranslateFont = ab.LoadAsset<TMP_FontAsset>($"NotoSansSC-VariableFont_wght SDF");
                if (TranslateFont != null && TMPTranslateFont != null)
                {
                    Logger.LogInfo($"Loaded {fontName}.");
                }
                else
                {
                    Logger.LogError($"The font file is damaged. Please check the file.");
                }
                ab.Unload(false);
            }
            else
            {
                Logger.LogError($"Font {fontName} not found, Please check the path: {path}");
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Load font exception:{e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 修改字体
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(Text), "OnEnable")]
    public static void FontPatch(Text __instance)
    {
        Logger.LogInfo($"FontPatch called");
        if (__instance.font.name != TranslateFont.name)
        {
            __instance.font = TranslateFont;
        }
    }

    /// <summary>
    /// 修改TMP字体
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
    public static void TMPFontPatch(TextMeshProUGUI __instance)
    {
        Logger.LogInfo($"TMPFontPatch called");
        if (__instance.font.name != TMPTranslateFont.name)
        {
            __instance.font = TMPTranslateFont;
        }
    }

    /// <summary>
    /// 如果有不显示的文本，则设置显示方式为溢出
    /// </summary>
    [HarmonyPostfix, HarmonyPatch(typeof(TextMeshProUGUI), "InternalUpdate")]
    public static void TMPFontPatch2(TextMeshProUGUI __instance)
    {
        Logger.LogInfo($"TMPFontPatch2 called");
        if (__instance.font == TMPTranslateFont)
        {
            if (__instance.overflowMode != TextOverflowModes.Overflow)
            {
                if (__instance.preferredWidth > 1 && __instance.bounds.extents == Vector3.zero)
                {
                    __instance.overflowMode = TextOverflowModes.Overflow;
                }
            }
        }
    }

    [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
    static void TextMeshProUGUI_Postfix(TextMeshProUGUI __instance)
    {
        Logger.LogInfo($"TextMeshProUGUI_Postfix called");
        if (TMPTranslateFont != null)
        {
            __instance.font = TMPTranslateFont;
            __instance.fontSize = 24; // 可根据需要调整字体大小
        }
    }

    [HarmonyPatch(typeof(TMP_Text))]
    [HarmonyPatch("Awake")]
    static void TMP_Text_Postfix(TMP_Text __instance)
    {
        Logger.LogInfo($"TMP_Text_Postfix called");
        if (TMPTranslateFont != null)
        {
            __instance.font = TMPTranslateFont;
        }
    }
}
