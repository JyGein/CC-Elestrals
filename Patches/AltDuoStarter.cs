using HarmonyLib;
using JyGein.Elestrals.Cards;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals;

public class AltDuoStarterManager
{
    public static void ApplyPatches(Harmony harmony, ILogger logger)
    {
        if (AccessTools.AllAssemblies().FirstOrDefault(a => a.FullName?.Contains("MoreDifficulties") ?? false) is not Assembly mdoAssembly)
        {
            logger.LogInformation("Unable to find More Difficulties Assembly.");
            return;
        }
        if (mdoAssembly.GetTypes().FirstOrDefault(t => t.Name == "AltStarters") is not Type AltStartersType)
        {
            logger.LogInformation("Unable to find AltStarters type within More Difficulties.");
            return;
        }


        CustomRunOptionsApi = Elestrals.Instance.Helper.ModRegistry.GetApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions")!;
        Elestrals.Instance.Helper.Events.OnSaveLoaded += (_, s) => CustomRunOptionsApi.RegisterPartialDuoDeck(Elestrals.Instance.Equilynx_Deck.Deck, new StarterDeck { cards = DuoStarterDecks[Elestrals.Instance.MoreDifficultiesApi!.AreAltStartersEnabled(s, Elestrals.Instance.Equilynx_Deck.Deck)] }); ;
        harmony.Patch(
            original: AccessTools.DeclaredMethod(AltStartersType, "SetAltStarters"),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AltStarters_SetAltStarters_Postfix))
        );
    }

    internal static ICustomRunOptionsApi CustomRunOptionsApi = null!;
    internal static Dictionary<bool, List<Card>> DuoStarterDecks = new()
    {
        { false, [
                    new EquilynxEarthStoneCard(),
                    new EquilynxNexusBlastCard(),
                    new EquilynxSandstormCard()
                ] },
        { true, [
                    new EquilynxNexusShotCard(),
                    new EquilynxFlowerStoneCard(),
                    new EquilynxNexusSwipeCard()
                ] }
    };

    public static void AltStarters_SetAltStarters_Postfix(Deck deck, bool on)
    {
        if (deck != Elestrals.Instance.Equilynx_Deck.Deck) return;
        CustomRunOptionsApi.RegisterPartialDuoDeck(Elestrals.Instance.Equilynx_Deck.Deck, new StarterDeck { cards = DuoStarterDecks[on] });
    }
}
