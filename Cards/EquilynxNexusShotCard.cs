using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxNexusShotCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("NexusShot", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "NexusShot", "name"]).Localize,
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = upgrade == Upgrade.A ? 0 : 1,
            art = Elestrals.Instance.Equilynx_Character_NexusCardBackground.Sprite,
            flippable = true
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        actions.Add(new AAttack()
        {
            damage = GetDmg(s, upgrade == Upgrade.B ? 3 : 1)
        });
        actions.Add(new ADroneMove()
        {
            dir = 1
        });
        if (upgrade == Upgrade.B)
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
