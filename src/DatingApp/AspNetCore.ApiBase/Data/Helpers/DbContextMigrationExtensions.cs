using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase.Data.Helpers
{
    public static class DbContextMigrationExtensions
    {
        #region Exists
        public static bool Exists(this DatabaseFacade databaseFacade)
        {
            var relationalDatabaseCreator = databaseFacade.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if(relationalDatabaseCreator != null)
            {
                return relationalDatabaseCreator.Exists();
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Migration Script
        public static string GenerateMigrationScript(this DbContext context, string fromMigration = null, string toMigration = null)
        {
            return context.GetService<IMigrator>().GenerateScript(fromMigration, toMigration);
        }
        #endregion

        #region Pending Migrations 
        public static IEnumerable<string> PendingMigrations(this DatabaseFacade databaseFacade)
        {
            return databaseFacade.GetPendingMigrations().ToList();
        }
        #endregion

        #region Migrations 
        public static IEnumerable<string> Migrations(this DbContext context)
        {
            return context.Database.GetMigrations().ToList();
        }
        #endregion

        #region Apply Pending Migrations
        public static void EnsureMigratedStepByStep(this DatabaseFacade databaseFacade)
        {
            var pendingMigrations = PendingMigrations(databaseFacade);
            if (pendingMigrations.Any())
            {
                var migrator = databaseFacade.GetService<IMigrator>();
                foreach (var targetMigration in pendingMigrations)
                {
                    var sql = migrator.GenerateScript(targetMigration, targetMigration);
                    migrator.Migrate(targetMigration);
                }
            }
        }
        #endregion

        #region Get Entity Model Types
        public static IEnumerable<IEntityType> GetEntityTypes(this DbContext context)
        {
            return context.Model.GetEntityTypes();
        }

        public static IEnumerable<Type> GetModelTypes(this DbContext context)
        {
            return context.Model.GetEntityTypes().Select(t => t.ClrType);
        }
        #endregion

        #region ClearData
        public static void ClearData(this DbContext context)
        {
            var modelTypes = context.GetModelTypes();
            foreach (var entityType in modelTypes)
            {
                var dbSet = Set(context, entityType);
                var removeRangeMethod = typeof(DbSet<>).MakeGenericType(entityType).GetMethod("RemoveRange");
                removeRangeMethod.Invoke(dbSet, new object[] { dbSet });
            }
        }

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));
        private static IQueryable Set(this DbContext context, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
        #endregion

        #region Ensure Tables Created at a DbContext Level
        public static void EnsureTablesCreated(this DbContext context)
        {
            var created = context.Database.EnsureCreated();
            if (!created)
            {
                var dependencies = context.Database.GetService<RelationalDatabaseCreatorDependencies>();
                var createTablesCommands = dependencies.MigrationsSqlGenerator.Generate(dependencies.ModelDiffer.GetDifferences(null, dependencies.Model), dependencies.Model);
                foreach (var createTableCommand in createTablesCommands)
                {
                    try
                    {
                        dependencies.MigrationCommandExecutor.ExecuteNonQuery(new MigrationCommand[] { createTableCommand }, dependencies.Connection);
                    }
                    catch
                    {

                    }
                }
            }
        }
        #endregion

        #region Delete Tables and Migrations DbContent Level
        public static void EnsureTablesAndMigrationsDeleted(this DbContext context)
        {
            if (context.Database.Exists())
            {
                var modelTypes = context.GetModelTypes();

                var tableNames = new List<string>();
                foreach (var entityType in modelTypes)
                {
                    var mapping = context.Model.FindEntityType(entityType).Relational();
                    var schema = mapping.Schema;
                    var tableName = mapping.TableName;

                    var schemaAndTableName = $"[{tableName}]";
                    tableNames.Add(schemaAndTableName);
                }

                var commands = new List<String>();
                using (var transaction = context.Database.BeginTransaction())
                {

                    //Drop tables
                    foreach (var tableName in tableNames)
                    {
                        foreach (var t in tableNames)
                        {
                            try
                            {
                                var command = $"DROP TABLE IF EXISTS {t}";
                                context.Database.ExecuteSqlCommand(new RawSqlString(command));
                                commands.Add(command);
                            }
                            catch
                            {

                            }
                        }
                    }

                    //Drop Migrations
                    //context.Database.ExecuteSqlCommand(new RawSqlString($"DROP TABLE IF EXISTS  [__EFMigrationsHistory]"));

                    //Delete migrations
                    foreach (var migrationId in context.Migrations())
                    {
                        try
                        {
                            var command = $"DELETE FROM [__EFMigrationsHistory] WHERE MigrationId = '{migrationId}'";
                            context.Database.ExecuteSqlCommand(command);
                            commands.Add(command);
                        }
                        catch
                        {

                        }
                    }

                    transaction.Rollback();
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    foreach (var command in commands)
                    {
                        context.Database.ExecuteSqlCommand(new RawSqlString(command));
                    }

                    transaction.Commit();
                }
            }
        }
        #endregion
    }
}
