using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;
using JyGein.Elestrals.ExternalAPI;
using JyGein.Elestrals.Features;
using JyGein.Elestrals.Midrow;
using Microsoft.Xna.Framework.Graphics;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxIsabelleArtifact : Artifact, IElestralsArtifact, IOnMoveArtifact, IRuptureHook
{
    private static ISpriteEntry Sprite = null!;
	private static TwosCompanySudoApi twosCompanySudoApi = null!;
    public static List<ISpriteEntry> LaunchedSprites = null!;
    public static List<ISpriteEntry> AttackedSprites = null!;
    public static List<ISpriteEntry> MovedSprites = null!;
    public static List<ISpriteEntry> RupturedSprites = null!;
    public bool launched = false;
    public bool attacked = false;
    public bool moved = false;
    public bool ruptured = false;
    public bool activated = false;

    public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.twosCompanyApi is not TwosCompanySudoApi _twosCompanyApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		twosCompanySudoApi = _twosCompanyApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        LaunchedSprites = [
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-2.png")), 
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-1.png"))
            ];
        AttackedSprites = [
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-4.png")),
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-3.png"))
            ];
        MovedSprites = [
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-6.png")),
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-5.png"))
            ];
        RupturedSprites = [
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-8.png")),
            helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isabelle-7.png"))
            ];

        Sprite = helper.Content.Sprites.RegisterDynamicSprite("EquilynxIsabelleWickedWaltz", () =>
        {
            List<Spr> sprites = [];
            G g = MG.inst.g;
            if (g.metaRoute == null && g.state.route is Combat c && g.state.EnumerateAllArtifacts().Any(a => a is EquilynxIsabelleArtifact))
            {
                EquilynxIsabelleArtifact artifact = (g.state.EnumerateAllArtifacts().First(a => a is EquilynxIsabelleArtifact) as EquilynxIsabelleArtifact)!;
                sprites.Add(LaunchedSprites[artifact.launched ? 1 : 0].Sprite);
                sprites.Add(AttackedSprites[artifact.attacked ? 1 : 0].Sprite);
                sprites.Add(MovedSprites[artifact.moved ? 1 : 0].Sprite);
                sprites.Add(RupturedSprites[artifact.ruptured ? 1 : 0].Sprite);
            }
            else
            {
                sprites.Add(LaunchedSprites[1].Sprite);
                sprites.Add(AttackedSprites[1].Sprite);
                sprites.Add(MovedSprites[1].Sprite);
                sprites.Add(RupturedSprites[1].Sprite);
            }
            return TextureUtils.CreateTexture(13, 13, () =>
            {
                foreach (Spr spr in sprites)
                {
                    Draw.Sprite(spr, 0, 0);
                }
            });
        });
		helper.Content.Artifacts.RegisterArtifact("EquilynxIsabelle", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Isabelle", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Isabelle", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, twosCompanySudoApi.IsabelleDeck.Deck]);
        //_ = new EquilynxIsabelleArtifactManager();
    }

    public override void OnTurnStart(State state, Combat combat)
    {
        launched = false;
        attacked = false;
        moved = false;
        ruptured = false;
        activated = false;
    }
    public void Movement(int dist, bool targetPlayer, bool fromEvade, Combat c, State s)
    {
        if (targetPlayer) moved = true;
        AttemptActivation(s, c);
    }

    public override void OnPlayerAttack(State state, Combat combat)
    {
        attacked = true;
        AttemptActivation(state, combat);
    }

    public override void OnPlayerSpawnSomething(State state, Combat combat, StuffBase thing)
    {
        launched = true;
        AttemptActivation(state, combat);
    }

    public void OnRupture(State s, Combat c, bool fromPlayer)
    {
        if (fromPlayer) ruptured = true;
        AttemptActivation(s, c);
    }

    public void AttemptActivation(State s, Combat c)
    {
        if (!activated && launched && attacked && moved && ruptured)
        {
            Pulse();
            c.Queue(new AStatus()
            {
                status = Elestrals.Instance.KokoroApiV2.StatusNextTurn.Overdrive,
                statusAmount = 1,
                targetPlayer = true
            });
            activated = true;
        }
    }

    public override Spr GetSprite()
        => Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [new TTGlossary("action.spawn"),
			.. new AAttack().GetTooltips(MG.inst.g?.state ?? DB.fakeState),
			.. new AMove() { targetPlayer = true }.GetTooltips(MG.inst.g?.state ?? DB.fakeState),
			.. new ABayRupture().GetTooltips(MG.inst.g?.state ?? DB.fakeState),
            .. StatusMeta.GetTooltips(Elestrals.Instance.KokoroApiV2.StatusNextTurn.Overdrive, 1)];


    internal sealed class EquilynxIsabelleArtifactManager
    {
        public EquilynxIsabelleArtifactManager()
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(AStatus), nameof(AStatus.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AStatus_Begin_Prefix))
            );
        }

        private static void AStatus_Begin_Prefix(AStatus __instance, State s, Combat c)
        {

        }
    }
}