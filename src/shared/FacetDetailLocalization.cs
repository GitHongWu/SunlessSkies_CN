using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Localize each complete source before the facet UI strips explanatory
    // italics and concatenates branch/result text. Never mutate the repository.
    public static class FacetDetailLocalization
    {
        static Func<string, string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static int traces;
        [ThreadStatic] static int choiceDepth;
        static readonly Regex TooltipTitle = new Regex(@"\A(?<name>[^<>\r\n]+)(?<amount> [+-]?[0-9]+)\z", RegexOptions.CultureInvariant);
        static readonly Regex Reward = new Regex(@"\A(?<amount>[+-]?[0-9]+ )(?<name>[^<>\r\n]+)\z", RegexOptions.CultureInvariant);
        // Exact output of the game's per-property preview formatter. Its color
        // tags intentionally remain open; the caller supplies list separators.
        static readonly Regex Preview = new Regex(@"\A(?<prefix><color=#FCF4DF>[+-]?[0-9]+ <color=#ADFBF7>)(?<name>[^<>\r\n]+)\z", RegexOptions.CultureInvariant);

        public static void Install(Assembly game, Func<string, string> translate, Func<bool> active, Action<string> logger)
        {
            var type = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.CharacterProgression.CurrentFacets.SelectedFacetDetailPresenter", true);
            var method = type.GetMethod("StripOriginExplanatoryText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null, new[] { typeof(string) }, null);
            if (method == null || method.ReturnType != typeof(string)) throw new MissingMethodException(type.FullName, "StripOriginExplanatoryText");
            var rewardType = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.CharacterProgression.CurrentFacets.FacetRewardPresenter", true);
            var rewardMethod = rewardType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Initialise" && m.ReturnType == typeof(void) && m.GetParameters().Length == 2 &&
                    m.GetParameters()[0].ParameterType.FullName == "UnityEngine.Sprite" && m.GetParameters()[1].ParameterType == typeof(string));
            var previewType = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.CharacterProgression.FacetSelection.QFacetEffectPresentationExtensions", true);
            var previewMethod = previewType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Single(m => m.Name == "ToFormattedString" && m.ReturnType == typeof(string) && m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType.FullName == "Skyless.Assets.Code.Failbetter.Presentation.PresentationData.QFacetEffectsPresentation");
            lookup = translate; enabled = active; log = logger;
            var choice = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.CharacterProgression.FacetTwistChoice.FacetTwistChoicePresenter", true)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Initialise" && m.GetParameters().Length == 4 && m.GetParameters()[0].ParameterType.FullName == "Failbetter.Core.Branch");
            var bind = game.GetType("Skyless.Assets.Code.Skyless.UI.Tooltips.Tooltip", true)
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(m => m.Name == "Bind" && m.GetParameters().Length == 3 && m.GetParameters()[0].ParameterType.FullName == "UnityEngine.GameObject" &&
                    m.GetParameters()[1].ParameterType == typeof(string) && m.GetParameters()[2].ParameterType == typeof(string));
            var harmony = new Harmony("githongwu.sunlessskies.facet-detail");
            try
            {
                harmony.Patch(method, prefix: new HarmonyMethod(typeof(FacetDetailLocalization), "Prefix"));
                harmony.Patch(rewardMethod, prefix: new HarmonyMethod(typeof(FacetDetailLocalization), "RewardPrefix"));
                harmony.Patch(previewMethod, postfix: new HarmonyMethod(typeof(FacetDetailLocalization), "PreviewPostfix"));
                harmony.Patch(choice, prefix: new HarmonyMethod(typeof(FacetDetailLocalization), "ChoicePrefix"), finalizer: new HarmonyMethod(typeof(FacetDetailLocalization), "ChoiceFinalizer"));
                harmony.Patch(bind, prefix: new HarmonyMethod(typeof(FacetDetailLocalization), "TooltipPrefix"));
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Facet localization ready: source bodies, reward names and selection previews.");
        }

        static void ChoicePrefix(out bool __state)
        {
            __state = enabled();
            if (__state) choiceDepth++;
        }

        static Exception ChoiceFinalizer(Exception __exception, bool __state)
        {
            if (__state) choiceDepth--;
            return __exception;
        }

        // Called by the existing story-formatter prefixes, before token expansion.
        // Only the choice UI's synchronous construction has this scope.
        public static string TranslateChoiceBody(string text)
        {
            if (choiceDepth == 0 || !enabled() || String.IsNullOrEmpty(text)) return text;
            try { return lookup(text) ?? text; }
            catch { return text; }
        }

        static void TooltipPrefix(ref string __1)
        {
            try
            {
                if (choiceDepth == 0 || !enabled() || String.IsNullOrEmpty(__1)) return;
                var match = TooltipTitle.Match(__1);
                if (!match.Success) return;
                string name = match.Groups["name"].Value;
                string translated = lookup(name);
                if (String.IsNullOrEmpty(translated) || translated == name || translated.IndexOfAny(new[] { '<', '>', '\r', '\n' }) >= 0) return;
                __1 = translated + match.Groups["amount"].Value;
            }
            catch (Exception e) { if (traces++ < 4) log("Facet tooltip fallback: " + e.Message); }
        }

        static void PreviewPostfix(ref string __result)
        {
            try { if (enabled()) __result = LocalizePreview(__result, lookup); }
            catch (Exception e) { if (traces++ < 4) log("Facet preview fallback: " + e.Message); }
        }

        public static string LocalizePreview(string text, Func<string, string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = Preview.Match(text);
            if (!match.Success) return text;
            string name = match.Groups["name"].Value;
            string translated = translate(name);
            if (String.IsNullOrEmpty(translated) || translated == name ||
                translated.IndexOfAny(new[] { '<', '>', '\r', '\n' }) >= 0) return text;
            return match.Groups["prefix"].Value + translated;
        }

        static void RewardPrefix(ref string __1)
        {
            try { if (enabled()) __1 = LocalizeReward(__1, lookup); }
            catch (Exception e) { if (traces++ < 4) log("Facet reward fallback: " + e.Message); }
        }

        public static string LocalizeReward(string text, Func<string, string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = Reward.Match(text);
            if (!match.Success) return text;
            string name = match.Groups["name"].Value;
            string translated = translate(name);
            if (String.IsNullOrEmpty(translated) || translated == name ||
                translated.IndexOfAny(new[] { '<', '>', '\r', '\n' }) >= 0) return text;
            return match.Groups["amount"].Value + translated;
        }

        static void Prefix(ref string __0)
        {
            try
            {
                if (!enabled() || String.IsNullOrEmpty(__0)) return;
                string translated = lookup(__0);
                if (String.IsNullOrEmpty(translated) || translated == __0) return;
                __0 = translated;
                if (traces++ < 4) log("Facet detail source translated.");
            }
            catch (Exception e) { if (traces++ < 4) log("Facet detail fallback: " + e.Message); }
        }
    }
}
