using FMOD;
using JyGein.Elestrals.Midrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class ABlossom : CardAction
    {
        public override void Begin(G g, State s, Combat c)
        {
            foreach (StuffBase stuffBase in Enumerable.ToList<StuffBase>((IEnumerable<StuffBase>)c.stuff.Values))
            {
                c.stuff.Remove(stuffBase.x);
                FlowerStone flowerstone1 = new FlowerStone();
                flowerstone1.x = stuffBase.x;
                flowerstone1.xLerped = stuffBase.xLerped;
                flowerstone1.bubbleShield = stuffBase.bubbleShield;
                flowerstone1.targetPlayer = stuffBase.targetPlayer;
                flowerstone1.age = stuffBase.age;
                FlowerStone flowerstone2 = flowerstone1;
                c.stuff[stuffBase.x] = (StuffBase)flowerstone2;
            }
            Audio.Play(new GUID?(FSPRO.Event.Status_PowerDown));
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            if (s.route is Combat route)
            {
                foreach (StuffBase stuffBase in route.stuff.Values)
                    stuffBase.hilight = 2;
            }
            List<Tooltip> tooltips = new List<Tooltip>();
            tooltips.Add(new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.action,
                () => Elestrals.Instance.BlossomIcon.Sprite,
                () => Elestrals.Instance.Localizations.Localize(["action", "Blossom", "name"]),
                () => Elestrals.Instance.Localizations.Localize(["action", "Blossom", "description"])
            ));
            tooltips.Concat(new FlowerStone().GetTooltips());
            return tooltips;
        }

        public override Icon? GetIcon(State s)
        {
            return new Icon?(new Icon(Elestrals.Instance.BlossomIcon.Sprite, new int?(), Colors.textMain));
        }
    }
}
