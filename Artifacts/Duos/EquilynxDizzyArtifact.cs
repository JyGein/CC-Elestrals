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

internal sealed class EquilynxDizzyArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Dizzy.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxDizzy", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dizzy", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dizzy", "description"]).Localize,
		});

		api.RegisterDuoArtifact<EquilynxDizzyArtifact>([Elestrals.Instance.Equilynx_Deck.Deck, Deck.dizzy]);
    }

    public override void OnPlayerDestroyDrone(State state, Combat combat)
    {
		Pulse();
		combat.QueueImmediate([
            new AStatus()
            {
                targetPlayer = true,
                status = Status.maxShield,
                statusAmount = 1,
                timer = 0
            },
            new AStatus()
            {
                targetPlayer = true,
                status = Status.shield,
                statusAmount = 1,
                timer = 0.5
            }
        ]);
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Status.maxShield, 1),
			.. StatusMeta.GetTooltips(Status.shield, 1)];
}