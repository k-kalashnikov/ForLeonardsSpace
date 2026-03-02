using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Masofa.DataAccess
{
    /// <summary>
    /// Фабрика создания контекста БД для dotnet ef tools
    /// </summary>
    /// <remarks>
    /// По умолчанию команда <c>dotnet ef migrations add/remove</c> пытается взять экземпляр из Program.cs 
    /// и там должна быть указана рабочая строка подключения к БД, 
    /// т.е. без подключения к БД команда создания миграции не сработает.
    /// <br/><br/>
    /// Это неудобно, т.к. не всегда локально развёрнута рабочая БД и к тому же может быть опасно, 
    /// т.к. команда <c>dotnet ef migrations remove</c>  всегда пытается подключиться к БД 
    /// и откатить удаляемую миграцию, если не использовать флаг <c>-f (--force)</c>. 
    /// <br/><br/>
    /// Благодаря этому классу команды <c>dotnet ef migrations add/remove</c> 
    /// гарантированно не будут подключаться ни к каким БД
    /// <br/><br/>
    /// https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    /// </remarks>
    public class MasofaDictionariesHistoryDbContextFactory : IDesignTimeDbContextFactory<MasofaDictionariesHistoryDbContext>
    {
        public MasofaDictionariesHistoryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MasofaDictionariesHistoryDbContext>();

            optionsBuilder.UseNpgsql(x =>
            {
                x.UseNodaTime();
                x.UseNetTopologySuite();
            });

            return new MasofaDictionariesHistoryDbContext(optionsBuilder.Options);
        }
    }
}
