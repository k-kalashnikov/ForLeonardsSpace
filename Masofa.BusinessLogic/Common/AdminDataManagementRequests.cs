using Masofa.Common.Resources;
using Masofa.BusinessLogic.Attributes;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models;
using Masofa.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Masofa.BusinessLogic.Common
{
    /// <summary>
    /// Информация о DbContext
    /// </summary>
    public class DbContextInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// Информация о таблице
    /// </summary>
    public class TableInfo
    {
        public string Name { get; set; }
        public string EntityTypeName { get; set; }
        public long? RowCount { get; set; }
    }

    /// <summary>
    /// Информация о колонке таблицы
    /// </summary>
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public string? ForeignKeyTableName { get; set; }
    }

    /// <summary>
    /// Упрощенная структура таблицы (для визуализации)
    /// </summary>
    public class SimpleTableStructure
    {
        public string TableName { get; set; }
        public string EntityTypeName { get; set; }
        public List<string> PrimaryKeys { get; set; } = new();
        public List<string> Columns { get; set; } = new();
        public List<string> ForeignKeys { get; set; } = new();
    }

    /// <summary>
    /// Информация о внешнем ключе
    /// </summary>
    public class ForeignKeyInfo
    {
        public string Name { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
        public string FromEntityTypeName { get; set; }
        public string ToEntityTypeName { get; set; }
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// Информация о связях таблиц в DbContext (для визуализации как в Navicat)
    /// </summary>
    public class TableRelationshipsInfo
    {
        public List<TableNodeInfo> Tables { get; set; } = new();
        public List<RelationshipEdgeInfo> Relationships { get; set; } = new();
    }

    /// <summary>
    /// Узел таблицы для визуализации
    /// </summary>
    public class TableNodeInfo
    {
        public string TableName { get; set; }
        public string EntityTypeName { get; set; }
        public string DisplayName { get; set; }
        public long? RowCount { get; set; }
        public int ColumnCount { get; set; }
        public List<string> PrimaryKeys { get; set; } = new();
        public List<string> ForeignKeys { get; set; } = new();
        public List<string> ColumnSamples { get; set; } = new();
        public double? X { get; set; } // Позиция для визуализации
        public double? Y { get; set; } // Позиция для визуализации
    }

    /// <summary>
    /// Ребро связи между таблицами
    /// </summary>
    public class RelationshipEdgeInfo
    {
        public string FromTable { get; set; }
        public string FromEntityTypeName { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToEntityTypeName { get; set; }
        public string ToColumn { get; set; }
        public string RelationshipType { get; set; } // "OneToOne", "OneToMany", "ManyToMany"
        public bool IsRequired { get; set; }
    }

    /// <summary>
    /// Запрос для получения списка всех DbContext'ов
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetDbContextsRequest : IRequest<List<DbContextInfo>>
    {
    }

    public class GetDbContextsRequestHandler : IRequestHandler<GetDbContextsRequest, List<DbContextInfo>>
    {
        private readonly ILogger<GetDbContextsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetDbContextsRequestHandler(
            ILogger<GetDbContextsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<DbContextInfo>> Handle(GetDbContextsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);

                var dbContexts = typeof(MasofaCommonDbContext).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(DbContext)) && t != typeof(DbContext))
                    .Select(t => new DbContextInfo
                    {
                        Name = t.Name,
                        FullName = t.FullName,
                        DisplayName = FormatDisplayName(t.Name)
                    })
                    .OrderBy(x => x.DisplayName)
                    .ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"ok"), requestPath);
                return dbContexts;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private string FormatDisplayName(string name)
        {
            return name
                .Replace("Masofa", "")
                .Replace("DbContext", "")
                .Replace("Common", "Common")
                .Replace("Identity", "Identity")
                .Replace("CropMonitoring", "Crop Monitoring")
                .Replace("Dictionaries", "Dictionaries")
                .Replace("Weather", "Weather")
                .Replace("Sentinel", "Sentinel")
                .Replace("Landsat", "Landsat")
                .Replace("Indices", "Indices")
                .Replace("AnaliticReport", "Analytic Report")
                .Replace("Ugm", "UGM")
                .Replace("IBMWeather", "IBM Weather")
                .Replace("UAV", "UAV")
                .Replace("Era", "ERA")
                .Replace("Tile", "Tile")
                .Trim();
        }
    }

    /// <summary>
    /// Запрос для получения списка таблиц в DbContext
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetTablesByDbContextRequest : IRequest<List<TableInfo>>
    {
        /// <summary>
        /// Полное имя типа DbContext
        /// </summary>
        public string DbContextFullName { get; set; }
    }

    public class GetTablesByDbContextRequestHandler : IRequestHandler<GetTablesByDbContextRequest, List<TableInfo>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GetTablesByDbContextRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetTablesByDbContextRequestHandler(
            IServiceProvider serviceProvider,
            ILogger<GetTablesByDbContextRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<TableInfo>> Handle(GetTablesByDbContextRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for context: {request.DbContextFullName}", requestPath);

                var dbContextType = DataManagementTypeResolver.Resolve(request.DbContextFullName);
                if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new ArgumentException($"DbContext type {request.DbContextFullName} not found");
                }

                await using var scope = _serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext {request.DbContextFullName} is not registered in DI");
                }

                var tables = new List<TableInfo>();

                // Получаем все DbSet свойства через рефлексию
                var dbSetProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

                foreach (var dbSetProperty in dbSetProperties)
                {
                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                    var dbSet = dbSetProperty.GetValue(dbContext) as IQueryable;

                    if (dbSet == null) continue;

                    var entityTypeMetadata = dbContext.Model.FindEntityType(entityType);
                    if (entityTypeMetadata == null) continue;

                    var tableName = entityTypeMetadata.GetTableName() ?? entityType.Name;
                    
                    long? rowCount = null;

                    rowCount = await DataManagementDbHelpers.TryGetRowCountAsync(dbContext, entityTypeMetadata, _logger, cancellationToken);

                    tables.Add(new TableInfo
                    {
                        Name = tableName,
                        EntityTypeName = entityType.FullName ?? entityType.Name,
                        RowCount = rowCount
                    });
                }

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);
                return tables.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }

    /// <summary>
    /// Запрос для получения структуры таблицы
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetTableStructureRequest : IRequest<List<ColumnInfo>>
    {
        /// <summary>
        /// Полное имя типа DbContext
        /// </summary>
        public string DbContextFullName { get; set; }

        /// <summary>
        /// Имя типа сущности (EntityTypeName)
        /// </summary>
        public string EntityTypeName { get; set; }
    }

    public class GetTableStructureRequestHandler : IRequestHandler<GetTableStructureRequest, List<ColumnInfo>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GetTableStructureRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetTableStructureRequestHandler(
            IServiceProvider serviceProvider,
            ILogger<GetTableStructureRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<List<ColumnInfo>> Handle(GetTableStructureRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for context: {request.DbContextFullName}, entity: {request.EntityTypeName}", requestPath);

                var dbContextType = DataManagementTypeResolver.Resolve(request.DbContextFullName);
                if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new ArgumentException($"DbContext type {request.DbContextFullName} not found");
                }

                var entityType = DataManagementTypeResolver.Resolve(request.EntityTypeName);

                await using var scope = _serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext {request.DbContextFullName} is not registered in DI");
                }

                var entityTypeMetadata = DataManagementTypeResolver.ResolveEntityTypeMetadata(dbContext, request.EntityTypeName, entityType);
                if (entityTypeMetadata == null)
                {
                    throw new InvalidOperationException($"Entity type {request.EntityTypeName} not found in DbContext");
                }

                var entityClrType = entityType ?? entityTypeMetadata.ClrType;

                var columns = new List<ColumnInfo>();

                foreach (var property in entityTypeMetadata.GetProperties())
                {
                    var propertyClrType = property.ClrType;
                    var typeName = GetTypeDisplayName(propertyClrType);

                    // Проверяем, является ли колонка внешним ключом
                    var foreignKey = entityTypeMetadata.GetForeignKeys()
                        .FirstOrDefault(fk => fk.Properties.Contains(property));
                    
                    string? foreignKeyTableName = null;
                    if (foreignKey != null)
                    {
                        foreignKeyTableName = foreignKey.PrincipalEntityType.GetTableName() ?? 
                                             foreignKey.PrincipalEntityType.Name;
                    }

                    columns.Add(new ColumnInfo
                    {
                        Name = property.Name,
                        Type = typeName,
                        IsNullable = property.IsNullable,
                        IsPrimaryKey = property.IsPrimaryKey(),
                        IsForeignKey = foreignKey != null,
                        ForeignKeyTableName = foreignKeyTableName
                    });
                }

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,"result"), requestPath);
                return columns.OrderBy(c => c.Name).ToList();
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private string GetTypeDisplayName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                var genericName = type.Name.Split('`')[0];
                return $"{genericName}<{string.Join(", ", genericArgs.Select(GetTypeDisplayName))}>";
            }

            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int",
                "Int64" => "long",
                "Boolean" => "bool",
                "DateTime" => "DateTime",
                "Guid" => "Guid",
                "Decimal" => "decimal",
                "Double" => "double",
                "Single" => "float",
                "Byte" => "byte",
                _ => type.Name
            };
        }
    }

    /// <summary>
    /// Запрос для получения данных таблицы с фильтрацией и пагинацией
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetTableDataRequest : IRequest<TableDataResult>
    {
        /// <summary>
        /// Полное имя типа DbContext
        /// </summary>
        public string DbContextFullName { get; set; }

        /// <summary>
        /// Имя типа сущности (EntityTypeName)
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Количество записей для получения
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Смещение для пагинации
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Поле для сортировки
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Направление сортировки (ASC/DESC)
        /// </summary>
        public bool SortDescending { get; set; } = false;

        /// <summary>
        /// Фильтры по полям BaseEntity
        /// </summary>
        public Dictionary<string, object>? Filters { get; set; }
    }

    /// <summary>
    /// Результат запроса данных таблицы
    /// </summary>
    public class TableDataResult
    {
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class GetTableDataRequestHandler : IRequestHandler<GetTableDataRequest, TableDataResult>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GetTableDataRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetTableDataRequestHandler(
            IServiceProvider serviceProvider,
            ILogger<GetTableDataRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<TableDataResult> Handle(GetTableDataRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for context: {request.DbContextFullName}, entity: {request.EntityTypeName}", requestPath);

                var dbContextType = DataManagementTypeResolver.Resolve(request.DbContextFullName);
                if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new ArgumentException($"DbContext type {request.DbContextFullName} not found");
                }

                var entityType = DataManagementTypeResolver.Resolve(request.EntityTypeName);

                await using var scope = _serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext {request.DbContextFullName} is not registered in DI");
                }

                var entityTypeMetadata = DataManagementTypeResolver.ResolveEntityTypeMetadata(dbContext, request.EntityTypeName, entityType);
                if (entityTypeMetadata == null)
                {
                    throw new InvalidOperationException($"Entity type {request.EntityTypeName} not found in DbContext");
                }

                var entityClrType = entityType ?? entityTypeMetadata.ClrType;

                // Получаем DbSet через рефлексию
                var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!
                    .MakeGenericMethod(entityClrType);
                var dbSet = setMethod.Invoke(dbContext, null) as IQueryable;

                if (dbSet == null)
                {
                    throw new InvalidOperationException($"DbSet for {request.EntityTypeName} not found");
                }

                // Применяем фильтры для BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entityClrType) && request.Filters != null)
                {
                    dbSet = ApplyBaseEntityFilters(dbSet, entityClrType, request.Filters);
                }

                // Подсчет общего количества
                var countMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Count" && m.GetParameters().Length == 1)
                    .MakeGenericMethod(entityClrType);
                var countResult = countMethod.Invoke(null, new object[] { dbSet });
                var totalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

                // Применяем сортировку
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    dbSet = ApplySorting(dbSet, entityClrType, request.SortBy, request.SortDescending);
                }

                // Применяем пагинацию
                if (request.Take.HasValue)
                {
                    var skipMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Skip" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(entityClrType);
                    dbSet = skipMethod.Invoke(null, new object[] { dbSet, request.Offset }) as IQueryable;

                    var takeMethod = typeof(Queryable).GetMethods()
                        .First(m => m.Name == "Take" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(entityClrType);
                    dbSet = takeMethod.Invoke(null, new object[] { dbSet, request.Take.Value }) as IQueryable;
                }

                // Получаем данные
                var toListMethod = typeof(Enumerable).GetMethod("ToList")!
                    .MakeGenericMethod(entityClrType);
                var entities = toListMethod.Invoke(null, new object[] { dbSet }) as IEnumerable;

                var result = new TableDataResult
                {
                    TotalCount = totalCount,
                    Data = new List<Dictionary<string, object>>()
                };

                if (entities != null)
                {
                    foreach (var entity in entities)
                    {
                        var dict = new Dictionary<string, object>();
                        var properties = entityClrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(entity);
                            dict[prop.Name] = DataManagementDbHelpers.NormalizeValue(value);
                        }
                        
                        result.Data.Add(dict);
                    }
                }

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private IQueryable ApplyBaseEntityFilters(IQueryable query, Type entityType, Dictionary<string, object> filters)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            System.Linq.Expressions.Expression? combinedExpression = null;

            foreach (var filter in filters)
            {
                var property = entityType.GetProperty(filter.Key);
                if (property == null) continue;

                var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
                var constant = System.Linq.Expressions.Expression.Constant(filter.Value);
                var equals = System.Linq.Expressions.Expression.Equal(propertyAccess, constant);

                combinedExpression = combinedExpression == null 
                    ? equals 
                    : System.Linq.Expressions.Expression.AndAlso(combinedExpression, equals);
            }

            if (combinedExpression == null) return query;

            var lambda = System.Linq.Expressions.Expression.Lambda(combinedExpression, parameter);
            var whereMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            return whereMethod.Invoke(null, new object[] { query, lambda }) as IQueryable;
        }

        private IQueryable ApplySorting(IQueryable query, Type entityType, string sortBy, bool descending)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            var property = entityType.GetProperty(sortBy);
            if (property == null) return query;

            var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
            var lambda = System.Linq.Expressions.Expression.Lambda(propertyAccess, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var orderByMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, property.PropertyType);

            return orderByMethod.Invoke(null, new object[] { query, lambda }) as IQueryable;
        }
    }

    /// <summary>
    /// Запрос для получения упрощенной структуры таблицы
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetSimpleTableStructureRequest : IRequest<SimpleTableStructure>
    {
        /// <summary>
        /// Полное имя типа DbContext
        /// </summary>
        public string DbContextFullName { get; set; }

        /// <summary>
        /// Имя типа сущности
        /// </summary>
        public string EntityTypeName { get; set; }
    }

    public class GetSimpleTableStructureRequestHandler : IRequestHandler<GetSimpleTableStructureRequest, SimpleTableStructure>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GetSimpleTableStructureRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetSimpleTableStructureRequestHandler(
            IServiceProvider serviceProvider,
            ILogger<GetSimpleTableStructureRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<SimpleTableStructure> Handle(GetSimpleTableStructureRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for context: {request.DbContextFullName}, entity: {request.EntityTypeName}", requestPath);

                var dbContextType = DataManagementTypeResolver.Resolve(request.DbContextFullName);
                if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new ArgumentException($"DbContext type {request.DbContextFullName} not found");
                }

                var entityType = DataManagementTypeResolver.Resolve(request.EntityTypeName);

                await using var scope = _serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext {request.DbContextFullName} is not registered in DI");
                }

                var entityTypeMetadata = DataManagementTypeResolver.ResolveEntityTypeMetadata(dbContext, request.EntityTypeName, entityType);
                if (entityTypeMetadata == null)
                {
                    throw new InvalidOperationException($"Entity type {request.EntityTypeName} not found in DbContext");
                }

                var clrType = entityType ?? entityTypeMetadata.ClrType;

                var tableName = entityTypeMetadata.GetTableName() ?? clrType?.Name ?? entityTypeMetadata.Name;
                var primaryKeys = entityTypeMetadata.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList() ?? new List<string>();
                var columns = entityTypeMetadata.GetProperties().Select(p => p.Name).ToList();
                var foreignKeys = entityTypeMetadata.GetForeignKeys()
                    .SelectMany(fk => fk.Properties.Select(p => p.Name))
                    .Distinct()
                    .ToList();

                var result = new SimpleTableStructure
                {
                    TableName = tableName,
                    EntityTypeName = clrType.FullName ?? clrType.Name,
                    PrimaryKeys = primaryKeys,
                    Columns = columns,
                    ForeignKeys = foreignKeys
                };

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }
    }

    /// <summary>
    /// Запрос для получения всех связей между таблицами в DbContext (для визуализации как в Navicat)
    /// </summary>
    [RequestPermission(ActionType = "Read")]
    public class GetTableRelationshipsRequest : IRequest<TableRelationshipsInfo>
    {
        /// <summary>
        /// Полное имя типа DbContext
        /// </summary>
        public string DbContextFullName { get; set; }
    }

    public class GetTableRelationshipsRequestHandler : IRequestHandler<GetTableRelationshipsRequest, TableRelationshipsInfo>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GetTableRelationshipsRequestHandler> _logger;
        private readonly IBusinessLogicLogger _businessLogicLogger;

        public GetTableRelationshipsRequestHandler(
            IServiceProvider serviceProvider,
            ILogger<GetTableRelationshipsRequestHandler> logger,
            IBusinessLogicLogger businessLogicLogger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _businessLogicLogger = businessLogicLogger;
        }

        public async Task<TableRelationshipsInfo> Handle(GetTableRelationshipsRequest request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await _businessLogicLogger.LogInformationAsync($"Request in {requestPath} for context: {request.DbContextFullName}", requestPath);

                var dbContextType = DataManagementTypeResolver.Resolve(request.DbContextFullName);
                if (dbContextType == null || !typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    throw new ArgumentException($"DbContext type {request.DbContextFullName} not found");
                }

                await using var scope = _serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetService(dbContextType) as DbContext;
                if (dbContext == null)
                {
                    throw new InvalidOperationException($"DbContext {request.DbContextFullName} is not registered in DI");
                }

                var result = new TableRelationshipsInfo();
                var tableNodes = new Dictionary<string, TableNodeInfo>();

                // Получаем все таблицы
                var dbSetProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

                foreach (var dbSetProperty in dbSetProperties)
                {
                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                    var entityTypeMetadata = dbContext.Model.FindEntityType(entityType);
                    if (entityTypeMetadata == null) continue;

                    var tableName = entityTypeMetadata.GetTableName() ?? entityType.Name;
                    var entityTypeName = entityType.FullName ?? entityType.Name;

                    // Получаем количество записей
                    long? rowCount = null;
                    if (typeof(BaseEntity).IsAssignableFrom(entityType))
                    {
                        rowCount = await DataManagementDbHelpers.TryGetRowCountAsync(dbContext, entityTypeMetadata, _logger, cancellationToken);
                    }

                    var columnCount = entityTypeMetadata.GetProperties().Count();
                    var primaryKeys = entityTypeMetadata.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList() ?? new List<string>();
                    var foreignKeys = entityTypeMetadata.GetForeignKeys()
                        .SelectMany(fk => fk.Properties.Select(p => p.Name))
                        .Distinct()
                        .ToList();
                    var columnSamples = entityTypeMetadata.GetProperties()
                        .OrderBy(p => p.Name)
                        .Take(6)
                        .Select(p => $"{p.Name} ({GetShortTypeName(p.ClrType)})")
                        .ToList();

                    tableNodes[entityTypeName] = new TableNodeInfo
                    {
                        TableName = tableName,
                        EntityTypeName = entityTypeName,
                        DisplayName = tableName,
                        RowCount = rowCount,
                        ColumnCount = columnCount,
                        PrimaryKeys = primaryKeys,
                        ForeignKeys = foreignKeys,
                        ColumnSamples = columnSamples
                    };
                }

                result.Tables = tableNodes.Values.OrderBy(t => t.TableName).ToList();
                ApplyLayout(result.Tables);

                // Получаем все связи
                var relationships = new List<RelationshipEdgeInfo>();

                foreach (var entityTypeMetadata in dbContext.Model.GetEntityTypes())
                {
                    var fromTableName = entityTypeMetadata.GetTableName() ?? entityTypeMetadata.Name;
                    var fromEntityTypeName = entityTypeMetadata.ClrType.FullName ?? entityTypeMetadata.ClrType.Name;

                    // Внешние ключи (от текущей таблицы к другим)
                    foreach (var foreignKey in entityTypeMetadata.GetForeignKeys())
                    {
                        var toEntityType = foreignKey.PrincipalEntityType;
                        var toTableName = toEntityType.GetTableName() ?? toEntityType.Name;
                        var toEntityTypeName = toEntityType.ClrType.FullName ?? toEntityType.ClrType.Name;

                        // Пропускаем связи с самим собой
                        if (fromEntityTypeName == toEntityTypeName) continue;

                        var fromColumn = foreignKey.Properties.FirstOrDefault()?.Name ?? "";
                        var toColumn = foreignKey.PrincipalKey.Properties.FirstOrDefault()?.Name ?? "";

                        // Определяем тип связи
                        var relationshipType = "OneToMany"; // По умолчанию
                        if (foreignKey.IsUnique)
                        {
                            relationshipType = "OneToOne";
                        }

                        relationships.Add(new RelationshipEdgeInfo
                        {
                            FromTable = fromTableName,
                            FromEntityTypeName = fromEntityTypeName,
                            FromColumn = fromColumn,
                            ToTable = toTableName,
                            ToEntityTypeName = toEntityTypeName,
                            ToColumn = toColumn,
                            RelationshipType = relationshipType,
                            IsRequired = foreignKey.Properties.Any(p => !p.IsNullable)
                        });
                    }

                    // Обратные связи (от других таблиц к текущей)
                    foreach (var foreignKey in dbContext.Model.GetEntityTypes()
                        .SelectMany(e => e.GetForeignKeys())
                        .Where(fk => fk.PrincipalEntityType == entityTypeMetadata && fk.DeclaringEntityType != entityTypeMetadata))
                    {
                        var fromEntityType = foreignKey.DeclaringEntityType;
                        var fromTableName2 = fromEntityType.GetTableName() ?? fromEntityType.Name;
                        var fromEntityTypeName2 = fromEntityType.ClrType.FullName ?? fromEntityType.ClrType.Name;

                        var fromColumn = foreignKey.Properties.FirstOrDefault()?.Name ?? "";
                        var toColumn = foreignKey.PrincipalKey.Properties.FirstOrDefault()?.Name ?? "";

                        // Проверяем, не добавили ли мы уже эту связь
                        if (!relationships.Any(r => r.FromEntityTypeName == fromEntityTypeName2 && 
                                                   r.ToEntityTypeName == fromEntityTypeName &&
                                                   r.FromColumn == fromColumn))
                        {
                            relationships.Add(new RelationshipEdgeInfo
                            {
                                FromTable = fromTableName2,
                                FromEntityTypeName = fromEntityTypeName2,
                                FromColumn = fromColumn,
                                ToTable = fromTableName,
                                ToEntityTypeName = fromEntityTypeName,
                                ToColumn = toColumn,
                                RelationshipType = "ManyToOne",
                                IsRequired = foreignKey.Properties.Any(p => !p.IsNullable)
                            });
                        }
                    }
                }

                result.Relationships = relationships
                    .GroupBy(r => $"{r.FromEntityTypeName}_{r.FromColumn}_{r.ToEntityTypeName}_{r.ToColumn}")
                    .Select(g => g.First())
                    .ToList();

                await _businessLogicLogger.LogInformationAsync(LogMessageResource.RequestFinishedWithResult(requestPath,result.ToString()), requestPath);
                return result;
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath,ex.Message);
                await _businessLogicLogger.LogCriticalAsync(msg, requestPath);
                _logger.LogCritical(ex, msg);
                throw;
            }
        }

        private void ApplyLayout(IList<TableNodeInfo> nodes)
        {
            if (nodes == null || nodes.Count == 0) return;

            var count = nodes.Count;
            var radius = count switch
            {
                <= 3 => 25,
                <= 6 => 32,
                <= 12 => 38,
                _ => 45
            };

            for (var i = 0; i < count; i++)
            {
                var angle = (2 * Math.PI * i) / count;
                var x = 50 + radius * Math.Cos(angle);
                var y = 50 + radius * Math.Sin(angle);

                nodes[i].X = Math.Round(x, 2);
                nodes[i].Y = Math.Round(y, 2);
            }
        }

        private static string GetShortTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                var genericName = type.Name.Split('`')[0];
                return $"{genericName}<{string.Join(", ", genericArgs.Select(GetShortTypeName))}>";
            }

            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int",
                "Int64" => "long",
                "Boolean" => "bool",
                "DateTime" => "DateTime",
                "Guid" => "Guid",
                "Decimal" => "decimal",
                "Double" => "double",
                "Single" => "float",
                "Byte" => "byte",
                _ => type.Name
            };
        }
    }

    internal static class DataManagementDbHelpers
    {
        public static async Task<long?> TryGetRowCountAsync(DbContext dbContext, IEntityType entityTypeMetadata, ILogger logger, CancellationToken cancellationToken)
        {
            try
            {
                var connection = dbContext.Database.GetDbConnection();
                if (connection == null)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(connection.ConnectionString))
                {
                    logger.LogDebug("Connection string not configured for context {DbContext}", dbContext.GetType().FullName);
                    return null;
                }

                var tableName = entityTypeMetadata.GetTableName();
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    return null;
                }

                var schema = entityTypeMetadata.GetSchema();
                var tableIdentifier = string.IsNullOrEmpty(schema)
                    ? QuoteIdentifier(tableName)
                    : $"{QuoteIdentifier(schema)}.{QuoteIdentifier(tableName)}";

                await using var command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM {tableIdentifier}";
                command.CommandType = CommandType.Text;

                var shouldClose = connection.State != ConnectionState.Open;
                if (shouldClose)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                try
                {
                    var result = await command.ExecuteScalarAsync(cancellationToken);
                    if (result == null || result == DBNull.Value)
                    {
                        return null;
                    }

                    return Convert.ToInt64(result);
                }
                finally
                {
                    if (shouldClose)
                    {
                        try
                        {
                            await connection.CloseAsync();
                        }
                        catch (NotImplementedException)
                        {
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to count rows for table {Table}", entityTypeMetadata.GetTableName());
                return null;
            }
        }

        private static string QuoteIdentifier(string identifier)
        {
            return $"\"{identifier.Replace("\"", "\"\"")}\"";
        }

        public static object? NormalizeValue(object? value)
        {
            if (value == null)
            {
                return null;
            }

            switch (value)
            {
                case Geometry geometry:
                    return new
                    {
                        type = geometry.GeometryType,
                        srid = geometry.SRID,
                        wkt = geometry.AsText()
                    };
                case DateTime dateTime:
                    return dateTime.ToString("O");
                case DateTimeOffset dateTimeOffset:
                    return dateTimeOffset.ToString("O");
                case Guid guid:
                    return guid.ToString();
            }

            var type = value.GetType();
            if (IsSimpleType(type))
            {
                return value;
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                var list = new List<object?>();
                foreach (var item in enumerable)
                {
                    list.Add(NormalizeValue(item));
                }
                return list;
            }

            return value.ToString();
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type.Equals(typeof(string)) ||
                   type.Equals(typeof(decimal)) ||
                   type.Equals(typeof(double)) ||
                   type.Equals(typeof(float)) ||
                   type.Equals(typeof(long)) ||
                   type.Equals(typeof(int)) ||
                   type.Equals(typeof(short)) ||
                   type.Equals(typeof(byte)) ||
                   type.Equals(typeof(bool));
        }
    }

    internal static class DataManagementTypeResolver
    {
        public static Type? Resolve(string? typeFullName)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
            {
                return null;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetType(typeFullName, throwOnError: false, ignoreCase: false);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch
                {
                    // Пропускаем проблемы с рефлексивной загрузкой отдельных сборок
                }
            }

            return null;
        }

        public static IEntityType? ResolveEntityTypeMetadata(DbContext dbContext, string entityTypeFullName, Type? resolvedType)
        {
            return dbContext.Model.GetEntityTypes().FirstOrDefault(et =>
                (resolvedType != null && et.ClrType == resolvedType) ||
                string.Equals(et.ClrType.FullName, entityTypeFullName, StringComparison.Ordinal) ||
                string.Equals(et.ClrType.Name, entityTypeFullName, StringComparison.Ordinal) ||
                string.Equals(et.Name, entityTypeFullName, StringComparison.Ordinal) ||
                string.Equals(et.GetTableName(), entityTypeFullName, StringComparison.OrdinalIgnoreCase));
        }
    }
}

