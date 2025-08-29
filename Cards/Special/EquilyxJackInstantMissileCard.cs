using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Jester;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Cards.Special;

internal class EquilyxJackInstantMissileCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("EquilyxJackInstantMissile", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.DuoApis.DuoArtifactsApi!.DuoArtifactVanillaDeck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "InstantMissile", "name"]).Localize
        });
    }

    public override CardData GetData(State state)
        => new()
        {
            cost = 0,
            exhaust = upgrade == Upgrade.B
        };

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];
        if (upgrade == Upgrade.A) actions.Add(new AStatus 
        {
            targetPlayer = false,
            status = Elestrals.Instance.DuoApis.jackApi!.LockOnStatus.Status,
            statusAmount = 1
        });
        if (upgrade == Upgrade.B) actions.Add(new ASpawn
        {
            thing = Elestrals.Instance.DuoApis.jackApi!.BlankMissile,
            offset = -1
        });
        actions.Add(new ASpawn
        {
            thing = Elestrals.Instance.DuoApis.jackApi!.BlankMissile
        });
        if (upgrade == Upgrade.B) actions.Add(new ASpawn
        {
            thing = Elestrals.Instance.DuoApis.jackApi!.BlankMissile,
            offset = 1
        });
        actions.Add(upgrade != Upgrade.B ? new ABayRupture() : new AAllRupture());
        return actions;
    }
}