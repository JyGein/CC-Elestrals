using HarmonyLib;
using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Artifacts;
using JyGein.Elestrals.Artifacts.Duos;
using JyGein.Elestrals.Cards;
using JyGein.Elestrals.Cards.Special;
using JyGein.Elestrals.Features;
using JyGein.Elestrals.Jester;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using JyGein.Elestrals.ExternalAPI;

namespace JyGein.Elestrals;

public sealed class Elestrals : SimpleMod
{
    internal static Elestrals Instance { get; private set; } = null!;
    internal Harmony Harmony { get; }
    internal DuoApis DuoApis { get; private set; } = null!;
    public ApiImplementation ApiImplementation { get; }
    internal IKokoroApi KokoroApi { get; }
    internal IKokoroApi.IV2 KokoroApiV2 { get; }
    //internal IEnergyApi EnergyApi { get; }
    internal IJesterApi? JesterApi { get; }
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }
    internal ISpriteEntry Equilynx_Character_DefaultCardBackground { get; }
    internal ISpriteEntry Equilynx_Character_NexusCardBackground { get; }
    internal ISpriteEntry Equilynx_Character_CardFrame { get; }
    internal ISpriteEntry Equilynx_Character_Panel { get; }
    internal ISpriteEntry Equilynx_Character_Neutral_0 { get; }
    internal ISpriteEntry Equilynx_Character_Neutral_1 { get; }
    internal ISpriteEntry Equilynx_Character_Neutral_2 { get; }
    internal ISpriteEntry Equilynx_Character_Neutral_3 { get; }
    internal ISpriteEntry Equilynx_Character_Mini_0 { get; }
    internal ISpriteEntry Equilynx_Character_Squint_0 { get; }
    internal ISpriteEntry Equilynx_Character_Squint_1 { get; }
    internal ISpriteEntry Equilynx_Character_Squint_2 { get; }
    internal ISpriteEntry Equilynx_Character_Squint_3 { get; }
    internal ISpriteEntry Equilynx_Character_Gameover_0 { get; }
    internal IDeckEntry Equilynx_Deck { get; }
    /*internal IShipEntry DemoMod_Ship { get; }*/
    //internal IStatusEntry AutododgeLeftNextTurn { get; }
    internal IStatusEntry WeakenCharge { get; }
    internal IStatusEntry EarthStoneDeposit { get; }
    internal IStatusEntry FlowerStoneDeposit { get; }
    internal IStatusEntry HyperFocus { get; }
    internal ISpriteEntry EarthStoneSprite { get; }
    internal ISpriteEntry MiniEarthStoneSprite { get; }
    internal ISpriteEntry BigEarthStoneSprite { get; }
    internal ISpriteEntry EarthStoneIcon { get; }
    internal ISpriteEntry MiniEarthStoneIcon { get; }
    internal ISpriteEntry BigEarthStoneIcon { get; }
    internal ISpriteEntry FlowerStoneSprite { get; }
    internal ISpriteEntry FlowerStoneIcon { get; }
    internal ISpriteEntry PowerStoneSprite { get; }
    internal ISpriteEntry PowerStoneIcon { get; }
    internal ISpriteEntry MiniRepairKitSprite { get; }
    internal ISpriteEntry MiniRepairKitIcon { get; }
    internal ISpriteEntry RuptureAIcon { get; }
    internal ISpriteEntry RuptureCIcon { get; }
    internal ISpriteEntry RuptureMIcon { get; }
    internal ISpriteEntry BlossomIcon { get; }
    internal ISpriteEntry DefaultDuoArtifactSprite { get; }
    internal ISpriteEntry DefaultInactiveDuoArtifactSprite { get; }

    internal static IReadOnlyList<Type> Equilynx_CommonCard_Types { get; } = [
        typeof(EquilynxEarthStoneCard),
        typeof(EquilynxNexusBlastCard),
        typeof(EquilynxFlowerStoneCard),
        typeof(EquilynxNexusShotCard),
        typeof(EquilynxNexusShiftCard),
        typeof(EquilynxBeatdownCard),
        typeof(EquilynxNexusSwipeCard),
        typeof(EquilynxPowerStoneCard),
        typeof(EquilynxSandstormCard)
    ];
    internal static IReadOnlyList<Type> Equilynx_UncommonCard_Types { get; } = [
        typeof(EquilynxBreakThroughCard),
        typeof(EquilynxEarthquakeCard),
        typeof(EquilynxGoldenAppleofDiscordCard),
        typeof(EquilynxBloomCard),
        typeof(EquilynxScrappyRepairKitCard),
        typeof(EquilynxTheBiggertheBetterCard),
        typeof(EquilynxMudslideCard)
    ];
    internal static IReadOnlyList<Type> Equilynx_RareCard_Types { get; } = [
        typeof(EquilynxAmbrosiaCard),
        typeof(EquilynxEarthStoneDepositCard),
        typeof(EquilynxFlowerStoneDepositCard),
        typeof(EquilynxBlossomCard),
        typeof(EquilynxDemetersAidCard)
    ];

    internal static readonly IReadOnlyList<Type> SpecialCardTypes = [
        typeof(EquilynxExeCard)
    ];

    internal static IEnumerable<Type> Elestrals_AllCard_Types
        => Equilynx_CommonCard_Types
        .Concat(Equilynx_UncommonCard_Types)
        .Concat(Equilynx_RareCard_Types)
        .Concat(SpecialCardTypes);

    /* We'll organize our artifacts the same way: making lists and then feed those to an IEnumerable */
    internal static IReadOnlyList<Type> Equilynx_CommonArtifact_Types { get; } = [
        typeof(EquilynxFoloiForestArtifact),
        typeof(EquilynxPoisonTippedArrowArtifact),
        typeof(EquilynxEmpoweredMunitionsArtifact),
        typeof(EquilynxPoisonedTunicArtifact)
    ];
    internal static IReadOnlyList<Type> Equilynx_BossArtifact_Types { get; } = [
        typeof(EquilynxScytheofDemeterArtifact),
        typeof(EquilynxTeratlasArtifact)
    ];
    internal static IReadOnlyList<Type> Equilynx_DuoArtifact_Types { get; } = [
        typeof(EquilynxDynaArtifact),
        typeof(EquilynxDizzyArtifact),
        typeof(EquilynxRiggsArtifact),
        typeof(EquilynxPeriArtifact),
        typeof(EquilynxIsaacArtifact),
        typeof(EquilynxDrakeArtifact),
        typeof(EquilynxMaxArtifact),
        typeof(EquilynxBooksArtifact),
        typeof(EquilynxCATArtifact),
        typeof(EquilynxAngderArtifact),
        typeof(EquilynxD26Artifact),
        typeof(EquilynxGrunanArtifact),
        typeof(EquilynxKobretteArtifact),
        typeof(EquilynxRandallArtifact),
        typeof(EquilynxDaveArtifact),
        typeof(EquilynxRuhigArtifact),
        typeof(EquilynxJackArtifact),
        typeof(EquilynxCleoArtifact),
        typeof(EquilynxJesterArtifact),
        typeof(EquilynxDestinyArtifact),
        typeof(EquilynxBucketArtifact),
        typeof(EquilynxNolaArtifact),
        typeof(EquilynxIsabelleArtifact),
        typeof(EquilynxIlyaArtifact),
        typeof(EquilynxJostArtifact),
        typeof(EquilynxGaussArtifact),
        typeof(EquilynxSorrelArtifact)
    ];
    internal static IEnumerable<Type> Equilynx_AllArtifact_Types
        => Equilynx_CommonArtifact_Types
        .Concat(Equilynx_BossArtifact_Types);


    public Elestrals(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;

        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!;
        KokoroApiV2 = KokoroApi.V2;
        //EnergyApi = helper.ModRegistry.GetApi<IEnergyApi>("JyGein.Energy")!;
        Harmony = new(package.Manifest.UniqueName);
        ApiImplementation = new ApiImplementation();
        _ = new CardScalingManager();
        _ = new EarthStoneDepositManager();
        _ = new FlowerStoneDepositManager();
        _ = new WeakenChargeManager();
        _ = new EquilynxDialogueManager();
        _ = new HyperFocusManager(Harmony);
        _ = new OnPlayerMoveLogicManager();
        CustomTTGlossary.ApplyPatches(Harmony);
        RuptureManager.ApplyPatches(Harmony);
        WeakenChargeManager.ApplyPatches(Harmony);
        EmpoweredMunitionsManager.ApplyPatches(Harmony);
        RandomDroneMoveLocaleFix.ApplyPatches(Harmony);
        NegativeStatusManager.ApplyPatches(Harmony);

        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        Equilynx_Character_DefaultCardBackground = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/cards/equilynx/defaultcardbackground.png"));
        Equilynx_Character_NexusCardBackground = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/cards/equilynx/NexusTemp.png"));
        Equilynx_Character_CardFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_cardframe.png"));
        Equilynx_Character_Panel = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_panel.png"));
        Equilynx_Character_Neutral_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_neutral_0.png"));
        Equilynx_Character_Neutral_1 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_neutral_1.png"));
        Equilynx_Character_Neutral_2 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_neutral_2.png"));
        Equilynx_Character_Neutral_3 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_neutral_3.png"));
        Equilynx_Character_Mini_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_mini_0.png"));
        Equilynx_Character_Squint_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_squint_0.png"));
        Equilynx_Character_Squint_1 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_squint_1.png"));
        Equilynx_Character_Squint_2 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_squint_2.png"));
        Equilynx_Character_Squint_3 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_squint_3.png"));
        Equilynx_Character_Gameover_0 = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/characters/equilynx/character_gameover_0.png"));

        EarthStoneSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/earthStone.png"));
        MiniEarthStoneSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/earthStoneMini.png"));
        BigEarthStoneSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/earthStoneBig.png"));
        EarthStoneIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/earthStone.png"));
        MiniEarthStoneIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/miniEarthStone.png"));
        BigEarthStoneIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/bigEarthStone.png"));
        FlowerStoneSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/flowerStone.png"));
        FlowerStoneIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/flowerStone.png"));
        PowerStoneSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/powerStone.png"));
        PowerStoneIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/powerStone.png"));
        MiniRepairKitSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/midrow/miniRepairKit.png"));
        MiniRepairKitIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/miniRepairKit.png"));
        RuptureAIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/ruptureA.png"));
        RuptureCIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/ruptureC.png"));
        RuptureMIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/ruptureM.png"));
        BlossomIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/blossom.png"));
        DefaultDuoArtifactSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/DefaultSprite.png"));
        DefaultInactiveDuoArtifactSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/DefaultInactiveSprite.png"));

        Equilynx_Deck = helper.Content.Decks.RegisterDeck("EquilynxDeck", new DeckConfiguration()
        {
            Definition = new DeckDef()
            {
                color = new Color("254d0c"),

                titleColor = new Color("FFFFFF")
            },
            DefaultCardArt = Equilynx_Character_DefaultCardBackground.Sprite,
            BorderSprite = Equilynx_Character_CardFrame.Sprite,

            Name = AnyLocalizations.Bind(["character", "Equilynx", "name"]).Localize,

            ShineColorOverride = (shineColorOverrideArgs) => shineColorOverrideArgs.DefaultShineColor.gain(0.5)
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = Equilynx_Deck.UniqueName,

            LoopTag = "neutral",

            Frames = new[]
            {
                Equilynx_Character_Neutral_0.Sprite,
                Equilynx_Character_Neutral_1.Sprite,
                Equilynx_Character_Neutral_2.Sprite,
                Equilynx_Character_Neutral_3.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = Equilynx_Deck.UniqueName,
            LoopTag = "mini",
            Frames = new[]
            {
                Equilynx_Character_Mini_0.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = Equilynx_Deck.UniqueName,
            LoopTag = "squint",
            Frames = new[]
            {
                Equilynx_Character_Squint_0.Sprite,
                Equilynx_Character_Squint_1.Sprite,
                Equilynx_Character_Squint_2.Sprite,
                Equilynx_Character_Squint_3.Sprite,
            }
        });

        helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
        {
            CharacterType = Equilynx_Deck.UniqueName,
            LoopTag = "gameover",
            Frames = new[]
            {
                Equilynx_Character_Gameover_0.Sprite
            }
        });
        helper.Content.Characters.V2.RegisterPlayableCharacter("Equilynx", new PlayableCharacterConfigurationV2()
        {
            Deck = Equilynx_Deck.Deck,

            Starters = new()
            {
                cards = [
                    new EquilynxEarthStoneCard(),
                    new EquilynxNexusBlastCard()
                ]
            },

            Description = AnyLocalizations.Bind(["character", "Equilynx", "description"]).Localize,

            BorderSprite = Equilynx_Character_Panel.Sprite,
            ExeCardType = typeof(EquilynxExeCard)
        });

        helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties", new SemanticVersion(1, 4, 4))?.RegisterAltStarters(
            deck: Equilynx_Deck.Deck,
            starterDeck: new StarterDeck
            {
                cards = [
                    new EquilynxFlowerStoneCard(),
                    new EquilynxNexusShotCard()
                ]
            }
        );
        foreach (var cardType in Elestrals_AllCard_Types)
            AccessTools.DeclaredMethod(cardType, nameof(IElestralsCard.Register))?.Invoke(null, [helper]);

        foreach (var artifactType in Equilynx_AllArtifact_Types)
            AccessTools.DeclaredMethod(artifactType, nameof(IElestralsArtifact.Register))?.Invoke(null, [helper]);

        EarthStoneDeposit = helper.Content.Statuses.RegisterStatus("EarthStoneDeposit", new()
        {
            Definition = new()
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/earthStoneDeposit.png")).Sprite,
                color = new("254d0c"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "EarthStoneDeposit", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "EarthStoneDeposit", "description"]).Localize
        });
        FlowerStoneDeposit = helper.Content.Statuses.RegisterStatus("FlowerStoneDeposit", new()
        {
            Definition = new()
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/flowerStoneDeposit.png")).Sprite,
                color = new("254d0c"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "FlowerStoneDeposit", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "FlowerStoneDeposit", "description"]).Localize
        });
        WeakenCharge = helper.Content.Statuses.RegisterStatus("WeakenCharge", new()
        {
            Definition = new()
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/weakenCharge.png")).Sprite,
                color = new("ff6666"),
                isGood = true
            },
            Name = AnyLocalizations.Bind(["status", "WeakenCharge", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "WeakenCharge", "description"]).Localize
        });
        HyperFocus = helper.Content.Statuses.RegisterStatus("HyperFocus", new()
        {
            Definition = new()
            {
                icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/icons/hyperFocus.png")).Sprite,
                color = new("ff6666"),
                isGood = false
            },
            Name = AnyLocalizations.Bind(["status", "HyperFocus", "name"]).Localize,
            Description = AnyLocalizations.Bind(["status", "HyperFocus", "description"]).Localize
        });

        IAppleShipyardApi? appleShipyardApi = helper.ModRegistry.GetApi<IAppleShipyardApi>("APurpleApple.Shipyard", new SemanticVersion(2, 0, 0));
        appleShipyardApi?.RegisterActionLooksForPartType(typeof(ABayRupture), PType.missiles);
        appleShipyardApi?.RegisterActionLooksForPartType(typeof(ACannonRupture), PType.cannon);

        JesterApi = helper.ModRegistry.GetApi<IJesterApi>("rft.Jester");
        JesterApi?.RegisterCharacterFlag("midrow", Equilynx_Deck.Deck);
        JesterApi?.RegisterCharacterFlag("destroyPositive", Equilynx_Deck.Deck);
        JesterApi?.RegisterProvider(new EquilynxJesterProvider());

        helper.Events.OnModLoadPhaseFinished += (_, phase) =>
        {
            if (phase != ModLoadPhase.AfterDbInit)
                return;
            DuoApis = new(helper);
            if (!DuoApis.RegisterDuos)
                return;

            foreach (Type type in Equilynx_DuoArtifact_Types)
                AccessTools.DeclaredMethod(type, nameof(IElestralsArtifact.Register))?.Invoke(null, [helper]);
        };
    }

    public override object? GetApi(IModManifest requestingMod)
        => ApiImplementation;
}
