using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class ACannonRupture : ARupture
    {
        public ACannonRupture()
        {
            this.ruptureType = RuptureType.Cannon;
        }
        public override void Begin(G g, State s, Combat c)
        {
            base.Begin(g, s, c);
        }
    }
}
