using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions;
using AspCoreCardGameEngine.Domain.Exceptions;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead
{
    public class ShitheadGameMutator
    {
        private readonly Player _player;
        private readonly Game _game;

        public ShitheadGameMutator(Player player)
        {
            _player = player;
            _game = player.Game;
        }

        public void DrawFromDeck()
        {
            var canDrawValidation = ValidatePlayerCanDrawFromDeck();
            if (canDrawValidation != null)
            {
                throw new DomainException(DomainErrorCode.BadRequest, canDrawValidation);
            }

            var playerPile = _game.GetPlayerPile(_player, PlayerHandTypesEnum.Hand);
            var deckPile = _game.GetDeckPile();

            var deckCard = deckPile.Cards.FirstOrDefault();
            if (deckCard == null)
            {
                throw new DomainException(DomainErrorCode.InconsistentData, $"Deck is empty, cannot draw card for {_game.Id}");
            }

            deckPile.Cards.Remove(deckCard);
            playerPile.Cards.Add(deckCard);
        }

        private string ValidatePlayerCanDrawFromDeck()
        {
            throw new NotImplementedException();
        }

        public void PlayToDeck(ShitheadGameConfig config, List<Card> cards)
        {
            if (cards.Count == 0)
            {
                throw new DomainException(DomainErrorCode.BadRequest, "At least one card must be specified to play to deck");
            }

            if (_player.Game.State != _player.Id.ToString())
            {
                throw new DomainException(DomainErrorCode.BadRequest, $"It is another player's turn, not player {_player.Id}");
            }

            var firstCardValue = cards.First().Value;
            var cardsWithOtherValue = cards.Where(c => c.Value != firstCardValue).ToList();
            if (cardsWithOtherValue.Any())
            {
                throw new DomainException(DomainErrorCode.BadRequest, "Cannot play cards with different values");
            }

            var firstNonEmptyPile = _game.GetFirstNonEmptyPile(_player, new[]
            {
                PlayerHandTypesEnum.Hand,
                PlayerHandTypesEnum.FaceUp,
                PlayerHandTypesEnum.FaceDown,
            });

            if (!firstNonEmptyPile.ContainsAllCards(cards, out var cardsMissingFromPile))
            {
                var ids = string.Join(", ", cardsMissingFromPile.Select(c => c.Id));
                var msg = $"Cards (with ids {ids}) cannot be played because they are not in the first non-empty pile {firstNonEmptyPile.Type.ToString()}";
                throw new DomainException(DomainErrorCode.BadRequest, msg);
            }

            var discardPile = _game.GetDiscardPile(ShitheadConstants.DISCARD_PILE_IDENTIFIER);
            var canPlayValidation = ValidateCanPlayCardsOnPile(config, discardPile, cards);
            if (canPlayValidation != null)
            {
                throw new DomainException(DomainErrorCode.BadRequest, canPlayValidation);
            }

            foreach (var card in cards)
            {
                firstNonEmptyPile.Cards.Remove(card);
                discardPile.Cards.Add(card);
            }

            _game.State = CalculateNextPlayer(_player, cards);
        }

        private string ValidateCanPlayCardsOnPile(ShitheadGameConfig config, Pile pile, List<Card> cardsToPlay)
        {
            var pileCards = pile.Cards;

            if (pileCards.Count == 0)
            {
                return null;
            }

            var cardToPlay = cardsToPlay.First();
            var valueToPlay = cardToPlay.Value;

            if (valueToPlay == config.Reset
                || valueToPlay == config.Invisible
                || valueToPlay == config.Burn
                || valueToPlay == config.Skip)
            {
                return null;
            }

            var pileCardsWithValues = pileCards.Where(c =>
                    c.Value != config.Reset
                    && c.Value != config.Invisible
                    && c.Value != config.Skip)
                .ToList();

            if (pileCardsWithValues.Count == 0)
            {
                // Pile ends with "invisible" or "no value" cards, so we can play on it
                return null;
            }

            var lastPileCard = pileCardsWithValues.Last();

            if (lastPileCard.Value == config.Reverse
                && valueToPlay > lastPileCard.Value)
            {
                return $"Card {cardToPlay} is not lower or equal to the reverse card";
            }

            if (lastPileCard.Value != config.Reverse
                && valueToPlay < lastPileCard.Value)
            {
                return $"Card {cardToPlay} is not higher or equal to card {lastPileCard}";
            }

            return null;
        }

        private string CalculateNextPlayer(Player player, List<Card> cards)
        {
            throw new NotImplementedException();
        }
    }
}