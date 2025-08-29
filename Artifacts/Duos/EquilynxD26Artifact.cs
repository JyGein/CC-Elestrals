using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Cards;
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

internal sealed class EquilynxD26Artifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static EchoesOfTheFutureSudoApi echoesOfTheFutureApi = null!;
	private int count = 0;

    public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.echoesOfTheFutureApi is not EchoesOfTheFutureSudoApi _echoesOfTheFutureApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        echoesOfTheFutureApi = _echoesOfTheFutureApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/D26.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxD26", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "D26", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "D26", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, echoesOfTheFutureApi.D26Deck.Deck]);
    }

    public override int? GetDisplayNumber(State s)
    {
        return count;
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        if (count >= 2) count = 0;
        count++;
        if (count == 2)
        {
            combat.QueueImmediate(new AAddCard
            {
                card = new EquilynxNexusBlastCard { temporaryOverride = true, exhaustOverride = true },
                destination = CardDestination.Hand
            });
            Pulse();
        }
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTCard() { card = new EquilynxNexusBlastCard { temporaryOverride = true, exhaustOverride = true }, showCardTraitTooltips = true }];
}