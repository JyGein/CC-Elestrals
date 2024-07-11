using JyGein.Elestrals.Midrow;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace JyGein.Elestrals;
internal sealed class FlowerStoneDepositManager : IStatusLogicHook, IStatusRenderHook
{
    public static Elestrals Instance => Elestrals.Instance;
    public FlowerStoneDepositManager()
    {
        /* We task Kokoro with the job to register our status into the game */
        Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
        Instance.KokoroApi.RegisterStatusRenderHook(this, 0);
    }
    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        /* Here we tell it what to do. Since it's a 'next turn gain something', we can also use this moment to do that something */
        if (status != Instance.FlowerStoneDeposit.Status)
            return false;
        if (timing != StatusTurnTriggerTiming.TurnStart)
            return false;

        if (amount > 0)
        {
            for(int i = 0; i < amount; i++)
            {
                Elestrals.Instance.Logger.Log(LogLevel.Information, "hi");
                combat.QueueImmediate(new ASpawn()
                {
                    thing = new FlowerStone()
                });
            }
        }
        return false;
    }
    public List<Tooltip> OverrideStatusTooltips(Status status, int amount, Ship? ship, List<Tooltip> tooltips)
        => status == Elestrals.Instance.FlowerStoneDeposit.Status ? tooltips.Concat(new FlowerStone { }.GetTooltips()).ToList() : tooltips;
}