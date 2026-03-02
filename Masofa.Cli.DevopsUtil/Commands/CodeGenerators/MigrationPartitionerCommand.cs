using Masofa.Cli;
using Masofa.Common.Attributes;
using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    public class MigrationPartitionerCommandParameters
    {
        [TaskParameter("Путь к папке с миграциями", false, "../../../../Masofa.DataAccess/Migrations")]
        public string MigrationsDir { get; set; } = "../../../../Masofa.DataAccess/Migrations";
        
        [TaskParameter("Путь к папке с моделями", false, "../../../../Masofa.Common/Models")]
        public string ModelsDir { get; set; } = "../../../../Masofa.Common/Models";
        
        [TaskParameter("Режим dry-run (только просмотр без изменений)", false, "false")]
        public bool DryRun { get; set; } = false;

        public static MigrationPartitionerCommandParameters Parse(string[] args)
        {
            var parameters = new MigrationPartitionerCommandParameters();
            
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
                parameters.MigrationsDir = args[0];
                
            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
                parameters.ModelsDir = args[1];
                
            if (args.Length > 2 && args[2].ToLower() == "true")
                parameters.DryRun = true;
                
            return parameters;
        }

        public static MigrationPartitionerCommandParameters GetFromUser()
        {
            Console.Write("Введите путь к папке с миграциями (или нажмите Enter для ../../Masofa.DataAccess/Migrations): ");
            var migrationsDir = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(migrationsDir))
                migrationsDir = "../../../../Masofa.DataAccess/Migrations";

            Console.Write("Введите путь к папке с моделями (или нажмите Enter для ../../Masofa.Common/Models): ");
            var modelsDir = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(modelsDir))
                modelsDir = "../../../../Masofa.Common/Models";

            Console.Write("Режим dry-run? (y/N): ");
            var dryRun = Console.ReadLine()?.Trim().ToLower() == "y";

            return new MigrationPartitionerCommandParameters 
            { 
                MigrationsDir = migrationsDir, 
                ModelsDir = modelsDir, 
                DryRun = dryRun 
            };
        }
    }

    [BaseCommand("Migration Partitioner", "Модифицирует миграции EF Core для добавления партиционирования", typeof(MigrationPartitionerCommandParameters))]
    public class MigrationPartitionerCommand : IBaseCommand
    {
        private readonly HashSet<string> _partitionedTables = new();
        private readonly Dictionary<string, string> _tableMappings = new(); // EntityName -> TableName

        public void Dispose()
        {
        }



        public async Task Execute()
        {
            var parameters = MigrationPartitionerCommandParameters.GetFromUser();
            await ExecuteCore(parameters);
        }

        public async Task Execute(string[] args)
        {
            var parameters = MigrationPartitionerCommandParameters.Parse(args);
            await ExecuteCore(parameters);
        }

        private async Task ExecuteCore(MigrationPartitionerCommandParameters parameters)
        {
            try
            {
                Console.WriteLine("=== Migration Partitioner для EF Core ===");
                Console.WriteLine($"\nПапка миграций: {parameters.MigrationsDir}");
                Console.WriteLine($"Папка моделей: {parameters.ModelsDir}");
                Console.WriteLine($"Режим: {(parameters.DryRun ? "DRY RUN" : "APPLY CHANGES")}");
                Console.WriteLine();

                if (!Directory.Exists(parameters.MigrationsDir))
                {
                    Console.WriteLine($"Ошибка: Папка миграций не найдена: {parameters.MigrationsDir}");
                    return;
                }

                if (!Directory.Exists(parameters.ModelsDir))
                {
                    Console.WriteLine($"Ошибка: Папка моделей не найдена: {parameters.ModelsDir}");
                    return;
                }

                FindPartitionedEntities(parameters.ModelsDir);

                if (_partitionedTables.Count == 0)
                {
                    Console.WriteLine("Не найдено партиционированных таблиц. Завершение.");
                    return;
                }

                ProcessAllMigrations(parameters.MigrationsDir, parameters.DryRun);

                Console.WriteLine("\n=== Завершено ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
        }

        private void FindPartitionedEntities(string modelsDir)
        {
            Console.WriteLine("Поиск сущностей с атрибутом [PartitionedTable]...");

            // Получаем все сборки
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t.GetCustomAttribute<PartitionedTableAttribute>() != null);

                    foreach (var type in types)
                    {
                        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                        var tableName = tableAttribute?.Name ?? type.Name;
                        
                        _partitionedTables.Add(tableName);
                        _tableMappings[type.Name] = tableName;
                        
                        Console.WriteLine($"  Найдена партиционированная сущность: {type.Name} -> {tableName}");
                    }
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки при загрузке сборок
                    continue;
                }
            }

            Console.WriteLine($"Всего найдено партиционированных таблиц: {_partitionedTables.Count}");
        }

        private void ProcessAllMigrations(string migrationsDir, bool dryRun)
        {
            Console.WriteLine($"Обработка миграций в папке: {migrationsDir}");

            var migrationFiles = new List<string>();
            foreach (var subdir in Directory.GetDirectories(migrationsDir))
            {
                migrationFiles.AddRange(Directory.GetFiles(subdir, "*.cs"));
            }

            Console.WriteLine($"Найдено файлов миграций: {migrationFiles.Count}");

            var modifiedCount = 0;
            foreach (var filePath in migrationFiles)
            {
                if (ProcessMigrationFile(filePath, dryRun))
                {
                    modifiedCount++;
                }
            }

            Console.WriteLine($"Обработано файлов: {modifiedCount}");
        }

        private bool ProcessMigrationFile(string filePath, bool dryRun)
        {
            Console.WriteLine($"Обработка файла: {filePath}");

            var content = File.ReadAllText(filePath, Encoding.UTF8);
            var originalContent = content;
            var modified = false;

            // Ищем все вызовы CreateTable
            var pattern = @"migrationBuilder\.CreateTable\(\s*name:\s*""([^""]+)""";
            var matches = Regex.Matches(content, pattern);

            foreach (Match match in matches.Cast<Match>().Reverse())
            {
                var tableName = match.Groups[1].Value;
                
                if (!_partitionedTables.Contains(tableName))
                    continue;

                Console.WriteLine($"  Найдена партиционированная таблица: {tableName}");

                // Находим конец CreateTable statement
                var stmtEnd = FindStatementEnd(content, match.Index);
                if (stmtEnd == -1)
                    continue;

                var createCall = content.Substring(match.Index, stmtEnd - match.Index);
                
                // Извлекаем блок колонок
                var columnsBlock = ExtractColumnsBlock(createCall);
                if (string.IsNullOrEmpty(columnsBlock))
                    continue;

                var columns = ParseColumns(columnsBlock);
                var indent = GetIndent(content, match.Index);
                var replacement = BuildPartitionedSql(tableName, columns, indent);

                // Заменяем полностью выражение
                content = content.Substring(0, match.Index) + replacement + content.Substring(stmtEnd);
                modified = true;
            }

            if (modified)
            {
                if (dryRun)
                {
                    Console.WriteLine($"  [DRY RUN] Файл будет изменен: {filePath}");
                    ShowDiff(originalContent, content);
                }
                else
                {
                    // Создаем backup
                    var backupPath = filePath + ".backup";
                    File.WriteAllText(backupPath, originalContent, Encoding.UTF8);
                    
                    // Записываем измененный контент
                    File.WriteAllText(filePath, content, Encoding.UTF8);
                    
                    Console.WriteLine($"  Файл изменен: {filePath}");
                    Console.WriteLine($"  Backup создан: {backupPath}");
                }
            }

            return modified;
        }

        private int FindStatementEnd(string content, int callStart)
        {
            var parOpen = content.IndexOf('(', callStart);
            if (parOpen == -1)
                return -1;

            var braceCount = 0;
            var inTable = false;
            var start = parOpen;

            for (int i = parOpen; i < content.Length; i++)
            {
                var ch = content[i];
                if (ch == '(')
                {
                    braceCount++;
                    if (braceCount == 1)
                        inTable = true;
                }
                else if (ch == ')')
                {
                    braceCount--;
                    if (braceCount == 0 && inTable)
                    {
                        // Ищем точку с запятой
                        var j = i + 1;
                        while (j < content.Length && char.IsWhiteSpace(content[j]))
                            j++;
                        
                        if (j < content.Length && content[j] == ';')
                            return j + 1;
                    }
                }
            }

            return -1;
        }

        private string ExtractColumnsBlock(string createTableCall)
        {
            var match = Regex.Match(createTableCall, @"columns:\s*table\s*=>\s*new\s*\{", RegexOptions.Singleline);
            if (!match.Success)
                return string.Empty;

            var startBrace = match.Index + match.Length - 1; // на '{'
            var braceCount = 0;
            
            for (int i = startBrace; i < createTableCall.Length; i++)
            {
                var ch = createTableCall[i];
                if (ch == '{')
                    braceCount++;
                else if (ch == '}')
                {
                    braceCount--;
                    if (braceCount == 0)
                        return createTableCall.Substring(startBrace, i - startBrace + 1);
                }
            }

            return string.Empty;
        }

        private List<(string Name, string SqlType, bool NotNull)> ParseColumns(string columnsBlock)
        {
            var columns = new List<(string Name, string SqlType, bool NotNull)>();

            // Убираем внешние { }
            var inner = columnsBlock.Trim();
            if (inner.StartsWith("{") && inner.EndsWith("}"))
                inner = inner.Substring(1, inner.Length - 2);

            // Поиск выражений вида: Name = table.Column<T>(type: "...", nullable: true/false, ...)
            var pattern = @"(\w+)\s*=\s*table\.Column<[^>]+>\((.*?)\)";
            var matches = Regex.Matches(inner, pattern, RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var colName = match.Groups[1].Value;
                var args = match.Groups[2].Value;

                // Ищем type: "..."
                var typeMatch = Regex.Match(args, @"type\s*:\s*""([^""]+)""");
                var sqlType = typeMatch.Success ? typeMatch.Groups[1].Value : "text";

                // Ищем nullable: true/false
                var nullMatch = Regex.Match(args, @"nullable\s*:\s*(true|false)");
                var notNull = true;
                if (nullMatch.Success)
                    notNull = (nullMatch.Groups[1].Value == "false");

                columns.Add((colName, sqlType, notNull));
            }

            return columns;
        }

        private string GetIndent(string content, int idx)
        {
            var lineStart = content.LastIndexOf('\n', idx);
            if (lineStart == -1)
                lineStart = 0;
            else
                lineStart++;

            var j = lineStart;
            while (j < content.Length && (content[j] == '\t' || content[j] == ' '))
                j++;

            return content.Substring(lineStart, j - lineStart);
        }

        private string BuildPartitionedSql(string tableName, List<(string Name, string SqlType, bool NotNull)> columns, string indent)
        {
            // Формируем строки колонок
            var colLines = new List<string>();
            foreach (var (name, sqlType, notNull) in columns)
            {
                var nullSql = notNull ? "NOT NULL" : "NULL";
                colLines.Add($"{indent}\t\t\t\"\"{name}\"\" {sqlType} {nullSql}");
            }

            var colsStr = string.Join(",\n", colLines);

            // Используем оригинальное название таблицы в кавычках для сохранения регистра
            var quotedTableName = $"\"\"{tableName}\"\"";
            
            // Строим блок Sql
            var sql = $"{indent}\tmigrationBuilder.Sql(@\"\n" +
                     $"{indent}\t\tCREATE TABLE IF NOT EXISTS {quotedTableName} (\n" +
                     $"{colsStr},\n" +
                     $"{indent}\t\t\tCONSTRAINT \"\"PK_{tableName}\"\" PRIMARY KEY (\"\"Id\"\", \"\"CreateAt\"\")\n" +
                     $"{indent}\t\t) PARTITION BY RANGE (\"\"CreateAt\"\");\n" +
                     $"{indent}\t\");";

            return sql;
        }

        private void ShowDiff(string original, string modified)
        {
            var originalLines = original.Split('\n');
            var modifiedLines = modified.Split('\n');

            Console.WriteLine("  Diff:");
            // Простой diff - показываем только первые несколько изменений
            var maxLines = Math.Min(originalLines.Length, modifiedLines.Length);
            for (int i = 0; i < Math.Min(maxLines, 10); i++)
            {
                if (originalLines[i] != modifiedLines[i])
                {
                    Console.WriteLine($"    - {originalLines[i]}");
                    Console.WriteLine($"    + {modifiedLines[i]}");
                }
            }
        }
    }
}
