﻿using Nickel;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts;

internal sealed class DemoArtifactCounting : Artifact, IElestralsArtifact
{
    public int counter = 0;
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("Counting", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Deck.colorless,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/counting.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Counting", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Counting", "description"]).Localize
        });
    }

    public override void OnTurnStart(State s, Combat c)
    {
        if (!c.isPlayerTurn)
            return;
        this.counter += 1;
        if (this.counter == 7)
        {
            c.QueueImmediate(new AEnergy()
            {
                changeAmount = 1
            });
            this.counter = 0;
            this.Pulse();
        }
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
