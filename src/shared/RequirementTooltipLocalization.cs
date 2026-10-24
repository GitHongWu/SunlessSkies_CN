using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    public static class RequirementTooltipLocalization
    {
        public const string Absent = "Unlocked when you do not have [q-name]{value1}[/q-name]";
        public const string Present = "Unlocked when you have [q-name]{value1}[/q-name]";
        static readonly Regex Pattern = new Regex(@"\AUnlocked when you (?<condition>do not have|have) \[q-name\](?<name>[^\[\]<>\r\n]+)\[/q-name\]\z", RegexOptions.CultureInvariant);
        static readonly Regex Markup = new Regex(@"\[[^\]]*\]|\{[^}]*\}|[<>]", RegexOptions.CultureInvariant);
        public const string StateList = "Unlocked when [q-name]{value1}[/q-name] is:[list]{value2}[/list]";
        public const string ObjectiveList = "Unlocked with [q-name]{value1}[/q-name]:[list]{value2}[/list]";
        public const string ObjectivePresent = "Unlocked with [q-name]{value1}[/q-name]";
        public const string ObjectiveAbsent = "Unlocked without [q-name]{value1}[/q-name]";
        static readonly Regex ListPattern = new Regex(@"\A(?<prefix>Unlocked when|Unlocked with) \[q-name\](?<name>[^\[\]<>\r\n]+)\[/q-name\](?<suffix> is:|:)\[list\](?<items>(?:\[listitem(?:-current)?\][\s\S]*?\[/listitem\])*)\[/list\]\z");
        static readonly Regex ListItem = new Regex(@"(?<open>\[listitem(?:-current)?\])(?<body>[\s\S]*?)(?<close>\[/listitem\])");
        static readonly Regex ObjectivePattern = new Regex(@"\AUnlocked (?<condition>with|without) \[q-name\](?<name>[^\[\]<>\r\n]+)\[/q-name\]\z");
        static Func<string,string> variableLookup;
        static Func<string,string> lookup;
        static Func<bool> enabled;
        static Action<string> log;
        static int traces;
        public static void Install(Assembly game, Func<string,string> translate, Func<bool> active, Action<string> logger, Func<string,string> variables = null)
        {
            var quality = game.GetType("Failbetter.Core.Quality", true);
            var methods = new[] { "WordyStatQRequirementText", "QuestQRequirementText", "ObjectiveQRequirementText" }.SelectMany(name =>
                game.GetType("Failbetter.Presentation.Formatters.RequirementTextFormatters." + name, true).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => new[] { "DesribeMinAndMax", "DescribeMin", "DescribeMax" }.Contains(m.Name))).Distinct().ToArray();
            if (methods.Length != 9 || methods.Any(m => m.ReturnType != typeof(string) || m.GetParameters()[0].ParameterType != quality)) throw new MissingMethodException("Unsupported requirement formatter API");
            variableLookup = variables ?? translate;
            lookup = translate; enabled = active; log = logger;
            var harmony = new Harmony("githongwu.sunlessskies.requirement-tooltip");
            try { foreach (var method in methods) harmony.Patch(method, postfix: new HarmonyMethod(typeof(RequirementTooltipLocalization), "Postfix")); }
            catch { harmony.UnpatchSelf(); throw; }
            log("Requirement tooltip localization ready: presence/absence and state-list formatters.");
        }
        static void Postfix(object __0, ref string __result)
        {
            try { if (enabled()) __result = LocalizeStates(__result, lookup, source => RemainingCompositionLocalization.TranslateBranch(__0, "LevelDescriptionText", source, variableLookup)); }
            catch (Exception e) { if (traces++ < 4) log("Requirement tooltip fallback: " + e.Message); }
        }
        public static string LocalizeStates(string text, Func<string,string> translate, Func<string,string> variables)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var list = ListPattern.Match(text);
            if (list.Success) {
                bool objective = list.Groups["prefix"].Value == "Unlocked with";
                if (list.Groups["suffix"].Value != (objective ? ":" : " is:")) return text;
                string key = objective ? ObjectiveList : StateList;
                string template = translate(key);
                if (String.IsNullOrEmpty(template) || template == key || !Markup.Matches(template).Cast<Match>().Select(m => m.Value).SequenceEqual(Markup.Matches(key).Cast<Match>().Select(m => m.Value))) return text;
                string items = list.Groups["items"].Value;
                if (String.Concat(ListItem.Matches(items).Cast<Match>().Select(m => m.Value).ToArray()) != items) return text;
                items = ListItem.Replace(items, m => {
                    string body = m.Groups["body"].Value;
                    string result = variables(body);
                    if (result == body || String.IsNullOrEmpty(result)) {
                        string prefix = body.StartsWith("[COMPLETE]", StringComparison.Ordinal) ? "[COMPLETE]" : body.StartsWith("[complete]", StringComparison.Ordinal) ? "[complete]" : "";
                        if (prefix.Length > 0) {
                            string bare = body.Substring(prefix.Length);
                            string value = variables(bare);
                            if (!String.IsNullOrEmpty(value) && value != bare) result = prefix + value;
                        } else {
                            // Some game formatters strip completion markers before displaying a state.
                            // Only accept an unambiguous translation and strip that same marker.
                            string candidate = null; bool conflict = false;
                            foreach (string marker in new[] { "[COMPLETE]", "[complete]" }) {
                                string source = marker + body;
                                string value = variables(source);
                                if (String.IsNullOrEmpty(value) || value == source || !value.StartsWith(marker, StringComparison.Ordinal)) continue;
                                value = value.Substring(marker.Length);
                                if (candidate != null && candidate != value) conflict = true;
                                candidate = value;
                            }
                            if (!conflict && !String.IsNullOrEmpty(candidate)) result = candidate;
                        }
                    }
                    if (String.IsNullOrEmpty(result) || !Markup.Matches(result).Cast<Match>().Select(t => t.Value).SequenceEqual(Markup.Matches(body).Cast<Match>().Select(t => t.Value))) result = body;
                    return m.Groups["open"].Value + result + m.Groups["close"].Value;
                });
                string name = SafeName(list.Groups["name"].Value, translate);
                return Regex.Replace(template, @"\{value[12]\}", m => m.Value == "{value1}" ? name : items);
            }
            var objectiveMatch = ObjectivePattern.Match(text);
            if (objectiveMatch.Success) {
                string key = objectiveMatch.Groups["condition"].Value == "with" ? ObjectivePresent : ObjectiveAbsent;
                string template = translate(key);
                if (String.IsNullOrEmpty(template) || template == key || !Markup.Matches(template).Cast<Match>().Select(m => m.Value).SequenceEqual(Markup.Matches(key).Cast<Match>().Select(m => m.Value))) return text;
                return template.Replace("{value1}", SafeName(objectiveMatch.Groups["name"].Value, translate));
            }
            return Localize(text, translate);
        }
        static string SafeName(string name, Func<string,string> translate) {
            string value = translate(name);
            return String.IsNullOrEmpty(value) || value.IndexOfAny(new[] {'[',']','<','>','{','}', '\r','\n'}) >= 0 ? name : value;
        }
        public static string Localize(string text, Func<string,string> translate)
        {
            if (String.IsNullOrEmpty(text)) return text;
            var match = Pattern.Match(text);
            if (!match.Success) return text;
            string key = match.Groups["condition"].Value == "have" ? Present : Absent;
            string template = translate(key);
            if (String.IsNullOrEmpty(template) || template == key) return text;
            // Keep the game's name formatting and exactly one slot, in order.
            if (!Markup.Matches(template).Cast<Match>().Select(m => m.Value).SequenceEqual(new[] { "[q-name]", "{value1}", "[/q-name]" })) return text;
            string name = match.Groups["name"].Value;
            string translated = translate(name);
            if (String.IsNullOrEmpty(translated) || translated.IndexOfAny(new[] { '[', ']', '<', '>', '{', '}', '\r', '\n' }) >= 0) translated = name;
            return template.Replace("{value1}", translated);
        }
    }
}
