using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Data.Extensions
{
    public static class DatabaseFacadeExtensions
    {
        public static void MigrateIfNotApplied(this DatabaseFacade databaseFacade, string targetMigration)
        {
            var connectionTimeout = databaseFacade.GetDbConnection().ConnectionTimeout;
            databaseFacade.SetCommandTimeout(connectionTimeout);

            var platformMigrator = databaseFacade.GetService<IMigrator>();
            var appliedMigrations = databaseFacade.GetAppliedMigrations();
            if (!appliedMigrations.Any(x => x.EqualsInvariant(targetMigration)))
            {
                platformMigrator.Migrate(targetMigration);
            }
        }

        /// <summary>
        /// Applies given migration retrospectively, that is without reverting later migrations if they have already been
        /// applied.
        /// </summary>
        public static void ApplyPatchMigration(this DatabaseFacade databaseFacade, string patchMigration)
        {
            var connectionTimeout = databaseFacade.GetDbConnection().ConnectionTimeout;
            databaseFacade.SetCommandTimeout(connectionTimeout);

            var platformMigrator = databaseFacade.GetService<IMigrator>();
            var firstMigrationAppliedAtOrAfterPatchTime = databaseFacade.GetAppliedMigrations()
                .SkipWhile(id => string.Compare(id, patchMigration, StringComparison.Ordinal) < 0)
                .FirstOrDefault();

            if (firstMigrationAppliedAtOrAfterPatchTime == null)
            {
                // No migrations applied after patch - we can apply patch normally
                platformMigrator.Migrate(patchMigration);
            }
            else if (string.Compare(firstMigrationAppliedAtOrAfterPatchTime, patchMigration, StringComparison.Ordinal) > 0)
            {
                // Migrations applied after patch - we need to apply the patch retrospectively
                databaseFacade.ApplyMigrationInIsolation(patchMigration);
            }
            // Else: patch already applied - do nothing
        }

        /// <summary>
        /// Applies a migration directly, without reverting or applying other migrations.
        /// </summary>
        private static void ApplyMigrationInIsolation(this DatabaseFacade databaseFacade, string migrationId)
        {
            if (databaseFacade.ProviderName == null)
            {
                throw new InvalidOperationException("Cannot apply migration when the database provider is not set");
            }

            var relationalCommandDiagnosticsLogger = databaseFacade.GetService<IRelationalCommandDiagnosticsLogger>();
            var currentDbContext = databaseFacade.GetService<ICurrentDbContext>();

            var migrationsAssembly = databaseFacade.GetService<IMigrationsAssembly>();
            var migrationType = migrationsAssembly.Migrations[migrationId];
            var migration = migrationsAssembly.CreateMigration(migrationType, databaseFacade.ProviderName);
            var migrationsSqlGenerator = databaseFacade.GetService<IMigrationsSqlGenerator>();
            var migrationCommands = migrationsSqlGenerator.Generate(migration.UpOperations);

            var rawSqlCommandBuilder = databaseFacade.GetService<IRawSqlCommandBuilder>();
            var migrationEntry = new HistoryRow(migrationId, ProductInfo.GetVersion());
            var historyRepository = databaseFacade.GetService<IHistoryRepository>();
            var insertMigrationEntrySqlCommand = rawSqlCommandBuilder.Build(historyRepository.GetInsertScript(migrationEntry));
            var insertMigrationEntryCommand = new MigrationCommand(insertMigrationEntrySqlCommand, currentDbContext.Context, relationalCommandDiagnosticsLogger);

            var migrationCommandExecutor = databaseFacade.GetService<IMigrationCommandExecutor>();
            var relationalConnection = databaseFacade.GetService<IRelationalConnection>();
            migrationCommandExecutor.ExecuteNonQuery(migrationCommands.Append(insertMigrationEntryCommand), relationalConnection);
        }
    }
}
