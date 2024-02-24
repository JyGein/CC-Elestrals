using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class DemoArtifactBookOfTails : Artifact, IElestralsArtifact
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("BookOfTails", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.DemoMod_Deck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/bookoftails.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "BookOfTails", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "BookOfTails", "description"]).Localize
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new List<Tooltip>
        {
            new TTCard
            {
                card = new EquilynxEarthStoneCard
                {
                    temporaryOverride = true
                }
            }
        }
        .ToList();

    public override void OnTurnStart(State s, Combat c)
    {
        if (!c.isPlayerTurn)
            return;
        c.QueueImmediate([
            new AAddCard
            {
                card = new EquilynxEarthStoneCard
                {
                    temporaryOverride = true
                },
                destination = CardDestination.Hand
            }
        ]);
    }
}
