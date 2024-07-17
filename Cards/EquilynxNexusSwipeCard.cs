using HarmonyLib;
using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Microsoft.Xna.Framework;
using Nanoray.PluginManager;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Logging;

namespace JyGein.Elestrals.Cards;

internal sealed class EquilynxNexusSwipeCard : Card, IElestralsCard
{
    private static List<ISpriteEntry> QuadArt = null!;
    private static List<ISpriteEntry> BQuadArt = null!;
    private static List<ISpriteEntry> QuadIcon = null!;

    [JsonProperty]
    public int FlipIndex { get; private set; } = 0;

    [JsonProperty]
    private bool LastFlipped { get; set; }

    public float ActionSpacingScaling
        => 1.5f;

    public Matrix ModifyNonTextCardRenderMatrix(G g, List<CardAction> actions)
    {
        return Matrix.Identity;
    }

    public Matrix ModifyCardActionRenderMatrix(G g, List<CardAction> actions, CardAction action, int actionWidth)
    {
        var spacing = 12 * g.mg.PIX_SCALE;
        var newXOffset = 12 * g.mg.PIX_SCALE;
        var newYOffset = 10 * g.mg.PIX_SCALE;
        var index = actions.IndexOf(action);
        return index switch
        {
            0 => Matrix.CreateTranslation(-newXOffset, -newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
            1 => Matrix.CreateTranslation(newXOffset, -newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
            2 => Matrix.CreateTranslation(newXOffset, newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
            3 => Matrix.CreateTranslation(-newXOffset, newYOffset - (int)((index - actions.Count / 2.0 + 0.5) * spacing), 0),
            _ => Matrix.Identity
        };
    }

    public static void Register(IModHelper helper)
    {
        QuadArt = Enumerable.Range(0, 4)
            .Select(i => helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile($"assets/cards/equilynx/NexusSwipeQuad{i}.png")))
            .ToList();
        BQuadArt = Enumerable.Range(0, 4)
            .Select(i => helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile($"assets/card/equilynx/NexusSwipeBQuad{i}.png")))
            .ToList();
        QuadIcon = Enumerable.Range(0, 4)
            .Select(i => helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile($"assets/icons/Quad{i}.png")))
            .ToList();

        helper.Content.Cards.RegisterCard("NexusSwipe", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Elestrals.Instance.Equilynx_Deck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/Cards/NexusSwipeQuad.png")).Sprite,
            Name = Elestrals.Instance.AnyLocalizations.Bind(["card", "NexusSwipe", "name"]).Localize
        });

        Elestrals.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(Render)),
            transpiler: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_Render_Transpiler))
        );
        Elestrals.Instance.Harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(Card), nameof(GetAllTooltips)),
            postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_GetAllTooltips_Postfix))
        );
    }

    public override CardData GetData(State state)
        => new()
        {
            art = upgrade == Upgrade.B ? BQuadArt[FlipIndex % 4].Sprite : QuadArt[FlipIndex % 4].Sprite,
            cost = upgrade != Upgrade.A ? 1 : 0,
            floppable = true
        };

    public override void ExtraRender(G g, Vec v)
    {
        base.ExtraRender(g, v);
        if (LastFlipped != flipped)
        {
            LastFlipped = flipped;
            FlipIndex = (FlipIndex + 1) % 4;
        }
    }

    public override List<CardAction> GetActions(State s, Combat c)
        => upgrade switch
        {
            Upgrade.B => [
                new ADroneMove
                {
                    dir = -1*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 0,
                },
                new ADroneMove
                {
                    dir = 1*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 1,
                },
                new ADroneMove
                {
                    dir = 2*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 2,
                },
                new ADroneMove
                {
                    dir = -2*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 3,
                },
                new ARupture
                {
                    ruptureType = ARupture.RuptureType.Missile
                }
            ],
            _ => [
                new ADroneMove
                {
                    dir = -1*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 0,
                },
                new ADroneMove
                {
                    dir = 1*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 1,
                },
                new ADroneMove
                {
                    dir = 2*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 2,
                },
                new ADroneMove
                {
                    dir = -2*(FlipIndex % 2 != 0 ? -1 : 1),
                    disabled = FlipIndex % 4 != 3,
                }
            ]
        };

    private static IEnumerable<CodeInstruction> Card_Render_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
    {
        try
        {
            return new SequenceBlockMatcher<CodeInstruction>(instructions)
                .Find(
                    ILMatches.Ldloc<CardData>(originalMethod),
                    ILMatches.Ldfld("floppable")
                )
                .Find(ILMatches.LdcI4((int)Spr.icons_floppable))
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_Render_Transpiler_ReplaceFloppableIcon)))
                )
                .Find(ILMatches.Ldfld("flipped"))
                .Insert(
                    SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Card_Render_Transpiler_ReplaceFlipped)))
                )
                .AllElements();
        }
        catch (Exception ex)
        {
            Elestrals.Instance.Logger.LogError("Could not patch method {Method} - {Mod} probably won't work.\nReason: {Exception}", originalMethod, Elestrals.Instance.Package.Manifest.GetDisplayName(@long: false), ex);
            return instructions;
        }
    }

    private static Spr Card_Render_Transpiler_ReplaceFloppableIcon(Spr sprite, Card card)
    {
        if (card is not EquilynxNexusSwipeCard nexusSwipeCard)
            return sprite;
        return QuadIcon[nexusSwipeCard.FlipIndex % QuadIcon.Count].Sprite;
    }

    private static bool Card_Render_Transpiler_ReplaceFlipped(bool flipped, Card card)
        => card is not EquilynxNexusSwipeCard && flipped;

    private static void Card_GetAllTooltips_Postfix(Card __instance, State s, bool showCardTraits, ref IEnumerable<Tooltip> __result)
    {
        if (!showCardTraits)
            return;
        if (__instance is not EquilynxNexusSwipeCard nexusSwipeCard)
            return;

        __result = __result
            .Select(tooltip =>
            {
                if (tooltip is not TTGlossary glossary || glossary.key != "cardtrait.floppable")
                    return tooltip;

                string buttonText = PlatformIcons.GetPlatform() switch
                {
                    Platform.NX => Loc.T("controller.nx.b"),
                    Platform.PS => Loc.T("controller.ps.circle"),
                    _ => Loc.T("controller.xbox.b"),
                };

                return new GlossaryTooltip("cardtrait.quad")
                {
                    Icon = QuadIcon[0].Sprite,
                    TitleColor = Colors.cardtrait,
                    Title = Elestrals.Instance.Localizations.Localize([
                        "cardTrait",
                        "quad",
                        "name"
                    ]),
                    Description = Elestrals.Instance.Localizations.Localize([
                        "cardTrait",
                        "quad",
                        "description",
                        PlatformIcons.GetPlatform() == Platform.MouseKeyboard ? "m&k" : "controller"
                    ], new { Button = buttonText })
                };
            });
    }
}