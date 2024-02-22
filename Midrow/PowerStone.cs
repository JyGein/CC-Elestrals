using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;

namespace JyGein.Elestrals.Midrow
{
    internal class PowerStone : StuffBase
    {
        public override Spr? GetIcon() => Elestrals.Instance.FlowerStoneIcon.Sprite;

        public override string GetDialogueTag() => "powerStone";

        public override double GetWiggleAmount() => 1.0;

        public override double GetWiggleRate() => 1.0;

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = [
                new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.midrow,
                () => GetIcon()!,
                () => Elestrals.Instance.Localizations.Localize(["midrow", "PowerStone", "name"]),
                () => Elestrals.Instance.Localizations.Localize(["midrow", "PowerStone", "description"])
            )
        ];
            if (this.bubbleShield)
                tooltips.Add((Tooltip)new TTGlossary("midrow.bubbleShield", Array.Empty<object>()));
            return tooltips;
        }

        public override void Render(G g, Vec v)
        {
            this.DrawWithHilight(g, Elestrals.Instance.FlowerStoneSprite.Sprite, v + this.GetOffset(g), Mutil.Rand((double)this.x + 0.1) > 0.5, flipY: this.targetPlayer);
        }

        public override List<CardAction>? GetActionsOnDestroyed(
          State s,
          Combat c,
          bool wasPlayer,
          int worldX)
        {
            List<CardAction> actions = new List<CardAction>();
            actions.Add((CardAction)new AStatus()
            {
                status = Status.overdrive,
                statusAmount = 1,
                targetPlayer = wasPlayer
            });
            return actions;
        }
    }
}
