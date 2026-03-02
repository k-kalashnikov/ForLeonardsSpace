
using Masofa.Common.Models;
using Masofa.Common.Models.Dictionaries;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System.Reflection;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.Cli.DbSeeders
{
    public class FieldAgroOperationDbSeeder
    {
        private IMediator Mediator { get; set; }
        private UserManager<Masofa.Common.Models.Identity.User> UserManager { get; set; }
        private Masofa.DataAccess.MasofaCropMonitoringDbContext MasofaCropMonitoringDbContext { get; set; }
        private Masofa.DataAccess.MasofaDictionariesDbContext MasofaDictionariesDbContext { get; set; }
        private List<Masofa.Common.Models.CropMonitoring.FieldAgroOperation> fieldAgroOperations = new List<Masofa.Common.Models.CropMonitoring.FieldAgroOperation>();
        private List<Type> fieldAgroOperationsTypes = new List<Type>()
        {
            typeof(Masofa.Common.Models.Dictionaries.Fertilizer),
            typeof(Masofa.Common.Models.Dictionaries.IrrigationMethod),
            typeof(Masofa.Common.Models.Dictionaries.Disease),
            typeof(Masofa.Common.Models.Dictionaries.AgroOperation),
            typeof(Masofa.Common.Models.Dictionaries.AgroMachineType)
        };

        /*
          Орошение / Мелиорация - Masofa.Common.Models.Dictionaries.IrrigationMethod
          Удобрения, пестициды, биозащита - Masofa.Common.Models.Dictionaries.Fertilizer
          Болезни и вредители - Masofa.Common.Models.Dictionaries.Disease
          Агротехнические мероприятия - Masofa.Common.Models.Dictionaries.AgroOperation
          Использование техники - Masofa.Common.Models.Dictionaries.AgroMachineType
         */

        public FieldAgroOperationDbSeeder(UserManager<Masofa.Common.Models.Identity.User> userManager, DataAccess.MasofaCropMonitoringDbContext masofaCropMonitoringDbContext, DataAccess.MasofaDictionariesDbContext masofaDictionariesDbContext, IMediator mediator)
        {
            UserManager = userManager;
            MasofaCropMonitoringDbContext = masofaCropMonitoringDbContext;
            MasofaDictionariesDbContext = masofaDictionariesDbContext;
            Mediator = mediator;
        }

        public async Task SeedAsync()
        {
            var adminUser = await UserManager.FindByNameAsync("Admin");
            var fieldAgroOperationTypes = await MasofaDictionariesDbContext.AgroOperations.ToListAsync();

            Guid tempId = Guid.Parse("01245d3b-f9ee-4397-aac4-0cb2a0aa835a");

            foreach (var item in fieldAgroOperationsTypes)
            {

                var dictionatyItems = await GetDictItems(item);
                if ((dictionatyItems == null) || (!dictionatyItems.Any()))
                {
                    continue;
                }
                var tempFAOType = await MasofaCropMonitoringDbContext.FieldAgroOperations.Where(m => m.FieldId == tempId)
                    .Where(m => m.OperationTypeFullName == item.FullName)
                    .ToListAsync();

                if ((tempFAOType != null) && (tempFAOType.Any())) 
                {
                    continue;
                }
                var count = Random.Shared.Next(5, 10);
                for (int i = 0; i < count; i++)
                {
                    var tempOper = dictionatyItems[Random.Shared.Next(0, (dictionatyItems.Count - 1))];
                    var tempFAO = new Masofa.Common.Models.CropMonitoring.FieldAgroOperation()
                    {
                        FieldId = tempId,
                        OperationTypeFullName = item.FullName,
                        CreateUser = adminUser.Id,
                        CreateAt = DateTime.UtcNow,
                        LastUpdateAt = DateTime.UtcNow,
                        LastUpdateUser = adminUser.Id,
                        OriginalDate = new DateTime(Random.Shared.Next(2023, 2024), Random.Shared.Next(1, 12), Random.Shared.Next(1, 28), 0, 0, 0, DateTimeKind.Utc),
                        OperationId = tempOper.Id,
                        OperationName =tempOper.Names
                    };
                    fieldAgroOperations.Add(tempFAO);
                }
            }

            await MasofaCropMonitoringDbContext.FieldAgroOperations.AddRangeAsync(fieldAgroOperations);
            await MasofaCropMonitoringDbContext.SaveChangesAsync();

        }

        private async Task<List<DictItemRecord>> GetDictItems(Type type)
        {
            if (type == typeof(Masofa.Common.Models.Dictionaries.Fertilizer))
            {
                return await MasofaDictionariesDbContext.Fertilizers.Select(m => new DictItemRecord
                {
                    Names = m.Names,
                    Id = m.Id,
                }).ToListAsync();
            }

            if (type == typeof(Masofa.Common.Models.Dictionaries.IrrigationMethod))
            {
                return await MasofaDictionariesDbContext.IrrigationMethods.Select(m => new DictItemRecord
                {
                    Names = m.Names,
                    Id = m.Id,
                }).ToListAsync();
            }

            if (type == typeof(Masofa.Common.Models.Dictionaries.Disease))
            {
                return await MasofaDictionariesDbContext.Diseases.Select(m => new DictItemRecord
                {
                    Names = m.Names,
                    Id = m.Id,
                }).ToListAsync();
            }

            if (type == typeof(Masofa.Common.Models.Dictionaries.AgroOperation))
            {
                return await MasofaDictionariesDbContext.AgroOperations.Select(m => new DictItemRecord
                {
                    Names = m.Names,
                    Id = m.Id,
                }).ToListAsync();
            }
            if (type == typeof(Masofa.Common.Models.Dictionaries.AgroMachineType))
            {
                return await MasofaDictionariesDbContext.AgroMachineTypes.Select(m => new DictItemRecord
                {
                    Names = m.Names,
                    Id = m.Id,
                }).ToListAsync();
            }

            return new List<DictItemRecord>();
        }

        private record DictItemRecord
        {
            public LocalizationString Names { get; set; }
            public Guid Id { get; set; }
        }
    }
}
