using System.Collections.Generic;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadMoveValidator
    {
        string PlayerCanDrawFromDeck(Game game, Player player);
        string CanPlayCardsOnPile(ShitheadGameConfig config, Pile pile, List<Card> cardsToPlay);
    }
}