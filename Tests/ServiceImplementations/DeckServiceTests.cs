using System.Linq;
using AspCoreCardGameEngine.Api.Domain.Models;
using AspCoreCardGameEngine.Api.Domain.Models.Database;
using AspCoreCardGameEngine.Api.Domain.Services;
using AspCoreCardGameEngine.Api.ServiceImplementations;
using NSubstitute;
using Xunit;

namespace Tests.ServiceImplementations
{
    public class DeckServiceTests
    {
        private readonly IShuffler _shuffler;
        private readonly DeckService _service;

        public DeckServiceTests()
        {
            _shuffler = Substitute.For<IShuffler>();
            _service = new DeckService(null, null, _shuffler);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 2)]
        public void CreateDeck_Joker_count_ExpectedBehavior(bool includeJokers, int expectedJokerCount)
        {
            // Arrange
            var options = new DeckService.CreateDeckOptions
            {
                IncludeJokers = includeJokers,
            };

            // Act
            var result = _service.CreateDeck(options);

            // Assert
            var jokerCount = result.Cards.Count(c => c.Suit == CardSuitEnum.Joker);
            Assert.Equal(expectedJokerCount, jokerCount);
        }

        [Theory]
        [InlineData(false, 52)]
        [InlineData(true, 54)]
        public void CreateDeck_Card_count_ExpectedBehavior(bool includeJokers, int expectedCardCount)
        {
            // Arrange
            var options = new DeckService.CreateDeckOptions
            {
                IncludeJokers = includeJokers,
            };

            // Act
            var result = _service.CreateDeck(options);

            // Assert
            Assert.Equal(expectedCardCount, result.Cards.Count);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateDeck_Shuffle_called_when_expected(bool shuffle)
        {
            // Arrange
            var options = new DeckService.CreateDeckOptions {Shuffled = shuffle};

            // Act
            _service.CreateDeck(options);

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