using JyGein.Elestrals.Cards;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JyGein.Elestrals.Actions;
using HarmonyLib;
using JyGein.Elestrals.Features;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using System.Reflection.Emit;
using System;
using System.Collections;

namespace JyGein.Elestrals.Artifacts;

internal sealed class EquilynxEmpoweredMunitionsArtifact : Artifact, IElestralsArtifact
{
    private static Elestrals Instance => Elestrals.Instance;
    public static void Register(IModHelper helper)
    {
        helper.Content.Artifacts.RegisterArtifact("EmpoweredMunitions", new()
        {
            ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                owner = Elestrals.Instance.Equilynx_Deck.Deck,
                pools = [ArtifactPool.Common]
            },
            Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/empoweredmunitions.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "EmpoweredMunitions", "name"]).Localize,
            Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "EmpoweredMunitions", "description"]).Localize
        });
    }

    public override List<Tooltip>? GetExtraTooltips()
        => new ARupture() { ruptureType = ARupture.RuptureType.Cannon }.GetTooltips(DB.fakeState);
}

internal sealed class EmpoweredMunitionsManager
{
    public static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Prefix))
        );
    }

    private static bool AAttack_Begin_Prefix(G g, State s, Combat c, AAttack __instance)
    {
        if (__instance.HasRuptured())
        {
            return true;
        }
        if (__instance.targetPlayer) return true;
        Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
        Ship ship2 = (__instance.targetPlayer ? c.otherShip : s.ship);
        if (ship == null || ship2 == null || ship.hull <= 0 || (__instance.fromDroneX.HasValue && !c.stuff.ContainsKey(__instance.fromDroneX.Value)))
        {
            return true;
        }
        if (!__instance.targetPlayer && !__instance.fromDroneX.HasValue && g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley)
        {
            return true;
        }
        foreach (Artifact item in s.EnumerateAllArtifacts())
        {
            if (item is EquilynxPoisonTippedArrowArtifact)
            {
                item.Pulse();
                var copy = Mutil.DeepCopy(__instance);
                Elestrals.Instance.Helper.ModData.SetModData(copy, "HasRuptured", true);
                c.QueueImmediate(copy);
                c.QueueImmediate(new ARupture
                {
                    ruptureType = ARupture.RuptureType.Cannon
                });
                return false;
            }
        }
        return true;
    }
    private static void AAttack_Begin_RuptureBeforeAttack(AAttack aAttack, State s, Combat c)
    {
        foreach (Artifact item in s.EnumerateAllArtifacts())
        {
            if (item is EquilynxEmpoweredMunitionsArtifact)
            {
                c.QueueImmediate(new ARupture
                {
                    ruptureType = ARupture.RuptureType.Cannon
                });
            }
        }
    }
}

internal static class EmpoweredMunitionsExt
{
    public static bool HasRuptured(this AAttack self)
        => Elestrals.Instance.Helper.ModData.GetModDataOrDefault<bool>(self, "HasRuptured");
}