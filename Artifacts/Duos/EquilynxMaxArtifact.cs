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

internal sealed class EquilynxMaxArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Max.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxMax", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Max", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Max", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, Deck.hacker]);

		_ = new EquilynxMaxArtifactManager();
    }

    public override void OnReceiveArtifact(State state) => state.ship.baseDraw -= 4;

    public override void OnRemoveArtifact(State state) => state.ship.baseDraw += 4;

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTGlossary("cardtrait.retain")];
}

internal sealed class EquilynxMaxArtifactManager
{
	public EquilynxMaxArtifactManager()
    {
        Elestrals.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetDataWithOverrides)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetDataWithOverrides_Postfix))
        );
        Elestrals.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Combat), nameof(Combat.GetDrawCount)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Combat_GetDrawCount_Postfix))
        );
    }

	private static void Card_GetDataWithOverrides_Postfix(State state, ref CardData __result)
	{
		if (state.EnumerateAllArtifacts().Any(a => a is EquilynxMaxArtifact)) __result.retain = true;
	}

	private static void Combat_GetDrawCount_Postfix(State s, Combat __instance, ref int __result)
	{
		if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxMaxArtifact)) return;
		if (__instance.hand.Count + __result < 5) __result = 5 - __instance.hand.Count;
	}
}