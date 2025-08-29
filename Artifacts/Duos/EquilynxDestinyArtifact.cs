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
using static JyGein.Elestrals.ExternalAPI.IDestinyApi.IHook;
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxDestinyArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static IDestinyApi destinyApi = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.destinyApi is not IDestinyApi _destinyApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		destinyApi = _destinyApi;

		Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        //Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Test.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxDestiny", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Destiny", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Destiny", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, destinyApi.DestinyDeck.Deck]);
		EquilynxDestinySteadyFocusCard.Register(helper);
		destinyApi.RegisterHook(new EquilynxDestinyArtifactManager());
    }

    public override void OnReceiveArtifact(State state)
    {
        state.GetCurrentQueue().QueueImmediate(new AAddCard
        {
            card = new EquilynxDestinySteadyFocusCard(),
            destination = CardDestination.Deck
        });
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. destinyApi.ExplosiveTrait.Configuration.Tooltips?.Invoke(MG.inst.g?.state ?? DB.fakeState, null) ?? [],
			.. StatusMeta.GetTooltips(Elestrals.Instance.HyperFocus.Status, 1),
			new TTCard { card = new EquilynxDestinySteadyFocusCard() }];
}

public sealed class EquilynxDestinyArtifactManager : IDestinyApi.IHook
{
    public void ModifyExplosiveDamage(IModifyExplosiveDamageArgs args)
	{
		foreach (Artifact a in args.State.EnumerateAllArtifacts())
        {
            if (a is EquilynxDestinyArtifact EDA && args.State.ship.Get(Elestrals.Instance.HyperFocus.Status) > 0)
            {
				args.CurrentDamage += 4;
				EDA.Pulse();
			}
        }
	}
}