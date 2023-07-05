using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>Defines database migration related event arguments.</summary>
    public class MigrationEventArgs : EventArgs
    {
        /// <summary>The name of the migration.</summary>
        public string Migration { get; }

        /// <summary>Constructor.</summary>
        /// <param name="migration">The name of the migration.</param>
        public MigrationEventArgs(string migration)
        {
            Migration = migration.WhenNotNull(nameof(migration));
        }
    }

    /// <summary>An abstract class providing the ability to report and apply pending migrations to a database.
    /// If the database does not exist then it will be created.</summary>
    /// <typeparam name="TDbContext">The <see cref="DbContext"/> representing the database to be updated.</typeparam>
    public abstract class DatabaseMigratorBase<TDbContext> : IDatabaseMigrator where TDbContext : DbContext
    {
        private readonly IDbContextFactory<TDbContext> _dbContextFactory;

        /// <summary>Raised with the name of a migration to be applied to the database.</summary>
        public event EventHandler<MigrationEventArgs> OnNewMigration;

        /// <summary>Constructor.</summary>
        /// <param name="dbContextFactory">The <see cref="DbContext"/> factory used to create an instance of the <typeparamref name="TDbContext"/>.</param>
        public DatabaseMigratorBase(IDbContextFactory<TDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory.WhenNotNull(nameof(dbContextFactory));
        }

        /// <inheritdoc />
        public async Task MigrateAsync()
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var allMigrations = dbContext.Database.GetMigrations();
                var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(CancellationToken.None);
                var pendingMigrations = allMigrations.Except(appliedMigrations).AsReadOnlyCollection();

                var onNewMigration = OnNewMigration;

                if (onNewMigration is not null && pendingMigrations.Any())
                {
                    foreach (var migration in pendingMigrations)
                    {
                        var eventArgs = new MigrationEventArgs(migration);

                        onNewMigration.Invoke(this, eventArgs);
                    }
                }

                // This will create the database if it does not exist
                await dbContext.Database.MigrateAsync(CancellationToken.None);
            }
        }
    }
}
