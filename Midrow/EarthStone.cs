﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JyGein.Elestrals;
using JyGein.Elestrals.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static System.Net.Mime.MediaTypeNames;

namespace JyGein.Elestrals.Midrow
{
    internal sealed class EarthStone : StuffBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum EarthStoneType
        {
            Mini, Normal, Big
        }

        [JsonProperty]
        public EarthStoneType Type = EarthStoneType.Normal;

        public override Spr? GetIcon() => new Spr?(Spr.icons_drone);

        public override string GetDialogueTag() => "earthStone";

        public override double GetWiggleAmount() => 1.0;

        public override double GetWiggleRate() => 1.0;

        public override bool IsHostile() => this.targetPlayer;

        private int AttackDamage()
        {
            switch (this.Type)
            {
                case EarthStoneType.Mini:
                    return 1;
                case EarthStoneType.Normal:
                    return 2;
                case EarthStoneType.Big:
                    return 3;
            }
            return 2;
        }

        public override List<Tooltip> GetTooltips()
        {
            List<Tooltip> tooltips = [
                new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.midrow,
                () => GetIcon()!,
                () => Elestrals.Instance.Localizations.Localize(["midrow", "EarthStones", Type.ToString(), "name"]),
                () => Elestrals.Instance.Localizations.Localize(["midrow", "EarthStones", Type.ToString(), "description"])
            )
        ];
            if (this.bubbleShield)
                tooltips.Add((Tooltip)new TTGlossary("midrow.bubbleShield", Array.Empty<object>()));
            return tooltips;
        }

        public override void Render(G g, Vec v)
        {
            this.DrawWithHilight(g, Elestrals.Instance.EarthStoneSprite.Sprite, v + this.GetOffset(g), Mutil.Rand((double)this.x + 0.1) > 0.5, flipY: this.targetPlayer);
        }

        public override List<CardAction>? GetActionsOnDestroyed(
          State s,
          Combat c,
          bool wasPlayer,
          int worldX)
        {
            List<CardAction> actions = new List<CardAction>();
            actions.Add((CardAction)new ADestroyedMidrowAttack()
            {
                fromDroneX = this.x,
                targetPlayer = !wasPlayer,
                damage = this.AttackDamage(),
            });
            return actions;
        }
    }
}
