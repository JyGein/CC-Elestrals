using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxPoisonTippedArrowArtifact : Artifact, IElestralsArtifact
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("PoisonTippedArrow", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/poisontippedarrow.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "PoisonTippedArrow", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "PoisonTippedArrow", "description"]).Localize
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new List<Tooltip>
        {
            new TTGlossary($"status.{Elestrals.Instance.WeakenCharge.Status}", new object[1] { 1 })
        }
        .ToList();

    public override void OnCombatStart(State state, Combat combat)
    {
        Pulse();
        state.ship.Add(Elestrals.Instance.WeakenCharge.Status, 1);
    }
}
