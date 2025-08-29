using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.ExternalAPI;

public interface IBucketApi
{
    Deck BucketDeck { get; }
    Status IngenuityStatus { get; }
    Status SalvageStatus { get; }
    Status SteamCoverStatus { get; }
}
