using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Midrow;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxDrakeArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static ISpriteEntry SpriteOff = null!;
    private int counter = 0;

    public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        //SpriteOff = Elestrals.Instance.DefaultInactiveDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Drake.png"));
        SpriteOff = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Drake_off.png"));

        helper.Content.Artifacts.RegisterArtifact("EquilynxDrake", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Drake", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Drake", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, Deck.eunice]);
    }
    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
        this.counter += 1;
        if (this.counter == 2)
        {
            combat.QueueImmediate(new AStatus()
            {
                status = Status.heat,
                statusAmount = -2,
                targetPlayer = true
            });
            this.Pulse();
        }
    }
    public override int? GetDisplayNumber(State s)
    {
        if (this.counter != 0)
            return this.counter;
        return null;
    }

    public override Spr GetSprite()
		=> counter < 2 ? Sprite.Sprite : SpriteOff.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTGlossary("action.spawn"),
            .. StatusMeta.GetTooltips(Status.heat, -2)];
}