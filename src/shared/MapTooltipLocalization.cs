using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Scope the common presenter to map hover calls. Never change landmark data.
    public static class MapTooltipLocalization
    {
        [ThreadStatic] static int depth;
        static Func<string,string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static int traces;
        static readonly Regex Parts = new Regex(@"(<[^>]+>|\r\n|\r|\n)", RegexOptions.CultureInvariant);
        public static void Install(Assembly game, Func<string,string> translate, Func<bool> active, Action<string> logger)
        {
            var chart = game.GetType("ChartLandmarkObject", true);
            var presenter = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Generic.TooltipPresenter", true);
            var showMap = AccessTools.Method(chart, "ShowTooltip", Type.EmptyTypes);
            var showText = AccessTools.Method(presenter, "Show", new[] { typeof(string), typeof(string) });
            if (showMap == null || showText == null) throw new MissingMethodException("Unsupported map tooltip API");
            lookup = translate; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.map-tooltip");
            try
            {
                harmony.Patch(showMap, prefix: new HarmonyMethod(typeof(MapTooltipLocalization), "Enter"), finalizer: new HarmonyMethod(typeof(MapTooltipLocalization), "Leave"));
                harmony.Patch(showText, prefix: new HarmonyMethod(typeof(MapTooltipLocalization), "Translate"));
            }
            catch { harmony.UnpatchSelf(); throw; }
            log("Map tooltip localization ready: ChartLandmarkObject.ShowTooltip -> TooltipPresenter.Show.");
        }
        static void Enter(out bool __state) { __state = enabled(); if (__state) depth++; }
        static Exception Leave(Exception __exception, bool __state) { if (__state) depth--; return __exception; }
        static void Translate(ref string __0, ref string __1)
        {
            if (depth == 0 || !enabled()) return;
            try
            {
                var title = Localize(__0, lookup);
                var body = Localize(__1, lookup);
                bool changed = title != __0 || body != __1;
                __0 = title; __1 = body;
                if (traces++ < 8) log("Map tooltip visited: matched=" + changed);
            }
            catch (Exception e) { if (traces++ < 8) log("Map tooltip fallback: " + e.Message); }
        }
        public static string Localize(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            // Composite legacy mappings must not shadow newer catalog entries for
            // individual descriptions. The catalog resolves source priority and
            // legacy fallback separately for each text node below.
            string[] parts = Parts.Split(text);
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (String.IsNullOrWhiteSpace(part) || part[0] == '<') continue;
                string translated = translate(part);
                if (translated == part)
                {
                    string trimmed = part.Trim();
                    string inner = translate(trimmed);
                    if (inner != trimmed)
                    {
                        int start = part.IndexOf(trimmed, StringComparison.Ordinal);
                        translated = part.Substring(0, start) + inner + part.Substring(start + trimmed.Length);
                    }
                }
                parts[i] = translated;
            }
            // Apply spacing after lookup so dictionary keys remain unchanged.
            for (int i = 0; i < parts.Length; i++)
            {
                if (!String.Equals(parts[i], "</i>", StringComparison.OrdinalIgnoreCase)) continue;
                char before = VisibleEdge(parts, i, -1);
                char after = VisibleEdge(parts, i, 1);
                if (before != '\0' && after != '\0' &&
                    !Char.IsWhiteSpace(before) && !Char.IsWhiteSpace(after))
                    parts[i] += "\u2009\u2009"; // Two thin spaces, outside the italic run.
            }
            return String.Concat(parts);
        }
        static char VisibleEdge(string[] parts, int start, int step)
        {
            for (int i = start + step; i >= 0 && i < parts.Length; i += step)
            {
                string part = parts[i];
                if (part.Length == 0) continue;
                if (part[0] == '<')
                {
                    if (part.StartsWith("<br", StringComparison.OrdinalIgnoreCase) ||
                        part.StartsWith("<space", StringComparison.OrdinalIgnoreCase)) return ' ';
                    continue;
                }
                return step < 0 ? part[part.Length - 1] : part[0];
            }
            return '\0';
        }
    }
}
