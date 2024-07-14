using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxBlossomCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Blossom", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Blossom", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        int Cost = upgrade switch
        {
            Upgrade.A => 0,
            Upgrade.B => 2,
            _ => 1
        };
        CardData data = new CardData()
        {
            cost = Cost,
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        /* The meat of the card, this is where we define what a card does, and some would say the most fun part of modding Cobalt Core happens here! */
        List<CardAction> actions = new();

        actions.Add(new ABlossom () { });
        if (upgrade == Upgrade.B)
        {
            actions.Add(new ASpawn()
            {
                thing = new FlowerStone(),
                offset = -1
            });
            actions.Add(new ASpawn()
            {
                thing = new FlowerStone(),
                offset = 1,
                omitFromTooltips = true
            });
        }
        return actions;
    }
}
