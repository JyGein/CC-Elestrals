using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class ABayRupture : ARupture
    {
        public ABayRupture ()
        {
            this.ruptureType = RuptureType.Missile;
        }
        public override void Begin(G g, State s, Combat c)
        {
            base.Begin(g, s, c);
        }
    }
}
