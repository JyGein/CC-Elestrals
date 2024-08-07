﻿using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxBreakThroughCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("BreakThrough", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "BreakThrough", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade != Upgrade.B ? 2 : 1
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        actions.Add(new ACannonRupture());

        actions.Add(new AAttack()
        {
            damage = upgrade == Upgrade.B ? GetDmg(s, 2) : GetDmg(s, 4),
            piercing = upgrade == Upgrade.A
        });
        return actions;
    }
}
