using HarmonyLib;
using JyGein.Elestrals.Midrow;
using Microsoft.Extensions.Logging;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace JyGein.Elestrals;
internal sealed class WeakenChargeManager : IStatusLogicHook, IStatusRenderHook
{
    public static Elestrals Instance => Elestrals.Instance;
    public WeakenChargeManager()
    {
        /* We task Kokoro with the job to register our status into the game */
        Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
        Instance.KokoroApi.RegisterStatusRenderHook(this, 0);
    }

    public static void ApplyPatches(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Transpiler))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.GetTooltips)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_GetTooltips_Postfix))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.MakeAllActionIcons)),
            transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_MakeAllActionIcons_Transpiler))
        );
    }

    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldfld("stunEnemy"),
                    ILMatches.Brtrue
                )
                .Find(
                    ILMatches.LdcI4(1),
                    ILMatches.Stfld("stunEnemy"),
                    ILMatches.Ldarg(0)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_MakeAttackWeaken))),
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .AllElements();
        }
        catch (Exception ex)
        {
            Elestrals.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, Elestrals.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
            return instructions;
        }
    }
    private static void AAttack_Begin_MakeAttackWeaken(AAttack attack, Ship ship)
    {
        if (!attack.brittle && !attack.weaken && !attack.armorize && ship.Get(Elestrals.Instance.WeakenCharge.Status) > 0 && !attack.fromDroneX.HasValue)
        {
            ship.Set(Elestrals.Instance.WeakenCharge.Status, ship.Get(Elestrals.Instance.WeakenCharge.Status) - 1);
            attack.weaken = true;
        }
    }

    private static void AAttack_GetTooltips_Postfix(AAttack __instance, State s, ref List<Tooltip> __result)
    {
        if(s.ship.Get(Elestrals.Instance.WeakenCharge.Status) > 0) __result.Add(new TTGlossary("parttrait.weak"));
    }


    private static IEnumerable<CodeInstruction> Card_MakeAllActionIcons_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.LdcI4((int) Status.bubbleJuice)
                )
                .Find(
                    ILMatches.Stloc(4)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_MakeAllActionIcons_MakeAttacksLookWeaken))),
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .AllElements();
        }
        catch (Exception ex)
        {
            Elestrals.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, Elestrals.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
            return instructions;
        }
    }

    private static List<CardAction> Card_MakeAllActionIcons_MakeAttacksLookWeaken(Card card, List<CardAction> actions, State s)
    {
        int num = s.ship.Get(Elestrals.Instance.WeakenCharge.Status);
        for (int i = 0; i < actions.Count; i++)
        {
            CardAction cardAction = actions[i];
            if (cardAction is AAttack aAttack && num > 0 && !aAttack.weaken && !aAttack.brittle && !aAttack.armorize)
            {
                aAttack.weaken = true;
                num--;
            }
        }
        return actions;
    }

    public List<Tooltip> OverrideStatusTooltips(Status status, int amount, Ship? ship, List<Tooltip> tooltips)
        => status == Elestrals.Instance.WeakenCharge.Status ? tooltips.Concat(new List<Tooltip>([(Tooltip)new TTGlossary("parttrait.weak")])).ToList() : tooltips;
}