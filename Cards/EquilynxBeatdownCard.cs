using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxBeatdownCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Beatdown", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Beatdown", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 2,
            exhaust = upgrade == Upgrade.B ? true : false
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        actions.Add(new AAttack()
        {
            damage = upgrade == Upgrade.A ? GetDmg(s, 7) : GetDmg(s, 5),
            weaken = upgrade == Upgrade.B ? true : false
        });
        actions.Add(new AStatus()
        {
            status = Status.overdrive,
            statusAmount = -1
        });
        return actions;
    }
}
