using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxSandstormCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Sandstorm", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Sandstorm", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 1
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        switch (upgrade)
        {
            case Upgrade.B:
                actions.Add(new AMove()
                {
                    dir = 1,
                    isRandom = true
                });
                actions.Add(new AAttack()
                {
                    damage = 1
                });
                actions.Add(new ADroneMove()
                {
                    dir = 1,
                    isRandom = true
                });
                actions.Add(new AAttack()
                {
                    damage = 1
                });
                actions.Add(new AMove()
                {
                    dir = 1,
                    isRandom = true
                });
                break;
            default:
                actions.Add(new ADroneMove()
                {
                    dir = 1,
                    isRandom = true
                });
                actions.Add(new AAttack()
                {
                    damage = upgrade == Upgrade.A ? 2 : 1
                });
                actions.Add(new AMove()
                {
                    dir = 1,
                    isRandom = true
                });
                break;
        }
        return actions;
    }
}
