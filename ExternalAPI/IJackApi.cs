using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.ExternalAPI;

public interface IJackApi
{
    IDeckEntry Jack_Deck { get; }
    IStatusEntry ScanBoost_Status { get; }
    IStatusEntry LockOnStatus { get; }
    IStatusEntry ALockOnStatus { get; }
    IStatusEntry MidrowHaltStatus { get; }
    IStatusEntry LoseDroneshiftNextStatus { get; }
    StuffBase MiniMissile { get; }
    StuffBase APRocket { get; }
    StuffBase BalisticMissile { get; }
    StuffBase ClusterMissile { get; }
    StuffBase BlankMissile { get; }
}
