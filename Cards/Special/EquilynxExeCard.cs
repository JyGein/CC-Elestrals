using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Cards.Special;

internal class EquilynxExeCard : Card, IElestralsCard
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Cards.RegisterCard("EquilynxExe", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Deck.colorless,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            //Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/DynaExe.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "EquilynxExe", "name"]).Localize
        });
    }

    private int GetChoiceCount()
        => upgrade == Upgrade.B ? 3 : 2;

    public override CardData GetData(State state)
        => new()
        {
            artTint = Elestrals.Instance.Equilynx_Deck.Configuration.Definition.color.ToString(),
            cost = upgrade == Upgrade.A ? 0 : 1,
            exhaust = true,
            description = Elestrals.Instance.Localizations.Localize(["card", "EquilynxExe", "description", upgrade.ToString()], new { Count = GetChoiceCount() })
        };

    public override List<CardAction> GetActions(State s, Combat c)
        => [
            new ACardOffering
            {
                amount = GetChoiceCount(),
                limitDeck = Elestrals.Instance.Equilynx_Deck.Deck,
                makeAllCardsTemporary = true,
                overrideUpgradeChances = false,
                canSkip = false,
                inCombat = true,
                discount = -1,
                dialogueSelector = $".summon{Elestrals.Instance.Equilynx_Deck.UniqueName}"
            }
        ];
}