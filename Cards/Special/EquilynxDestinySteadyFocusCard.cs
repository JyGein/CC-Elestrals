using JyGein.Elestrals.Actions;
using JyGein.Elestrals.ExternalAPI;
using JyGein.Elestrals.Jester;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static JyGein.Elestrals.ExternalAPI.IKokoroApi.IV2.IActionCostsApi;

namespace JyGein.Elestrals.Cards.Special;

internal class EquilynxDestinySteadyFocusCard : Card, IElestralsCard
{
    private static IDestinyApi destinyApi = null!;
    public static void Register(IModHelper helper)
    {
        ICardEntry thisEntry = helper.Content.Cards.RegisterCard("EquilynxDestinySteadyFocus", new()

        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.DuoApis.DuoArtifactsApi!.DuoArtifactVanillaDeck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "SteadyFocus", "name"]).Localize
        });

        IStatusResource shardResource = Elestrals.Instance.KokoroApiV2.ActionCosts.MakeStatusResource(Status.shard);
        destinyApi = Elestrals.Instance.DuoApis.destinyApi!;
        destinyApi.SetEnchantLevelCost(thisEntry.UniqueName, Upgrade.None, 1, Elestrals.Instance.KokoroApiV2.ActionCosts.MakeResourceCost(shardResource, 2));
        destinyApi.SetEnchantLevelCost(thisEntry.UniqueName, Upgrade.A, 1, Elestrals.Instance.KokoroApiV2.ActionCosts.MakeResourceCost(shardResource, 1));
        destinyApi.SetEnchantLevelCost(thisEntry.UniqueName, Upgrade.B, 1, Elestrals.Instance.KokoroApiV2.ActionCosts.MakeResourceCost(shardResource, 2));
    }

    public override CardData GetData(State state)
        => new()
        {
            cost = 0,
            exhaust = true,
            buoyant = upgrade != Upgrade.B,
            retain = upgrade == Upgrade.A,
            art = destinyApi.GetEnchantedCardArt(this),
            artTint = "ffffff"
        };

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        IDestinyApi destinyApi = Elestrals.Instance.DuoApis.destinyApi!;
        actions.Add(new AStatus 
        {
            targetPlayer = true,
            status = Elestrals.Instance.HyperFocus.Status,
            statusAmount = 1
        });
        actions.Add(destinyApi.MakeEnchantGateAction(1).AsCardAction);
        actions.Add(destinyApi.MakeEnchantedAction(uuid, 1, new AStatus
        {
            targetPlayer = true,
            status = Status.powerdrive,
            statusAmount = upgrade == Upgrade.B ? 2 : 1
        }).AsCardAction);
        return actions;
    }
}