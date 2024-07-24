using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxGoldenAppleofDiscordCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("GoldenAppleofDiscord", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "GoldenAppleofDiscord", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.B ? 2 : 1,
            exhaust = upgrade != Upgrade.B
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        if (upgrade == Upgrade.A)
        {
            actions.Add(new ACannonRupture());
        }
        actions.Add(new ADiscard()
        {
            count = 1
        });

        actions.Add(new AAttack()
        {
            damage = GetDmg(s, upgrade == Upgrade.A ? 1 : 0),
            stunEnemy = true
        });

        actions.Add(new AStatus()
        {
            status = Status.overdrive,
            statusAmount = -2,
            targetPlayer = false
        });

        return actions;
    }
}
