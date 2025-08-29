using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Features;

internal interface IRuptureHook
{
    public void OnRuptureMiss(State s, Combat c) { }
    public void OnRuptureHit(State s, Combat c, StuffBase stuffHit) { }
    public void OnRupture(State s, Combat c) { }
}
