using FSPRO;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Features;

internal class PatchLogic
{
    public static void MoveBegin(AMove __instance, State s, Combat c, out int __state)
    {
        __state = __instance.targetPlayer ? s.ship.x : c.otherShip.x;
        bool flag = FeatureFlags.Debug && Input.shift;
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        if (!flag && ship == s.ship && (Enumerable.Any<TrashAnchor>(Enumerable.OfType<TrashAnchor>(c.hand)) ||
            ship.Get(Status.lockdown) > 0 || ship.Get(Status.engineStall) > 0))
            return;

        return;
    }
    public static void MoveEnd(AMove __instance, State s, Combat c, int __state)
    {
        if (__instance.dir == 0 && __instance.ignoreHermes)
            return;

        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        int dist = (__instance.targetPlayer ? s.ship.x : c.otherShip.x) - __state;
        if (dist != 0)
        {
            if (__instance.targetPlayer)
                foreach (Artifact enumerateAllArtifact in s.EnumerateAllArtifacts())
                    if (enumerateAllArtifact is IOnMoveArtifact moveArtifact)
                        moveArtifact.Movement(dist, __instance.targetPlayer, __instance.fromEvade, c, s);
        }
        return;
    }
}
internal interface IOnMoveArtifact
{
    void Movement(int dist, bool targetPlayer, bool fromEvade, Combat c, State s);
}

internal class OnPlayerMoveLogicManager
{
    public OnPlayerMoveLogicManager()
    {
        Elestrals.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AMove), nameof(AMove.Begin)),
            prefix: new HarmonyMethod(typeof(PatchLogic), nameof(PatchLogic.MoveBegin)),
            postfix: new HarmonyMethod(typeof(PatchLogic), nameof(PatchLogic.MoveEnd))
        );
    }
}
