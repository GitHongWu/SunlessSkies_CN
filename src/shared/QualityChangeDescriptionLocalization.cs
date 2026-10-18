using System;
using System.Reflection;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Translate the display result only after the game has selected its branch.
    // The supplied catalog checks tokens and prefers unambiguous branch entries
    // over legacy standalone sentences. No Quality fields or levels are changed.
    public static class QualityChangeDescriptionLocalization
    {
        static Func<string, string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static int errors;
        public static void Install(Assembly game, Func<string, string> translate, Func<bool> active, Action<string> logger)
        {
            var type = game.GetType("Failbetter.Core.Quality", true);
            var method = AccessTools.Method(type, "GetDescriptionForChange", new[] {
                game.GetType("Failbetter.Interfaces.ResultInterfaces.IQualityChangeMessage", true) });
            if (method == null || method.ReturnType != typeof(string)) throw new MissingMethodException("Unsupported quality change description API");
            lookup = translate; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.quality-change-description");
            try { harmony.Patch(method, postfix: new HarmonyMethod(typeof(QualityChangeDescriptionLocalization), "Postfix")); }
            catch { harmony.UnpatchSelf(); throw; }
            log("Quality change description localization ready.");
        }
        static void Postfix(ref string __result)
        {
            if (String.IsNullOrEmpty(__result)) return;
            try {
                if (!enabled()) return;
                string translated = lookup(__result);
                if (!String.IsNullOrEmpty(translated)) __result = translated;
            }
            catch (Exception e) { if (errors++ < 4) log("Quality change description fallback: " + e.Message); }
        }
    }
}
