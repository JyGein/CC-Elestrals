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

internal sealed class EquilynxRiggsArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry ActiveSprite = null!;
    private static ISpriteEntry InactiveSprite = null!;
    private int count = 0;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

        ActiveSprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Riggs.png"));
        InactiveSprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Riggs_off.png"));

        helper.Content.Artifacts.RegisterArtifact("EquilynxRiggs", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = ActiveSprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Riggs", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Riggs", "description"]).Localize,
		});

		api.RegisterDuoArtifact<EquilynxRiggsArtifact>([Elestrals.Instance.Equilynx_Deck.Deck, Deck.riggs]);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
		count = 0;
    }

    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
		if (thing is EarthStone && count == 0)
		{
			Pulse();
			count += 1;
			combat.QueueImmediate([
				new ADrawCard() {
					count = 1,
					timer = 0
				},
				new AStatus() {
					targetPlayer = true,
					status = Status.evade,
					statusAmount = 1,
					timer = 0.5
                }
			]);
		}
    }

    public override Spr GetSprite()
		=> count == 0 ? ActiveSprite.Sprite : InactiveSprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. new ADrawCard { count = 1 }.GetTooltips(MG.inst.g?.state ?? DB.fakeState),
			.. StatusMeta.GetTooltips(Status.evade, 1)];
}