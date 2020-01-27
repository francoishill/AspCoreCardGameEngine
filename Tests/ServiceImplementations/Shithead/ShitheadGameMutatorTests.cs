using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead;
using AspCoreCardGameEngine.Domain.Models.Database;
using System.Collections.Generic;
using AspCoreCardGameEngine.Domain.Services;
using Xunit;

namespace Tests.ServiceImplementations.Shithead
{
    public class ShitheadGameMutatorTests
    {
        [Fact]
        public void DrawFromDeck_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var game = new Game(ShitheadConstants.GAME_TYPE);
            var player = new Player(game);
            var shitheadGameMutator = new ShitheadGameMutator(player);

            // Act
            shitheadGameMutator.DrawFromDeck();

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void PlayToDeck_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var game = new Game(ShitheadConstants.GAME_TYPE);
            var player = new Player(game);
            var shitheadGameMutator = new ShitheadGameMutator(player);
            ShitheadGameConfig config = ShitheadGameConfig.Default;
            List<Card> cards = null;

            // Act
            shitheadGameMutator.PlayToDeck(
                config,
                cards);

            // Assert
            Assert.True(false);
        }
    }
}
