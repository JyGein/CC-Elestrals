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
	public int count = 0;
	public int damage = 0;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.bucketApi is not IBucketApi _bucketApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		bucketApi = _bucketApi;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Bucket.png"));

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
		_ = new EquilynxBucketArtifactManager(helper);
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        count = 0;
        damage = 0;
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
        count = 0;
        damage = 0;
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Elestrals.Instance.KokoroApiV2.RedrawStatus.Status, 2)];
    internal sealed class EquilynxBucketArtifactManager
    {
        public EquilynxBucketArtifactManager(IModHelper helper)
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Postfix))
            );
        }

        private static void AAttack_Begin_Postfix(AAttack __instance, G g, State s, Combat c)
        {
            if (__instance.multiCannonVolley || __instance.isBeam || __instance.targetPlayer || __instance.fromDroneX.HasValue) return;
            foreach (Artifact a in s.EnumerateAllArtifacts())
            {
                if (a is EquilynxBucketArtifact EBA)
                {
                    EBA.count++;
                    EBA.damage = __instance.damage;
                }
            }
        }
    }
}
