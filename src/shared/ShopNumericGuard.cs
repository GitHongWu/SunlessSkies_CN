using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace SunlessSkiesCN.Shared
{
    // Numeric inputs are also read by the game. Never translate or reformat their values.
    // XUnity's IgnoreTextComponent alone does not prevent DiscoverComponent changing fonts.
    internal static class ShopNumericGuard
    {
        sealed class Marker { }
        sealed class Binding
        {
            public FieldInfo Field;
            public PropertyInfo Property;
            public PropertyInfo TextComponent;
            public object Read(object owner)
            {
                object value = Field != null ? Field.GetValue(owner) : Property.GetValue(owner, null);
                return value == null || TextComponent == null ? value : TextComponent.GetValue(value, null);
            }
        }

        static readonly ConditionalWeakTable<object, Marker> protectedInfo = new ConditionalWeakTable<object, Marker>();
        static readonly Dictionary<Type, Binding[]> bindings = new Dictionary<Type, Binding[]>();
        static readonly HashSet<Type> reported = new HashSet<Type>();
        static MethodInfo getInfo, unfont, unresize;
        static PropertyInfo shouldIgnore;
        static Action<string> log;
        static Harmony guardHarmony;
        static bool reportedError;

        static Binding Bind(Type type, string name, bool input)
        {
            var field = AccessTools.Field(type, name);
            var property = field == null ? AccessTools.Property(type, name) : null;
            if (field == null && property == null) throw new MissingMemberException(type.FullName, name);
            var memberType = field != null ? field.FieldType : property.PropertyType;
            var text = input ? AccessTools.Property(memberType, "textComponent") : null;
            if (input && text == null) throw new MissingMemberException(memberType.FullName, "textComponent");
            return new Binding { Field = field, Property = property, TextComponent = text };
        }

        public static void Install(Assembly core, Assembly game, Action<string> logger)
        {
            if (guardHarmony != null) return;
            log = logger;
            var info = core.GetType("XUnity.AutoTranslator.Plugin.Core.TextTranslationInfo", true);
            var extensions = core.GetType("XUnity.AutoTranslator.Plugin.Core.Extensions.ComponentExtensions", true);
            getInfo = AccessTools.Method(extensions, "GetOrCreateTextTranslationInfo", new[] { typeof(object) });
            unfont = AccessTools.Method(info, "UnchangeFont", new[] { typeof(object) });
            unresize = AccessTools.Method(info, "UnresizeUI", new[] { typeof(object) });
            shouldIgnore = AccessTools.Property(info, "ShouldIgnore");
            var font = AccessTools.Method(info, "ChangeFont", new[] { typeof(object) });
            var resize = AccessTools.Method(info, "ResizeUI");
            if (getInfo == null || unfont == null || unresize == null || shouldIgnore == null || font == null || resize == null)
                throw new MissingMethodException("Unsupported AutoTranslator numeric control API");

            var bazaar = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Bazaar.BazaarAvailabilityPresenter", true);
            var shop = game.GetType("Skyless.Assets.Code.Skyless.UI.Presenters.Shops.AvailabilityPresenter", true);
            bindings[bazaar] = new[] { Bind(bazaar, "BargainQuantity", true), Bind(bazaar, "BargainsAvailable", false),
                Bind(bazaar, "ProspectQuantity", true), Bind(bazaar, "ProspectCappedOwnedQuantity", false) };
            bindings[shop] = new[] { Bind(shop, "BuyInputField", true), Bind(shop, "SellInputField", true),
                Bind(shop, "QuantityOfItemsWhenBuying", false), Bind(shop, "QuantityOfItemsWhenSelling", false),
                Bind(shop, "QuantityPlayerOwnsWhenSellingText", false) };
            var entries = new[] { AccessTools.Method(bazaar, "InitBargain"), AccessTools.Method(bazaar, "InitProspect"),
                AccessTools.Method(shop, "Initialize") };
            foreach (var entry in entries) if (entry == null) throw new MissingMethodException("Missing shop initialization method");

            var h = new Harmony("githongwu.sunlessskies.numeric-controls");
            try
            {
                var preserve = new HarmonyMethod(typeof(ShopNumericGuard), "PreserveNumericStyle");
                h.Patch(font, prefix: preserve);
                h.Patch(resize, prefix: preserve);
                foreach (var entry in entries)
                    h.Patch(entry, prefix: new HarmonyMethod(typeof(ShopNumericGuard), "BeforeInitialize"));
                guardHarmony = h;
                log("Shop numeric guard installed: original quantity fonts/layout and input values preserved.");
            }
            catch { h.UnpatchSelf(); throw; }
        }

        static bool PreserveNumericStyle(object __instance)
        {
            Marker marker;
            return !protectedInfo.TryGetValue(__instance, out marker);
        }

        static void Protect(object text)
        {
            if (text == null) return;
            var info = getInfo.Invoke(null, new[] { text });
            if (info == null) return;
            Marker marker;
            if (protectedInfo.TryGetValue(info, out marker)) return;
            protectedInfo.Add(info, new Marker());
            shouldIgnore.SetValue(info, true, null);
            // Discovery may run as the prefab is enabled, before the presenter is initialized.
            // Undo only XUnity's recorded changes, not the game's font size or layout settings.
            unfont.Invoke(info, new[] { text });
            unresize.Invoke(info, new[] { text });
        }

        static void BeforeInitialize(object __instance)
        {
            try
            {
                Binding[] list;
                if (__instance == null || !bindings.TryGetValue(__instance.GetType(), out list)) return;
                foreach (var binding in list) Protect(binding.Read(__instance));
                if (reported.Add(__instance.GetType())) log("Shop numeric guard active: " + __instance.GetType().Name);
            }
            catch (Exception ex)
            {
                if (!reportedError) { reportedError = true; log("Shop numeric guard could not protect a control: " + ex); }
            }
        }
    }
}
