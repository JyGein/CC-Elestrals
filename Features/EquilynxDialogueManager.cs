using HarmonyLib;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals;

internal sealed class EquilynxDialogueManager
{
    public static Elestrals Instance => Elestrals.Instance;
    public EquilynxDialogueManager()
    {
        Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Shout), nameof(Shout.GetText)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Shout_GetText_Postfix))
        );
    }

    private static void Shout_GetText_Postfix(Shout __instance, ref string __result)
    {
        if (__instance.who != Instance.Equilynx_Deck.UniqueName) return;
        if (GetMeowLastLocalText(__instance) != DB.currentLocale.locale)
        {
            SetMeowCache(__instance, null);
            SetMeowLastLocalText(__instance, DB.currentLocale.locale);
        }
        if (GetMeowCache(__instance) == null)
        {
            string meowText = __result switch
            {
                { Length: int i } when i > 40 => Localize(new List<string>() { "meowThree", "mreowThree" }.Shuffle(MG.inst.g.state.rngScript).First()),
                { Length: int i } when i > 20 => Localize("meowTwo"),
                _ => Localize(new List<string>() { "meow", "mreow" }.Shuffle(MG.inst.g.state.rngScript).First()),
            };
            if (__instance.loopTag == "squint") meowText += "..";
            if (__instance.loopTag == "gameover") meowText = Localize(new List<string>() { "meowYell", "mreowYell", "meowScream", "mreowScream" }.Shuffle(MG.inst.g.state.rngScript).First());
            if (__instance.loopTag == "neutral" && MG.inst.g.state.rngScript.Next() < 0.01) meowText = Localize("meowRare");
            SetMeowCache(__instance, meowText);
        }
        __result = GetMeowCache(__instance)!;
        static string Localize(string s) => Instance.Localizations.Localize(["dialogue", "Equilynx", s]);
        static string? GetMeowCache(Shout s) => Elestrals.Instance.Helper.ModData.GetOptionalModData<string>(s, "meowCache");
        static void SetMeowCache(Shout s, string? meow) => Elestrals.Instance.Helper.ModData.SetOptionalModData(s, "meowCache", meow);
        static string? GetMeowLastLocalText(Shout s) => Elestrals.Instance.Helper.ModData.GetOptionalModData<string>(s, "meowLastLocalText");
        static void SetMeowLastLocalText(Shout s, string? meow) => Elestrals.Instance.Helper.ModData.SetOptionalModData(s, "meowLastLocalText", meow);
    }
}
