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

internal sealed class EquilynxCleoArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static ICleoApi cleoApi = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.cleoApi is not ICleoApi _cleoApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		cleoApi = _cleoApi;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Cleo.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxCleo", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Cleo", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Cleo", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, cleoApi.CleoDeck.Deck]);
		_ = new EquilynxCleoArtifactManager();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Status.overdrive, 1)];
    internal sealed class EquilynxCleoArtifactManager
    {
        public EquilynxCleoArtifactManager()
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AStatus_Begin_Prefix))
            );
        }

        private static void AStatus_Begin_Prefix(AStatus __instance, State s, Combat c)
        {
            if (!(
                c.isPlayerTurn //it's the player's turn
                && //and the status is negative overdrive/pulsedrive or positive underdrive
                (
                    (__instance.status == Status.overdrive || __instance.status == Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive) && __instance.statusAmount <= 0
                    ||
                    (__instance.status == Elestrals.Instance.KokoroApiV2.DriveStatus.Underdrive) && __instance.statusAmount >= 0
                )
            )) return;
            foreach (Artifact a in s.EnumerateAllArtifacts())
            {
                if (a is EquilynxCleoArtifact)
                {
                    a.Pulse();
                    __instance.statusAmount += __instance.status == Elestrals.Instance.KokoroApiV2.DriveStatus.Underdrive ? -1 : 1;
                }
            }
        }
    }
}
