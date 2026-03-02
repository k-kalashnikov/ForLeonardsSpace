using Masofa.Common.Models;

namespace Masofa.BusinessLogic.Cli.Resources
{
    public static class CodeGeneratorResources
    {
        public static LocalizationString CGetDbSetAddCodeGenCommandNames { get; } = new Dictionary<string, string>()
        {
            {
                "en-US", "Compare GetDbSet CodeGenerator"
            }
        };

        public static LocalizationString CGetDbSetAddCodeGenCommandDescriptions { get; } = new Dictionary<string, string>()
        {
            {
                "ru-Ru", "Генератор кода для добавления DbSet методов"
            }
        };
    }
}
