using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead;
using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Api;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;
using Xunit;

namespace Tests.ServiceImplementations.Shithead
{
    public class ShitheadMoveValidatorTests
    {
        [Fact]
        public void PlayerCanDrawFromDeck_Invalid_if_another_players_turn()
        {
            // Arrange
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Game game = new Game(string.Empty, string.Empty) {State = "game-state"};
            Player player = new Player(null) {Id = Guid.Parse("83B48D11-2243-42DB-88AC-BE1DE09B82CC")};

            // Act
            var result = shitheadMoveValidator.PlayerCanDrawFromDeck(game, player);

            // Assert
            Assert.Equal(string.Format(Resources.It_is_another_player_s_turn__not_player__0_, player.Id.ToString()), result);
        }

        [Fact]
        public void PlayerCanDrawFromDeck_Valid_when_same_player()
        {
            // Arrange
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Player player = new Player(null) {Id = Guid.NewGuid()};
            Game game = new Game(string.Empty, string.Empty) {State = player.Id.ToString()};

            // Act
            var result = shitheadMoveValidator.PlayerCanDrawFromDeck(game, player);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PlayerCanDrawFromDeck_Valid_when_same_player_but_id_casing_differs()
        {
            // Arrange
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Player player = new Player(null) {Id = Guid.Parse("83B48D11-2243-42DB-88AC-BE1DE09B82CC")};
            Game game = new Game(string.Empty, string.Empty) {State = "83B48D11-2243-42db-88AC-be1de09b82cc"};

            // Act
            var result = shitheadMoveValidator.PlayerCanDrawFromDeck(game, player);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanPlayCardsOnPile_Valid_when_pile_empty()
        {
            // Arrange
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Pile pile = new Pile(null, PileType.Deck, "");

            // Act
            var result = shitheadMoveValidator.CanPlayCardsOnPile(null, pile, null);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(15)]
        public void CanPlayCardsOnPile_Valid_when_card_can_be_played_on_anything(int cardToPlayValue)
        {
            // Arrange
            var config = new ShitheadGameConfig {Joker = 15, Reset = 2, Invisible = 3, Reverse = 7, Burn = 10, Skip = 15};
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Pile pile = new Pile(null, PileType.Deck, "")
            {
                Cards = new List<Card>
                {
                    new Card(null, CardSuit.Clubs, 14)
                },
            };
            var cardsToPlay = new List<Card>
            {
                new Card(null, CardSuit.Clubs, cardToPlayValue)
            };

            // Act
            var result = shitheadMoveValidator.CanPlayCardsOnPile(config, pile, cardsToPlay);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(new[] {2, 3, 15}, 4)]
        [InlineData(new[] {2, 3, 15}, 14)]
        [InlineData(new[] {15, 3, 2}, 4)]
        [InlineData(new[] {15, 3, 2}, 14)]
        [InlineData(new[] {15, 2, 3}, 4)]
        [InlineData(new[] {15, 2, 3}, 14)]
        public void CanPlayCardsOnPile_Valid_when_pile_has_only_NonValue_cards(int[] nonValueCards, int cardToPlayValue)
        {
            // Arrange
            var config = new ShitheadGameConfig {Joker = 15, Reset = 2, Invisible = 3, Reverse = 7, Burn = 10, Skip = 15};
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Pile pile = new Pile(null, PileType.Deck, "")
            {
                Cards = nonValueCards.Select(v => new Card(null, CardSuit.Clubs, v)).ToList(),
            };
            var cardsToPlay = new List<Card>
            {
                new Card(null, CardSuit.Clubs, cardToPlayValue)
            };

            // Act
            var result = shitheadMoveValidator.CanPlayCardsOnPile(config, pile, cardsToPlay);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CanPlayCardsOnPile_Invalid_when_reverse_order_and_card_is_higher_than_reverse()
        {
            // Arrange
            var config = new ShitheadGameConfig {Joker = 15, Reset = 2, Invisible = 3, Reverse = 7, Burn = 10, Skip = 15};
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Pile pile = new Pile(null, PileType.Deck, "")
            {
                Cards = new List<Card>
                {
                    new Card(null, CardSuit.Clubs, config.Reverse),
                },
            };
            var cardsToPlay = new List<Card>
            {
                new Card(null, CardSuit.Clubs, 14)
            };

            // Act
            var result = shitheadMoveValidator.CanPlayCardsOnPile(config, pile, cardsToPlay);

            // Assert
            var expectedValidationResult = string.Format(Resources.Card__0__is_not_lower_or_equal_to_the_reverse_card__1_, cardsToPlay.First(), pile.Cards.First());
            Assert.Equal(expectedValidationResult, result);
        }

        [Fact]
        public void CanPlayCardsOnPile_Invalid_when_card_is_lower_than_pile()
        {
            // Arrange
            var config = new ShitheadGameConfig {Joker = 15, Reset = 2, Invisible = 3, Reverse = 7, Burn = 10, Skip = 15};
            var shitheadMoveValidator = new ShitheadMoveValidator();
            Pile pile = new Pile(null, PileType.Deck, "")
            {
                Cards = new List<Card>
                {
                    new Card(null, CardSuit.Clubs, 14),
                },
            };
            var cardsToPlay = new List<Card>
            {
                new Card(null, CardSuit.Clubs, 5)
            };

            // Act
            var result = shitheadMoveValidator.CanPlayCardsOnPile(config, pile, cardsToPlay);

            // Assert
            var expectedValidationResult = string.Format(Resources.Card__0__is_not_higher_or_equal_to_card__1_, cardsToPlay.First(), pile.Cards.First());
            Assert.Equal(expectedValidationResult, result);
        }
    }
}