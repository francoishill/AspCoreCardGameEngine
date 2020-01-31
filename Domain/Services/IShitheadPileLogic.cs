using System.Collections.Generic;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadPileLogic
    {
        void DrawFromDeck(Player player);
        void PlayToDeck(ShitheadGameConfig config, List<Card> cardsToPlay, Player player);
    }
}