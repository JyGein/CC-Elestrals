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
using static Nanoray.Shrike.SequenceAnchors;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxRuhigArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static RuhigSudoApi ruhigApi = null!;

    public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.ruhigApi is not RuhigSudoApi _ruhigApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        ruhigApi = _ruhigApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Ruhig.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxRuhig", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Ruhig", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Ruhig", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, ruhigApi.RuhigDeck.Deck]);
    }

    public override void OnCombatStart(State state, Combat combat)
    {
		Pulse();
		combat.QueueImmediate(new AAddCard
		{
			card = new EquilynxAmbrosiaCard { temporaryOverride = true, upgrade = Upgrade.B },
			destination = CardDestination.Deck,
			timer = 0.5
		});
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTCard { card = new EquilynxAmbrosiaCard { temporaryOverride = true, upgrade = Upgrade.B }, showCardTraitTooltips = true }];
}