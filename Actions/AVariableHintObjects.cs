using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals.Actions
{
    internal class AVariableHintObjects : AVariableHint
    {
        public int setAmount = 0;

        public AVariableHintObjects() : base()
        {
            hand = true;
        }

        public override Icon? GetIcon(State s)
        {
            return new Icon(Spr.icons_spawn, null, Colors.textMain);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            List<Tooltip> list = [];
            string parentheses = "";
            if (s.route is Combat && setAmount >= 0)
            {
                DefaultInterpolatedStringHandler stringHandler = new(22, 1);
                stringHandler.AppendLiteral(" </c>(<c=keyword>");
                stringHandler.AppendFormatted(setAmount);
                stringHandler.AppendLiteral("</c>)");

                parentheses = stringHandler.ToStringAndClear();
            }
            list.Add(new TTText(Elestrals.Instance.Localizations.Localize(["action", "VariableHintObjects", "description"], new { Amount = parentheses })));
            return list;
        }
    }
}
