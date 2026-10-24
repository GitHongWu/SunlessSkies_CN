using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Translate the formatter's display result, never the quality or possession.
    public static class QualityTitleLocalization
    {
        static PropertyInfo associatedQuality, qualityName;
        static Func<string, string> lookup, variables;
        static Func<bool> enabled;
        static Action<string> log;
        static int errors;
        static readonly Regex Number = new Regex(@"\A(?:-?[0-9]+| \(-?[0-9]+\))\z");
        static readonly Regex Markup = new Regex(@"<[^>]*>|\[[^\]]*\]|\{[^}]*\}");

        public static void Install(Assembly game, Func<string, string> translate, Func<string, string> translateVariable, Func<bool> active, Action<string> logger)
        {
            var possession = game.GetType("Failbetter.Core.QAssoc.BaseClasses.BaseQPossession", true);
            associatedQuality = possession.GetProperty("AssociatedQuality");
            qualityName = game.GetType("Failbetter.Core.Quality", true).GetProperty("Name");
            var formatter = game.GetType("Failbetter.Presentation.Formatters.BaseStoryFormatter", true);
            var method = AccessTools.Method(formatter, "GetQualityLevelDescription", new[] { possession });
            if (method == null || method.ReturnType != typeof(string) || associatedQuality == null || qualityName == null || qualityName.PropertyType != typeof(string))
                throw new MissingMethodException("Unsupported quality title formatter");
            lookup = translate; variables = translateVariable; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.quality-title");
            try { harmony.Patch(method, postfix: new HarmonyMethod(typeof(QualityTitleLocalization), "Postfix")); }
            catch { harmony.UnpatchSelf(); throw; }
            log("Quality title localization ready: names, counts and level descriptions.");
        }

        static void Postfix(object __0, ref string __result)
        {
            try
            {
                if (!enabled() || __0 == null) return;
                var quality = associatedQuality.GetValue(__0, null);
                if (quality != null) __result = Localize(__result, qualityName.GetValue(quality, null) as string, lookup, source => RemainingCompositionLocalization.TranslateBranch(quality, "LevelDescriptionText", source, variables));
            }
            catch (Exception e) { if (errors++ < 4) log("Quality title fallback: " + e.Message); }
        }

        public static string Localize(string text, string name, Func<string, string> translate, Func<string, string> translateVariable)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(name) || !text.StartsWith(name, StringComparison.Ordinal)) return text;
            string suffix = text.Substring(name.Length);
            if (suffix.Length != 0 && !Number.IsMatch(suffix) && !suffix.StartsWith(": ", StringComparison.Ordinal)) return text;
            string translatedName = Safe(name, translate);
            if (suffix.StartsWith(": ", StringComparison.Ordinal))
            {
                string source = suffix.Substring(2);
                string description = Safe(source, translateVariable);
                suffix = ": " + description;
            }
            return translatedName + suffix;
        }

        static string Safe(string source, Func<string, string> translate)
        {
            string result = translate(source);
            if (String.IsNullOrWhiteSpace(result) || result.IndexOfAny(new[] { '\r', '\n' }) >= 0 ||
                !Markup.Matches(source).Cast<Match>().Select(m => m.Value).SequenceEqual(Markup.Matches(result).Cast<Match>().Select(m => m.Value))) return source;
            return result;
        }
    }
}
