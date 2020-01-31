using System.Linq;
using AspCoreCardGameEngine.Api.ServiceImplementations;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;
using AspCoreCardGameEngine.Domain.Services.Structs;
using NSubstitute;
using Xunit;

namespace Tests.ServiceImplementations
{
    public class DeckServiceTests
    {
        private readonly IShuffler _shuffler;
        private readonly DeckFactory _factory;

        public DeckServiceTests()
        {
            _shuffler = Substitute.For<IShuffler>();
            _factory = new DeckFactory(_shuffler);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 2)]
        public void CreateDeck_Joker_count_ExpectedBehavior(bool includeJokers, int expectedJokerCount)
        {
            // Arrange
            var options = new CreateDeckOptions
            {
                IncludeJokers = includeJokers,
            };
            var pile = new Pile(null, PileType.Deck, "test-pile");

            // Act
            _factory.AddDeckCardsToPile(ShitheadGameConfig.Default, pile, options);

            // Assert
            var jokerCount = pile.Cards.Count(c => c.Suit == CardSuit.Joker);
            Assert.Equal(expectedJokerCount, jokerCount);
        }

        [Theory]
        [InlineData(false, 52)]
        [InlineData(true, 54)]
        public void CreateDeck_Card_count_ExpectedBehavior(bool includeJokers, int expectedCardCount)
        {
            // Arrange
            var options = new CreateDeckOptions
            {
                IncludeJokers = includeJokers,
            };
            var pile = new Pile(null, PileType.Deck, "test-pile");

            // Act
            _factory.AddDeckCardsToPile(ShitheadGameConfig.Default, pile, options);

            // Assert
            Assert.Equal(expectedCardCount, pile.Cards.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateDeck_Shuffle_called_when_expected(bool shuffle)
        {
            // Arrange
            var options = new CreateDeckOptions {Shuffled = shuffle};
            var pile = new Pile(null, PileType.Deck, "test-pile");

            // Act
            _factory.AddDeckCardsToPile(ShitheadGameConfig.Default, pile, options);

            // Assert
            if (shuffle)
            {
                _shuffler.ReceivedWithAnyArgs(1).Shuffle<Card>(null);
            }
            else
            {
                _shuffler.DidNotReceiveWithAnyArgs().Shuffle<Card>(null);
            }
        }
    }
}