using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SunlessSkiesCN.Shared
{
    // Unity objects are inspected only on the Unity thread, after normal text updates.
    public sealed class MissingTranslationMonitor : MonoBehaviour
    {
#if STEAM
        public MissingTranslationMonitor(IntPtr pointer) : base(pointer) { }
#endif
        static MissingTranslationReport report;
        static string directory;
        static Func<bool> active, queueIdle;
        static Action<string> log;
        static MethodInfo translationInfo;
        static PropertyInfo stabilizing, settingText;
        static int errors;
        float nextScan, nextSave;
        int scan;
        bool ready;

        public static void Initialize(string platform, string pluginVersion, Func<bool> translatedMode, MethodInfo decode, Action<string> logger)
        {
            if (report != null) return;
            log = logger;
            var core = decode.DeclaringType.Assembly;
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var translator = core.GetType("XUnity.AutoTranslator.Plugin.Core.AutoTranslationPlugin", true);
            var current = translator.GetField("Current", flags);
            var managerField = translator.GetField("TranslationManager", flags);
            var managerType = core.GetType("XUnity.AutoTranslator.Plugin.Core.TranslationManager", true);
            var ongoing = managerType.GetProperty("OngoingTranslations", flags);
            var unstarted = managerType.GetProperty("UnstartedTranslations", flags);
            translationInfo = core.GetType("XUnity.AutoTranslator.Plugin.Core.Extensions.ComponentExtensions", true).GetMethod("GetTextTranslationInfo", flags);
            var infoType = core.GetType("XUnity.AutoTranslator.Plugin.Core.TextTranslationInfo", true);
            stabilizing = infoType.GetProperty("IsStabilizingText", flags);
            settingText = infoType.GetProperty("IsCurrentlySettingText", flags);
            if (current == null || managerField == null || ongoing == null || unstarted == null || translationInfo == null || stabilizing == null || settingText == null)
                throw new MissingMemberException("Unsupported AutoTranslator diagnostic state");
            active = translatedMode;
            queueIdle = () => {
                var instance = current.GetValue(null);
                var manager = instance == null ? null : managerField.GetValue(instance);
                return manager != null && (int)ongoing.GetValue(manager, null) == 0 && (int)unstarted.GetValue(manager, null) == 0;
            };
            string pluginHash = Hash(typeof(MissingTranslationMonitor).Assembly.Location);
            string indexPath = Path.Combine(Paths.BepInExRootPath, "Translation/zh/missing-translation-index.tsv");
            string indexHash = Hash(indexPath);
            string gameVersion = Application.version;
            var dictionaries = new[] { "Text/ui.txt", "qvd-branches.txt", "qvd-context.txt" };
            string dictionaryHash = Hash(dictionaries.Select(relative => Path.Combine(Paths.BepInExRootPath, "Translation/zh/" + relative)).ToArray());
            // Isolate builds and evidence snapshots so old observations never acquire new version labels.
            directory = Path.Combine(Paths.BepInExRootPath, "reports/MissingTranslations/" + platform + "-" + System.Text.RegularExpressions.Regex.Replace(gameVersion, @"[^A-Za-z0-9._-]", "_") + "-" + pluginHash.Substring(0, 12) + "-" + indexHash.Substring(0, 12) + "-" + dictionaryHash.Substring(0, 12));
            string path = Path.Combine(directory, "missing-translations.json");
            var document = File.Exists(path) ? MissingTranslationReport.Read(path) : new MissingTextDocument {
                Platform = platform, GameVersion = gameVersion, PluginVersion = pluginVersion, PluginSha256 = pluginHash, IndexSha256 = indexHash, DictionarySha256 = dictionaryHash
            };
            if (document.Platform != platform || document.GameVersion != gameVersion || document.PluginSha256 != pluginHash || document.IndexSha256 != indexHash || document.DictionarySha256 != dictionaryHash)
                throw new InvalidDataException("Report version mismatch; preserve the report and move it before restarting.");
            var loaded = new MissingTranslationReport(document);
            foreach (string relative in dictionaries)
            {
                foreach (string line in File.ReadLines(Path.Combine(Paths.BepInExRootPath, "Translation/zh/" + relative)))
                {
                    var pair = (string[])decode.Invoke(null, new object[] { line });
                    if (pair == null || pair.Length != 2 || pair[0].StartsWith("r:") || pair[0].StartsWith("sr:")) continue;
                    string original = pair[0];
                    if (relative == "qvd-context.txt") { int split = original.IndexOf('\t'); if (split < 0) continue; original = original.Substring(split + 1); }
                    if (!String.IsNullOrWhiteSpace(pair[1]) && pair[1] != original) loaded.AddEvidence(original, true, "runtime:" + relative);
                }
            }
            loaded.LoadIndex(indexPath);
            loaded.Save(directory);
            report = loaded;
            log("Missing translation monitor ready: " + directory + "; local-only, 0.5s scans, >=0.75s stable text.");
        }

        static string Hash(params string[] paths)
        {
            using (var sha = SHA256.Create()) using (var sink = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write))
            {
                foreach (string path in paths) using (var stream = File.OpenRead(path)) stream.CopyTo(sink);
                sink.FlushFinalBlock();
                return BitConverter.ToString(sha.Hash).Replace("-", "").ToLowerInvariant();
            }
        }

        void LateUpdate()
        {
            if (report == null || Time.realtimeSinceStartup < nextScan) return;
            float now = Time.realtimeSinceStartup;
            nextScan = now + 0.5f;
            try
            {
                scan++;
                if (active())
                {
                    ready = queueIdle();
                    foreach (var text in UnityEngine.Object.FindObjectsOfType<TMP_Text>())
                        if (text && text.isActiveAndEnabled && text.color.a > 0.01f && GraphicVisible(text)) Observe(text, text.text, now);
                    foreach (var text in UnityEngine.Object.FindObjectsOfType<Text>())
                        if (text && text.isActiveAndEnabled && text.color.a > 0.01f && GraphicVisible(text)) Observe(text, text.text, now);
                    foreach (var text in UnityEngine.Object.FindObjectsOfType<TextMesh>())
                    {
                        if (!text || !text.gameObject.activeInHierarchy || text.color.a <= 0.01f) continue;
                        var renderer = text.GetComponent<Renderer>();
                        if (renderer && renderer.enabled && renderer.isVisible) Observe(text, text.text, now);
                    }
                }
                report.EndScan(scan);
                if (now >= nextSave)
                {
                    nextSave = now + 15f;
                    if (report.Dirty) report.Save(directory);
                }
            }
            catch (Exception error) { if (errors++ < 5) log("Missing translation monitor: " + error.Message); }
        }

        static bool GraphicVisible(Graphic text)
        {
            var world = text.GetComponent<MeshRenderer>();
            if (world) return world.enabled && world.isVisible;
            var canvas = text.canvas;
            if (!canvas || !canvas.isActiveAndEnabled || text.canvasRenderer.cull || text.canvasRenderer.GetAlpha() <= 0.01f) return false;
            bool ignoreGroups = false;
            for (var node = text.transform; node; node = node.parent)
            {
                var parentCanvas = node.GetComponent<Canvas>();
                if (parentCanvas && !parentCanvas.isActiveAndEnabled) return false;
                if (ignoreGroups) continue;
                foreach (var group in node.GetComponents<CanvasGroup>())
                {
                    if (!group.enabled) continue;
                    if (group.alpha <= 0.01f) return false;
                    if (group.ignoreParentGroups) ignoreGroups = true;
                }
            }
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : (canvas.worldCamera ? canvas.worldCamera : Camera.main);
            var rect = text.rectTransform.rect;
            float left = Single.PositiveInfinity, right = Single.NegativeInfinity, bottom = Single.PositiveInfinity, top = Single.NegativeInfinity;
            bool inFront = camera == null;
            for (int corner = 0; corner < 4; corner++)
            {
                var worldPoint = text.rectTransform.TransformPoint((corner & 1) == 0 ? rect.xMin : rect.xMax, (corner & 2) == 0 ? rect.yMin : rect.yMax, 0);
                var point = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
                if (camera && camera.WorldToScreenPoint(worldPoint).z > 0) inFront = true;
                left = Math.Min(left, point.x); right = Math.Max(right, point.x);
                bottom = Math.Min(bottom, point.y); top = Math.Max(top, point.y);
            }
            if (!inFront || right < 0 || top < 0 || left > Screen.width || bottom > Screen.height) return false;
            // shortcut: Unity culling/alpha is a visibility approximation; overlapping windows and partial text clipping need manual review.
            return true;
        }

        void Observe(Component component, string text, float now)
        {
            try
            {
                var info = translationInfo.Invoke(null, new object[] { component });
                bool settled = ready && (info == null || (!(bool)stabilizing.GetValue(info, null) && !(bool)settingText.GetValue(info, null)));
                string path = "";
                for (var node = component.transform; node; node = node.parent) path = "/" + node.name + "[" + node.GetSiblingIndex() + "]" + path;
                path = component.gameObject.scene.name + path + "::" + component.GetType().FullName;
                report.Observe(component.GetInstanceID(), text, path, now, scan, DateTime.UtcNow, settled);
            }
            catch (Exception error) { if (errors++ < 5) log("Missing translation component skipped: " + error.Message); }
        }

        void OnApplicationQuit()
        {
            try { if (report != null && report.Dirty) report.Save(directory); }
            catch (Exception error) { log("Missing translation report save failed: " + error.Message); }
        }
    }
}
