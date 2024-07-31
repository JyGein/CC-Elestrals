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
internal sealed class HyperFocusManager : IStatusLogicHook
{
    public static Elestrals Instance => Elestrals.Instance;
    public HyperFocusManager(Harmony harmony)
    {
        Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.MakeAllActionIcons)),
            transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_MakeAllActionIcons_Transpiler))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActualDamage)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetActualDamage_Postfix))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
            transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_Transpiler))
        );
    }
    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        if (timing != StatusTurnTriggerTiming.TurnStart)
            return false;

        Elestrals.Instance.Helper.ModData.SetModData(ship, "Has Attacked This Turn", false);

        return false;
    }

    private static IEnumerable<CodeInstruction> Card_MakeAllActionIcons_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.LdcI4((int)Status.bubbleJuice)
                )
                .Find(
                    ILMatches.Stloc(4)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_MakeAllActionIcons_MakeAttackLook0))),
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
    private static List<CardAction> Card_MakeAllActionIcons_MakeAttackLook0(List<CardAction> actions, State s)
    {
        if (s.ship.Get(Elestrals.Instance.HyperFocus.Status) < 1) return actions;
        int num = 1;
        for (int i = 0; i < actions.Count; i++)
        {
            CardAction cardAction = actions[i];
            if (cardAction is AAttack aAttack)
            {
                if (num < 1)
                {
                    aAttack.damage = 0;
                }
                num--;
            }
        }
        return actions;
    }

    private static void Card_GetActualDamage_Postfix(ref int __result, State s, bool targetPlayer)
    {
        if (s.route is Combat c)
        {
            Ship ship = (targetPlayer ? c.otherShip : s.ship);
            if (ship.Get(Elestrals.Instance.HyperFocus.Status) < 1) return;
            bool attacked;
            Elestrals.Instance.Helper.ModData.TryGetModData(ship, "Has Attacked This Turn", out attacked);
            if (attacked) __result = 0;
        }
    }

    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldarg(3),
                    ILMatches.Ldloc(0),
                    ILMatches.Ldloc(3)
                )
                .Find(
                    ILMatches.Instruction(OpCodes.Ret)
                )
                .Find(
                    ILMatches.Ldarg(0)
                )
                .ExtractLabels(out IReadOnlySet<Label> labels);
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldarg(0),
                    ILMatches.Ldarg(3),
                    ILMatches.Ldloc(0),
                    ILMatches.Ldloc(3)
                )
                .Find(
                    ILMatches.Instruction(OpCodes.Ret)
                )
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(AAttack_Begin_ItHasAttacked)))
                )
                .AllElements();
        }
        catch (Exception ex)
        {
            Elestrals.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, Elestrals.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
            return instructions;
        }
    }
    private static void AAttack_Begin_ItHasAttacked(AAttack aAttack, Ship ship)
    {
        if (aAttack.fromDroneX.HasValue)
        {
            return;
        }
        if (ship.Get(Elestrals.Instance.HyperFocus.Status) > 0)
        {
            bool attacked;
            Elestrals.Instance.Helper.ModData.TryGetModData(ship, "Has Attacked This Turn", out attacked);
            if (attacked) aAttack.damage = 0;
        }
        Elestrals.Instance.Helper.ModData.SetModData(ship, "Has Attacked This Turn", true);
    }
}