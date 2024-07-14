using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxPoisonedTunicArtifact : Artifact, IElestralsArtifact
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("PoisonedTunic", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/poisonedtunic.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "PoisonedTunic", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "PoisonedTunic", "description"]).Localize
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new List<Tooltip>
        {
            new TTGlossary("parttrait.weak"),
            new TTGlossary("parttrait.brittle")
        }
        .ToList();

    public override void OnEnemyGetHit(State state, Combat combat, Part? part)
    {
        if (part == null) return;
        if (part.GetDamageModifier() == PDamMod.weak || part.GetDamageModifier() == PDamMod.brittle)
        {
            this.Pulse();
            combat.otherShip.NormalDamage(state, combat, 1, null);
        }
    }
}
