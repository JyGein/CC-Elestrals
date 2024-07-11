using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxEarthStoneDepositCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("EarthStoneDeposit", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "EarthStoneDeposit", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        int Cost;
        switch(upgrade)
        {
            case Upgrade.None:
                Cost = 2;
                break;
            case Upgrade.A:
                Cost = 1;
                break;
            case Upgrade.B:
                Cost = 3;
                break;
            default:
                Cost = 2;
                break;
        }
        CardData data = new CardData()
        {
            cost = Cost,
            exhaust = true
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        actions.Add(new AStatus()
        {
            status = Elestrals.Instance.EarthStoneDeposit.Status,
            targetPlayer = true,
            statusAmount = upgrade == Upgrade.B ? 2 : 1
        });

        return actions;
    }
}
