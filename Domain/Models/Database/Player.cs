using System;

namespace AspCoreCardGameEngine.Domain.Models.Database
{
    public class Player : ICreatedDate, IUpdatedDate
    {
        public Guid Id { get; set; }
        public Game Game { get; set; }
        public bool Accepted { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Player()
        {
        }

        public Player(Game game)
            : this()
        {
            Game = game;
        }

        public override string ToString()
        {
            return $"Id={Id}, Accepted={(Accepted ? "true" : "false")}";
        }
    }
}