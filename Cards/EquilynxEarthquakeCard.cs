using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxEarthquakeCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Earthquake", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Earthquake", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade != Upgrade.B ? 1 : 2,
            exhaust = upgrade != Upgrade.A
        };
        if (upgrade == Upgrade.B)
        {
            data.description = Elestrals.Instance.Localizations.Localize(["card", "Earthquake", "description", "B"], new { Damage = 1 });
        }
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        if (upgrade == Upgrade.B)
        {
            actions.Add(new AAllMidrowAttack()
            {
                damage = 1,
            });
        }

        actions.Add(new AAllRupture());
        return actions;
    }
}
