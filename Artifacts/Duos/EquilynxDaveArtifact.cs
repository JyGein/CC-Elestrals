using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Midrow;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxDaveArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static DaveSudoApi daveApi = null!;

    public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.daveApi is not DaveSudoApi _daveApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        daveApi = _daveApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Dave.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxDave", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dave", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dave", "description"]).Localize,
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, daveApi.DaveDeck.Deck]);
        //_ = new EquilynxDaveArtifactManager();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. StatusMeta.GetTooltips(Status.overdrive, 1)];

    public override int ModifyBaseDamage(int baseDamage, Card? card, State state, Combat? combat, bool fromPlayer)
    {
        if (!fromPlayer) return 0;
        int output = 0;
        if (state.ship.Get(Status.overdrive) < 0) output -= 2 * state.ship.Get(Status.overdrive);
        if (state.ship.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive) < 0) output -= 2 * state.ship.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive);
        if (state.ship.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Underdrive) > 0) output += 2 * state.ship.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Underdrive);
        return output;
    }

    internal sealed class EquilynxDaveArtifactManager
    {
        public EquilynxDaveArtifactManager()
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActualDamage)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetActualDamage_Postfix))
            );
        }

        private static void Card_GetActualDamage_Postfix(int __result, State s)
        {
            if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxDaveArtifact)) return;
            //uhh i realized at this point that i can just use Artifact.ModifyBaseDamage
        }

        private static IEnumerable<CodeInstruction> Card_GetActualDamage_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
        {
            try
            {
                return new SequenceBlockMatcher<CodeInstruction>(instructions)
                    .Find(
                        ILMatches.LdcI4(Status.overdrive),
                        ILMatches.Call(nameof(Ship.Get))
                    )
                    .Insert(
                        SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(MaybeAbsOverdrive)))
                    )
                    .AllElements();
            }
            catch (Exception ex)
            {
                Elestrals.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, Elestrals.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
                return instructions;
            }
        }

        private static int MaybeAbsOverdrive(int overdriveAmount, State s)
        {
            if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxDaveArtifact)) return overdriveAmount;
            return Math.Abs(overdriveAmount);
        }
    }
}