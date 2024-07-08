using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxNexusBlastCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("NexusBlast", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "NexusBlast", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade != Upgrade.A ? 1 : 0,
            flippable = upgrade == Upgrade.B
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        if (upgrade == Upgrade.B)
        {
            actions.Add(new ADroneMove()
            {
                dir = 1
            });
        }
        actions.Add(new ARupture()
        {
            ruptureType = ARupture.RuptureType.Missile,
            offset = -1
        });
        actions.Add(new ARupture()
        {
            ruptureType = ARupture.RuptureType.Missile,
            omitFromTooltips = true
        });
        actions.Add(new ARupture()
        {
            ruptureType = ARupture.RuptureType.Missile,
            offset = 1,
            omitFromTooltips = true
        });
        actions.Add(new AStatus()
        {
            status = Status.tempShield,
            statusAmount = 1,
            targetPlayer = true
        });
        return actions;
    }
}
