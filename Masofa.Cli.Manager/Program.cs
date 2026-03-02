using Microsoft.Extensions.Hosting;

namespace Masofa.Cli.Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            return builder;
        }
    }
}
