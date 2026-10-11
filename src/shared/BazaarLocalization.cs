using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    public static class BazaarLocalization
    {
        [ThreadStatic] static int depth;
        static Func<string,string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static FieldInfo availableField;
        static PropertyInfo availableProperty, textProperty;
        const string Slot = "{value1}";
        static readonly string[] Suffixes = {
            " You may find bargains here, or fulfil prospects you have claimed.",
            " Here, you can acquire 'prospects': opportunities to sell a good at a distant port for an excellent price. Accept a prospect to claim it, source the goods, and deliver them."
        };
        public static void Install(Assembly game, Func<string,string> translate, Func<bool> active, Action<string> logger)
        {
            lookup = translate; enabled = active; log = logger;
            var controller = game.GetType("Skyless.Assets.Code.Skyless.UI.Controllers.BazaarController", true);
            var panel = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Generic.ShopsPanelPresenter", true);
            var presenter = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Bazaar.BazaarPresenter", true);
            var show = AccessTools.Method(controller, "Show", Type.EmptyTypes);
            var initPanel = AccessTools.Method(panel, "Init", new[] { typeof(string), typeof(string) });
            var init = presenter.GetMethods().Single(m => m.Name == "Initialize" &&
                m.GetParameters().Length == 6 && m.GetParameters().Take(5).Select(p => p.ParameterType)
                .SequenceEqual(new[] { typeof(string), typeof(string), typeof(int), typeof(int), typeof(bool) }));
            availableField = AccessTools.Field(presenter, "AvailableText");
            availableProperty = AccessTools.Property(presenter, "AvailableText");
            var textType = availableField != null ? availableField.FieldType : availableProperty.PropertyType;
            textProperty = AccessTools.Property(textType, "text");
            if (show == null || initPanel == null || textProperty == null || !textProperty.CanWrite)
                throw new MissingMemberException("Unsupported bazaar display API");
            var harmony = new Harmony("githongwu.sunlessskies.bazaar");
            try
            {
                harmony.Patch(show, prefix: new HarmonyMethod(typeof(BazaarLocalization), "Enter"), finalizer: new HarmonyMethod(typeof(BazaarLocalization), "Leave"));
                harmony.Patch(initPanel, prefix: new HarmonyMethod(typeof(BazaarLocalization), "PanelPrefix"));
                harmony.Patch(init, postfix: new HarmonyMethod(typeof(BazaarLocalization), "RefreshPostfix"));
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Bazaar localization ready: scoped panel title/body and refresh countdown.");
        }
        static void Enter(out bool __state) { __state = enabled(); if (__state) depth++; }
        static Exception Leave(Exception __exception, bool __state) { if (__state) depth--; return __exception; }
        static void PanelPrefix(ref string __0, ref string __1)
        {
            if (depth == 0 || !enabled()) return;
            try
            {
                string title = Title(__0, lookup), body = Body(__1, lookup);
                __0 = title; __1 = body;
            }
            catch (Exception e) { log("Bazaar panel fallback: " + e.Message); }
        }
        static void RefreshPostfix(object __instance, int __3, bool __4)
        {
            if (!enabled() || __3 <= 0) return;
            try
            {
                string translated = Countdown(__3, __4, lookup);
                if (translated == null) return;
                var label = availableField != null ? availableField.GetValue(__instance) : availableProperty.GetValue(__instance, null);
                if (label != null) textProperty.SetValue(label, translated, null);
            }
            catch (Exception e) { log("Bazaar countdown fallback: " + e.Message); }
        }
        // Export placeholders are metadata, not game formatter syntax. Validate
        // the single slot before substitution; never invent missing translations.
        static string Expand(string template, string value, Func<string,string> translate)
        {
            string translated = translate(template);
            if (String.IsNullOrEmpty(translated) || translated == template) return null;
            int pos = translated.IndexOf(Slot, StringComparison.Ordinal);
            if (pos < 0 || translated.IndexOf(Slot, pos + Slot.Length, StringComparison.Ordinal) >= 0) return null;
            string rest = translated.Remove(pos, Slot.Length);
            if (rest.Contains("{value")) return null;
            return translated.Substring(0, pos) + value + translated.Substring(pos + Slot.Length);
        }
        public static string Title(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            const string suffix = " Bazaar";
            if (text.EndsWith(suffix, StringComparison.Ordinal))
            {
                string place = text.Substring(0, text.Length - suffix.Length);
                string result = Expand(Slot + suffix, translate(place), translate);
                if (result != null) return result;
            }
            return translate(text);
        }
        public static string Body(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            // Extra prospect-limit notices are separate paragraphs: preserve them
            // verbatim unless the existing dictionary contains that paragraph.
            int split = text.IndexOf("\n\n", StringComparison.Ordinal);
            if (split >= 0) return Body(text.Substring(0, split), translate) + "\n\n" + Body(text.Substring(split + 2), translate);
            foreach (string suffix in Suffixes)
                if (text.EndsWith(suffix, StringComparison.Ordinal))
                {
                    string description = text.Substring(0, text.Length - suffix.Length);
                    string result = Expand(Slot + suffix, translate(description), translate);
                    if (result != null) return result;
                }
            return translate(text);
        }
        public static string Countdown(int days, bool majorDock, Func<string,string> translate)
        {
            if (days <= 0) return null;
            return Expand("New " + (majorDock ? "prospects" : "bargains") + " available in " + Slot + " days.", days.ToString(CultureInfo.InvariantCulture), translate);
        }
    }
}
