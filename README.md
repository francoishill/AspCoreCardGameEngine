# AspCoreCardGameEngine
A card game engine API written in AspNet Core.

Inspired by https://deckofcardsapi.com/ (https://github.com/crobertsbmw/deckofcards written in python). This project is to create an open source AspNet Core engine.

## Documentation

0. [ ] Contribution guidelines
0. [ ] Production setup

## Contribute

Contributions are super welcome!

* Database migrations are applied automatically in Startup via StartupHelper
* To generate migrate script, call `dotnet ef migrations add Your_new_migration_name`

## Roadmap

### Engine

0. [x] Plumbing with EntityFramework (using Mysql)
0. [ ] Add sql indexes on fields we search on
0. [ ] Ensure Ace has value 14 in Shithead
0. [ ] Auto-draw card if less than 4 cards in hand and deck still have cards
0. [ ] Block shithead moves when not all players are Accepted
0. [x] Handle DomainException in middleware
0. [ ] Add DelayMiddleware for development
0. [x] Setup swagger/Swashbuckle
0. [x] Create deck (shuffled, not shuffled, multiple decks, jokers)
0. [x] ~~Card images~~ - removed from backend, the frontend can take care of this
0. [ ] Draw from deck
0. [ ] Shuffle deck
0. [ ] Create partial deck
0. [ ] Create pile (discarding, player hands)
0. [ ] Shuffle pile
0. [ ] List cards in pile
0. [ ] Draw from pile (top, bottom, random)
0. [ ] Rules engine (to implement games)
0. [ ] Realtime updates (SignalR)
0. [ ] Deal with all `throw new NotImplementedException()`

### Games

0. [ ] Shithead
0. [ ] Rummy
