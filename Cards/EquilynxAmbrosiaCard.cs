using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxAmbrosiaCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("Ambrosia", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "Ambrosia", "name"]).Localize,
            Art = Elestrals.Instance.Helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/cards/equilynx/Ambrosia.png")).Sprite
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 2 : 1,
            exhaust = upgrade != Upgrade.B,
            singleUse = upgrade == Upgrade.B,
            retain = upgrade == Upgrade.B
        };
        return data;
    }

    public static int GetX(State s, Combat c)
    {
        return c.exhausted.Count;
    }

    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();
        var amt = GetX(s, c);
        switch (upgrade)
        {
            case Upgrade.B:
                actions.Add(new ADrawCard()
                {
                    count = 1
                });
                actions.Add(new AHullMax()
                {
                    amount = 1,
                    targetPlayer = true
                });
                actions.Add(new AVariableHintExhaust()
                {
                    setAmount = amt
                });
                actions.Add(new AHeal()
                {
                    healAmount = amt,
                    targetPlayer = true,
                    xHint = 1
                });
                actions.Add(new AEnergy()
                {
                    changeAmount = amt,
                    xHint = 1
                });
                break;
            default:
                actions.Add(new AHeal()
                {
                    healAmount = upgrade == Upgrade.None ? 1 : 2,
                    targetPlayer = true
                });
                actions.Add(new AEnergy()
                {
                    changeAmount = upgrade == Upgrade.None ? 1 : 2,
                });
                break;
        }
        return actions;
    }
}
