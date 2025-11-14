using FMOD;
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
using static ASpawn;
using static JyGein.Elestrals.IDynaApi;

namespace JyGein.Elestrals.Artifacts.Duos;

internal sealed class EquilynxIsaacArtifact : Artifact, IElestralsArtifact
{
	private static ISpriteEntry Sprite = null!;

	public static void Register(IModHelper helper)
	{
		if (!Elestrals.Instance.DuoApis.RegisterDuos /*|| Elestrals.Instance.DuoApis.TestApi is not ITestApi testApi*/)
			return;
		IDuoArtifactsApi api = Elestrals.Instance.DuoApis.DuoArtifactsApi!;

		//Sprite = Elestrals.Instance.DefaultDuoArtifactSprite;
        Sprite = helper.Content.Sprites.RegisterSprite(Elestrals.Instance.Package.PackageRoot.GetRelativeFile("assets/artifacts/Duos/Isaac.png"));

		helper.Content.Artifacts.RegisterArtifact("EquilynxIsaac", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = Sprite.Sprite,
			Name = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Isaac", "name"]).Localize,
			Description = Elestrals.Instance.AnyLocalizations.Bind(["artifact", "Duos", "Isaac", "description"]).Localize,
		});

		api.RegisterDuoArtifact<EquilynxIsaacArtifact>([Elestrals.Instance.Equilynx_Deck.Deck, Deck.goat]);

		_ = new EquilynxIsaacArtifactManager();
    }

    public override Spr GetSprite()
		=> Sprite.Sprite;

	public override List<Tooltip> GetExtraTooltips()
		=> [.. new EarthStone().GetTooltips(),
			.. new FlowerStone().GetTooltips(),
			.. new PowerStone().GetTooltips(),
			.. new RepairKit().GetTooltips()];
    internal sealed class EquilynxIsaacArtifactManager
    {
        public EquilynxIsaacArtifactManager()
        {
            Elestrals.Instance.Harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(ASpawn), nameof(ASpawn.Begin)),
                prefix: new HarmonyMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(ASpawn_Begin_Prefix))
            );
        }

        private static void ASpawn_Begin_Prefix(ASpawn __instance, G g, State s, Combat c)
        {
            if (!s.EnumerateAllArtifacts().Any(a => a is EquilynxIsaacArtifact)) return;
            EquilynxIsaacArtifact artifact = (s.EnumerateAllArtifacts().First(a => a is EquilynxIsaacArtifact) as EquilynxIsaacArtifact)!;
            State s2 = s;
            Ship ship = (__instance.fromPlayer ? s2.ship : c.otherShip);
            if (!__instance.fromX.HasValue && ship.parts.FindIndex((Part p) => p.type == PType.missiles && p.active) == -1)
            {
                return;
            }

            if (__instance.fromPlayer && g.state.ship.GetPartTypeCount(PType.missiles) > 1 && !__instance.multiBayVolley)
            {
                return;
            }

            StuffBase launchedThing = __instance.thing;

            if (!(launchedThing is EarthStone || launchedThing is FlowerStone || launchedThing is PowerStone || launchedThing is RepairKit || launchedThing is MiniRepairKit)) return;

            int worldX = __instance.GetWorldX(s, c);

            if (c.stuff.TryGetValue(worldX, out StuffBase? existingThing))
            {
                if (!(existingThing is EarthStone || existingThing is RepairKit || existingThing is PowerStone || existingThing is MiniRepairKit))
                {
                    artifact.Pulse();
                    existingThing.bubbleShield = true;
                    Audio.Play(new GUID?(FSPRO.Event.Status_PowerUp));
                }
            }
        }
    }
}
