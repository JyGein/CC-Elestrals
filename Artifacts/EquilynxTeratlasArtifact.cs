using JyGein.Elestrals.Cards;
using JyGein.Elestrals.Midrow;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxTeratlasArtifact : Artifact, IElestralsArtifact
{
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("Teratlas", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Boss]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/teratlas.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Teratlas", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Teratlas", "description"]).Localize
        });
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        List<StuffBase> objects = combat.stuff.Values.ToList();
        foreach(StuffBase stuff in objects)
        {
            if (stuff is FlowerStone || stuff is EarthStone)
            {
                combat.QueueImmediate(new AEnergy() { changeAmount = 1});
                return;
            }
        }
        if (state.ship.Get(Elestrals.Instance.EarthStoneDeposit.Status) > 0 || state.ship.Get(Elestrals.Instance.FlowerStoneDeposit.Status) > 0)
        {
            combat.QueueImmediate(new AEnergy() { changeAmount = 1 });
            return;
        }
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new List<Tooltip> { }
        .Concat(new EarthStone { }.GetTooltips())
        .Concat(new FlowerStone { }.GetTooltips())
        .ToList();
}
