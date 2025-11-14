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

internal sealed class EquilynxJostArtifact : Artifact, IElestralsArtifact
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
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Jost.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxJost", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jost", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Jost", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, twosCompanySudoApi.JostDeck.Deck]);
        Elestrals.Instance.KokoroApiV2.StatusLogic.RegisterHook(new EquilynxJostArtifactManager());
    }

    public override Spr GetSprite()
        => Sprite.Sprite;

    public override List<Tooltip> GetExtraTooltips()
        => [.. StatusMeta.GetTooltips(Status.overdrive, 1),
            .. StatusMeta.GetTooltips(Status.shield, 1)];

    internal sealed class EquilynxJostArtifactManager : IKokoroApi.IV2.IStatusLogicApi.IHook
    {
        public int ModifyStatusChange(IModifyStatusChangeArgs args)
        {
            if (!args.State.EnumerateAllArtifacts().Any(a => a is EquilynxJostArtifact) || !(args.Status == Status.overdrive || args.Status == Status.shield) || args.OldAmount <= args.NewAmount || args.Ship != args.State.ship) return args.NewAmount;
            args.State.EnumerateAllArtifacts().First(a => a is EquilynxJostArtifact).Pulse();
            args.Combat.QueueImmediate(new AStatus() { status = args.Status == Status.overdrive ? Status.shield : Status.overdrive, statusAmount = 1, targetPlayer = true });
            return args.NewAmount;
        }
    }
}