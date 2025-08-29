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

internal sealed class EquilynxKobretteArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static EchoesOfTheFutureSudoApi echoesOfTheFutureApi = null!;

    public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.echoesOfTheFutureApi is not EchoesOfTheFutureSudoApi _echoesOfTheFutureApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        echoesOfTheFutureApi = _echoesOfTheFutureApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Kobrette.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxKobrette", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Kobrette", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Kobrette", "description"]).Localize,
		});

        api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, echoesOfTheFutureApi.KobretteDeck.Deck]);
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        combat.Queue(new AStatus()
        {
            targetPlayer = true,
            status = Elestrals.Instance.EarthStoneDeposit.Status,
            statusAmount = 4,
            timer = 0.5
        });
        Pulse();
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        combat.Queue(new AStatus()
        {
            targetPlayer = true,
            status = Status.lockdown,
            statusAmount = 1,
            timer = 0.5
        });
        Pulse();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Elestrals.Instance.EarthStoneDeposit.Status, 4),
            .. StatusMeta.GetTooltips(Status.lockdown, 1)];
}