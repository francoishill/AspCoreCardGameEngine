using System.Collections.Generic;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadPileLogic
    {
        void DrawFromDeck(Player player);
        void PlayToDeck(ShitheadGameConfig config, Player player, List<Card> cardsToPlay);
        void PickUpDiscardPile(ShitheadGameConfig config, Player player, Pile playerHandPile);
    }
}