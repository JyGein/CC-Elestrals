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

internal sealed class EquilynxPeriArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Peri.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxPeri", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Peri", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Peri", "description"]).Localize,
		});

		api.RegisterDuoArtifact<EquilynxPeriArtifact>([Elestrals.Instance.Equilynx_Deck.Deck, Deck.peri]);
    }

	public override void OnCombatStart(State state, Combat combat)
	{
		combat.Queue(new AStatus() {
			targetPlayer = true,
			status = Status.powerdrive,
			statusAmount = 2,
			timer = 0.5
		});
        Pulse();
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        combat.Queue(new AStatus()
        {
            targetPlayer = true,
            status = Status.overdrive,
            statusAmount = -1,
            timer = 0.5
        });
        Pulse();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Status.powerdrive, 2),
            .. StatusMeta.GetTooltips(Status.overdrive, -1)];
}