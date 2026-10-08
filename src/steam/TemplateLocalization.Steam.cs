using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace SunlessSkiesCNSteam
{
    // Display-only lookup: never changes repositories, IDs, qualities or save data.
    public sealed class TemplateCatalog
    {
        readonly Dictionary<string, string> literals = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> variables = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> templates = new Dictionary<string, string>(StringComparer.Ordinal);
        static readonly Regex Tokens = new Regex(@"\[[^:\]\r\n]+:[^\]\r\n]+\]|\[(?:gendertitle|genderdescription|genderdescriptionsimple|he|his|him|Actor|CharacterName)\]", RegexOptions.CultureInvariant);
        static readonly Regex Direction = new Regex(@"\A(?<place>.+) lies(?<distance> a long way| some way)? to the (?<direction>northwest|northeast|southwest|southeast|north|south|east|west) of (?<origin>.+)\z", RegexOptions.CultureInvariant);
        public int Count { get { return templates.Count; } }
        public int Rejected { get; private set; }
        public static string Normalize(string text) { return text == null ? null : text.Replace("\r\n", "\n"); }
        public static string[] TokenList(string text)
        {
            return Tokens.Matches(text).Cast<Match>().Select(m => m.Value).OrderBy(s => s, StringComparer.Ordinal).ToArray();
        }
        public void Add(string original, string translation)
        {
            if (String.IsNullOrEmpty(original) || String.IsNullOrEmpty(translation) || original.StartsWith("sr:") || original.StartsWith("r:")) return;
            original = Normalize(original);
            translation = Normalize(translation);
            // The shipped table has already resolved ParaTranz/legacy precedence.
            literals[original] = translation;
            var tokens = TokenList(original);
            if (tokens.Length == 0) return;
            if (!tokens.SequenceEqual(TokenList(translation), StringComparer.Ordinal)) { Rejected++; templates.Remove(original); return; }
            templates[original] = translation;
        }
        public string Translate(string text)
        {
            if (text == null || text.IndexOf('[') < 0) return text;
            string translated;
            return templates.TryGetValue(Normalize(text), out translated) ? translated : text;
        }
        public int VariableCount { get { return variables.Count; } }
        public void AddVariable(string original, string translation)
        {
            if (String.IsNullOrEmpty(original) || String.IsNullOrEmpty(translation)) return;
            original = Normalize(original); translation = Normalize(translation);
            if (TokenList(original).SequenceEqual(TokenList(translation), StringComparer.Ordinal))
                variables[original] = translation;
        }
        public string TranslateVariable(string text)
        {
            if (String.IsNullOrEmpty(text)) return text;
            string translated;
            string original = Normalize(text);
            if (variables.TryGetValue(original, out translated)) return translated;
            if (literals.TryGetValue(original, out translated) &&
                TokenList(original).SequenceEqual(TokenList(translated), StringComparer.Ordinal)) return translated;
            return text;
        }
        public string Name(string text)
        {
            string translated;
            return literals.TryGetValue(text, out translated) ? translated : text;
        }
        public string TranslateDirection(string text)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = Direction.Match(text);
            if (!match.Success) return text;
            var directions = new Dictionary<string, string> { {"north", "北"}, {"south", "南"}, {"east", "东"}, {"west", "西"}, {"northeast", "东北"}, {"northwest", "西北"}, {"southeast", "东南"}, {"southwest", "西南"} };
            string distance = match.Groups["distance"].Value;
            return Name(match.Groups["place"].Value) + "位于" + Name(match.Groups["origin"].Value) + "的" + directions[match.Groups["direction"].Value] + "方" + (distance == " a long way" ? "很远处" : distance == " some way" ? "一段距离处" : "");
        }
    }

    [BepInPlugin("githongwu.sunlessskies.template-localization", "Sunless Skies Template Localization", "0.2.5")]
    [BepInDependency("gravydevsupreme.xunity.autotranslator")]
    public sealed class TemplateLocalization : BasePlugin
    {
        static TemplateCatalog catalog;
        static FieldInfo translatorCurrent, translatedMode;
        [ThreadStatic] static int translatedDepth;
        // BepInEx's Unity component can be destroyed during scene transitions.
        // These static display hooks and their catalog belong to the process,
        // not to that component; do not unpatch them in OnDestroy.
        static Harmony harmony;
        static BepInEx.Logging.ManualLogSource log;
        static int traceCount;
        static bool targetTraced;
        static void Trace(string stage, string before, string after, bool enabled)
        {
            if (before == null) return;
            bool target = before.IndexOf("The host at Magdalene", StringComparison.Ordinal) >= 0;
            if (target ? targetTraced : traceCount >= 3 || before.IndexOf("[dir:", StringComparison.Ordinal) < 0) return;
            if (target) targetTraced = true; else traceCount++;
            string sample = before.Length > 1600 ? before.Substring(0, 1600) : before;
            log.LogInfo("Template trace: stage=" + stage + "; enabled=" + enabled + "; matched=" + !String.Equals(before, after, StringComparison.Ordinal) + "; length=" + before.Length + "; input=" + sample.Replace("\r", "\\r").Replace("\n", "\\n"));
        }
        static bool Enabled()
        {
            var current = translatorCurrent.GetValue(null);
            return current != null && (bool)translatedMode.GetValue(current);
        }
        public override void Load()
        {
            if (harmony != null) return;
            log = base.Log;
            if (!Config.Bind("General", "Enabled", true, "Translate display templates before token expansion. Restart after changing.").Value) return;
            try
            {
                var core = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "XUnity.AutoTranslator.Plugin.Core");
                var helper = core.GetType("XUnity.AutoTranslator.Plugin.Core.Utilities.TextHelper", true);
                var decode = helper.GetMethod("ReadTranslationLineAndDecode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var translator = core.GetType("XUnity.AutoTranslator.Plugin.Core.AutoTranslationPlugin", true);
                translatorCurrent = translator.GetField("Current", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                translatedMode = translator.GetField("_isInTranslatedMode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (decode == null || translatorCurrent == null || translatedMode == null) throw new MissingMemberException("Unsupported AutoTranslator version");
                var loaded = new TemplateCatalog();
                foreach (var line in File.ReadLines(Path.Combine(Paths.BepInExRootPath, "Translation/zh/Text/ui.txt")))
                {
                    var pair = (string[])decode.Invoke(null, new object[] { line });
                    if (pair != null && pair.Length == 2) loaded.Add(pair[0], pair[1]);
                }
                var variableFile = Path.Combine(Paths.BepInExRootPath, "Translation/zh/qvd-branches.txt");
                if (File.Exists(variableFile)) foreach (var line in File.ReadLines(variableFile))
                {
                    var pair = (string[])decode.Invoke(null, new object[] { line });
                    if (pair != null && pair.Length == 2) loaded.AddVariable(pair[0], pair[1]);
                }
                var game = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
                try { SunlessSkiesCN.Shared.ShopNumericGuard.Install(core, game, message => Log.LogInfo(message)); }
                catch (Exception guardError) { Log.LogError("Shop numeric guard disabled: " + guardError); }
                var formatter = game.GetType("Failbetter.Presentation.Formatters.BaseStoryFormatter", true);
                var dynamic = game.GetType("Failbetter.Presentation.Formatters.DynamicTokenReplacement", true);
                var character = game.GetType("Failbetter.Core.Character", true);
                var all = AccessTools.Method(formatter, "ReplaceAllTokens", new[] { typeof(string), character });
                var format = AccessTools.Method(formatter, "ReplaceFormattingTokens", new[] { typeof(string) });
                var expand = AccessTools.Method(dynamic, "ReplaceDynamicVariableNameTokens", new[] { typeof(string), character });
                var direction = AccessTools.Method(dynamic, "GetDirectionForToken", new[] { character, typeof(string) });
                var variable = AccessTools.Method(dynamic, "GetQualityVariableForToken", new[] { character, typeof(string) });
                if (new[] { all, format, expand, direction, variable }.Any(m => m == null || m.ReturnType != typeof(string))) throw new MissingMethodException("Unsupported game formatter signatures");
                catalog = loaded;
                harmony = new Harmony("githongwu.sunlessskies.template-localization");
                var prefix = new HarmonyMethod(typeof(TemplateLocalization), "TemplatePrefix");
                harmony.Patch(all, prefix: prefix);
                harmony.Patch(format, prefix: prefix);
                harmony.Patch(expand, prefix: new HarmonyMethod(typeof(TemplateLocalization), "DynamicPrefix"), finalizer: new HarmonyMethod(typeof(TemplateLocalization), "DynamicFinalizer"));
                harmony.Patch(variable, postfix: new HarmonyMethod(typeof(TemplateLocalization), "VariablePostfix"));
                harmony.Patch(direction, postfix: new HarmonyMethod(typeof(TemplateLocalization), "DirectionPostfix"));
                Log.LogInfo("Template localization ready: " + loaded.Count + " templates; " + loaded.Rejected + " unsafe token mappings skipped; " + loaded.VariableCount + " variable branches loaded.");
            }
            catch (Exception ex)
            {
                if (harmony != null) { harmony.UnpatchSelf(); harmony = null; }
                Log.LogError("Template localization disabled; original formatter retained. " + ex);
            }
        }
        static void TemplatePrefix(ref string __0)
        {
            string before = __0;
            bool enabled = Enabled();
            if (enabled) __0 = catalog.Translate(__0);
            Trace("formatter", before, __0, enabled);
        }
        static void DynamicPrefix(ref string __0, out bool __state)
        {
            __state = false;
            bool enabled = Enabled();
            string before = __0;
            if (!enabled) { Trace("dynamic", before, __0, false); return; }
            __0 = catalog.Translate(__0);
            Trace("dynamic", before, __0, true);
            // Only localize generated directions within an already translated display template.
            if (__0 != null && __0.Any(c => c >= '\u3400' && c <= '\u9fff'))
            {
                translatedDepth++;
                __state = true;
            }
        }
        static Exception DynamicFinalizer(Exception __exception, bool __state)
        {
            if (__state) translatedDepth--;
            return __exception;
        }
        static void VariablePostfix(ref string __result)
        {
            // Only translate display output inside a translated template, never game state.
            if (translatedDepth > 0 && Enabled()) __result = catalog.TranslateVariable(__result);
        }
        static void DirectionPostfix(ref string __result)
        {
            if (translatedDepth > 0) __result = catalog.TranslateDirection(__result);
        }
    }
}


