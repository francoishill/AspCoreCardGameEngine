using System;
using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead;
using AspCoreCardGameEngine.Domain.Models.Database;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Api;
using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions;
using AspCoreCardGameEngine.Domain.Exceptions;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Services;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Tests.ServiceImplementations.Shithead
{
    public class ShitheadPileLogicTests
    {
        private readonly Game _game;
        private readonly Pile _discardPile;
        private readonly Pile _deckPile;
        private readonly Player _player1;
        private readonly IShitheadMoveValidator _shitheadMoveValidator;
        private readonly ShitheadPileLogic _shitheadPileLogic;

        public ShitheadPileLogicTests()
        {
            _game = new Game(ShitheadConstants.GAME_TYPE, string.Empty) {Id = Guid.Parse("D0199003-D79D-493A-8118-50F9EE2A7755")};

            _discardPile = new Pile(_game, PileType.Discard, ShitheadConstants.PileIdentifiers.DISCARD);
            _game.Piles.Add(_discardPile);

            var burnPile = new Pile(_game, PileType.Discard, ShitheadConstants.PileIdentifiers.BURN);
            _game.Piles.Add(burnPile);

            _deckPile = new Pile(_game, PileType.Deck, ShitheadConstants.PileIdentifiers.DECK);
            _game.Piles.Add(_deckPile);

            _player1 = AddPlayer(Guid.Parse("E4F475AA-0FAB-4FAA-883E-C7ED1D26F5C0"));
            _game.State = _player1.Id.ToString();

            _shitheadMoveValidator = Substitute.For<IShitheadMoveValidator>();
            var shitheadPlayerLogic = Substitute.For<IShitheadPlayerLogic>();
            _shitheadPileLogic = new ShitheadPileLogic(_shitheadMoveValidator, shitheadPlayerLogic);
        }

        [Fact]
        public void DrawFromDeck_Fail_if_validation_of_PlayerCanDrawFromDeck_fails()
        {
            // Arrange
            var player1HandPile = _player1.GetHandPile();

            _shitheadMoveValidator.PlayerCanDrawFromDeck(Arg.Any<Game>(), Arg.Any<Player>()).Returns("Cannot draw validation error");

            // Act
            Assert.Empty(_deckPile.Cards);
            Assert.Empty(player1HandPile.Cards);
            var exception = Assert.Throws<DomainException>(() => _shitheadPileLogic.DrawFromDeck(_player1));

            // Assert
            Assert.Equal("Cannot draw validation error", exception.Message);
        }

        [Theory]
        [MemberData(nameof(AllNonBurnCards))]
        public void DrawFromDeck_Draw_card_moves_card_into_player_hand(CardSuit cardSuit, int cardValue)
        {
            // Arrange
            _deckPile.Cards.Add(new Card(_deckPile, cardSuit, cardValue));
            var player1HandPile = _player1.GetHandPile();

            _shitheadMoveValidator.PlayerCanDrawFromDeck(Arg.Any<Game>(), Arg.Any<Player>()).ReturnsNull();

            // Act
            Assert.Single(_deckPile.Cards);
            Assert.Empty(player1HandPile.Cards);
            _shitheadPileLogic.DrawFromDeck(_player1);

            // Assert
            Assert.Empty(_deckPile.Cards);
            Assert.Single(player1HandPile.Cards);
        }

        [Fact]
        public void PlayToDeck_Fail_if_zero_cards_to_play()
        {
            // Arrange
            ShitheadGameConfig config = ShitheadGameConfig.Default;
            List<Card> cards = new List<Card>();

            // Act
            var exception = Assert.Throws<DomainException>(() => _shitheadPileLogic.PlayToDeck(config, cards, _player1));

            // Assert
            Assert.Equal(Resources.At_least_one_card_must_be_specified_to_play_to_deck, exception.Message);
        }

        [Fact]
        public void PlayToDeck_Fail_if_another_players_turn()
        {
            // Arrange
            _game.State = "";
            ShitheadGameConfig config = ShitheadGameConfig.Default;
            List<Card> cards = new List<Card>
            {
                new Card(null, CardSuit.Clubs, 1)
            };

            // Act
            var exception = Assert.Throws<DomainException>(() => _shitheadPileLogic.PlayToDeck(config, cards, _player1));

            // Assert
            Assert.Equal(string.Format(Resources.It_is_another_player_s_turn__not_player__0_, _player1.Id), exception.Message);
        }

        [Fact]
        public void PlayToDeck_Fail_if_cards_to_play_have_different_values()
        {
            // Arrange
            ShitheadGameConfig config = ShitheadGameConfig.Default;
            List<Card> cards = new List<Card>
            {
                new Card(null, CardSuit.Clubs, 1),
                new Card(null, CardSuit.Diamonds, 1),
                new Card(null, CardSuit.Clubs, 2),
                new Card(null, CardSuit.Clubs, 2),
            };

            // Act
            var exception = Assert.Throws<DomainException>(() => _shitheadPileLogic.PlayToDeck(config, cards, _player1));

            // Assert
            Assert.Equal(Resources.Cannot_play_cards_with_different_values, exception.Message);
        }

        [Fact]
        public void PlayToDeck_Fail_if_first_non_empty_player_pile_does_not_contains_cards()
        {
            // Arrange
            var player1HandPile = _player1.GetHandPile();
            var card1 = new Card(null, CardSuit.Clubs, 1) {Id = 1};
            var card2 = new Card(null, CardSuit.Diamonds, 1) {Id = 2};
            player1HandPile.Cards.Add(card1);
            player1HandPile.Cards.Add(card2);

            ShitheadGameConfig config = ShitheadGameConfig.Default;
            List<Card> cards = new List<Card>
            {
                card1,
                new Card(null, CardSuit.Spades, 1) {Id = 3},
            };

            // Act
            var exception = Assert.Throws<DomainException>(() => _shitheadPileLogic.PlayToDeck(config, cards, _player1));

            // Assert
            var expectedExceptionMessage = string.Format(Resources.Cards__with_ids__0___cannot_be_played_because_they_are_not_in_the_first_non_empty_pile__1__of_player__2_,
                "3",
                player1HandPile.Type.ToString(),
                _player1.Id);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Theory]
        [MemberData(nameof(AllNonBurnCards))]
        public void PlayToDeck_Player_can_play_any_card_on_empty_Discard_pile(CardSuit cardSuit, int cardValue)
        {
            // Arrange
            _game.Mode = ShitheadConstants.GameModes.NORMAL;
            var player1HandPile = _player1.GetHandPile();
            var card = new Card(null, cardSuit, cardValue);
            player1HandPile.Cards.Add(card);

            ShitheadGameConfig config = ShitheadGameConfig.Default;

            _shitheadMoveValidator.CanPlayCardsOnPile(config, Arg.Any<Pile>(), Arg.Any<List<Card>>()).ReturnsNull();

            List<Card> cards = new List<Card>
            {
                card,
            };

            // Act
            Assert.Single(player1HandPile.Cards);
            Assert.Empty(_discardPile.Cards);
            _shitheadPileLogic.PlayToDeck(config, cards, _player1);

            // Assert
            Assert.Empty(player1HandPile.Cards);
            Assert.Single(_discardPile.Cards);
        }

        #region Helper stuff

        private Player AddPlayer(Guid id)
        {
            if (_game.Players.Any(p => p.Id == id))
            {
                throw new Exception($"Cannot add duplicate player id {id}");
            }

            var player = new Player(_game)
            {
                Id = id,
            };

            _game.Players.Add(player);

            var handPile = new Pile(_game, PileType.PlayerHand, Game.GetPlayerIdentifier(player, PlayerHandType.Hand.ToString()));
            _game.Piles.Add(handPile);

            var faceUpPile = new Pile(_game, PileType.PlayerHand, Game.GetPlayerIdentifier(player, PlayerHandType.FaceUp.ToString()));
            _game.Piles.Add(faceUpPile);

            var faceDownPile = new Pile(_game, PileType.PlayerHand, Game.GetPlayerIdentifier(player, PlayerHandType.FaceDown.ToString()));
            _game.Piles.Add(faceDownPile);

            return player;
        }

        public static IEnumerable<object[]> AllNonBurnCards()
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                for (int value = 0; value <= 13; value++)
                {
                    if (value == 10)
                    {
                        continue;
                    }

                    yield return new object[] {suit, value};
                }
            }

            yield return new object[] {CardSuit.Joker, ShitheadGameConfig.Default.Joker};
        }

        #endregion Helper stuff
    }
}