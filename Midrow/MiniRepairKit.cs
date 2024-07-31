using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;

namespace JyGein.Elestrals.Midrow
{
    internal class MiniRepairKit : StuffBase
    {
        private double particlesToEmit;
        public override Spr? GetIcon() => Elestrals.Instance.MiniRepairKitIcon.Sprite;

        public override string GetDialogueTag() => "miniRepairKit";

        public override double GetWiggleAmount() => 1.0;

        public override double GetWiggleRate() => 1.0;

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = [
                new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.midrow,
                () => GetIcon()!,
                () => Elestrals.Instance.Localizations.Localize(["midrow", "MiniRepairKit", "name"]),
                () => Elestrals.Instance.Localizations.Localize(["midrow", "MiniRepairKit", "description"])
            )
        ];
            if (this.bubbleShield)
                tooltips.Add((Tooltip)new TTGlossary("midrow.bubbleShield", Array.Empty<object>()));
            return tooltips;
        }

        public override void Render(G g, Vec v)
        {
            Vec offset = GetOffset(g);
            DrawWithHilight(g, Elestrals.Instance.MiniRepairKitSprite.Sprite, v + offset, Mutil.Rand((double)x + 0.1) > 0.5);
            particlesToEmit += g.dt * 15.0;
            while (particlesToEmit >= 1.0)
            {
                PFX.combatAdd.Add(new Particle
                {
                    color = new Color(0.1, 0.3, 1.0),
                    pos = new Vec(x * 16 + 1, v.y - 24.0) + offset + new Vec(7.5, 7.5) + Mutil.RandVel().normalized() * 6.0,
                    vel = Mutil.RandVel() * 20.0,
                    lifetime = 1.0,
                    size = 1.0 + Mutil.NextRand() * 1.5,
                    dragCoef = 1.0
                });
                particlesToEmit -= 1.0;
            }
        }

        public override List<CardAction>? GetActionsOnDestroyed(
          State s,
          Combat c,
          bool wasPlayer,
          int worldX)
        {
            List<CardAction> actions = new List<CardAction>();
            actions.Add((CardAction)new AHeal()
            {
                healAmount = 1,
                targetPlayer = wasPlayer
            });
            return actions;
        }
    }
}
