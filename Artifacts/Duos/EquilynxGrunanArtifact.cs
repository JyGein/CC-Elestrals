using HarmonyLib;
using JyGein.Elestrals;
using JyGein.Elestrals.Cards;
using JyGein.Elestrals.Midrow;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxGrunanArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
    private static EchoesOfTheFutureSudoApi echoesOfTheFutureApi = null!;

    public static void Register(IModHelper helper)
    {
        if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.echoesOfTheFutureApi is not EchoesOfTheFutureSudoApi _echoesOfTheFutureApi)
            return;
        IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
        echoesOfTheFutureApi = _echoesOfTheFutureApi;

        //Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Grunan.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxGrunan", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Grunan", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Grunan", "description"]).Localize,
		});

        api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [Elestrals.Instance.Equilynx_Deck.Deck, echoesOfTheFutureApi.GrunanDeck.Deck]);
        _ = new EquilynxGrunanArtifactManager();
    }

    public override void OnCombatStart(State state, Combat combat)
    {
        Card Fireball = (echoesOfTheFutureApi.Fireball.Configuration.CardType.CreateInstance() as Card)!;
        Fireball.discount -= 1;
        combat.QueueImmediate(new AAddCard
        {
            card = Fireball,
            destination = CardDestination.Hand
        });
        Pulse();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
	{
		Card Fireball = (echoesOfTheFutureApi.Fireball.Configuration.CardType.CreateInstance() as Card)!;
		Fireball.discount -= 1;
        return [new TTCard { card = Fireball, showCardTraitTooltips = true }];
    }
    internal sealed class EquilynxGrunanArtifactManager
    {
        public EquilynxGrunanArtifactManager()
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(Elestrals.Instance.DuoApis.echoesOfTheFutureApi!.Fireball.Configuration.CardType, nameof(Card.GetActions)),
                postfix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Fireball_GetActions_Postfix))
            );
        }

        private static void Fireball_GetActions_Postfix(ref List<CardAction> __result, State s, Combat c)
        {
            if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxGrunanArtifact)) return;
            __result.Add(new AStatus
            {
                status = Status.overdrive,
                statusAmount = -1,
                targetPlayer = true
            });
        }
    }
}
