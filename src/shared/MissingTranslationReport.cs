using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SunlessSkiesCN.Shared
{
    [DataContract]
    public sealed class MissingTextEntry
    {
        [DataMember] public string Text, DisplaySample, FirstSeenUtc, LastSeenUtc, Kind, Classification;
        [DataMember] public string IssueKey, LookupText, ReviewReason;
        [DataMember] public long Count;
        [DataMember] public List<string> Paths = new List<string>();
        [DataMember] public List<string> Sources = new List<string>();
    }

    [DataContract]
    public sealed class MissingTextDocument
    {
        [DataMember] public int Schema = 1;
        [DataMember] public string Platform, GameVersion, PluginVersion, PluginSha256, IndexSha256, DictionarySha256, GeneratedUtc;
        [DataMember] public string CountMeaning = "One stable visible episode per component; polling does not increment counts.";
        [DataMember] public long DroppedAtLimit;
        [DataMember] public List<MissingTextEntry> Entries = new List<MissingTextEntry>();
    }

    public sealed class MissingTranslationReport
    {
        const int MaxEntries = 5000;
        static readonly Regex Tags = new Regex(@"<\/?(?:b|i|u|s|color|size|font|material|style|link|align|alpha|voffset|cspace|mspace|indent|line-indent|line-height|margin|mark|nobr|noparse|pos|rotate|smallcaps|uppercase|lowercase|sub|sup|width|sprite|space|br)(?:\s[^<>]*|=[^<>]*)?\s*\/?>|<#[0-9a-fA-F]{6,8}>|\[(?:/?(?:b|i|u|em|color|size)(?:=[^\]]*)?|/?[0-9a-fA-F]{6}|-)\]", RegexOptions.IgnoreCase);
        static readonly Regex Space = new Regex(@"\s+");
        static readonly Regex Words = new Regex(@"[A-Za-z]+(?:['’-][A-Za-z]+)*");
        static readonly Regex Chinese = new Regex(@"[\u3400-\u9fff\uf900-\ufaff]");
        static readonly Regex NumberUnit = new Regex(@"\A[\s\d.,:+%°/×x()\[\]−-]*(?:(?:km/h|m/s|fps|rpm|kg|km|ms|px|hz|mb|gb|hp|m|s|h|t|w|v|k)\b[\s\d.,:+%°/×x()\[\]−-]*)+\z", RegexOptions.IgnoreCase);
        static readonly HashSet<string> Keys = new HashSet<string>(new[] { "esc", "escape", "space", "enter", "return", "tab", "shift", "ctrl", "control", "alt", "backspace", "delete", "insert", "home", "end", "pageup", "pagedown", "up", "down", "left", "right", "lmb", "rmb", "mmb", "lb", "rb", "lt", "rt", "lstick", "rstick" }, StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, MissingTextEntry> entries = new Dictionary<string, MissingTextEntry>(StringComparer.Ordinal);
        readonly Dictionary<int, Pending> pending = new Dictionary<int, Pending>();
        readonly Dictionary<string, Evidence> index = new Dictionary<string, Evidence>(StringComparer.Ordinal);
        public readonly MissingTextDocument Document;
        public bool Dirty { get; private set; }
        sealed class Pending { public string Raw; public double Since; public DateTime FirstSeen; public bool Recorded, Ready; public int Scan; }
        sealed class Evidence { public bool Translated; public readonly List<string> Sources = new List<string>(); }

        public MissingTranslationReport(MissingTextDocument document)
        {
            Document = document;
            if (document.Schema != 1 || document.Entries == null || document.Entries.Count > MaxEntries) throw new InvalidDataException("Unsupported report");
            foreach (var row in document.Entries)
            {
                if (row == null || row.Text == null || row.Paths == null || row.Sources == null) throw new InvalidDataException("Invalid report entry");
                entries.Add(row.IssueKey ?? row.Text, row);
            }
        }

        public static string Clean(string text)
        {
            if (String.IsNullOrEmpty(text)) return "";
            return Space.Replace(Tags.Replace(Regex.Replace(text, @"<br\s*/?>", " ", RegexOptions.IgnoreCase), "").Replace("\u200b", "").Replace("\u00ad", ""), " ").Trim();
        }

        public static string SuspectKind(string text)
        {
            text = Clean(text);
            if (text.Length == 0 || NumberUnit.IsMatch(text)) return null;
            var words = Words.Matches(text).Cast<Match>().Select(m => m.Value).ToArray();
            if (words.Length == 0) return null;
            // shortcut: single-letter labels and common keys are ambiguous; revisit with component-specific exceptions if needed.
            if (words.All(w => w.Length == 1 || Keys.Contains(w)) || Regex.IsMatch(text, @"\AF(?:[1-9]|1[0-9]|2[0-4])\z", RegexOptions.IgnoreCase)) return null;
            return Chinese.IsMatch(text) ? "mixed" : "english";
        }

        public void AddEvidence(string original, bool translated, string source)
        {
            string key = Clean(original);
            if (key.Length == 0) return;
            Evidence evidence;
            if (!index.TryGetValue(key, out evidence)) index[key] = evidence = new Evidence();
            evidence.Translated |= translated;
            if (evidence.Sources.Count < 8 && !evidence.Sources.Contains(source)) evidence.Sources.Add(source);
        }

        public void LoadIndex(string path)
        {
            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("#", StringComparison.Ordinal)) continue;
                var parts = line.Split('\t');
                if (parts.Length != 3 || (parts[0] != "T" && parts[0] != "E")) throw new InvalidDataException("Invalid audit index");
                string original = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
                foreach (string source in Encoding.UTF8.GetString(Convert.FromBase64String(parts[2])).Split('\n')) AddEvidence(original, parts[0] == "T", source);
            }
            foreach (var row in Document.Entries) Classify(row);
        }

        void Classify(MissingTextEntry row)
        {
            Evidence evidence;
            row.Classification = index.TryGetValue(row.LookupText ?? row.Text, out evidence) ? (evidence.Translated ? "translated_but_english" : "exported_without_translation") : "uncollected_candidate";
            if (row.ReviewReason == "possible_player_name") row.Classification = "possible_player_name";
            row.Sources = evidence == null ? new List<string>() : new List<string>(evidence.Sources);
        }

        static string LookupText(string text, string path)
        {
            if (path.Contains("/TransientMessage(Clone)["))
                return Regex.Replace(text, @"\A\d{4}年\d{1,2}月\d{1,2}日\s+", "");
            return text;
        }

        static string ReviewReason(string text, string path)
        {
            if (path.Contains("/Profile(Clone)[") && Regex.IsMatch(path, @"/Name\[\d+\]::")) return "possible_player_name";
            if (path.Contains("/SurvivalHUD(Clone)[") && Regex.IsMatch(path, @"/Date\[\d+\]::") &&
                Regex.IsMatch(text, @"\A\d{4}年[A-Za-z]+月\d{1,2}日\z")) return "untranslated_month";
            return null;
        }

        public void Observe(int id, string raw, string path, double now, int scan, DateTime utc, bool ready = true)
        {
            if (String.IsNullOrEmpty(raw) || raw.Length > 16000) { pending.Remove(id); return; }
            string text = Clean(raw);
            if (Regex.IsMatch(path, @"/Version\[\d+\]::") && Regex.IsMatch(text, @"\ASunless Skies v[0-9][A-Za-z0-9.\-]*\z")) { pending.Remove(id); return; }
            string lookupText = LookupText(text, path), reason = ReviewReason(text, path);
            string key = reason == "untranslated_month" ? "HUD:untranslated_month" : reason == "possible_player_name" ? "ProfileName:" + text : lookupText;
            Pending state;
            if (!pending.TryGetValue(id, out state) || state.Raw != text)
                pending[id] = state = new Pending { Raw = text, Since = now, FirstSeen = utc, Ready = ready };
            state.Scan = scan;
            if (!ready) { state.Ready = false; return; }
            if (!state.Ready) { state.Ready = true; state.Since = now; }
            MissingTextEntry row;
            if (state.Recorded)
            {
                if (entries.TryGetValue(key, out row)) { row.LastSeenUtc = utc.ToUniversalTime().ToString("o"); Dirty = true; }
                return;
            }
            if (now - state.Since < 0.75) return;
            state.Recorded = true;
            string kind = SuspectKind(text);
            if (kind == null) return;
            string stamp = utc.ToUniversalTime().ToString("o");
            if (!entries.TryGetValue(key, out row))
            {
                if (entries.Count >= MaxEntries) { Document.DroppedAtLimit++; Dirty = true; return; }
                row = new MissingTextEntry { Text = text, DisplaySample = raw, IssueKey = key, LookupText = lookupText, ReviewReason = reason, FirstSeenUtc = state.FirstSeen.ToUniversalTime().ToString("o"), Kind = kind };
                Classify(row);
                entries.Add(key, row); Document.Entries.Add(row);
            }
            row.Count++; row.LastSeenUtc = stamp;
            if (row.Paths.Count < 8 && !row.Paths.Contains(path)) row.Paths.Add(path);
            Dirty = true;
        }

        public void EndScan(int scan)
        {
            foreach (int key in pending.Where(p => p.Value.Scan != scan).Select(p => p.Key).ToArray()) pending.Remove(key);
        }

        public static MissingTextDocument Read(string path)
        {
            using (var stream = File.OpenRead(path)) return (MissingTextDocument)new DataContractJsonSerializer(typeof(MissingTextDocument)).ReadObject(stream);
        }

        static void Replace(string temporary, string path)
        {
            if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
        }

        static string Markdown(string text)
        {
            return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\\", "\\\\").Replace("|", "\\|").Replace("`", "\\`").Replace("[", "\\[").Replace("]", "\\]").Replace("*", "\\*").Replace("_", "\\_").Replace("\r", " ").Replace("\n", " ");
        }

        public void Save(string directory)
        {
            Directory.CreateDirectory(directory);
            Document.GeneratedUtc = DateTime.UtcNow.ToString("o");
            string path = Path.Combine(directory, "missing-translations.json");
            using (var stream = File.Create(path + ".tmp")) new DataContractJsonSerializer(typeof(MissingTextDocument)).WriteObject(stream, Document);
            Replace(path + ".tmp", path);
            var md = new StringBuilder("# 疑似漏译（待审核）\n\n");
            md.AppendLine("平台：" + Document.Platform + "；游戏：" + Document.GameVersion + "；自定义插件：" + Document.PluginVersion + "（不是发布 tag）。");
            md.AppendLine("\n生成时间（UTC）：" + Document.GeneratedUtc + "；插件 SHA256：" + Document.PluginSha256 + "；索引 SHA256：" + Document.IndexSha256);
            md.AppendLine("\n按稳定显示次数计数，同一组件持续显示不重复累计。最多保留 5000 条、每条 8 个路径／来源；容量限制后忽略次数：" + Document.DroppedAtLimit + "。");
            md.AppendLine("\n比对采用去格式及空白归一化后的整段匹配，不证明翻译入口已命中；动态拼接和专有名词需人工核对。原始显示样本见 JSON。不会自动导出或上传。\n");
            md.AppendLine("航行消息仅在比对时去除数字日期前缀；HUD 英文月份合并为一个显示问题，保留首次原文样本。角色页姓名单列待审核，不作为普通待翻译词条。\n");
            foreach (string status in new[] { "translated_but_english", "exported_without_translation", "uncollected_candidate", "possible_player_name" })
            {
                var rows = Document.Entries.Where(r => r.Classification == status).OrderByDescending(r => r.Count).ToArray();
                md.AppendLine("## " + (status == "translated_but_english" ? "已有译文，实际仍显示英文" : status == "exported_without_translation" ? "已导出，尚无译文" : status == "possible_player_name" ? "疑似玩家自定义姓名（人工审核）" : "尚未收录的候选（含动态组合）") + "（" + rows.Length + "）\n");
                md.AppendLine("| 文本 | 次数 | 首次出现 UTC | 组件路径 | 词典／导出来源 |\n| --- | ---: | --- | --- | --- |");
                foreach (var row in rows) md.AppendLine("| " + Markdown(row.Text) + " | " + row.Count + " | " + row.FirstSeenUtc + " | " + Markdown(String.Join("; ", row.Paths.ToArray())) + " | " + Markdown(String.Join("; ", row.Sources.ToArray())) + " |");
                md.AppendLine();
            }
            path = Path.Combine(directory, "missing-translations.md");
            File.WriteAllText(path + ".tmp", md.ToString(), new UTF8Encoding(false)); Replace(path + ".tmp", path);
            Dirty = false;
        }
    }
}
