using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions
{
    public static class ShitheadGameExtensions
    {
        public static Pile GetPlayerPile(this Game game, Player player, PlayerHandTypesEnum handType)
        {
            return game.GetPlayerPile(player, handType.ToString());
        }

        public static Pile GetFirstNonEmptyPile(this Game game, Player player, PlayerHandTypesEnum[] types)
        {
            foreach (var type in types)
            {
                var pile = game.GetPlayerPile(player, type);

                if (!pile.IsEmpty())
                {
                    return pile;
                }
            }

            return null;
        }

        public static Pile CreatePlayerPile(this Game game, Player player, PlayerHandTypesEnum handType)
        {
            return game.CreatePlayerPile(player, handType.ToString());
        }
    }
}