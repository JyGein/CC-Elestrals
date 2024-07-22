using JyGein.Elestrals.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class ARupture : CardAction
    {
        public enum RuptureType
        {
            Cannon,
            Missile,
            All
        }
        public required RuptureType ruptureType;

        public int? fromX;

        public int offset;

        public bool multiBayVolley;

        public bool fromPlayer = true;

        public int GetWorldX(State s, Combat c)
        {
            Ship ship = (fromPlayer ? s.ship : c.otherShip);
            return (fromX + ship.x) ?? (ship.parts.FindIndex((Part p) => p.type == (ruptureType == RuptureType.Cannon ? PType.cannon : PType.missiles) && p.active) + ship.x);
        }

        public override void Begin(G g, State s, Combat c)
        {
            State s2 = s;
            Ship ship = (fromPlayer ? s2.ship : c.otherShip);
            if (ruptureType == RuptureType.All)
            {
                foreach(StuffBase stuff in c.stuff.Values)
                {
                    if (stuff.Invincible())
                    {
                        c.QueueImmediate(stuff.GetActionsOnShotWhileInvincible(s2, c, fromPlayer, 2));
                    }
                    else /*if (RuptureManager.IsNatural(stuff))*/
                    {
                        c.DestroyDroneAt(s, stuff.x, fromPlayer);
                    }
                }
            }

            PType partType = ruptureType == RuptureType.Cannon ? PType.cannon : PType.missiles;

            if (!fromX.HasValue && ship.parts.FindIndex((Part p) => p.type == partType && p.active) == -1)
            {
                return;
            }

            if (fromPlayer && g.state.ship.GetPartTypeCount(partType) > 1 && !multiBayVolley)
            {
                c.QueueImmediate(new AVolleyRupture
                {
                    rupture = Mutil.DeepCopy(this)
                });
                timer = 0.0;
                return;
            }

            int worldX1 = this.GetWorldX(s, c);
            int worldX2 = worldX1;
            int num1 = worldX1 + this.offset;
            Part? partAtWorldX = ship.GetPartAtWorldX(worldX2);
            if (partAtWorldX != null)
                partAtWorldX.pulse = 1.0;
            StuffBase? existingThing;
            if (c.stuff.TryGetValue(num1, out existingThing))
            {
                if (existingThing.Invincible())
                {
                    c.QueueImmediate(existingThing.GetActionsOnShotWhileInvincible(s2, c, fromPlayer, 2));
                }
                else
                {
                    c.DestroyDroneAt(s, existingThing.x, fromPlayer);
                }
            } else
            {
                s.AddShake(0.5);
                c.fx.Add((FX)new AsteroidExplosion()
                {
                    pos = (new Vec((double)(num1 * 16), 60.0) + new Vec(7.5, 4.0))
                });
            }
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = new List<Tooltip>();
            switch (ruptureType) {
                case RuptureType.Cannon:
                    int num = s.ship.x;
                    foreach (Part part in s.ship.parts)
                    {
                        if (part.type == PType.cannon && part.active)
                        {
                            if (s.route is Combat combat && combat.stuff.ContainsKey(num))
                            {
                                combat.stuff[num].hilight = 2;
                            }

                            part.hilight = true;
                        }

                        num++;
                    }

                    if (s.route is Combat combat2)
                    {
                        foreach (StuffBase value in combat2.stuff.Values)
                        {
                            if (value is JupiterDrone)
                            {
                                value.hilight = 2;
                            }
                        }
                    }

                    if (s.route is Combat && !DoWeHaveCannonsThough(s))
                    {
                        list.Add(new TTGlossary("action.attackFailWarning.desc"));
                    }

                    list.Add(new CustomTTGlossary(
                        CustomTTGlossary.GlossaryType.action,
                        () => offset == 0 ? RuptureManager.RuptureArrowIcon.Sprite : offset < 0 ? RuptureManager.RuptureOffsetLeftArrowIcon.Sprite : RuptureManager.RuptureOffsetRightArrowIcon.Sprite,
                        () => Elestrals.Instance.Localizations.Localize(["action", "Rupture", "name"]),
                        () => string.Format(Elestrals.Instance.Localizations.Localize(["action", "Rupture", offset == 0 ? "notOffset" : "offset", "description"]), Math.Abs(offset).ToString(), offset < 0 ? "left" : "right", "cannon")
                    ));

                    break;
                case RuptureType.Missile:
                    List<Tooltip> list2 = new List<Tooltip>();
                    int num2 = s.ship.x;
                    foreach (Part part in s.ship.parts)
                    {
                        if (part.type == PType.missiles && part.active)
                        {
                            if (s.route is Combat combat && combat.stuff.ContainsKey(num2))
                            {
                                combat.stuff[num2].hilight = 2;
                            }

                            part.hilight = true;
                        }

                        num2++;
                    }
                    list.Add(new CustomTTGlossary(
                        CustomTTGlossary.GlossaryType.action,
                        () => offset == 0 ? RuptureManager.RuptureArrowIcon.Sprite : offset < 0 ? RuptureManager.RuptureOffsetLeftArrowIcon.Sprite : RuptureManager.RuptureOffsetRightArrowIcon.Sprite,
                        () => Elestrals.Instance.Localizations.Localize(["action", "Rupture", "name"]),
                        () => string.Format(Elestrals.Instance.Localizations.Localize(["action", "Rupture", offset == 0 ? "notOffset" : "offset", "description"]), Math.Abs(offset).ToString(), offset < 0 ? "left" : "right", "missile bay")
                    ));
                    break;
                default:
                    if (s.route is Combat route)
                    {
                        foreach (StuffBase stuffBase in route.stuff.Values)
                            //if (RuptureManager.IsNatural(stuffBase)) { 
                                stuffBase.hilight = 2;
                            //}
                    }
                    list.Add(new CustomTTGlossary(
                        CustomTTGlossary.GlossaryType.action,
                        () => Elestrals.Instance.RuptureAIcon.Sprite,
                        () => Elestrals.Instance.Localizations.Localize(["action", "Rupture", "name"]),
                        () => Elestrals.Instance.Localizations.Localize(["action", "Rupture", "all", "description"])
                    ));
                    break;
            }
            return list;
        }

        private bool DoWeHaveCannonsThough(State s)
        {
            foreach (Part part in s.ship.parts)
            {
                if (part.type == PType.cannon)
                {
                    return true;
                }
            }

            return false;
        }

        public override Icon? GetIcon(State s)
        {
            switch (ruptureType)
            {
                case RuptureType.Cannon:
                    return new Icon(Elestrals.Instance.RuptureCIcon.Sprite, null, Colors.textMain);
                case RuptureType.Missile:
                    return new Icon(Elestrals.Instance.RuptureMIcon.Sprite, null, Colors.textMain);
                default:
                    return new Icon(Elestrals.Instance.RuptureAIcon.Sprite, null, Colors.textMain);
            }
        }
    }
}
