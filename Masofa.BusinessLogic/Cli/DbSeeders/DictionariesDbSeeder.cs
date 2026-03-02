using Masofa.DataAccess;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class DictionariesDbSeeder : AbstractCsvSeeder<MasofaDictionariesDbContext>
    {
        public DictionariesDbSeeder(MasofaDictionariesDbContext context)
            : base(context, Path.Combine("Cli", "DbSeeders", "Csv", "Dictionaries"))
        {
        }
    }
}
