using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxFoloiForestArtifact : Artifact, IElestralsArtifact
{
    public int counter = 0;
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("FoloiForest", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/foloiforest.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "FoloiForest", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "FoloiForest", "description"]).Localize
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new List<Tooltip>
        {
            new TTGlossary($"status.{Elestrals.Instance.OverdriveNextTurn.Status}", new object[1] { 1 })
        }
        .ToList();

    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
        this.counter += 1;
        if (this.counter == 2)
        {
            combat.QueueImmediate(new AStatus()
            {
                status = Elestrals.Instance.OverdriveNextTurn.Status,
                statusAmount = 1,
                targetPlayer = true
            });
            this.Pulse();
        }
    }
    public override void OnTurnStart(State state, Combat combat)
    {
        this.counter = 0;
    }
    public override void OnReceiveArtifact(State state)
    {
        this.counter = 0;
    }
    public override int? GetDisplayNumber(State s)
    {
        if (this.counter != 0)
            return this.counter;
        return null;
    }
}
