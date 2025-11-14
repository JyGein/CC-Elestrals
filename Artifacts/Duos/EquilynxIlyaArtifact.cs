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
using static JyGein.Elestrals.ExternalAPI.IKokoroApi.IV2.IStatusLogicApi.IHook;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxIlyaArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static TwosCompanySudoApi twosCompanySudoApi = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.twosCompanyApi is not TwosCompanySudoApi _twosCompanyApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		twosCompanySudoApi = _twosCompanyApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Ilya.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxIlya", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Ilya", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Ilya", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, twosCompanySudoApi.IlyaDeck.Deck]);
		Elestrals.Instance.KokoroApiV2.StatusLogic.RegisterHook(new EquilynxIlyaArtifactManager());
    }

	public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Status.overdrive, 1),
			.. StatusMeta.GetTooltips(Status.heat, 1)];

	internal sealed class EquilynxIlyaArtifactManager : IKokoroApi.IV2.IStatusLogicApi.IHook
    {
        public int ModifyStatusChange(IModifyStatusChangeArgs args)
		{
			if(!args.State.EnumerateAllArtifacts().Any(a => a is EquilynxIlyaArtifact) || args.Status != Status.overdrive || args.OldAmount <= args.NewAmount || args.Ship != args.State.ship ) return args.NewAmount;
			int changeAmt = args.Ship.Get(Status.heat) - args.Ship.heatMin > args.OldAmount - args.NewAmount ? args.OldAmount - args.NewAmount : args.Ship.Get(Status.heat) - args.Ship.heatMin;
			if (changeAmt > 0)
			{
				args.State.EnumerateAllArtifacts().First(a => a is EquilynxIlyaArtifact).Pulse();
				args.Combat.QueueImmediate(new AStatus() { status = Status.heat, statusAmount = -changeAmt, targetPlayer = true });
			}
			return args.NewAmount + changeAmt;
        }
    }
}