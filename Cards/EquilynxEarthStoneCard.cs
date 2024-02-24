using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

/* Like other namespaces, this can be named whatever
 * However it's recommended that you follow the structure defined by ModEntry of <JyGein>.<ModName> or <JyGein>.<ModName>.Cards*/
namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxEarthStoneCard : Card, IElestralsCard
{
    /* For a bit more info on the Register Method, look at InternalInterfaces.cs and 1. CARDS section in ModEntry */
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("EarthStone", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                /* We don't assign cards to characters, but rather to decks! It's important to keep that in mind */
                deck = Elestrals.Instance.DemoMod_Deck.Deck,

                /* The vanilla rarities are Rarity.common, Rarity.uncommon, Rarity.rare */
                rarity = Rarity.common,

                /* Some vanilla cards don't upgrade, some only upgrade to A, but most upgrade to either A or B */
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            /* AnyLocalizations.Bind().Localize will find the 'name' of 'Foxtale' in 'card', in the locale file, and feed it here. The output for english in-game from this is 'Fox Tale' */
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "EarthStone", "name"]).Localize
        });
    }
    public override CardData GetData(State state)
    {
        CardData data = new CardData()
        {
            /* Give your card some meta data, such as giving it an energy cost, making it exhaustable, and more */
            cost = 1,

            /* if we don't set a card specific 'art' (a 'Spr' type) here, the game will give it the deck's 'DefaultCardArt'
            /* if we don't set a card specific 'description' (a 'string' type) here, the game will attempt to use iconography using the provided CardAction types from GetActions() */
        };
        return data;
    }
    public override List<CardAction> GetActions(State s, Combat c)
    {
        /* The meat of the card, this is where we define what a card does, and some would say the most fun part of modding Cobalt Core happens here! */
        List<CardAction> actions = new();

        actions.Add(new ASpawn
        {
            thing = new EarthStone
            {
                Type = upgrade != Upgrade.A ? EarthStone.EarthStoneType.Normal : EarthStone.EarthStoneType.Big
            }
        });
        if(upgrade == Upgrade.B)
        {
            actions.Add(new AStatus()
            {
                status = Status.droneShift,
                statusAmount = 1,
                targetPlayer = true
            });
        }
        return actions;
    }
}
