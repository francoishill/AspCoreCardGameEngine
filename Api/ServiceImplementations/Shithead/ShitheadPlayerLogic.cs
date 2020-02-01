using System;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead
{
    public class ShitheadPlayerLogic : IShitheadPlayerLogic
    {
        public string CalculateNextPlayer(
            Game game,
            Player player,
            (bool PileGotBurnt, bool PlayedCardIsReverse, bool PlayedCardIsSkip) flags)
        {
            var (pileGotBurnt, playedCardIsReverse, playedCardIsSkip) = flags;
            if (pileGotBurnt)
            {
                // Player can play again after burn
                return player.Id.ToString();
            }

            if (playedCardIsReverse)
            {
                game.Mode = game.Mode == ShitheadConstants.GameModes.NORMAL
                    ? ShitheadConstants.GameModes.REVERSE
                    : ShitheadConstants.GameModes.NORMAL;
            }

            Player nextPlayer;
            var players = game.Players.ToList();
            var playerIndex = players.IndexOf(player);
            if (playerIndex == -1)
            {
                throw new Exception($"Player id {player.Id} is not found in game {game.Id} Players list");
            }

            switch (game.Mode)
            {
                case ShitheadConstants.GameModes.NORMAL:
                {
                    playerIndex++;

                    if (playedCardIsSkip)
                    {
                        playerIndex++;
                    }

                    if (playerIndex >= players.Count)
                    {
                        playerIndex -= players.Count;
                    }

                    nextPlayer = players[playerIndex];
                    break;
                }
                case ShitheadConstants.GameModes.REVERSE:
                {
                    playerIndex--;

                    if (playedCardIsSkip)
                    {
                        playerIndex--;
                    }

                    if (playerIndex < 0)
                    {
                        playerIndex += players.Count;
                    }

                    nextPlayer = players[playerIndex];
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(game.Mode), string.Format(Resources.Unknown_game_mode__0_, game.Mode));
            }

            return nextPlayer.Id.ToString();
        }
    }
}