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
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxBucketArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static IBucketApi bucketApi = null!;
	private int count = 0;
	private int damage = 0;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.bucketApi is not IBucketApi _bucketApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		bucketApi = _bucketApi;

		Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        //Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Test.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxBucket", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Bucket", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Bucket", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, bucketApi.BucketDeck]);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count = 0;
		damage = 0;
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
		count++;
		if (combat.cardActions[0] is AAttack aAttack)
		{
			damage = aAttack.damage;
		}
    }

    public override void OnTurnEnd(State state, Combat combat)
    {
        if (count == 1)
		{
			Pulse();
			combat.Queue(new AStatus
			{
				status = Elestrals.Instance.KokoroApiV2.RedrawStatus.Status,
				statusAmount = damage > 8 ? 10 : 2,
                targetPlayer = true,
				timer = 0.5
			});
		}
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Elestrals.Instance.KokoroApiV2.RedrawStatus.Status, 2)];
}