using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Microsoft.Extensions.Logging;
using Nickel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Jester;

internal class EquilynxJesterProvider : IJesterApi.IProvider
{
    public IEnumerable<(double, IJesterApi.IEntry)> GetEntries(IJesterApi.IJesterRequest request)
    {
        List<(double, IJesterApi.IEntry)> ProviderList = new List<(double, IJesterApi.IEntry)>();
        var offsets = Elestrals.Instance.JesterApi!.GetJesterUtil().GetDeployOptions(request.OccupiedMidrow);
        if (!Elestrals.Instance.JesterApi!.HasCardFlag("exhaust", request))
        {
            //midrow
            ProviderList.Concat(offsets.SelectMany(o => new List<(double, IJesterApi.IEntry)>
            {
                (0.5, new EarthStoneEntry
                {
                    Offset = o,
                    Type = EarthStone.EarthStoneType.Normal
                }),
                (0.25, new EarthStoneEntry
                {
                    Offset = o,
                    Type = EarthStone.EarthStoneType.Mini
                }),
                (0.25, new EarthStoneEntry
                {
                    Offset = o,
                    Type = EarthStone.EarthStoneType.Big
                }),
                (1.0, new FlowerStoneEntry
                {
                    Offset = o,
                    Shielded = false
                }),
                (1.0, new PowerStoneEntry
                {
                    Offset = o
                })
            }));
            //status
            ProviderList.Add(
                (1.0, new OverdriveNextTurnEntry
                {
                    Amount = 1
                })
            );
            //action
            ProviderList.Add(
                (0.1, new BlossomEntry())
            );
        } else
        {
            //midrow
            ProviderList.Concat(offsets.SelectMany(o => new List<(double, IJesterApi.IEntry)>
            {
                (1.0, new MiniRepairKitEntry
                {
                    Offset = o
                })
            }));
            //status
            ProviderList.Add(
                (1.0, new OverdriveNextTurnEntry
                {
                    Amount = 1
                })
            );
            ProviderList.Add(
                (0.25, new EarthStoneDepositEntry
                {
                    Amount = 1
                })
            );
            ProviderList.Add(
                (0.25, new FlowerStoneDepositEntry
                {
                    Amount = 1
                })
            );
            ProviderList.Add(
                (0.25, new WeakenChargeEntry())
            );
            //action
            ProviderList.Add(
                (0.1, new BlossomEntry())
            );
        }
        if (Elestrals.Instance.JesterApi!.HasCharacterFlag("destroyPositive"))
        {
            if (!Elestrals.Instance.JesterApi!.HasCardFlag("exhaust", request))
            {
                ProviderList.Concat(offsets.SelectMany(o => new List<(double, IJesterApi.IEntry)>
                {
                    (2, new BayRuptureEntry
                    {
                        Offset = o
                    }),
                    (2, new CannonRuptureEntry
                    {
                        Offset = o
                    })
                }));
            } else
            {
                ProviderList.Add(
                    (0.4, new AllRuptureEntry())
                );
            }
        }
        if (Elestrals.Instance.JesterApi!.HasCardFlag("singleUse", request))
        {
            ProviderList.Add(
                (0.1, new HyperFocusEntry())
            );
        }
        return ProviderList;
    }
}

internal class EarthStoneEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }
    [Required] public EarthStone.EarthStoneType Type { get; init; }

    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "offensive",
                "drone",
                "attack"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new EarthStone
                {
                    StoneType = Type
                },
                offset = Offset
            }
        };

    public int GetCost() => (Type == EarthStone.EarthStoneType.Mini ? 6 : Type == EarthStone.EarthStoneType.Normal ? 12 : 20);

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        var options = new List<(double, IJesterApi.IEntry)>();
        if (Type == EarthStone.EarthStoneType.Mini)
            options.Add((1.0, new EarthStoneEntry
            {
                Offset = Offset,
                Type = EarthStone.EarthStoneType.Normal
            }));
        if (Type == EarthStone.EarthStoneType.Normal)
            options.Add((1.0, new EarthStoneEntry
            {
                Offset = Offset,
                Type = EarthStone.EarthStoneType.Big
            }));
        return options;
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("shot");
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class FlowerStoneEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }
    [Required] public bool Shielded { get; init; }

    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "defensive",
                "drone",
                "utility"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new FlowerStone
                {
                    bubbleShield = Shielded
                },
                offset = Offset
            }
        };

    public int GetCost() => 12 + (Shielded ? 12 : 0);

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        var options = new List<(double, IJesterApi.IEntry)>();
        if (!Shielded)
            options.Add((1.0, new FlowerStoneEntry
            {
                Offset = Offset,
                Shielded = true
            }));
        return options;
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class MiniRepairKitEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }

    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "drone",
                "utility",
                "heal"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new MiniRepairKit(),
                offset = Offset
            }
        };

    public int GetCost() => 30;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("heal");
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class PowerStoneEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }

    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "drone",
                "utility",
                "overdrive"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ASpawn
            {
                thing = new PowerStone(),
                offset = Offset
            }
        };

    public int GetCost() => 30;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class OverdriveNextTurnEntry : IJesterApi.IEntry
{
    [Required] public int Amount { get; init; }
    public IReadOnlySet<string> Tags => new HashSet<string> { "offensive", "status", "overdriveNextTurn" };

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("overdriveNextTurn");
    }

    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];

        actions.Add(
        new AStatus
        {
            targetPlayer = true,
            status = Elestrals.Instance.OverdriveNextTurn.Status,
            statusAmount = Amount
        });

        return actions;
    }

    public int GetCost()
    {
        return Amount == 1 ? 20 : Amount == 2 ? 50 : Amount*40;
    }

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>
        {
            (1, new OverdriveNextTurnEntry
            {
                Amount = Amount + 1
            })
        };
    }
}

internal class EarthStoneDepositEntry : IJesterApi.IEntry
{
    [Required] public int Amount { get; init; }
    public IReadOnlySet<string> Tags => new HashSet<string> { "offensive", "status", "earthStoneDeposit", "mustExhaust" };

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("earthStoneDeposit");
    }

    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];

        actions.Add(
        new AStatus
        {
            targetPlayer = true,
            status = Elestrals.Instance.EarthStoneDeposit.Status,
            statusAmount = Amount
        });

        return actions;
    }

    public int GetCost()
    {
        return Amount == 1 ? 60 : Amount == 2 ? 80 : Amount * 40;
    }

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        var options = new List<(double, IJesterApi.IEntry)>();
        if (upDir == Upgrade.B && Amount == 1)
            options.Add((1.0, new EarthStoneDepositEntry
            {
                Amount = 2
            }));
        return options;
    }
}

internal class FlowerStoneDepositEntry : IJesterApi.IEntry
{
    [Required] public int Amount { get; init; }
    public IReadOnlySet<string> Tags => new HashSet<string> { "defensive", "status", "flowerStoneDeposit", "mustExhaust" };

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("flowerStoneDeposit");
    }

    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];

        actions.Add(
        new AStatus
        {
            targetPlayer = true,
            status = Elestrals.Instance.FlowerStoneDeposit.Status,
            statusAmount = Amount
        });

        return actions;
    }

    public int GetCost()
    {
        return Amount == 1 ? 60 : Amount == 2 ? 80 : Amount * 40;
    }

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        var options = new List<(double, IJesterApi.IEntry)>();
        if (upDir == Upgrade.B && Amount == 1)
            options.Add((1.0, new FlowerStoneDepositEntry
            {
                Amount = 2
            }));
        return options;
    }
}

internal class HyperFocusEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags => new HashSet<string> { "cost", "status", "hyperFocus" };

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("hyperFocus");
    }

    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];

        actions.Add(
        new AStatus
        {
            targetPlayer = true,
            status = Elestrals.Instance.HyperFocus.Status,
            statusAmount = 1
        });

        return actions;
    }

    public int GetCost()
    {
        return -100;
    }

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }
}

internal class WeakenChargeEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags => new HashSet<string> { "offensive", "status", "weakenCharge", "mustExhaust" };

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("weakenCharge");
    }

    public IEnumerable<CardAction> GetActions(State s, Combat c)
    {
        List<CardAction> actions = [];

        actions.Add(
        new AStatus
        {
            targetPlayer = true,
            status = Elestrals.Instance.WeakenCharge.Status,
            statusAmount = 1
        });

        return actions;
    }

    public int GetCost()
    {
        return 80;
    }

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }
}

internal class BlossomEntry : IJesterApi.IEntry
{

    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "blossom",
                "utility"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ABlossom()
        };

    public int GetCost() => 10;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("blossom");
    }
}

internal class BayRuptureEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }
    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "rupture",
                "utility"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ABayRupture
            {
                offset = Offset
            }
        };

    public int GetCost() => 4;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("rupture");
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class CannonRuptureEntry : IJesterApi.IEntry
{
    [Required] public int Offset { get; init; }
    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "rupture",
                "offensive"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new ACannonRupture
            {
                offset = Offset
            }
        };

    public int GetCost() => 4;

    public IEnumerable<(double, IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double, IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("rupture");
        request.OccupiedMidrow.Add(Offset);
    }
}

internal class AllRuptureEntry : IJesterApi.IEntry
{
    public IReadOnlySet<string> Tags =>
        new HashSet<string>
        {
                "rupture",
                "utility"
        };

    public IEnumerable<CardAction> GetActions(State s, Combat c) => new List<CardAction>
        {
            new AAllRupture()
        };

    public int GetCost() => 10;

    public IEnumerable<(double,IJesterApi.IEntry)> GetUpgradeOptions(IJesterApi.IJesterRequest request, Upgrade upDir)
    {
        return new List<(double,IJesterApi.IEntry)>();
    }

    public void AfterSelection(IJesterApi.IJesterRequest request)
    {
        request.Blacklist.Add("rupture");
    }
}