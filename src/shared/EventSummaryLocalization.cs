using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Display cards and presentation GenerateTeaser calls only; Event.Teaser is serialized.
    public static class EventSummaryLocalization
    {
        static PropertyInfo rootEvent, description;
        static Func<string, string> translate;
        static Func<bool> enabled;
        static Action<string> log;
        static int traces;
        static readonly Regex Tag = new Regex(@"^<(?<close>/)?(?<name>[A-Za-z][A-Za-z0-9-]*)(?:\s[^<>]*|=[^<>]*)?\s*/?>$", RegexOptions.CultureInvariant);
        static readonly HashSet<string> Paired = new HashSet<string>(new[] { "style", "b", "i", "u", "s", "color", "size", "font", "link", "mark", "sup", "sub", "smallcaps", "uppercase", "lowercase", "voffset", "align", "indent", "line-indent", "cspace", "mspace", "nobr" }, StringComparer.OrdinalIgnoreCase);
        public static void Install(Assembly game, Func<string,string> lookup, Func<bool> active, Action<string> logger)
        {
            var card = game.GetType("Failbetter.Core.StoryletCard", true);
            rootEvent = card.GetProperty("RootEvent");
            description = game.GetType("Failbetter.Core.Event", true).GetProperty("Description");
            var getter = card.GetProperty("Teaser").GetGetMethod();
            var generate = game.GetType("Failbetter.Core.Event", true).GetMethod("GenerateTeaser", new[] { typeof(string) });
            if (rootEvent == null || description == null || getter.ReturnType != typeof(string) || generate == null || generate.ReturnType != typeof(string)) throw new MissingMemberException("Unsupported event summary API");
            translate = lookup; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.event-summary");
            try {
                harmony.Patch(getter, prefix: new HarmonyMethod(typeof(EventSummaryLocalization), "Prefix"));
                harmony.Patch(generate, prefix: new HarmonyMethod(typeof(EventSummaryLocalization), "GeneratePrefix"));
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Event summary localization ready: StoryletCard.Teaser + Event.GenerateTeaser(string).");
        }
        static bool GeneratePrefix(string __0, ref string __result)
        {
            try { return Localize(__0, ref __result); }
            catch (Exception e) { if (traces++ < 6) log("Generated summary fallback: " + e.Message); return true; }
        }
        static bool Localize(string original, ref string result)
        {
            if (!enabled() || String.IsNullOrEmpty(original)) return true;
            var translated = translate(original);
            string summary;
            if (String.IsNullOrEmpty(translated) || !HasChinese(translated) ||
                !TrySummarize(translated, Int32.MaxValue, out summary)) return true;
            result = summary;
            if (traces++ < 6) log("Event summary localized: body=" + original.Length + "; summary=" + summary.Length);
            return false;
        }
        static bool Prefix(object __instance, ref string __result)
        {
            try
            {
                if (!enabled()) return true;
                var ev = rootEvent.GetValue(__instance, null);
                if (ev == null) return true;
                var original = description.GetValue(ev, null) as string;
                if (String.IsNullOrEmpty(original)) return true;
                return Localize(original, ref __result);
            }
            catch (Exception e)
            {
                if (traces++ < 6) log("Event summary fallback: " + e.Message);
                return true;
            }
        }
        static bool HasChinese(string value)
        {
            foreach (char c in value) if (c >= '\u3400' && c <= '\u9fff') return true;
            return false;
        }
        // Tags and bracket expressions are atomic. Only visible prose marks a sentence.
        // Unknown/malformed markup falls back instead of emitting a broken tag.
        public static bool TrySummarize(string text, int maxVisible, out string result)
        {
            result = null;
            if (String.IsNullOrEmpty(text) || maxVisible <= 0) return false;
            var output = new StringBuilder();
            var stack = new Stack<string>();
            int visible = 0;
            bool sentence = false, clipped = false;
            for (int i = 0; i < text.Length;)
            {
                char c = text[i];
                if (sentence && c != '”' && c != '’' && c != '"' && c != '\'' && c != '）' && c != ')') break;
                if (c == '<')
                {
                    int end = text.IndexOf('>', i + 1);
                    if (end < 0) return false;
                    string raw = text.Substring(i, end - i + 1);
                    if (Regex.IsMatch(raw, @"^<#[0-9a-fA-F]{6}([0-9a-fA-F]{2})?>$"))
                    {
                        stack.Push("color"); output.Append(raw); i = end + 1; continue;
                    }
                    var m = Tag.Match(raw);
                    if (!m.Success) return false;
                    string name = m.Groups["name"].Value.ToLowerInvariant();
                    if (m.Groups["close"].Success)
                    {
                        if (stack.Count == 0 || stack.Peek() != name) return false;
                        stack.Pop();
                    }
                    else if (Paired.Contains(name) && !raw.EndsWith("/>", StringComparison.Ordinal)) stack.Push(name);
                    output.Append(raw); i = end + 1; continue;
                }
                if (visible >= maxVisible) { clipped = true; break; }
                if (c == '[')
                {
                    int end = text.IndexOf(']', i + 1);
                    if (end < 0) return false;
                    output.Append(text, i, end - i + 1); i = end + 1; visible++; continue;
                }
                if (c == '\r' || c == '\n')
                {
                    if (visible > 0) break;
                    i++; continue;
                }
                int size = Char.IsHighSurrogate(c) && i + 1 < text.Length && Char.IsLowSurrogate(text[i + 1]) ? 2 : 1;
                output.Append(text, i, size); i += size; visible++;
                sentence = sentence || "。！？!?".IndexOf(c) >= 0 || (c == '.' && (i == text.Length || Char.IsWhiteSpace(text[i]) || text[i] == '<' || text[i] == '"'));
            }
            if (clipped) output.Append('…');
            while (stack.Count > 0) output.Append("</").Append(stack.Pop()).Append('>');
            result = output.ToString();
            return visible > 0;
        }
    }
}
