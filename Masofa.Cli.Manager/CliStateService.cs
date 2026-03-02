using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.Manager
{
    public static class CliStateService
    {
        public static string CurrentLocale { get; set; } = "en-US";
        public static Dictionary<int, CommandItem> CommandTypes { get; set; } = new Dictionary<int, CommandItem>();
        public static async Task ExecuteAsync(IServiceScope serviceScope, string utilsName, string[] args)
        {
            Console.WriteLine($"Welcome to the {utilsName}!\n" + $"Choose number of command:");

            foreach (var command in CommandTypes)
            {
                Console.WriteLine($"{command.Key}. {command.Value.CommandType.GetCustomAttribute<CliCommandAttribute>()?.CommandName ?? string.Empty}");
            }

            while (true)
            {
                var choosenCommand = Console.ReadLine();
                try
                {
                    int commandIndex = int.Parse(choosenCommand);
                    if (!CommandTypes.ContainsKey(commandIndex))
                    {
                        Console.WriteLine($"Command with number {commandIndex} not available");
                        continue;
                    }
                    using (var command = (IBaseCommand)serviceScope.ServiceProvider.GetRequiredService(CommandTypes[commandIndex].CommandType))
                    {
                        await command.Execute();
                    }
                }
                catch (Exception ex)
                {
                    if (choosenCommand.ToLower() == "exit")
                    {
                        return;
                    }
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message ?? string.Empty);
                    Console.WriteLine("This is not command number or \"exit\"");
                }
            }
        }
    }

    /// <summary>
    /// Атрибут для маркировки CLI команд
    /// </summary>
    public class CliCommandAttribute : Attribute
    {
        public LocalizationString CommandName { get; set; } = new Dictionary<string, string>();
        public LocalizationString Description { get; set; } = new Dictionary<string, string>();
        public Type? ParametersType { get; set; }

        public CliCommandAttribute() { }

        public CliCommandAttribute(LocalizationString commandName)
        {
            CommandName = commandName;
        }

        public CliCommandAttribute(LocalizationString commandName, LocalizationString description)
        {
            CommandName = commandName;
            Description = description;
        }

        public CliCommandAttribute(LocalizationString commandName, LocalizationString description, Type parametersType)
        {
            CommandName = commandName;
            Description = description;
            ParametersType = parametersType;
        }
    }

    /// <summary>
    /// Информация о команде
    /// </summary>
    public class CliCommandItem
    {
        public Type CommandType { get; set; }
        public LocalizationString CommandName { get; set; }
    }
}
