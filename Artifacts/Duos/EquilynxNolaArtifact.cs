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

internal sealed class EquilynxNolaArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static TwosCompanySudoApi twosCompanySudoApi = null!;
    public int counter = 0;

    public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.twosCompanyApi is not TwosCompanySudoApi _twosCompanyApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		twosCompanySudoApi = _twosCompanyApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Nola.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxNola", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Nola", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Nola", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, twosCompanySudoApi.NolaDeck.Deck]);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        counter = 0;
    }

    public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
    {
		if (card.GetDataWithOverrides(state).cost == 0)
		{
			counter++;
			if (counter == 5) { Pulse(); combat.QueueImmediate(new AStatus() { status = Elestrals.Instance.WeakenCharge.Status, statusAmount = 1, targetPlayer = true }); }
		}
    }

    public override int? GetDisplayNumber(State s)
    {
        if (this.counter != 0)
            return this.counter;
        return null;
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Elestrals.Instance.WeakenCharge.Status, 1)];
}