using HarmonyLib;
using JyGein.Elestrals.ExternalAPI;
using JyGein.Elestrals.Features;
using System.Reflection;

namespace JyGein.Elestrals;

internal sealed class NegativeStatusManager : IStatusLogicHook
{

    public static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.CanBeNegative)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Ship_CanBeNegative_Postfix))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.OnAfterTurn)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Ship_OnAfterTurn_Postfix))
        );
    }
    private static void Ship_CanBeNegative_Postfix(Status status, ref bool __result)
    {
        if (status == Status.overdrive)
            __result = true;
        if (status == Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive)
            __result = true;
    }
    private static void Ship_OnAfterTurn_Postfix(Ship __instance, State s, Combat c)
    {

        if (__instance.Get(Status.timeStop) <= 0)
        {
            if (__instance.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive) < 0)
                c.QueueImmediate((CardAction)new AStatus()
                {
                    status = Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive,
                    statusAmount = __instance.Get(Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive) * -1,
                    targetPlayer = __instance.isPlayerShip
                });
        }
    }
}