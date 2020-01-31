using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions
{
    public static class ShitheadPileExtensions
    {
        public static Pile GetPile(this Player player, PlayerHandType handType)
        {
            return player.Game.GetPlayerPile(player, handType.ToString());
        }

        public static Pile GetHandPile(this Player player)
        {
            return player.GetPile(PlayerHandType.Hand);
        }

        public static Pile GetFaceUpPile(this Player player)
        {
            return player.GetPile(PlayerHandType.FaceUp);
        }

        public static Pile GetFaceDownPile(this Player player)
        {
            return player.GetPile(PlayerHandType.FaceDown);
        }

        public static Pile GetFirstNonEmptyPile(this Player player)
        {
            return player.Game.GetFirstNonEmptyPile(player, new[]
            {
                PlayerHandType.Hand,
                PlayerHandType.FaceUp,
                PlayerHandType.FaceDown,
            });
        }
    }
}