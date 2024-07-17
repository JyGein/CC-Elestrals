using HarmonyLib;
using JyGein.Elestrals.Features;
using System.Reflection;

namespace JyGein.Elestrals;

internal sealed class NegativeOverdriveManager : IStatusLogicHook
{
    private static void Ship_CanBeNegative_Postfix(Status status, ref bool __result)
    {
        if (status == Status.overdrive)
            __result = true;
    }

    public static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.CanBeNegative)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Ship_CanBeNegative_Postfix))
        );
    }
}