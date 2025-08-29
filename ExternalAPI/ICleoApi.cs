using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.ExternalAPI;

public interface ICleoApi
{
    IDeckEntry CleoDeck { get; }

    ICardTraitEntry ImprovedACardTrait { get; }
    ICardTraitEntry ImprovedBCardTrait { get; }
    Tooltip GetImprovedATooltip(bool onOrOff);
    Tooltip GetImprovedBTooltip(bool onOrOff);
    Tooltip GetImpairedTooltip(bool onOrOff);
    bool GetIsImprovedA(Card card);
    void SetIsImprovedA(Card card, bool value);
    void AddImprovedA(Card card, State s);
    bool GetIsImprovedB(Card card);
    void SetIsImprovedB(Card card, bool value);
    void AddImprovedB(Card card, State s);
    bool GetIsImpaired(Card card);
    void SetIsImpaired(Card card, bool value);
    void AddImpaired(Card card, State s);
}