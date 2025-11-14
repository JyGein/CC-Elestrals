using JyGein.Elestrals.ExternalAPI;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals;

public interface IEquilynxApi
{
    public IARupture ABayRupure { get; }
    public IARupture ACannonRupture { get; }
    public IARupture AAllRupture { get; }
    interface IARupture : IKokoroApi.IV2.ICardAction<CardAction>
    {
        public int? fromX { get; set; }

        public int offset { get; set; }

        public bool multiBayVolley { get; set; }

        public bool fromPlayer { get; set; }
    }
    public CardAction ABlossom { get; }
    public StuffBase MiniEarthStone { get; }
    public StuffBase EarthStone { get; }
    public StuffBase BigEarthStone { get; }
    public StuffBase FlowerStone { get; }
    public StuffBase MiniRepairKit { get; }
    public StuffBase PowerStone { get; }
    public IStatusEntry HyperFocus { get; }
    public IStatusEntry WeakenCharge { get; }
    public IStatusEntry EarthStoneDeposit { get; }
    public IStatusEntry FlowerStoneDeposit { get; }
    public IDeckEntry EquilynxDeck { get; }
}
