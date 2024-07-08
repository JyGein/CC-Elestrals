using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class AVolleyRupture : CardAction
    {
        public required ARupture rupture;

        public override void Begin(G g, State s, Combat c)
        {
            timer = 0.0;
            rupture.multiBayVolley = true;
            List<ARupture> list = new List<ARupture>();
            int num = 0;
            foreach (Part part in s.ship.parts)
            {
                if (part.type == PType.missiles && part.active && rupture.ruptureType == ARupture.RuptureType.Missile)
                {
                    rupture.fromX = num;
                    list.Add(Mutil.DeepCopy(rupture));
                }
                if (part.type == PType.cannon && part.active && rupture.ruptureType == ARupture.RuptureType.Cannon)
                {
                    rupture.fromX = num;
                    list.Add(Mutil.DeepCopy(rupture));
                }

                num++;
            }

            c.QueueImmediate(list);
        }
    }
}
