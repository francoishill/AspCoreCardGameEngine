using System.Collections.Generic;
using AspCoreCardGameEngine.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AspCoreCardGameEngine.Api
{
    public class StartupHelper
    {
        private readonly CardsDbContext _dbContext;

        public StartupHelper(CardsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<string> ApplyMysqlSchema()
        {
            var migrations = _dbContext.Database.GetPendingMigrations();
            _dbContext.Database.Migrate();
            return migrations;
        }
    }
}