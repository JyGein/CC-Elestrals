using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxBloomCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Bloom", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Bloom", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade != Upgrade.A ? 3 : 2,
            exhaust = true
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        if (upgrade == Upgrade.B)
        {
            actions.Add(new ASpawn()
            {
                thing = new FlowerStone(),
                offset = -2,
                omitFromTooltips = true
            });
        }
        actions.Add(new ASpawn()
        {
            thing = new FlowerStone(),
            offset = -1
        });
        actions.Add(new ASpawn()
        {
            thing = new FlowerStone(),
            omitFromTooltips = true
        });
        actions.Add(new ASpawn()
        {
            thing = new FlowerStone(),
            offset = 1,
            omitFromTooltips = true
        });
        if (upgrade == Upgrade.B)
        {
            actions.Add(new ASpawn()
            {
                thing = new FlowerStone(),
                offset = 2,
                omitFromTooltips = true
            });
        }
        return actions;
    }
}
