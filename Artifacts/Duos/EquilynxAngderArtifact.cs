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

internal sealed class EquilynxAngderArtifact : Artifact, IElestralsArtifact
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
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Angder.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxAngder", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Angder", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Angder", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, echoesOfTheFutureApi.AngderDeck.Deck]);
		_ = new EquilynxAngderArtifactManager();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(echoesOfTheFutureApi.AngderIsMissing.Status, 1),
			.. new ADroneMove().GetTooltips(MG.inst.g?.state ?? DB.fakeState)];
}

internal sealed class EquilynxAngderArtifactManager
{
	public EquilynxAngderArtifactManager()
	{
		Elestrals.Instance.Harmony.Patch(
			original: AccessTools.DeclaredMethod(typeof(ADroneMove), nameof(ADroneMove.Begin)),
			postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ADroneMove_Begin_Postfix))
		);
	}

	private static void ADroneMove_Begin_Postfix(ADroneMove __instance, G g, State s, Combat c)
	{
		if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxAngderArtifact) || __instance.dir == 0 || s.ship.Get(Elestrals.Instance.DuoApis.echoesOfTheFutureApi!.AngderIsMissing.Status) < 1) return;
		c.QueueImmediate(new AHurt
		{
			hurtAmount = 1,
			targetPlayer = false,
			hurtShieldsFirst = false
		});
	}
}