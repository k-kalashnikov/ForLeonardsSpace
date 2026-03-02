
using Masofa.Common.Models.SystemCrical;
using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo;
using System.Reflection;
using System.Text;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    [BaseCommand("Compare GetDbSet CodeGenerator", "Генератор кода для добавления DbSet методов")]
    public class CGetDbSetAddCodeGenCommand : IBaseCommand
    {
        public void Dispose()
        {

        }

        public async Task Execute()
        {

        }

        public Task Execute(string[] args)
        {
            return Execute();
        }
    }
}
