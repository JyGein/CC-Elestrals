using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;
using Nickel;

namespace JyGein.Elestrals.Midrow
{
    internal class FlowerStone : StuffBase
    {
        public override Spr? GetIcon() => Elestrals.Instance.FlowerStoneIcon.Sprite;

        public override string GetDialogueTag() => "flowerStone";

        public override double GetWiggleAmount() => 1.0;

        public override double GetWiggleRate() => 1.0;

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = [
                new GlossaryTooltip($"{Elestrals.Instance.Package.Manifest.UniqueName}::{GetType()}")
                {
                    Icon = GetIcon()!,
                    Title = Elestrals.Instance.Localizations.Localize(["midrow", "FlowerStone", "name"]),
                    Description = Elestrals.Instance.Localizations.Localize(["midrow", "FlowerStone", "description"])
                }
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
            actions.Add((CardAction)new ADrawCard()
            {
                count = 1
            });
            return actions;
        }
    }
}
