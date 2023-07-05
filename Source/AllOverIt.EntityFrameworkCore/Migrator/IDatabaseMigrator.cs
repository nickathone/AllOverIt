using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Migrator
{
    /// <summary>Represents a database migrator.</summary>
    public interface IDatabaseMigrator
    {
        /// <summary>Applies pending migrations to a database. If the database does not exist then it will be created.</summary>
        /// <returns>A task that completes when the migration is completely applied.</returns>
        Task MigrateAsync();
    }
}
