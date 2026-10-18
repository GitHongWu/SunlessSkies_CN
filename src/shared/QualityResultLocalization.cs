using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    public static class QualityResultLocalization
    {
        public const string Template = "You now have {value1} x {value2}";
        public const string GainTemplate = "You've gained {value1} x {value2} (new total {value3}).";
        public const string LossTemplate = "You've lost {value1} x {value2} (new total {value3}).";
        static readonly Regex LossPattern = new Regex(@"\AYou've (?<verb>lost|gained) (?<amount>[0-9]+) x (?<name>[^<>\r\n]+?) \(new total (?<total>[^<>\r\n]+)\)\.\z", RegexOptions.CultureInvariant);
        static readonly Regex Pattern = new Regex(@"\AYou now have (?<amount>[^<>\r\n]+?) x (?<name>[^<>\r\n]+)\z", RegexOptions.CultureInvariant);
        static readonly Regex Slots = new Regex(@"\{value[^{}]*\}", RegexOptions.CultureInvariant);
        static Func<string,string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static int traces;
        public static void Install(Assembly game, Func<string,string> translate, Func<bool> active, Action<string> logger)
        {
            var type = game.GetType("Failbetter.Presentation.Formatters.ResultTextFormatters.StandardQualityChangeText", true);
            var method = AccessTools.Method(type, "DescribeNewQuality", new[] {
                game.GetType("Failbetter.Interfaces.ResultInterfaces.IQualityChangeMessage", true), game.GetType("Failbetter.Core.Quality", true) });
            if (method == null || method.ReturnType != typeof(string)) throw new MissingMethodException("Unsupported quality result API");
            var increment = AccessTools.Method(type, "DescribeIncrement", new[] {
                game.GetType("Failbetter.Interfaces.ResultInterfaces.IQualityChangeMessage", true), game.GetType("Failbetter.Core.Quality", true) });
            if (increment == null || increment.ReturnType != typeof(string)) throw new MissingMethodException("Unsupported quality increment API");
            lookup = translate; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.quality-result");
            try {
                harmony.Patch(method, postfix: new HarmonyMethod(typeof(QualityResultLocalization), "Postfix"));
                harmony.Patch(increment, postfix: new HarmonyMethod(typeof(QualityResultLocalization), "LossPostfix"));
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Quality result localization ready: DescribeNewQuality and DescribeIncrement gain/loss branches.");
        }
        static void LossPostfix(ref string __result)
        {
            try { if (enabled()) __result = LocalizeLoss(__result, lookup); }
            catch (Exception e) { if (traces++ < 4) log("Quality increment fallback: " + e.Message); }
        }
        public static string LocalizeLoss(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = LossPattern.Match(text);
            if (!match.Success) return text;
            string key = match.Groups["verb"].Value == "gained" ? GainTemplate : LossTemplate;
            string template = translate(key);
            if (String.IsNullOrEmpty(template) || template == key) return text;
            var slots = Slots.Matches(template).Cast<Match>().Select(m => m.Value).OrderBy(s => s).ToArray();
            if (!slots.SequenceEqual(new[] { "{value1}", "{value2}", "{value3}" }) || template.IndexOfAny(new[] {'<','>','[',']'}) >= 0) return text;
            string name = match.Groups["name"].Value;
            string translatedName = translate(name);
            if (String.IsNullOrEmpty(translatedName) || translatedName.IndexOfAny(new[] {'<','>','\r','\n','{','}','[',']'}) >= 0) translatedName = name;
            // Keep the game's rendered amount and level description verbatim.
            // Never calculate inventory totals or infer a singular item name.
            return Slots.Replace(template, m => m.Value == "{value1}" ? match.Groups["amount"].Value : m.Value == "{value2}" ? translatedName : match.Groups["total"].Value);
        }
        static void Postfix(ref string __result)
        {
            try { if (enabled()) __result = Localize(__result, lookup); }
            catch (Exception e) { if (traces++ < 4) log("Quality result fallback: " + e.Message); }
        }
        public static string Localize(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = Pattern.Match(text);
            if (!match.Success) return text;
            string template = translate(Template);
            if (String.IsNullOrEmpty(template) || template == Template) return text;
            var slots = Slots.Matches(template).Cast<Match>().Select(m => m.Value).OrderBy(s => s).ToArray();
            if (!slots.SequenceEqual(new[] { "{value1}", "{value2}" }) || template.IndexOfAny(new[] {'<','>'}) >= 0) return text;
            string name = match.Groups["name"].Value;
            string translatedName = translate(name);
            if (String.IsNullOrEmpty(translatedName) || translatedName.IndexOfAny(new[] {'<','>','\r','\n','{','}'}) >= 0) translatedName = name;
            // Level descriptions can be textual; retain exactly what the game
            // produced instead of deriving a quantity from inventory or changes.
            return Slots.Replace(template, m => m.Value == "{value1}" ? match.Groups["amount"].Value : translatedName);
        }
    }
}
