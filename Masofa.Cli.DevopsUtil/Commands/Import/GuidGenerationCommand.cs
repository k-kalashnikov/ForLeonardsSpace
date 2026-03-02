using Masofa.Common.Models.SystemCrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Cli.DevopsUtil.Commands.Import
{
    [BaseCommand("GUID generator", "GUID generator")]
    public class GuidGenerationCommand : IBaseCommand
    {
        public void Dispose()
        {
            
        }

        public async Task Execute()
        {
            var newGuid = Guid.NewGuid();
            Console.WriteLine(newGuid);
        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}
