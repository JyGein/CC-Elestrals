using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Cards.Special;
using JyGein.Elestrals.ExternalAPI;
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

internal sealed class EquilynxJackArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static IJackApi jackApi = null!;

	public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.jackApi is not IJackApi _jackApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        jackApi = _jackApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Jack.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxJack", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jack", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jack", "description"]).Localize,
		});

		EquilyxJackInstantMissileCard.Register(helper);
		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, jackApi.Jack_Deck.Deck]);
    }

    public override void OnReceiveArtifact(State state)
    {
		state.GetCurrentQueue().QueueImmediate(new AAddCard
		{
			card = new EquilyxJackInstantMissileCard(),
			destination = CardDestination.Deck
		});
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTCard { card = new EquilyxJackInstantMissileCard() }];
}