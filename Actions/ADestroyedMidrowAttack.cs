using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    public class ADestroyedMidrowAttack : CardAction
    {
        public int damage;

        public bool isBeam;

        public int? givesEnergy;

        public Status? status;

        public int statusAmount;

        public Card? cardOnHit;

        public CardDestination destination;

        public int moveEnemy;

        public bool stunEnemy;

        public bool weaken;

        public bool brittle;

        public bool armorize;

        public bool piercing;

        public bool targetPlayer;

        public bool fast;

        public bool paybackAttack;

        public bool multiCannonVolley;

        public int paybackCounter;

        public bool storyFromStrafe;

        public bool storyFromPayback;

        public int fromDroneX;

        public List<CardAction>? onKillActions;

        private int? GetFromX(State s, Combat c)
        {
            int num = (targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (num != -1)
            {
                return num;
            }

            return null;
        }

        public override void Begin(G g, State s, Combat c)
        {
            Ship ship = (targetPlayer ? s.ship : c.otherShip);
            Ship ship2 = (targetPlayer ? c.otherShip : s.ship);
            if (ship == null || ship2 == null || ship.hull <= 0)
            {
                return;
            }

            int? num = GetFromX(s, c);
            RaycastResult raycastResult = CombatUtils.RaycastGlobal(c, ship, fromDrone: true, fromDroneX);
            
            if (raycastResult != null && ApplyAutododge(c, ship, raycastResult))
            {
                return;
            }

            if (!targetPlayer)
            {
                foreach (Artifact item3 in s.EnumerateAllArtifacts())
                {
                    if (item3.OnDroneAttackEnemyMakeItPierce(s, c) == true)
                    {
                        piercing = true;
                        item3.Pulse();
                    }
                }
            }

            if (raycastResult == null)
            {
                timer = 0.0;
                {
                    foreach (Artifact item4 in s.EnumerateAllArtifacts())
                    {
                        item4.OnPlayerAttack(s, c);
                    }

                    return;
                }
            }

            if (raycastResult.hitDrone)
            {
                bool flag2 = c.stuff[raycastResult.worldX].Invincible();
                foreach (Artifact item5 in s.EnumerateAllArtifacts())
                {
                    if (item5.ModifyDroneInvincibility(s, c, c.stuff[raycastResult.worldX]) == true)
                    {
                        flag2 = true;
                        item5.Pulse();
                    }
                }

                if (c.stuff[raycastResult.worldX].bubbleShield && !piercing)
                {
                    c.stuff[raycastResult.worldX].bubbleShield = false;
                }
                else if (flag2)
                {
                    c.QueueImmediate(c.stuff[raycastResult.worldX].GetActionsOnShotWhileInvincible(s, c, !targetPlayer, damage));
                }
                else
                {
                    c.DestroyDroneAt(s, raycastResult.worldX, !targetPlayer);
                }
            }

            timer = (fast ? 0.2 : 0.4);
            DamageDone dmg = new DamageDone();
            if (raycastResult.hitShip)
            {
                dmg = (isBeam ? new DamageDone
                {
                    hitHull = true,
                    hitShield = false,
                    poppedShield = false
                } : ship.NormalDamage(s, c, damage, raycastResult.worldX, piercing));
                Part? partAtWorldX = ship.GetPartAtWorldX(raycastResult.worldX);
                if (partAtWorldX != null && partAtWorldX.stunModifier == PStunMod.stunnable)
                {
                    stunEnemy = true;
                }

                if (!isBeam && (ship.Get(Status.payback) > 0 || ship.Get(Status.tempPayback) > 0) && paybackCounter < 100)
                {
                    c.QueueImmediate(new ADestroyedMidrowAttack
                    {
                        paybackCounter = paybackCounter + 1,
                        damage = Card.GetActualDamage(s, ship.Get(Status.payback) + ship.Get(Status.tempPayback), !targetPlayer),
                        targetPlayer = !targetPlayer,
                        fast = true,
                        storyFromPayback = true
                    });
                }

                if (moveEnemy != 0)
                {
                    c.QueueImmediate(new AMove
                    {
                        dir = moveEnemy,
                        targetPlayer = targetPlayer
                    });
                }

                if (status.HasValue)
                {
                    c.QueueImmediate(new AStatus
                    {
                        status = status.Value,
                        statusAmount = statusAmount,
                        targetPlayer = targetPlayer
                    });
                }

                if (givesEnergy.HasValue && targetPlayer)
                {
                    c.QueueImmediate(new AEnergy
                    {
                        changeAmount = 1
                    });
                }

                if (cardOnHit != null)
                {
                    c.QueueImmediate(new AAddCard
                    {
                        card = Mutil.DeepCopy(cardOnHit),
                        destination = destination
                    });
                }

                if (stunEnemy)
                {
                    c.QueueImmediate(new AStunPart
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (weaken)
                {
                    c.QueueImmediate(new AWeaken
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (brittle)
                {
                    c.QueueImmediate(new ABrittle
                    {
                        worldX = raycastResult.worldX
                    });
                }

                if (ship.Get(Status.reflexiveCoating) >= 1)
                {
                    c.QueueImmediate(new AArmor
                    {
                        worldX = raycastResult.worldX,
                        targetPlayer = targetPlayer,
                        justTheActiveOverride = true
                    });
                }

                if (armorize)
                {
                    c.QueueImmediate(new AArmor
                    {
                        worldX = raycastResult.worldX,
                        targetPlayer = targetPlayer
                    });
                }

                if (!isBeam)
                {
                    if (targetPlayer)
                    {
                        if (!raycastResult.hitShip && !raycastResult.hitDrone)
                        {
                            g.state.storyVars.enemyShotJustMissed = true;
                        }

                        if (raycastResult.hitShip)
                        {
                            g.state.storyVars.enemyShotJustHit = true;
                        }

                        foreach (Artifact item6 in s.EnumerateAllArtifacts())
                        {
                            item6.OnEnemyAttack(s, c);
                        }

                        if (!raycastResult.hitShip && !raycastResult.hitDrone)
                        {
                            foreach (Artifact item7 in s.EnumerateAllArtifacts())
                            {
                                item7.OnPlayerDodgeHit(s, c);
                            }
                        }
                    }
                    else
                    {
                        if (raycastResult.hitDrone)
                        {
                            g.state.storyVars.playerJustShotAMidrowObject = true;
                        }

                        if (!raycastResult.hitShip && !raycastResult.hitDrone)
                        {
                            g.state.storyVars.playerShotJustMissed = true;
                        }
                        else
                        {
                            g.state.storyVars.playerShotJustMissed = false;
                        }

                        if (raycastResult.hitShip)
                        {
                            g.state.storyVars.playerShotJustHit = true;
                        }

                        g.state.storyVars.playerShotWasFromStrafe = storyFromStrafe;
                        g.state.storyVars.playerShotWasFromPayback = storyFromPayback;

                        if (raycastResult.hitShip)
                        {
                            /*if (c.otherShip.ai != null)
                            {
                                c.otherShip.ai.OnHitByAttack(s, c, raycastResult.worldX, this);
                            } this might be necessary commenting out for now*/

                            foreach (Artifact item9 in s.EnumerateAllArtifacts())
                            {
                                item9.OnEnemyGetHit(s, c, c.otherShip.GetPartAtWorldX(raycastResult.worldX));
                            }
                        }

                        if (!raycastResult.hitShip && !raycastResult.hitDrone)
                        {
                            bool flag3 = false;
                            for (int i = -1; i <= 1; i += 2)
                            {
                                if (CombatUtils.RaycastGlobal(c, ship, fromDrone: true, raycastResult.worldX + i).hitShip)
                                {
                                    flag3 = true;
                                }
                            }

                            if (flag3)
                            {
                                foreach (Artifact item11 in s.EnumerateAllArtifacts())
                                {
                                    item11.OnEnemyDodgePlayerAttackByOneTile(s, c);
                                }
                            }
                        }
                    }

                    if (ship.hull <= 0 && onKillActions != null)
                    {
                        List<CardAction> list = Mutil.DeepCopy(onKillActions);
                        foreach (CardAction item12 in list)
                        {
                            item12.canRunAfterKill = true;
                        }

                        c.QueueImmediate(list);
                    }
                }

                c.stuff.TryGetValue(fromDroneX, out StuffBase? value);
                if (value is AttackDrone attackDrone)
                {
                    attackDrone.pulse = 1.0;
                }

                if (value is EnergyDrone energyDrone)
                {
                    energyDrone.pulse = 1.0;
                }

                if (value is ShieldDrone shieldDrone)
                {
                    shieldDrone.pulse = 1.0;
                }

                if (value is JupiterDrone jupiterDrone)
                {
                    jupiterDrone.pulse = 1.0;
                }

                EffectSpawner.Cannon(g, targetPlayer, raycastResult, dmg, isBeam);
            }
        }

        private bool ApplyAutododge(Combat c, Ship target, RaycastResult ray)
        {
            if (ray.hitShip && !isBeam)
            {
                if (target.Get(Status.autododgeRight) > 0)
                {
                    target.Add(Status.autododgeRight, -1);
                    int dir = ray.worldX - target.x + 1;
                    c.QueueImmediate(new List<CardAction>
                {
                    new AMove
                    {
                        targetPlayer = targetPlayer,
                        dir = dir
                    },
                    this
                });
                    timer = 0.0;
                    return true;
                }

                if (target.Get(Status.autododgeLeft) > 0)
                {
                    target.Add(Status.autododgeLeft, -1);
                    int dir2 = ray.worldX - target.x - target.parts.Count;
                    c.QueueImmediate(new List<CardAction>
                {
                    new AMove
                    {
                        targetPlayer = targetPlayer,
                        dir = dir2
                    },
                    this
                });
                    timer = 0.0;
                    return true;
                }
            }

            return false;
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = new List<Tooltip>();
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

            if (piercing)
            {
                list.Add(new TTGlossary("action.attackPiercing"));
            }
            else
            {
                list.Add(new TTGlossary("action.attack.name"));
            }

            if (status.HasValue)
            {
                list.AddRange(StatusMeta.GetTooltips(status.Value, statusAmount));
            }

            if (stunEnemy || s.ship.Get(Status.stunCharge) > 0)
            {
                list.Add(new TTGlossary("action.stun"));
            }

            if (weaken)
            {
                list.Add(new TTGlossary("parttrait.weak"));
            }

            if (brittle)
            {
                list.Add(new TTGlossary("parttrait.brittle"));
            }

            if (armorize)
            {
                list.Add(new TTGlossary("parttrait.armor"));
            }

            if (moveEnemy < 0)
            {
                list.Add(new TTGlossary("action.moveLeftEnemy", Math.Abs(moveEnemy)));
            }

            if (moveEnemy > 0)
            {
                list.Add(new TTGlossary("action.moveRightEnemy", moveEnemy));
            }

            if (s.route is Combat && !DoWeHaveCannonsThough(s))
            {
                list.Add(new TTGlossary("action.attackFailWarning.desc"));
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

            if (s.route is Combat combat)
            {
                foreach (StuffBase value in combat.stuff.Values)
                {
                    if (value is JupiterDrone)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void DoLibraEffect(Combat c, Ship source)
        {
            c.QueueImmediate(new AStatus
            {
                targetPlayer = !targetPlayer,
                status = Status.tempShield,
                statusAmount = source.Get(Status.libra)
            });
        }

        public override Icon? GetIcon(State s)
        {
            if (DoWeHaveCannonsThough(s))
            {
                return new Icon(piercing ? Spr.icons_attackPiercing : Spr.icons_attack, damage, Colors.redd);
            }

            return new Icon(piercing ? Spr.icons_attackPiercingFail : Spr.icons_attackFail, damage, Colors.attackFail);
        }
    }

}
