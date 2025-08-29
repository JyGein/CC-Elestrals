using FMOD;
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

internal sealed class EquilynxRandallArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static RandallSudoApi randallApi = null!;

    public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.randallApi is not RandallSudoApi _randallApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		randallApi = _randallApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Randall.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxRandall", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Randall", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Randall", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, randallApi.RandallDeck.Deck]);
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        List<Card> cards = [.. state.deck, .. combat.hand, .. combat.discard, .. combat.exhausted];
        IKeyAndTokensBoundLocalizationProvider keyAndTokensBoundLocalizationProvider = Elestrals.Instance.AnyLocalizations.Bind(["card", "Nexus"]);
		string nexusLocalized = keyAndTokensBoundLocalizationProvider.Localize(DB.currentLocale.locale) ?? keyAndTokensBoundLocalizationProvider.Localize("en")!;
		bool flag = false;
        foreach (Card card in cards)
		{
            if (card.GetLocName().Contains(nexusLocalized))
			{
                Elestrals.Instance.Helper.Content.Cards.SetCardTraitOverride(state, card, randallApi.Synergy, true, permanent: false);
				flag = true;
            }
		}
		if (flag) Pulse();
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
        IKeyAndTokensBoundLocalizationProvider keyAndTokensBoundLocalizationProvider = Elestrals.Instance.AnyLocalizations.Bind(["card", "Nexus"]);
        string nexusLocalized = keyAndTokensBoundLocalizationProvider.Localize(DB.currentLocale.locale) ?? keyAndTokensBoundLocalizationProvider.Localize("en")!;
        if (card.GetLocName().Contains(nexusLocalized))
        {
            Elestrals.Instance.Helper.Content.Cards.SetCardTraitOverride(state, card, randallApi.Synergy, true, permanent: false);
			Pulse();
        }
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. randallApi.Synergy.Configuration.Tooltips?.Invoke(MG.inst.g?.state ?? DB.fakeState, null) ?? []];
}