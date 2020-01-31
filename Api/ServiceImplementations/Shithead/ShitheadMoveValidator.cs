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
                return string.Format(Resources.Card__0__is_not_lower_or_equal_to_the_reverse_card__1_, cardToPlay, lastPileCard);
            }

            if (lastPileCard.Value != config.Reverse
                && valueToPlay < lastPileCard.Value)
            {
                return string.Format(Resources.Card__0__is_not_higher_or_equal_to_card__1_, cardToPlay, lastPileCard);
            }

            return null;
        }
    }
}