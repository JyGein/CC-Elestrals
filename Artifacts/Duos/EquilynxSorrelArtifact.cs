using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.ExternalAPI;
using JyGein.Elestrals.Midrow;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxSorrelArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static TwosCompanySudoApi twosCompanySudoApi = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.twosCompanyApi is not TwosCompanySudoApi _twosCompanyApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		twosCompanySudoApi = _twosCompanyApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Sorrel.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxSorrel", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Sorrel", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Sorrel", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, twosCompanySudoApi.SorrelDeck.Deck]);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
		CardAction ForceEnemyAttack = (CardAction)twosCompanySudoApi.AForceEnemyAttack.CreateInstance();
		ForceEnemyAttack.timer = 0.5;
        combat.Queue([new AStatus()
        {
            targetPlayer = true,
            status = twosCompanySudoApi.BullettimeStatus.Status,
            statusAmount = 1,
            timer = 0.5
        },
		ForceEnemyAttack]);
        Pulse();
    }

	public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
        => [..StatusMeta.GetTooltips(twosCompanySudoApi.BullettimeStatus.Status, 1),
            ..((CardAction) twosCompanySudoApi.AForceEnemyAttack.CreateInstance()).GetTooltips(MG.inst.g?.state ?? DB.fakeState)];
}