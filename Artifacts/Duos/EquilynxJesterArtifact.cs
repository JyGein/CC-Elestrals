using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Features;
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

internal sealed class EquilynxJesterArtifact : Artifact, IElestralsArtifact, IRuptureHook
{
	private static ISpriteEntry Sprite = null!;
	private static JesterSudoApi jesterApi = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.jesterApi is not JesterSudoApi _jesterApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		jesterApi = _jesterApi;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Jester.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxJester", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jester", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jester", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, jesterApi.JesterDeck.Deck]);
    }

	public void OnRuptureMiss(State s, Combat c)
	{
		c.QueueImmediate(new AStatus
		{
			status = Status.energyFragment,
			statusAmount = 1,
			targetPlayer = true
		});
		Pulse();
	}

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. new ABayRupture().GetTooltips(MG.inst.g?.state ?? DB.fakeState),
			.. StatusMeta.GetTooltips(Status.energyFragment, 1)];
}