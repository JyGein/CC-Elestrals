using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace JyGein.Elestrals;

internal sealed class CardScalingManager : ICardRenderHook
{
    public CardScalingManager()
    {
        Elestrals.Instance.KokoroApi.RegisterCardRenderHook(this, 0);
    }

    public Matrix ModifyNonTextCardRenderMatrix(G g, Card card, List<CardAction> actions)
        => (card as IElestralsCard)?.ModifyNonTextCardRenderMatrix(g, actions) ?? Matrix.Identity;

    public Matrix ModifyCardActionRenderMatrix(G g, Card card, List<CardAction> actions, CardAction action, int actionWidth)
        => (card as IElestralsCard)?.ModifyCardActionRenderMatrix(g, actions, action, actionWidth) ?? Matrix.Identity;
}
