using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Features;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxTheBiggertheBetterCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("TheBiggertheBetter", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "TheBiggertheBetter", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 3 : 2,
            exhaust = upgrade == Upgrade.B,
            retain = upgrade == Upgrade.B/*,
            description = Elestrals.Instance.Localizations.Localize(["card", "TheBiggertheBetter`", "description", upgrade.ToString()])*/
        };
        return data;
    }

    public static int GetX(State s, Combat c)
    {
        int i = 0;
        foreach(StuffBase stuff in c.stuff.Values)
        {
            if (RuptureManager.IsNatural(stuff))
            {
                i++;
            }
        }
        return i;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();
        var amt = GetX(s, c);
        actions.Add(new AVariableHintObjects
        {
            setAmount = amt
        });
        actions.Add(new AAttack()
        {
            damage = GetDmg(s, amt * (upgrade == Upgrade.B ? 2 : 1)),
            xHint = upgrade == Upgrade.B ? 2 : 1
        });
        if (upgrade != Upgrade.A)
        {
            actions.Add(new AStatus()
            {
                status = Status.overdrive,
                statusAmount = -1,
                targetPlayer = true
            });
        }
        return actions;
    }
}
