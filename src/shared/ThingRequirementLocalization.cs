using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    public static class ThingRequirementLocalization
    {
        public static readonly string[] Templates = new string[] { "You unlocked this by having no [q-name]{value1}[/q-name]",
            "You unlocked this by having no [q-name]{value1}[/q-name] [em](you needed {value2} at most)[/em]",
            "You unlocked this with no [q-name]{value1}[/q-name] [em](you needed {value2})[/em]",
            "You unlocked this with no [q-name]{value1}[/q-name] [em](you needed {value2} at most)[/em]",
            "You unlocked this with no [q-name]{value1}[/q-name] [em](you needed {value2}-{value3})[/em]",
            "You unlocked this with no [q-name]{value1}[/q-name] [em](you needed exactly {value2})[/em]",
            "You unlocked this with a [q-name]{value1}[/q-name] [em](you needed {value2})[/em]",
            "You unlocked this with a [q-name]{value1}[/q-name] [em](you needed {value2} at most)[/em]",
            "You unlocked this with a [q-name]{value1}[/q-name] [em](you needed {value2}-{value3})[/em]",
            "You unlocked this with a [q-name]{value1}[/q-name] [em](you needed exactly {value2})[/em]",
            "You unlocked this with an [q-name]{value1}[/q-name] [em](you needed {value2})[/em]",
            "You unlocked this with an [q-name]{value1}[/q-name] [em](you needed {value2} at most)[/em]",
            "You unlocked this with an [q-name]{value1}[/q-name] [em](you needed {value2}-{value3})[/em]",
            "You unlocked this with an [q-name]{value1}[/q-name] [em](you needed exactly {value2})[/em]",
            "You unlocked this with {value1} [q-name]{value2}[/q-name] [em](you needed {value3})[/em]",
            "You unlocked this with {value1} [q-name]{value2}[/q-name] [em](you needed {value3} at most)[/em]",
            "You unlocked this with {value1} [q-name]{value2}[/q-name] [em](you needed {value3}-{value4})[/em]",
            "You unlocked this with {value1} [q-name]{value2}[/q-name] [em](you needed exactly {value3})[/em]",
            "You unlocked this with {value1} {value2} [em](you have {value3} in all)[/em]",
            "You can't do this when you have any [q-name]{value1}[/q-name]",
            "You can't do this when you have more than a [q-name]{value1}[/q-name]",
            "You can't do this when you have more than an [q-name]{value1}[/q-name]",
            "You can't do this when you have more than {value1} [q-name]{value2}[/q-name]",
            "You need a [q-name]{value1}[/q-name]",
            "You need a [q-name]{value1}[/q-name] [em](you have {value2})[/em]",
            "You need an [q-name]{value1}[/q-name]",
            "You need an [q-name]{value1}[/q-name] [em](you have {value2})[/em]",
            "You need {value1} [q-name]{value2}[/q-name]",
            "You need {value1} [q-name]{value2}[/q-name] [em](you have {value3})[/em]",
            "You need {value1}-{value2} [q-name]{value3}[/q-name]",
            "You need {value1}-{value2} [q-name]{value3}[/q-name] [em](you have {value4})[/em]",
            "You need exactly a [q-name]{value1}[/q-name]",
            "You need exactly a [q-name]{value1}[/q-name] [em](you have {value2})[/em]",
            "You need exactly an [q-name]{value1}[/q-name]",
            "You need exactly an [q-name]{value1}[/q-name] [em](you have {value2})[/em]",
            "You need exactly {value1} [q-name]{value2}[/q-name]",
            "You need exactly {value1} [q-name]{value2}[/q-name] [em](you have {value3})[/em]",
            "This will cost {value1} [q-name]{value2}[/q-name]" };
        static readonly Regex Slots = new Regex(@"\{value[0-9]+\}");
        static readonly Regex Markup = new Regex(@"\[[^\]]*\]|[<>]|\{[^}]*\}");
        sealed class Rule {
            public string Template; public Regex Pattern; public int NameSlot;
            public Rule(string template) {
                Template = template;
                var slots = Slots.Matches(template).Cast<Match>().ToArray();
                // The name is inside q-name except the game's one-item minimum branch.
                int namePos = template.IndexOf("[q-name]", StringComparison.Ordinal);
                NameSlot = namePos >= 0 ? Array.FindIndex(slots, m => m.Index > namePos) + 1 : 2;
                string pattern = ""; int last = 0;
                for (int i = 0; i < slots.Length; i++) {
                    pattern += Regex.Escape(template.Substring(last, slots[i].Index - last));
                    pattern += "(?<v" + (i+1) + ">" + (i+1 == NameSlot ? @"[^<>\[\]{}\r\n]+?" : "[0-9]+") + ")";
                    last = slots[i].Index + slots[i].Length;
                }
                Pattern = new Regex(@"\A" + pattern + Regex.Escape(template.Substring(last)) + @"\z", RegexOptions.CultureInvariant);
            }
        }
        static readonly Rule[] Rules = Templates.Select(t => new Rule(t)).ToArray();
        static Func<string,string> lookup; static Func<bool> enabled; static Action<string> log; static int errors;
        public static void Install(Assembly game, Func<string,string> translate, Func<bool> active, Action<string> logger) {
            var quality = game.GetType("Failbetter.Core.Quality", true);
            var methods = new System.Collections.Generic.List<MethodInfo>();
            foreach (var typeName in new[] {"LockedThingQRequirementText", "UnlockedThingQRequirementText"}) {
                var type = game.GetType("Failbetter.Presentation.Formatters.RequirementTextFormatters."+typeName,true);
                foreach(var name in new[] {"DesribeMinAndMax", "DescribeMin", "DescribeMax"}) {
                    var args = name == "DescribeMax" ? new[] {quality,typeof(int),typeof(int)} : name == "DescribeMin" ? new[] {quality,typeof(int),typeof(int),typeof(bool)} : new[] {quality,typeof(int),typeof(int),typeof(int)};
                    var method = AccessTools.Method(type,name,args);
                    if(method == null || method.ReturnType != typeof(string)) throw new MissingMethodException(typeName+"."+name);
                    methods.Add(method);
                }
            }
            lookup=translate; enabled=active; log=logger;
            var harmony=new Harmony("githongwu.sunlessskies.thing-requirement");
            try {foreach(var m in methods) harmony.Patch(m,postfix:new HarmonyMethod(typeof(ThingRequirementLocalization),"Postfix"));}
            catch {harmony.UnpatchSelf();throw;}
            log("Thing requirement localization ready: six display formatters.");
        }
        static void Postfix(ref string __result) {
            try {if(enabled()) __result=Localize(__result,lookup);}
            catch(Exception e){if(errors++<4)log("Thing requirement fallback: "+e.Message);}
        }
        public static string Localize(string text,Func<string,string> translate) {
            if(String.IsNullOrEmpty(text)) return text;
            foreach(var rule in Rules) {
                var match=rule.Pattern.Match(text);if(!match.Success)continue;
                var translated=translate(rule.Template);
                if(String.IsNullOrEmpty(translated)||translated==rule.Template)return text;
                // Preserve the ordered formatting tags; permit reordering only value slots.
                var tokens=Markup.Matches(translated).Cast<Match>().Select(m=>m.Value).ToArray();
                var expected=Markup.Matches(rule.Template).Cast<Match>().Select(m=>m.Value).ToArray();
                if(!tokens.Where(s=>!s.StartsWith("{")).SequenceEqual(expected.Where(s=>!s.StartsWith("{"))) ||
                   !tokens.Where(s=>s.StartsWith("{")).OrderBy(s=>s).SequenceEqual(expected.Where(s=>s.StartsWith("{")).OrderBy(s=>s)))return text;
                if(rule.Template.Contains("[q-name]") && !translated.Contains("[q-name]{value"+rule.NameSlot+"}[/q-name]"))return text;
                return Slots.Replace(translated,m=>{
                    int i=Int32.Parse(m.Value.Substring(6,m.Value.Length-7));
                    string value=match.Groups["v"+i].Value;
                    if(i!=rule.NameSlot)return value;
                    string name=translate(value);
                    return String.IsNullOrEmpty(name)||name.IndexOfAny(new[]{'<','>','[',']','{','}','\r','\n'})>=0?value:name;
                });
            }
            return text;
        }
    }
}
