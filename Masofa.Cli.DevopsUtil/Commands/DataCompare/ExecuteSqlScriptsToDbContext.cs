using DocumentFormat.OpenXml.InkML;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlTypes;

namespace Masofa.Cli.DevopsUtil.Commands.DataCompare
{
    [BaseCommand("ExecuteSqlScriptsToDbContext", "ExecuteSqlScriptsToDbContext")]
    public class ExecuteSqlScriptsToDbContext : IBaseCommand
    {
        private IServiceProvider ServiceProvider { get; set; }
        public ExecuteSqlScriptsToDbContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task Execute()
        {

            var dbContexts = GetDbContexts();
            Console.WriteLine("Enter pls Save Path");
            var path = Console.ReadLine();
            var dbContextIndex = 0;
            Console.Clear();
            foreach (var context in dbContexts)
            {
                DrawProgressBar(3, dbContextIndex, dbContexts.Count, "Progress for ", $"Execute for DbContext - {context.GetType().Name}");

                var dbContextPath = Path.Combine(path, context.GetType().Name);
                if (!Directory.Exists(dbContextPath))
                {
                    DrawProgressBar(3, dbContextIndex, dbContexts.Count, "Progress for ", $"DbContext folder is not exist => {dbContextPath}");

                }
                var scripts = Directory.GetFiles(dbContextPath, "*.sql")?.ToList() ?? new List<string>();
                if (!scripts.Any())
                {
                    DrawProgressBar(3, dbContextIndex, dbContexts.Count, "Progress for ", $"DbContext folder {dbContextPath} is not contains the scripts");
                }

                int scriptIndex = 0;
                foreach (var script in scripts)
                {
                    var errorMessages = new List<string>();
                    var textScriptStrings = File.ReadAllLines(script);
                    
                    DrawProgressBar(7, scriptIndex, scripts.Count, "Progress for ", $"File {script} Contains {textScriptStrings.Length} lines");

                    int progessString = 0;
                    foreach (var sqlString in textScriptStrings)
                    {
                        DrawProgressBar(11, progessString, textScriptStrings.Length, "Progress for ", $"Execute sql string");
                        try
                        {

                            await context.Database.ExecuteSqlRawAsync(sqlString);
                        }
                        catch (Exception ex)
                        {
                            DrawProgressBar(11, progessString, textScriptStrings.Length, "Progress for", ex.Message);
                            errorMessages.Add($"-- {ex.Message}");
                            errorMessages.Add(sqlString);
                        }
                        progessString++;
                    }
                    File.AppendAllLines(Path.Combine(dbContextPath, $"{Path.GetFileNameWithoutExtension(script)}_Errors_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt"), errorMessages);
                    File.AppendAllText(Path.Combine(dbContextPath, $"{Path.GetFileNameWithoutExtension(script)}_Errors_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt"), $"\n\n {errorMessages.Count} Errors of {textScriptStrings.Length} queries");
                    scriptIndex++;
                }



                dbContextIndex++;
            }

            //while (true)
            //{
            //    Console.WriteLine("Choose DbContext or enter \"exit\"");
            //    var indexDbContext = 0;
            //    foreach (var context in dbContexts)
            //    {
            //        Console.WriteLine($"{indexDbContext} {context.GetType().Name}");
            //        indexDbContext++;
            //    }
            //    var dbContextNumber = Console.ReadLine();
            //    if (dbContextNumber.ToLower() == "exit")
            //    {
            //        break;
            //    }
            //    var dbContext = dbContexts[int.Parse(dbContextNumber)];
            //    var dbContextPath = Path.Combine(path, dbContext.GetType().Name);
            //    if (!Directory.Exists(dbContextPath))
            //    {
            //        Console.WriteLine($"DbContext folder is not exist => {dbContextPath}");
            //    }
            //    var scripts = Directory.GetFiles(dbContextPath)?.ToList() ?? new List<string>();
            //    if (!scripts.Any())
            //    {
            //        Console.WriteLine($"DbContext folder {dbContextPath} is not contains the scripts");
            //    }

            //    while (true)
            //    {
            //        int indexScripts = 0;
            //        Console.WriteLine("Choose DbContext scritps or enter \"exit\"");

            //        foreach (var script in scripts)
            //        {
            //            Console.WriteLine($"{indexScripts} {Path.GetFileName(script)}");
            //            indexScripts++;
            //        }

            //        var scriptNumber = Console.ReadLine();
            //        if (scriptNumber.ToLower() == "exit")
            //        {
            //            break;
            //        }
            //        var choosenScript = scripts[int.Parse(scriptNumber)];
            //        var textScriptStrings = File.ReadAllLines(choosenScript);
            //        Console.WriteLine($"File {choosenScript} Contains {textScriptStrings.Length} lines");
            //        int progessString = 0;
            //        Console.Clear();
            //        var errorMessages = new List<string>();
            //        foreach (var sqlString in textScriptStrings)
            //        {
            //            DrawProgressBar(3, progessString, textScriptStrings.Length, "Progress for ", $"Execute sql string");
            //            try
            //            {

            //                await dbContext.Database.ExecuteSqlRawAsync(sqlString);
            //            }
            //            catch (Exception ex)
            //            {
            //                DrawProgressBar(3, progessString, textScriptStrings.Length, "Progress for", ex.Message);
            //                errorMessages.Add($"-- {ex.Message}");
            //                errorMessages.Add(sqlString);
            //            }
            //            progessString++;
            //        }
            //        File.WriteAllLines(Path.Combine(dbContextPath, $"{Path.GetFileNameWithoutExtension(choosenScript)}_Errors_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt"), errorMessages);
            //        File.AppendAllText($"{Path.GetFileNameWithoutExtension(choosenScript)}_Errors_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt", $"\n\n {errorMessages.Count} Errors of {textScriptStrings.Length} queries");
            //    }
            //}
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }


        public List<DbContext> GetDbContexts()
        {
            var dbContextTypes = typeof(MasofaAnaliticReportDbContext).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DbContext)))
                .Where(t => !t.Name.Contains("History"))
                .ToList();
            var result = new List<DbContext>();

            foreach (var dbContextType in dbContextTypes)
            {
                try
                {
                    if (ServiceProvider.GetService(dbContextType) != null)
                    {
                        result.Add((DbContext)ServiceProvider.GetService(dbContextType));
                        continue;
                    }
                    var dbContextOption = typeof(DbContextOptions<>).MakeGenericType(dbContextType);
                    result.Add((DbContext)Activator.CreateInstance(dbContextType, dbContextOption));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        private void DrawProgressBar(int line, int current, int total, string taksName, string message)
        {
            // Ограничиваем текущий прогресс
            current = Math.Min(current, total);
            int filledLength = ((int)Math.Round((double)current / total) * 100);

            string bar = new string('█', filledLength).PadRight(100);
            string text = $"{taksName}: [{bar}] {current} of {total.ToString()}";
            Console.SetCursorPosition(0, line);
            Console.Write(message.PadRight(Console.WindowWidth - 1)); // затираем старый текст
            Console.SetCursorPosition(0, line + 1);
            Console.Write(text.PadRight(Console.WindowWidth - 1));
        }
    }
}
