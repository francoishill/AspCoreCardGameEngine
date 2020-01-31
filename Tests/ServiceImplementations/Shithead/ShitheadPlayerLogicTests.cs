using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead;
using System;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using Xunit;

namespace Tests.ServiceImplementations.Shithead
{
    public class ShitheadPlayerLogicTests
    {
        private readonly ShitheadPlayerLogic _shitheadPlayerLogic;
        private readonly Game _game;
        private readonly Player _player1;
        private readonly Player _player2;
        private readonly Player _player3;
        private readonly Player _player4;
        private readonly Player _player5;

        public ShitheadPlayerLogicTests()
        {
            _game = new Game(null, ShitheadConstants.GameModes.NORMAL);
            _player1 = AddPlayer(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            _player2 = AddPlayer(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            _player3 = AddPlayer(Guid.Parse("00000000-0000-0000-0000-000000000003"));
            _player4 = AddPlayer(Guid.Parse("00000000-0000-0000-0000-000000000004"));
            _player5 = AddPlayer(Guid.Parse("00000000-0000-0000-0000-000000000005"));

            _shitheadPlayerLogic = new ShitheadPlayerLogic();
        }

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

        [Fact]
        public void CalculateNextPlayer_keep_same_player_when_burnt()
        {
            // Arrange
            _game.State = _player1.Id.ToString();
            var flags = (PileGotBurnt: true, PlayedCardIsReverse: false);

            // Act
            var result = _shitheadPlayerLogic.CalculateNextPlayer(_game, _player1, flags);

            // Assert
            Assert.Equal(_player1.Id.ToString(), result);
        }

        [Theory]
        [InlineData(5, 1)]
        [InlineData(4, 5)]
        [InlineData(3, 4)]
        [InlineData(2, 3)]
        [InlineData(1, 2)]
        public void CalculateNextPlayer_next_player_when_non_reverse(int thisMovePlayerNum, int nextMovePlayerNum)
        {
            // Arrange
            var players = new[] {_player1, _player2, _player3, _player4, _player5};
            var thisPlayer = players[thisMovePlayerNum - 1];
            _game.State = thisPlayer.Id.ToString();
            var flags = (PileGotBurnt: false, PlayedCardIsReverse: false);

            // Act
            var result = _shitheadPlayerLogic.CalculateNextPlayer(_game, thisPlayer, flags);

            // Assert
            Assert.Equal(players[nextMovePlayerNum - 1].Id.ToString(), result);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(5, 4)]
        [InlineData(4, 3)]
        [InlineData(3, 2)]
        [InlineData(2, 1)]
        public void CalculateNextPlayer_previous_player_when_non_reverse(int thisMovePlayerNum, int nextMovePlayerNum)
        {
            // Arrange
            var players = new[] {_player1, _player2, _player3, _player4, _player5};
            var thisPlayer = players[thisMovePlayerNum - 1];
            _game.State = thisPlayer.Id.ToString();
            var flags = (PileGotBurnt: false, PlayedCardIsReverse: true);

            // Act
            var result = _shitheadPlayerLogic.CalculateNextPlayer(_game, thisPlayer, flags);

            // Assert
            Assert.Equal(players[nextMovePlayerNum - 1].Id.ToString(), result);
        }
    }
}