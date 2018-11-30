using AspNetCore.ApiBase.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Data.Helpers
{
    public static class JsonQueryableExtensions
    {
        public static JsonQueryable<TEntity> AsJsonQueryable<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
        {
            return new JsonQueryable<TEntity>(dbSet);
        }
    }

    public class JsonQueryable<TEntity>
         where TEntity : class
    {
        private readonly IEntityType _entityType;
        private readonly DbSet<TEntity> _dbSet;
        private readonly List<(string column, string operation, string key, string param)> _orConditions = new List<(string column, string operation, string key, string param)>();

        public JsonQueryable(DbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
            var dbContext = _dbSet.GetDbContext();

            var model = dbContext.Model;
            var entityTypes = model.GetEntityTypes();
            _entityType = entityTypes.First(t => t.ClrType == typeof(TEntity));
        }

        public JsonQueryable<TEntity> Like(Expression<Func<TEntity, LocalizedString>> propertyLambda, string value)
        {
            var jsonKey = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return Like(propertyLambda, jsonKey, value);
        }

        public JsonQueryable<TEntity> Like<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
        where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
            var col = _entityType.GetColumnName(propertyInfo);
            jsonKey = $".{jsonKey}";

            _orConditions.Add((col, "Like", jsonKey, value));

            return this;
        }

        public JsonQueryable<TEntity> StartsWith<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
      where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
            var col = _entityType.GetColumnName(propertyInfo);
            jsonKey = $".{jsonKey}";

            _orConditions.Add((col, "StartsWith", jsonKey, value));

            return this;
        }

        public JsonQueryable<TEntity> EndsWith<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
      where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
            var col = _entityType.GetColumnName(propertyInfo);
            jsonKey = $".{jsonKey}";

            _orConditions.Add((col, "EndsWith", jsonKey, value));

            return this;
        }

        public JsonQueryable<TEntity> Equals<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
        where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
        var col = _entityType.GetColumnName(propertyInfo);
        jsonKey = $".{jsonKey}";

            _orConditions.Add((col, "=", jsonKey, value));

            return this;
        }

        public JsonQueryable<TEntity> NotEquals<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
        where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
            var col = _entityType.GetColumnName(propertyInfo);
            jsonKey = $".{jsonKey}";

            _orConditions.Add((col, "!=", jsonKey, value));

            return this;
        }

        public JsonQueryable<TEntity> ArrayContains<TProperty>(Expression<Func<TEntity, TProperty>> propertyLambda, string jsonKey, string value)
        where TProperty : class
        {
            var propertyInfo = EntityPropertyExtensions.GetPropertyInfo(propertyLambda);
            var col = _entityType.GetColumnName(propertyInfo);
            if (!string.IsNullOrEmpty(jsonKey))
            {
                jsonKey = $".{jsonKey}";
            }

            _orConditions.Add((col, "ArrayContains", jsonKey, value));

            return this;
        }

        public List<TEntity> ToList()
        {
            return AsQueryable().ToList();
        }

        public Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return AsQueryable().ToListAsync(cancellationToken);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            var tableName = _dbSet.GetTableName();

            var parameters = new List<object>();

            var sb = new StringBuilder();
            sb.AppendLine($"SELECT * FROM {tableName} WHERE 1=1");

            foreach (var condition in _orConditions)
            {
                switch (condition.operation)
                {
                    case "Like":
                        sb.AppendLine($"OR (JSON_VALUE({condition.column},'${condition.key}') LIKE '%{{{parameters.Count()}}}%')");
                        break;
                    case "StartsWith":
                        sb.AppendLine($"OR (JSON_VALUE({condition.column},'${condition.key}') LIKE '{{{parameters.Count()}}}%')");
                        break;
                    case "EndsWith":
                        sb.AppendLine($"OR (JSON_VALUE({condition.column},'${condition.key}') LIKE '%{{{parameters.Count()}}}')");
                        break;
                    case "=":
                        sb.AppendLine($"OR (JSON_VALUE({condition.column},'${condition.key}') = '{{{parameters.Count()}}}')");
                        break;
                    case "!=":
                        sb.AppendLine($"OR (JSON_VALUE({condition.column},'${condition.key}') != '{{{parameters.Count()}}}')");
                        break;
                    case "ArrayContains":
                        sb.AppendLine($"OR ('{parameters.Count()}' IN(SELECT value FROM OPENJSON({condition.column},'${condition.key}')))");
                        break;
                    default:
                        throw new Exception("Unsupported operation");
                }
                parameters.Add(condition.param);
            }

            var finalQuery = sb.ToString();
            var queryable = _dbSet.FromSql(finalQuery, parameters);

            return queryable;
        }
    }

    public static class EntityPropertyExtensions
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
       Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        public static string GetColumnName(this IEntityType entityType, PropertyInfo propertyInfo)
        {
            return entityType.FindProperty("").Relational().ColumnName;
        }
    }

    public static class DbSetExtensions
    {
        public static DbContext GetDbContext<T>(this IQueryable<T> dbSet) where T : class
        {
            var infrastructure = dbSet as IInfrastructure<IServiceProvider>;
            var serviceProvider = infrastructure.Instance;
            var currentDbContext = serviceProvider.GetService(typeof(ICurrentDbContext))
                                       as ICurrentDbContext;
            return currentDbContext.Context;
        }

        public static string GetTableName<T>(this DbSet<T> dbSet) where T : class
        {
            var dbContext = dbSet.GetDbContext();

            var model = dbContext.Model;
            var entityTypes = model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == typeof(T));
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
            var tableName = tableNameAnnotation.Value.ToString();
            return tableName;
        }
    }

}
