using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadPlayerLogic
    {
        string CalculateNextPlayer(
            Game game,
            Player player,
            (bool PileGotBurnt, bool PlayedCardIsReverse, bool PlayedCardIsSkip) flags);

        string CalculatePreviousPlayer(
            Game game,
            Player player);
    }
}