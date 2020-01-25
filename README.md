# AspCoreCardGameEngine
A card game engine API written in AspNet Core.

Inspired by https://deckofcardsapi.com/ (https://github.com/crobertsbmw/deckofcards written in python). This project is to create an open source AspNet Core engine.


## Roadmap

### Documentation

0. [ ] Contribution guidelines
0. [ ] Production setup

### Contribute

Contributions are super welcome!

* Database migrations are applied automatically in Startup via StartupHelper
* To generate migrate script, call `dotnet ef migrations add Your_new_migration_name`

### Engine

0. [x] Plumbing with EntityFramework (using Mysql)
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

### Games

0. [ ] Shithead
0. [ ] Rummy
