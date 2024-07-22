using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxMudslideCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Mudslide", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Mudslide", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 2,
            flippable = upgrade == Upgrade.A
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        actions.Add(new ASpawn()
        {
            thing = new EarthStone()
            {
                StoneType = EarthStone.EarthStoneType.Mini
            }
        });
        actions.Add(new ASpawn()
        {
            thing = new EarthStone()
            {
                StoneType = upgrade == Upgrade.B ? EarthStone.EarthStoneType.Normal : EarthStone.EarthStoneType.Mini
            },
            offset = 1,
            omitFromTooltips = true
        });
        actions.Add(new ASpawn()
        {
            thing = new EarthStone()
            {
                StoneType = upgrade == Upgrade.B ? EarthStone.EarthStoneType.Normal : EarthStone.EarthStoneType.Mini
            },
            offset = 2,
            omitFromTooltips = true
        });
        actions.Add(new ASpawn()
        {
            thing = new EarthStone()
            {
                StoneType = EarthStone.EarthStoneType.Mini
            },
            offset = 3,
            omitFromTooltips = true
        });
        return actions;
    }
}
