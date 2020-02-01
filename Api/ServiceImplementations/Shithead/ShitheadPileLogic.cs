using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions;
using AspCoreCardGameEngine.Domain.Exceptions;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead
{
    public class ShitheadPileLogic : IShitheadPileLogic
    {
        private readonly IShitheadMoveValidator _shitheadMoveValidator;
        private readonly IShitheadPlayerLogic _shitheadPlayerLogic;

        public ShitheadPileLogic(IShitheadMoveValidator shitheadMoveValidator, IShitheadPlayerLogic shitheadPlayerLogic)
        {
            _shitheadMoveValidator = shitheadMoveValidator;
            _shitheadPlayerLogic = shitheadPlayerLogic;
        }

        public void DrawFromDeck(Player player)
        {
            var game = player.Game;
            var canDrawValidation = _shitheadMoveValidator.PlayerCanDrawFromDeck(game, player);
            if (canDrawValidation != null)
            {
                throw new DomainException(DomainErrorCode.BadRequest, canDrawValidation);
            }

            var playerHandPile = player.GetHandPile();
            var deckPile = game.GetDeckPile();

            var deckCard = deckPile.Cards.FirstOrDefault();
            if (deckCard == null)
            {
                throw new DomainException(DomainErrorCode.InconsistentData, string.Format(Resources.Deck_is_empty__cannot_draw_card_for_game__0_, game.Id));
            }

            deckPile.Cards.Remove(deckCard);
            playerHandPile.Cards.Add(deckCard);
        }

        public void PlayToDeck(ShitheadGameConfig config, Player player, List<Card> cardsToPlay)
        {
            var game = player.Game;
            if (cardsToPlay.Count == 0)
            {
                throw new DomainException(DomainErrorCode.BadRequest, Resources.At_least_one_card_must_be_specified_to_play_to_deck);
            }

            if (game.State != player.Id.ToString())
            {
                throw new DomainException(DomainErrorCode.BadRequest, string.Format(Resources.It_is_another_player_s_turn__not_player__0_, player.Id));
            }

            var firstCardValue = cardsToPlay.First().Value;
            var cardsWithOtherValue = cardsToPlay.Where(c => c.Value != firstCardValue).ToList();
            if (cardsWithOtherValue.Any())
            {
                throw new DomainException(DomainErrorCode.BadRequest, Resources.Cannot_play_cards_with_different_values);
            }

            var firstNonEmptyPile = player.GetFirstNonEmptyPile();

            if (!firstNonEmptyPile.ContainsAllCards(cardsToPlay, out var cardsMissingFromPile))
            {
                var msg = string.Format(Resources.Cards__with_ids__0___cannot_be_played_because_they_are_not_in_the_first_non_empty_pile__1__of_player__2_,
                    string.Join(", ", cardsMissingFromPile.Select(c => c.Id)),
                    firstNonEmptyPile.Type.ToString(),
                    player.Id);
                throw new DomainException(DomainErrorCode.BadRequest, msg);
            }

            var discardPile = game.GetDiscardPile(ShitheadConstants.PileIdentifiers.DISCARD);
            var canPlayValidation = _shitheadMoveValidator.CanPlayCardsOnPile(config, discardPile, cardsToPlay);
            if (canPlayValidation != null)
            {
                throw new DomainException(DomainErrorCode.BadRequest, canPlayValidation);
            }

            foreach (var card in cardsToPlay)
            {
                firstNonEmptyPile.Cards.Remove(card);
                discardPile.Cards.Add(card);
            }

            var pileGotBurnt = ShouldBurnPile(config, discardPile);
            if (pileGotBurnt)
            {
                var burnPile = game.GetDiscardPile(ShitheadConstants.PileIdentifiers.BURN);
                var discardedCards = discardPile.Cards.ToList();
                foreach (var discardedCard in discardedCards)
                {
                    discardPile.Cards.Remove(discardedCard);
                    burnPile.Cards.Add(discardedCard);
                }
            }

            var playedCardIsReverse = firstCardValue == config.Reverse;
            var playedCardIsSkip = firstCardValue == config.Skip;

            var flags = (PileGotBurnt: pileGotBurnt, PlayedCardIsReverse: playedCardIsReverse, PlayedCardIsSkip: playedCardIsSkip);
            game.State = _shitheadPlayerLogic.CalculateNextPlayer(game, player, flags);

            var deckPile = game.GetDeckPile();
            var playerHandPile = player.GetHandPile();
            while (!deckPile.IsEmpty() && playerHandPile.Cards.Count < config.HandCount)
            {
                var firstDeckCard = deckPile.Cards.First();
                deckPile.Cards.Remove(firstDeckCard);
                playerHandPile.Cards.Add(firstDeckCard);
            }
        }

        private static bool ShouldBurnPile(ShitheadGameConfig config, Pile discardPile)
        {
            var lastDiscardedCard = discardPile.Cards.Last();

            if (lastDiscardedCard.Value == config.Burn)
            {
                return true;
            }

            return discardPile.Cards.Reverse().TakeWhile(c => c.Value == lastDiscardedCard.Value).Count() >= 4;
        }
    }
}