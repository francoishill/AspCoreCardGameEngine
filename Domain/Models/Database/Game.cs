using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Domain.Exceptions;

namespace AspCoreCardGameEngine.Domain.Models.Database
{
    public class Game : ICreatedDate, IUpdatedDate
    {
        public Guid Id { get; set; }

        public string Type { get; set; }
        public string State { get; set; }
        public string Mode { get; set; }

        public ICollection<Pile> Piles { get; set; }
        public ICollection<Player> Players { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Game()
        {
            Piles = new HashSet<Pile>();
            Players = new HashSet<Player>();
        }

        public Game(string type, string mode)
            : this()
        {
            Type = type;
            Mode = mode;
        }

        public override string ToString()
        {
            return $"Id={Id}, Type={Type}, State={State}, Mode={Mode}";
        }

        public Pile GetDeckPile()
        {
            var deckPile = Piles.SingleOrDefault(p => p.Type == PileType.Deck);

            if (deckPile == null)
            {
                throw new DomainException(DomainErrorCode.InconsistentData, "No deck pile found");
            }

            return deckPile;
        }

        public Pile GetDiscardPile(string identifier)
        {
            var discardPile = Piles.SingleOrDefault(p => p.Type == PileType.Discard && p.Identifier == identifier);
            if (discardPile == null)
            {
                throw new DomainException(DomainErrorCode.InconsistentData, $"No discard pile for game {Id}");
            }

            return discardPile;
        }

        public static string GetPlayerIdentifier(Player player, string identifierSuffix)
        {
            return $"{player.Id.ToString()}/{identifierSuffix}";
        }

        public Pile GetPlayerPile(Player player, string identifierSuffix)
        {
            var identifier = GetPlayerIdentifier(player, identifierSuffix);

            var playerPile = Piles.SingleOrDefault(p => p.Type == PileType.PlayerHand && p.Identifier == identifier);
            if (playerPile == null)
            {
                throw new DomainException(DomainErrorCode.InconsistentData, $"No player pile for game {Id} and player {player.Id}");
            }

            return playerPile;
        }

        public Pile CreatePlayerPile(Player player, string identifierSuffix)
        {
            var identifier = GetPlayerIdentifier(player, identifierSuffix);

            return new Pile(this, PileType.PlayerHand, identifier);
        }
    }
}