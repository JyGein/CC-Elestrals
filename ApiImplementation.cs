using JyGein.Elestrals.Actions;
using JyGein.Elestrals.Midrow;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JyGein.Elestrals.IEquilynxApi;

namespace JyGein.Elestrals;

public class ApiImplementation : IEquilynxApi
{
    public IARupture ABayRupure => new ABayRupture();

    public IARupture ACannonRupture => new ACannonRupture();

    public IARupture AAllRupture => new AAllRupture();

    public CardAction ABlossom => new ABlossom();

    public StuffBase MiniEarthStone => new EarthStone { StoneType = Midrow.EarthStone.EarthStoneType.Mini };

    public StuffBase EarthStone => new EarthStone { StoneType = Midrow.EarthStone.EarthStoneType.Normal };

    public StuffBase BigEarthStone => new EarthStone { StoneType = Midrow.EarthStone.EarthStoneType.Big };

    public StuffBase FlowerStone => new FlowerStone();

    public StuffBase MiniRepairKit => new MiniRepairKit();

    public StuffBase PowerStone => new PowerStone();

    public IStatusEntry HyperFocus => Elestrals.Instance.HyperFocus;

    public IStatusEntry WeakenCharge => Elestrals.Instance.WeakenCharge;

    public IStatusEntry EarthStoneDeposit => Elestrals.Instance.EarthStoneDeposit;

    public IStatusEntry FlowerStoneDeposit => Elestrals.Instance.FlowerStoneDeposit;

    public IDeckEntry EquilynxDeck => Elestrals.Instance.Equilynx_Deck;
}
