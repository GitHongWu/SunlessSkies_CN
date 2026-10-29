using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    public static partial class RemainingCompositionLocalization
    {
        static Func<string,string> lookup, variables;
        static Func<string,string,string> contextLookup;
        [ThreadStatic] static Dictionary<int,int> questQualityIds;
        public static void ConfigureContexts(Func<string,string,string> translate) { contextLookup = translate; }
        public static string TranslateBranch(object quality, string field, string text, Func<string,string> fallback)
        {
            if (quality == null) return fallback(text);
            return TranslateBranch((int)quality.GetType().GetProperty("Id").GetValue(quality, null), field, text, fallback);
        }
        public static string TranslateBranch(int id, string field, string text, Func<string,string> fallback)
        {
            if (contextLookup == null) return fallback(text);
            return TranslateCompletedBranch(text, source => contextLookup(id + "$" + field, source));
        }
        static Func<bool> enabled;
        static Action<string> log;
        static int errors;
        static int questTraces;
        [ThreadStatic] static int displayDepth;
        [ThreadStatic] static int categoryDepth;
        [ThreadStatic] static int questDepth;
        static readonly Regex Slots = new Regex(@"\{value([0-9]+)\}");
        // Bracketed prose is translatable; markup names and control expressions must remain intact.
        static readonly Regex Tags = new Regex(@"<[^>]*>|\[(?:/?[A-Za-z][A-Za-z0-9_-]*(?:[=:][^\]\r\n]*)?|#[A-Fa-f0-9]+)\]");
        static readonly Regex Reward = new Regex(@"\A(?<name>[^<>\[\]{}\r\n]+?) (?<number>\+?-?[0-9]+)\z");
        static readonly Regex Level = new Regex(@"\A(?<number>-?[0-9]+ - )(?<body>.+)\z");

        sealed class Rule
        {
            public readonly string Template, Kinds, Prefix;
            public readonly Regex Pattern;
            public Rule(string template, string kinds)
            {
                Template = template; Kinds = kinds;
                int first = template.IndexOf("{value", StringComparison.Ordinal);
                Prefix = first < 0 ? template : template.Substring(0, first);
                int index = 0, end = 0;
                string pattern = @"\A";
                foreach (Match slot in Regex.Matches(template, @"\{value([0-9]+)\}"))
                {
                    pattern += Regex.Escape(template.Substring(end, slot.Index - end));
                    char kind = kinds[index];
                    string capture = kind == 'N' ? @"[+-]?[0-9]+(?:[.,][0-9]+)?" : kind == 'C' ? @"[A-Fa-f0-9]{6,8}" :
                        kind == 'B' ? @"[\s\S]{0,8192}?" : kind == 'K' ? @"[^\r\n]{1,256}?" : kind == 'T' ? @"(?:-?[0-9]+ - )?(?:\[(?i:COMPLETE)\])?[^<>\[\]{}\r\n]{1,512}?" : @"[^<>\[\]{}\r\n]{1,512}?";
                    pattern += "(?<v" + (++index) + ">" + capture + ")";
                    end = slot.Index + slot.Length;
                }
                if (index != kinds.Length) throw new ArgumentException("Invalid composition rule");
                Pattern = new Regex(pattern + Regex.Escape(template.Substring(end)) + @"\z", RegexOptions.CultureInvariant);
            }
        }

        public static void Install(Assembly game, Func<string,string> translate, Func<string,string> variableTranslate, Func<bool> active, Action<string> logger)
        {
            lookup = translate; variables = variableTranslate; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.remaining-compositions");
            int count = 0;
            try
            {
                foreach (string target in FormatterTargets.Concat(DisplayTargets).Distinct())
                {
                    string[] parts = target.Split(new[] { "::" }, StringSplitOptions.None);
                    var type = game.GetType(parts[0], true);
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                        .Where(m => m.Name == parts[1]).ToArray();
                    if (methods.Length == 0) throw new MissingMethodException(target);
                    foreach (var method in methods)
                    {
                        if (method.ReturnType == typeof(string))
                            harmony.Patch(method, postfix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "ResultPostfix"));
                        else if (DisplayTargets.Contains(target))
                            harmony.Patch(method, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "Enter"), finalizer: new HarmonyMethod(typeof(RemainingCompositionLocalization), "Leave"));
                        else throw new MissingMethodException("Non-string formatter: " + target);
                        count++;
                    }
                }
                var tmp = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("TMPro.TMP_Text")).FirstOrDefault(t => t != null);
                var setter = tmp == null ? null : AccessTools.PropertySetter(tmp, "text");
                if (setter == null) throw new MissingMethodException("TMP_Text.text");
                harmony.Patch(setter, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "TextPrefix"));
                foreach (string name in new[] { "QualityCategoryPresenter", "StatusQualityCategoryPresenter" })
                {
                    var init = AccessTools.Method(game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Generic." + name, true), "Init");
                    if (init == null) throw new MissingMethodException(name + ".Init");
                    harmony.Patch(init, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "CategoryEnter"), finalizer: new HarmonyMethod(typeof(RemainingCompositionLocalization), "CategoryLeave"));
                }
                var tooltip = game.GetType("Skyless.Assets.Code.Skyless.UI.Tooltips.Tooltip", true);
                foreach (var bind in tooltip.GetMethods().Where(m => m.Name.StartsWith("Bind", StringComparison.Ordinal) && m.GetParameters().Length >= 3 && m.GetParameters()[1].ParameterType == typeof(string) && m.GetParameters()[2].ParameterType == typeof(string)))
                    harmony.Patch(bind, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "TooltipPrefix"));
                var possession = game.GetType("Failbetter.Presentation.PresentationData.PossessionPresentation", true);
                var quest = AccessTools.Method(possession, "AssignQuestValues");
                if (quest == null) throw new MissingMethodException("AssignQuestValues");
                harmony.Patch(quest, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestFactoryPrefix"),
                    postfix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestPostfix"),
                    finalizer: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestFinalizer"));
                // IL2CPP cannot detour constructors; these native factories call the inlined quest path.
                var service = game.GetType("Skyless.Assets.Code.Skyless.Game.Services.CharacterService", true);
                foreach (string name in new[] { "GetPossessionsForCategories", "GetObjectivesForQuest" })
                {
                    var factory = service.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                        .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                        .Single(m => (m.Name.StartsWith("<" + name + ">b__", StringComparison.Ordinal) ||
                            m.Name.StartsWith("_" + name + "_b__", StringComparison.Ordinal)) &&
                            m.ReturnType == possession && m.GetParameters().Length == 1);
                    harmony.Patch(factory, prefix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestFactoryPrefix"),
                        postfix: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestFactoryPostfix"),
                        finalizer: new HarmonyMethod(typeof(RemainingCompositionLocalization), "QuestFinalizer"));
                }
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Remaining composition localization ready: " + count + " formatter/display methods; scoped TMP and tooltip inputs.");
        }

        static void Enter(out bool __state) { __state = enabled(); if (__state) displayDepth++; }
        static Exception Leave(Exception __exception, bool __state) { if (__state) displayDepth--; return __exception; }
        // Translate only the title write; Init also stores the original name for game logic.
        static void CategoryEnter(out bool __state) { __state = enabled(); if (__state) categoryDepth++; }
        static Exception CategoryLeave(Exception __exception, bool __state) { if (__state) categoryDepth--; return __exception; }
        static void Error(Exception e) { if (errors++ < 4) log("Composition fallback: " + e.Message); }
        static void ResultPostfix(ref string __result)
        {
            try { if (enabled()) __result = Localize(__result, lookup, variables); }
            catch (Exception e) { Error(e); }
        }
        static void TextPrefix(ref string __0)
        {
            try
            {
                if (!enabled()) return;
                if (categoryDepth > 0 && !String.IsNullOrEmpty(__0))
                {
                    string direct = Safe(__0, lookup);
                    if (direct != __0) __0 = direct;
                    else
                    {
                        var region = Regex.Match(__0, @"\A(?<region>Reach|Albion|Eleutheria|Blue Kingdom): (?<place>.+?)(?<suffix>:\s*)?\z");
                        if (region.Success)
                        {
                            // Reach is a region here, not the verb in the legacy dictionary.
                            string name = region.Groups["region"].Value;
                            string key = name == "Reach" || name == "Blue Kingdom" ? "The " + name : name;
                            string place = region.Groups["place"].Value;
                            string translatedRegion = Safe(key, lookup), translatedPlace = Safe(place, lookup);
                            if (translatedRegion != key && translatedPlace != place)
                                __0 = translatedRegion + "：" + translatedPlace + region.Groups["suffix"].Value;
                        }
                        else
                        {
                            var suffix = Regex.Match(__0, @":\s*\z");
                            if (suffix.Success) __0 = Safe(__0.Substring(0, suffix.Index), lookup) + suffix.Value;
                        }
                    }
                }
                if (displayDepth > 0) __0 = Localize(__0, lookup, variables);
            }
            catch (Exception e) { Error(e); }
        }
        static void TooltipPrefix(ref string __1, ref string __2)
        {
            try
            {
                if (displayDepth == 0 || !enabled()) return;
                string title = Localize(__1, lookup, variables), body = Localize(__2, lookup, variables);
                __1 = title; __2 = body;
            }
            catch (Exception e) { Error(e); }
        }
        static void QuestPrefix(out bool __state) { __state = enabled(); if (__state) questDepth++; }
        static Exception QuestFinalizer(Exception __exception, bool __state) { if (__state) { if (questQualityIds != null) questQualityIds.Remove(questDepth); questDepth--; } return __exception; }

        static void QuestFactoryPrefix(object __0, out bool __state)
        {
            __state = false;
            try
            {
                if (!enabled() || __0 == null) return;
                var quality = __0.GetType().GetProperty("AssociatedQuality").GetValue(__0, null);
                string category = quality.GetType().GetProperty("Category").GetValue(quality, null).ToString();
                if (category == "Quest" || category == "Objective") {
                    int id = (int)quality.GetType().GetProperty("Id").GetValue(quality, null);
                    if (questQualityIds == null) questQualityIds = new Dictionary<int,int>();
                    questQualityIds[++questDepth] = id;
                    __state = true;
                }
            }
            catch (Exception e) { Error(e); }
        }

        static void QuestFactoryPostfix(object __result, bool __state)
        {
            if (!__state || __result == null) return;
            QuestPostfix(__result);
            if (questTraces++ < 4) log("Journal factory localization reached: quest/objective presentation.");
        }

        public static string NormalizeCompletionMarker(string original, string translation)
        {
            foreach (string marker in new[] { "[COMPLETE]", "[complete]" }) {
                if (!original.StartsWith(marker, StringComparison.Ordinal)) continue;
                foreach (string prefix in new[] { "[完成]", "[COMPLETE]", "[complete]" })
                    if (translation.StartsWith(prefix, StringComparison.Ordinal))
                        return marker + translation.Substring(prefix.Length);
            }
            return translation;
        }

        // Keep the control marker for game formatting; strip it only for an already bare display string.
        public static string TranslateCompletedBranch(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            string direct = translate(text);
            foreach (string marker in new[] { "[COMPLETE]", "[complete]" }) {
                if (text.StartsWith(marker, StringComparison.Ordinal)) {
                    if (!String.IsNullOrEmpty(direct) && direct.StartsWith("[完成]", StringComparison.Ordinal))
                        return marker + direct.Substring(4);
                    return direct;
                }
            }
            if (!String.IsNullOrEmpty(direct) && direct != text) return direct;
            string candidate = null;
            foreach (string marker in new[] { "[COMPLETE]", "[complete]" }) {
                string source = marker + text, value = translate(source);
                if (String.IsNullOrEmpty(value) || value == source) continue;
                if (value.StartsWith("[完成]", StringComparison.Ordinal)) value = value.Substring(4);
                else if (value.StartsWith(marker, StringComparison.Ordinal)) value = value.Substring(marker.Length);
                else continue;
                if (String.IsNullOrWhiteSpace(value) || !SameTags(text, value)) continue;
                if (candidate != null && candidate != value) return text;
                candidate = value;
            }
            return candidate ?? text;
        }

        // Match the selected branch before the formatter replaces its direction tokens.
        public static string TranslateQuestBody(string text)
        {
            if (questDepth == 0 || !enabled() || String.IsNullOrEmpty(text)) return text;
            try {
                int id;
                if (questQualityIds != null && questQualityIds.TryGetValue(questDepth, out id))
                    return Safe(text, source => TranslateBranch(id, "LevelDescriptionText", source, variables));
                return Safe(text, variables);
            }
            catch (Exception e) { Error(e); return text; }
        }

        static void QuestPostfix(object __instance)
        {
            try
            {
                if (!enabled()) return;
                // These are presentation-only strings, not the quality or save.
                var type = __instance.GetType();
                var body = type.GetProperty("Description");
                var level = type.GetProperty("LevelDescriptionOnly");
                string a = body.GetValue(__instance, null) as string, b = level.GetValue(__instance, null) as string;
                int id = (int)type.GetProperty("QualityId").GetValue(__instance, null);
                string translatedBody = Safe(a, lookup), translatedLevel = Safe(b, source => TranslateBranch(id, "LevelDescriptionText", source, variables));
                body.SetValue(__instance, translatedBody, null);
                level.SetValue(__instance, translatedLevel, null);
            }
            catch (Exception e) { Error(e); }
        }

        public static string Localize(string text, Func<string,string> translate, Func<string,string> variableTranslate)
        {
            return Localize(text, translate, variableTranslate, 0);
        }
        static string Localize(string text, Func<string,string> translate, Func<string,string> variableTranslate, int depth)
        {
            if (String.IsNullOrEmpty(text) || text.Length > 16384 || depth > 2) return text;
            string componentFallback = text;
            foreach (var rule in Rules)
            {
                if (!text.StartsWith(rule.Prefix, StringComparison.Ordinal)) continue;
                var match = rule.Pattern.Match(text);
                if (!match.Success) continue;
                string translated = translate(rule.Template);
                bool valid = !String.IsNullOrWhiteSpace(translated) && translated != rule.Template &&
                    Slots.Matches(translated).Cast<Match>().Select(m => m.Value).OrderBy(s => s).SequenceEqual(Slots.Matches(rule.Template).Cast<Match>().Select(m => m.Value).OrderBy(s => s)) &&
                    SameTags(rule.Template, translated) && !Slots.Replace(translated, "").Contains("{") && !Slots.Replace(translated, "").Contains("}");
                string[] values = new string[rule.Kinds.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    string value = match.Groups["v" + (i + 1)].Value;
                    bool numeric = Regex.IsMatch(value, @"\A[+-]?[0-9]+(?:[.,][0-9]+)?\z");
                    values[i] = rule.Kinds[i] == 'T' && !numeric ? Safe(value, variableTranslate) : rule.Kinds[i] == 'B' ? Localize(value, translate, variableTranslate, depth + 1) : value;
                    // Only the trailing locomotive model is a dictionary name; the prefix belongs to the player.
                    if (rule.Kinds[i] == 'L' && value.EndsWith(")", StringComparison.Ordinal))
                    {
                        int split = value.LastIndexOf(" (", StringComparison.Ordinal);
                        if (split > 0)
                            values[i] = value.Substring(0, split + 2) + Safe(value.Substring(split + 2, value.Length - split - 3), translate) + ")";
                    }
                    if (rule.Kinds[i] == 'T' && values[i] == value)
                    {
                        var levelValue = Level.Match(value);
                        if (levelValue.Success) values[i] = levelValue.Groups["number"].Value + Safe(levelValue.Groups["body"].Value, variableTranslate);
                    }
                    if (rule.Kinds[i] == 'T' && values[i].IndexOfAny(new[] { '\r', '\n' }) >= 0) values[i] = value;
                }
                // Even without a sentence translation, known property names can be translated.
                string result = Slots.Replace(valid ? translated : rule.Template, m => values[Int32.Parse(m.Groups[1].Value) - 1]);
                if (SameTags(text, result) && result != text)
                {
                    if (valid) return result;
                    if (componentFallback == text) componentFallback = result;
                }
            }
            if (componentFallback != text) return componentFallback;
            string exact = Safe(text, variableTranslate);
            if (exact != text) return exact;
            string named = Regex.Replace(text, @"(?<open>\[q-name\])(?<name>[^\[\]<>\r\n]+)(?<close>\[/q-name\])", m => m.Groups["open"].Value + Safe(m.Groups["name"].Value, translate) + m.Groups["close"].Value);
            if (named != text) return named;
            var levelMatch = Level.Match(text);
            if (levelMatch.Success) return levelMatch.Groups["number"].Value + Safe(levelMatch.Groups["body"].Value, variableTranslate);
            var reward = Reward.Match(text);
            if (reward.Success) return Safe(reward.Groups["name"].Value, translate) + " " + reward.Groups["number"].Value;
            // Preserve separators while translating independent lines/paragraphs.
            if (depth < 2 && text.Contains("\n"))
                return String.Concat(Regex.Split(text, @"(\r\n|\n)").Select(s => s == "\n" || s == "\r\n" ? s : Localize(s, translate, variableTranslate, depth + 1)).ToArray());
            return text;
        }
        static bool SameTags(string a, string b)
        {
            return Tags.Matches(a).Cast<Match>().Select(m => m.Value).SequenceEqual(Tags.Matches(b).Cast<Match>().Select(m => m.Value));
        }
        static string Safe(string source, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(source)) return source;
            string result = translate(source);
            if (String.IsNullOrWhiteSpace(result) || !SameTags(source, result) ||
                !Regex.Matches(source, @"\{[^}]*\}").Cast<Match>().Select(m => m.Value).SequenceEqual(Regex.Matches(result, @"\{[^}]*\}").Cast<Match>().Select(m => m.Value))) return source;
            return result;
        }
    }
}
