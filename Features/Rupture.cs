using FMOD.Studio;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;
using System.Reflection;

namespace JyGein.Elestrals.Features;

public class RuptureManager
{
    private static Elestrals Instance => Elestrals.Instance;
    private static IModData ModData => Elestrals.Instance.Helper.ModData;

    internal static readonly string NaturalKey = "IsntNatural";
    internal static ISpriteEntry RuptureArrowIcon { get; private set; } = null!;
    internal static ISpriteEntry RuptureOffsetLeftArrowIcon { get; private set; } = null!;
    internal static ISpriteEntry RuptureOffsetRightArrowIcon { get; private set; } = null!;
    public static void SetNatural(StuffBase thing, bool IsNatural)
    {
        ModData.SetModData(thing, NaturalKey, !IsNatural);
    }

    public static bool IsNatural(StuffBase thing)
    {
        return (ModData.TryGetModData<bool>(thing, NaturalKey, out var data) && data);
    }

    public static void ApplyPatches(Harmony harmony)
    {
        RuptureOffsetLeftArrowIcon = Elestrals.Instance.Helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/icons/ruptureOffsetLeft.png"));
        RuptureArrowIcon = Elestrals.Instance.Helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/icons/rupture.png"));
        RuptureOffsetRightArrowIcon = Elestrals.Instance.Helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/icons/ruptureOffsetRight.png"));

        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ASpawn), nameof(ASpawn.Begin)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ASpawn_Begin_Postfix))
        );
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.RenderAction)),
            prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_RenderAction_Prefix))
        );
    }

    private static void ASpawn_Begin_Postfix(ASpawn __instance, Combat c)
    {
        if(c.turn>0)
        {
            RuptureManager.SetNatural(__instance.thing, false);
        }
    }
    private static bool Card_RenderAction_Prefix(G g, State state, CardAction action, bool dontDraw, ref int __result)
    {
        if (action is not ARupture ruptureAction)
            return true;

        var box = g.Push();

        if (!dontDraw && ruptureAction.ruptureType != ARupture.RuptureType.All)
            Draw.Sprite(ruptureAction.offset switch
            {
                < 0 => RuptureOffsetLeftArrowIcon.Sprite,
                > 0 => RuptureOffsetRightArrowIcon.Sprite,
                _ => RuptureArrowIcon.Sprite
            }, box.rect.x + __result, box.rect.y, color: action.disabled ? Colors.disabledIconTint : Colors.white);
        if (ruptureAction.ruptureType != ARupture.RuptureType.All) { __result += 8; }

        if (ruptureAction.offset != 0 && ruptureAction.ruptureType != ARupture.RuptureType.All)
        {
            __result += 2;

            if (!dontDraw)
                BigNumbers.Render(Math.Abs(ruptureAction.offset), box.rect.x + __result, box.rect.y, color: action.disabled ? new Color("4B4B4B") : new Color("FF0000"));
            __result += Math.Abs(ruptureAction.offset).ToString().Length * 6;
        }

        if (ruptureAction.ruptureType != ARupture.RuptureType.All) { __result += 2; }

        Icon icon = (Icon) ruptureAction.GetIcon(state)!;
        if (!dontDraw)
            Draw.Sprite(icon.path, box.rect.x + __result, box.rect.y, color: action.disabled ? Colors.disabledIconTint : Colors.white);
        __result += 9;

        g.Pop();

        return false;
    }
}
