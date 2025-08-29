using HarmonyLib;
using JyGein.Elestrals;
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

internal sealed class EquilynxDynaArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;
	private static IDynaApi? DynaApi = null;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos || Elestrals.Instance.DuoApis.dynaApi is not IDynaApi dynaApi)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;
		DynaApi = dynaApi;

        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Dyna.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxDyna", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dyna", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Dyna", "description"]).Localize,
		});

		api.RegisterDuoArtifact<EquilynxDynaArtifact>([Elestrals.Instance.Equilynx_Deck.Deck, dynaApi.DynaDeck.Deck]);
        DynaApi?.RegisterHook(new EquilynxDynaArtifactManager(), 0);
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> DynaApi?.MakeFireChargeAction(new FakeCharge()).GetTooltips(MG.inst.g?.state ?? DB.fakeState)?
			.Concat(new EarthStone { StoneType = EarthStone.EarthStoneType.Mini }.GetTooltips())
			.ToList() ?? [.. new EarthStone { StoneType = EarthStone.EarthStoneType.Mini }.GetTooltips()];
}

internal sealed class FakeCharge : IDynaCharge
{
    public double YOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Spr GetIcon(State state)
    {
        throw new NotImplementedException();
    }

    public string Key()
    {
        throw new NotImplementedException();
    }
}

internal sealed class EquilynxDynaArtifactManager : IDynaHook
{
    public void OnChargeSticked(State state, Combat combat, Ship ship, int worldX)
    {
		if (ship == state.ship) return;
		foreach (Artifact artifact in state.EnumerateAllArtifacts())
        {
			if (artifact is EquilynxDynaArtifact) combat.QueueImmediate(new ASpawn()
				{
					thing = new EarthStone()
					{
						StoneType = EarthStone.EarthStoneType.Mini
					},
					fromX = worldX - state.ship.x
				});
        }
    }
}