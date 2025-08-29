using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Cards.Special;
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

internal sealed class EquilynxCATArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/CAT.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxCAT", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "CAT", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "CAT", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, Deck.colorless ]);
    }

    public override void OnReceiveArtifact(State state)
    {
        state.GetCurrentQueue().QueueImmediate(new AAddCard()
        {
            card = new EquilynxExeCard(),
            showCardTraitTooltips = true,
        });
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
		if (deck == Elestrals.Instance.Equilynx_Deck.Deck && card.GetDataWithOverrides(state).temporary) card.discount -= 1;
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTCard() { card = new EquilynxExeCard() },
            new TTGlossary("cardtrait.discount")];
}