using JyGein.Elestrals.Midrow;
using System.Collections.Generic;
using System.Linq;

namespace JyGein.Elestrals;
internal sealed class OverdriveNextTurnManager : IStatusLogicHook, IStatusRenderHook
{
    public static Elestrals Instance => Elestrals.Instance;
    public OverdriveNextTurnManager()
    {
        /* We task Kokoro with the job to register our status into the game */
        Instance.KokoroApi.RegisterStatusLogicHook(this, 0);
        Instance.KokoroApi.RegisterStatusRenderHook(this, 0);
    }
    public bool HandleStatusTurnAutoStep(State state, Combat combat, StatusTurnTriggerTiming timing, Ship ship, Status status, ref int amount, ref StatusTurnAutoStepSetStrategy setStrategy)
    {
        /* Here we tell it what to do. Since it's a 'next turn gain something', we can also use this moment to do that something */
        if (status != Instance.OverdriveNextTurn.Status)
            return false;
        if (timing != StatusTurnTriggerTiming.TurnStart)
            return false;

        if (amount > 0)
        {
            combat.QueueImmediate(new AStatus()
            {
                status = Status.overdrive,
                statusAmount = amount,
                targetPlayer = ship.isPlayerShip,
                timer = 0
            });
            amount = 0;
        }
        return false;
    }

    public List<Tooltip> OverrideStatusTooltips(Status status, int amount, Ship? ship, List<Tooltip> tooltips)
        => status == Elestrals.Instance.OverdriveNextTurn.Status ? tooltips.Concat(StatusMeta.GetTooltips(Status.overdrive, amount)).ToList() : tooltips;
}