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

internal sealed class EquilynxBooksArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Books.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxBooks", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Books", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Books", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, Deck.shard]);
    }

    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
        if (thing is EarthStone)
        {
            combat.QueueImmediate(new AStatus()
            {
                status = Status.shard,
                statusAmount = 1,
                targetPlayer = true
            });
            Pulse();
        }
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. new EarthStone().GetTooltips(),
			.. StatusMeta.GetTooltips(Status.shard, 1)];
}