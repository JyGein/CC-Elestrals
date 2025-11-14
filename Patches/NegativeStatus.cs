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
    }
    private static void Ship_CanBeNegative_Postfix(Status status, ref bool __result)
    {
        if (status == Status.overdrive)
            __result = true;
        if (status == Elestrals.Instance.KokoroApiV2.DriveStatus.Pulsedrive)
            __result = true;
    }
}