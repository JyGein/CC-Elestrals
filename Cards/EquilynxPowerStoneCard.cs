using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxPowerStoneCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("PowerStone", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "PowerStone", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 1,
            exhaust = upgrade == Upgrade.B,
            flippable = upgrade == Upgrade.A
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        /* The meat of the card, this is where we define what a card does, and some would say the most fun part of modding Cobalt Core happens here! */
        List<CardAction> actions = new();

        if (upgrade == Upgrade.A)
        {
            actions.Add(new ADroneMove
            {
                dir = 1,
                omitFromTooltips = upgrade == Upgrade.B
            });
        }
        actions.Add(new ASpawn
        {
            thing = new PowerStone { }
        });
        if (upgrade == Upgrade.B)
        {
            actions.Add(new ASpawn
            {
                thing = new PowerStone { },
                offset = 1
            });
        }
        return actions;
    }
}
