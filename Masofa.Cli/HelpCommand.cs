using Masofa.Common.Models.SystemCrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli
{
    [BaseCommand(CommandName = "Help")]
    public class HelpCommand : IBaseCommand
    {
        public void Dispose()
        {

        }

        public Task Execute()
        {

            Console.WriteLine($"Welcome to the Data Migration program!\n" +
                $"This all available commands:");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var availableCommands = new List<Type>();

            foreach (var assembly in assemblies)
            {
                availableCommands.AddRange(assembly.GetTypes()
                    .Where(m => m.GetCustomAttribute<BaseCommandAttribute>() != null)
                    .ToList());
            }

            var index = 1;

            foreach (var command in availableCommands)
            {
                Console.WriteLine($"{index++}. {command.Name}");
            }

            return Task.CompletedTask;
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
