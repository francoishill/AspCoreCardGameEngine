using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AspCoreCardGameEngine.Api.Persistence
{
    public class CardsDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public IQueryable<Game> GamesIncludingPlayersAndPilesAndCards => Games
            .Include(g => g.Piles).ThenInclude(p => p.Cards)
            .Include(g => g.Players);

        public CardsDbContext(DbContextOptions<CardsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().Property(c => c.Suit).IsRequired().HasMaxLength(32).HasConversion(new EnumToStringConverter<CardSuitEnum>());
            modelBuilder.Entity<Card>().Property(c => c.Value).IsRequired();
            modelBuilder.Entity<Card>().HasOne(c => c.Pile).WithMany(p => p.Cards);

            modelBuilder.Entity<Pile>().Property(c => c.Type).IsRequired().HasMaxLength(64).HasConversion(new EnumToStringConverter<PileTypeEnum>());
            modelBuilder.Entity<Pile>().Property(c => c.Identifier).IsRequired().HasMaxLength(512);
            modelBuilder.Entity<Pile>().HasIndex(r => new {r.GameId, r.Type, r.Identifier}).IsUnique();
            modelBuilder.Entity<Pile>().HasOne(p => p.Game).WithMany(g => g.Piles);

            modelBuilder.Entity<Player>().HasOne(p => p.Game).WithMany(g => g.Players);

            modelBuilder.Entity<Game>().Property(p => p.Type).IsRequired().HasMaxLength(64);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetCreatedAndUpdatedDates();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            SetCreatedAndUpdatedDates();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetCreatedAndUpdatedDates()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                if (entry.State == EntityState.Added && entry.Entity is ICreatedDate createdDate)
                {
                    if (createdDate.Created == DateTime.MinValue)
                    {
                        createdDate.Created = DateTime.Now;
                    }
                }

                if (entry.State != EntityState.Added && entry.Entity is IUpdatedDate updatedDate)
                {
                    updatedDate.Updated = DateTime.Now;
                }
            }
        }
    }
}