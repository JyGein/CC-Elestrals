using System;

namespace JyGein.Elestrals;

public interface IAppleShipyardApi
{
    void RegisterActionLooksForPartType(Type actionType, PType partType);
}