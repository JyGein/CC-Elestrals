using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JyGein.Elestrals
{
    public interface IMoreDifficultiesApi
    {
        bool AreAltStartersEnabled(State state, Deck deck);
        void RegisterAltStarters(Deck deck, StarterDeck starterDeck);
    }
}
