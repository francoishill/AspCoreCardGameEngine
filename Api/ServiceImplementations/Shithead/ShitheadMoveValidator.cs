using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead
{
    public class ShitheadMoveValidator : IShitheadMoveValidator
    {
        public string PlayerCanDrawFromDeck(Game game, Player player)
        {
            if (!string.Equals(game.State.ToLower(), player.Id.ToString().ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format(Resources.It_is_another_player_s_turn__not_player__0_, player.Id);
            }

            return null;
        }

        public string CanPlayCardsOnPile(ShitheadGameConfig config, Pile pile, List<Card> cardsToPlay)
        {
            var pileCards = pile.Cards;

            if (pileCards.Count == 0)
            {
                return null;
            }

            var cardToPlay = cardsToPlay.First();
            var valueToPlay = cardToPlay.Value;

            if (valueToPlay == 1)
            {
                // Ace
                valueToPlay = 14;
            }

            if (valueToPlay == config.Reset
                || valueToPlay == config.Invisible
                || valueToPlay == config.Burn
                || valueToPlay == config.Skip)
            {
                return null;
            }

            // TODO: Ordering by .Updated and ThenBy .Id is not super robust
            var pileCardsWithValues = pileCards
                .Where(c =>
                    c.Value != config.Invisible
                    && c.Value != config.Skip)
                .OrderBy(c => c.Updated ?? DateTime.MinValue).ThenBy(c => c.Id)
                .ToList();

            if (pileCardsWithValues.Count == 0)
            {
                // Pile ends with "invisible" or "no value" cards, so we can play on it
                return null;
            }

            var lastPileCard = pileCardsWithValues.Last();

            var lastPileCardValue = lastPileCard.Value;
            if (lastPileCardValue == 1)
            {
                // Ace
                lastPileCardValue = 14;
            }

            if (lastPileCardValue == config.Reverse
                && valueToPlay > lastPileCardValue)
            {
                return string.Format(Resources.Card__0__is_not_lower_or_equal_to_the_reverse_card__1_, cardToPlay, lastPileCard);
            }

            if (lastPileCardValue != config.Reverse
                && valueToPlay < lastPileCardValue)
            {
                return string.Format(Resources.Card__0__is_not_higher_or_equal_to_card__1_, cardToPlay, lastPileCard);
            }

            return null;
        }
    }
}