using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Common.Models.SystemCrical
{
    /// <summary>
    /// Базовый интерфейс для CLI команд
    /// </summary>
    public interface IBaseCommand : IDisposable
    {
        Task Execute();
        Task Execute(string[] args);
    }

    /// <summary>
    /// Атрибут для маркировки CLI команд
    /// </summary>
    public class BaseCommandAttribute : Attribute
    {
        public string CommandName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Type? ParametersType { get; set; }

        public BaseCommandAttribute() { }

        public BaseCommandAttribute(string commandName)
        {
            CommandName = commandName;
        }

        public BaseCommandAttribute(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
        }

        public BaseCommandAttribute(string commandName, string description, Type parametersType)
        {
            CommandName = commandName;
            Description = description;
            ParametersType = parametersType;
        }
    }

    /// <summary>
    /// Информация о команде
    /// </summary>
    public class CommandItem
    {
        public Type CommandType { get; set; }
        public string CommandName { get; set; }
    }

    /// <summary>
    /// Сервис для работы с CLI командами
    /// </summary>
    public static class BaseCommandCliService
    {
        public static Dictionary<int, CommandItem> CommandTypes { get; set; } = new Dictionary<int, CommandItem>();

        public static void FillCommandTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var availableCommands = new List<Type>();

            foreach (var assembly in assemblies)
            {
                availableCommands.AddRange(assembly.GetTypes().Where(m => m.GetCustomAttribute<BaseCommandAttribute>() != null)
                    .ToList());
            }

            availableCommands = availableCommands.OrderBy(ac => ac.Name).ToList();

            var index = 1;
            foreach (var availableCommand in availableCommands)
            {
                CommandTypes.Add(index++, new CommandItem()
                {
                    CommandName = availableCommand.Name,
                    CommandType = availableCommand
                });
            }
        }

        public static async Task ExecuteAsync(IServiceScope serviceScope, string utilsName, string[] args)
        {
            FillCommandTypes();
            Console.WriteLine($"Welcome to the {utilsName}!\n" +
                $"Choose number of command:");

            foreach (var command in CommandTypes)
            {
                Console.WriteLine($"{command.Key}. {command.Value.CommandType.GetCustomAttribute<BaseCommandAttribute>()?.CommandName ?? string.Empty}");
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
}
