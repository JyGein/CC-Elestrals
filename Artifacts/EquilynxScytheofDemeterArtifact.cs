using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxScytheofDemeterArtifact : Artifact, IElestralsArtifact
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("ScytheofDemeter", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/scytheofdemeter.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "ScytheofDemeter", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "ScytheofDemeter", "description"]).Localize
        });
    }

    public override void OnAsteroidIsDestroyed(State state, Combat combat, bool wasPlayer, int worldX)
    {
        if (wasPlayer) state.ship.Add(Status.shield, 1);
    }

    public override List<Tooltip>? GetExtraTooltips()
        => StatusMeta.GetTooltips(Status.shield, 1);

}
