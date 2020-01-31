using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions
{
    public static class ShitheadGameExtensions
    {
        public static Pile GetFirstNonEmptyPile(this Game game, Player player, PlayerHandType[] types)
        {
            foreach (var type in types)
            {
                var pile = player.GetPile(type);

                if (!pile.IsEmpty())
                {
                    return pile;
                }
            }

            return null;
        }

        public static Pile CreatePlayerPile(this Game game, Player player, PlayerHandType handType)
        {
            return game.CreatePlayerPile(player, handType.ToString());
        }
    }
}