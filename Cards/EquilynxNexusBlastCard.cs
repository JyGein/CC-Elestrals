using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxNexusBlastCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("NexusBlast", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "NexusBlast", "name"]).Localize,
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            cost = 0,
            art = Elestrals.Instance.Equilynx_Character_NexusCardBackground.Sprite
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = new();

        if (upgrade == Upgrade.A)
        {
            actions.Add(new ADrawCard()
            {
                count = 1
            });
        }
        actions.Add(new ABayRupture()
        {
            offset = -1
        });
        actions.Add(new ABayRupture()
        {
            omitFromTooltips = true
        });
        actions.Add(new ABayRupture()
        {
            offset = 1,
            omitFromTooltips = true
        });
        actions.Add(new AStatus()
        {
            status = Status.tempShield,
            statusAmount = upgrade == Upgrade.B ? 3 : 1,
            targetPlayer = true
        });
        return actions;
    }
}
