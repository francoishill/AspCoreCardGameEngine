using System;
using System.Threading;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Models;
using AspCoreCardGameEngine.Api.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AspCoreCardGameEngine.Api.Persistence
{
    public class CardsDbContext : DbContext
    {
        public DbSet<Deck> Decks { get; set; }
        public DbSet<Card> Cards { get; set; }

        public CardsDbContext(DbContextOptions<CardsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().Property(c => c.Suit).IsRequired().HasMaxLength(32).HasConversion(new EnumToStringConverter<CardSuitEnum>());
            modelBuilder.Entity<Card>().Property(c => c.Value).IsRequired();

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