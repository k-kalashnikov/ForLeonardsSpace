using Masofa.Common.Resources;
using Masofa.Common.Resources;
using CsvHelper;
using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Identity;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Masofa.BusinessLogic.Identity.Users
{
    public class UserImportCommand : IRequest<BulkUserCreateResult>
    {
        /// <summary>
        /// Тип пользователя, физ или юр лицо
        /// </summary>
        public UserBusinessType UserBusinessType { get; set; }

        /// <summary>
        /// Импортируемый csv файл
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Пользователь-автор изменений
        /// </summary>
        [Required]
        public string Author { get; set; }
    }

    public class UserImportCommandHandler : IRequestHandler<UserImportCommand, BulkUserCreateResult>
    {
        private ILogger Logger { get; set; }
        private IMediator Mediator { get; set; }
        private IBusinessLogicLogger BusinessLogicLogger { get; set; }

        public UserImportCommandHandler(ILogger<UserCreateCommandHandler> logger, IMediator mediator, IBusinessLogicLogger businessLogicLogger)
        {
            Logger = logger;
            Mediator = mediator;
            BusinessLogicLogger = businessLogicLogger;
        }

        public async Task<BulkUserCreateResult> Handle(UserImportCommand request, CancellationToken cancellationToken)
        {
            var requestPath = $"{GetType().FullName}=>{nameof(Handle)}";
            try
            {
                await BusinessLogicLogger.LogInformationAsync(LogMessageResource.RequestStarted(requestPath), requestPath);
                var results = new List<BulkUserCreateItemResult>();
                var records = await ParseCsvAsync(request.File, results);

                foreach (var record in records)
                {
                    var command = new UserCreateCommand
                    {
                        UserName = record.UserName,
                        FirstName = record.FirstName,
                        SecondName = record.SecondName,
                        LastName = record.LastName,
                        Password = record.Password,
                        Email = record.Email,
                        Approved = record.Approved,
                        EmailConfirmed = record.EmailConfirmed,
                        LockUser = record.LockUser,
                        LockoutStart = record.LockoutStart,
                        LockoutEnd = record.LockoutEnd,
                        Roles = string.IsNullOrWhiteSpace(record.Roles)
                            ? new List<string>()
                            : record.Roles.Split(',', ';').Select(r => r.Trim()).ToList(),
                        UserBusinessType = record.UserBusinessType,
                        UserBusinessId = record.UserBusinessId,
                        Author = request.Author
                    };

                    try
                    {
                        var result = await Mediator.Send(command, cancellationToken);
                        var bucir = new BulkUserCreateItemResult
                        {
                            UserName = command.UserName,
                            Success = false,
                        };
                        if (result.Errors == null || result.Errors.Count == 0)
                        {
                            bucir.Success = true;
                            bucir.Id = result.Id;
                        }

                        results.Add(bucir);
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BulkUserCreateItemResult
                        {
                            UserName = command.UserName,
                            Success = false,
                            Error = ex.Message
                        });
                    }
                }

                var totalCount = results.Count;
                var successCount = results.Count(r => r.Success);
                var failedCount = totalCount - successCount;

                return new BulkUserCreateResult()
                {
                    TotalCount = totalCount,
                    SuccessCount = successCount,
                    FailedCount = failedCount,
                    Results = results
                };
            }
            catch (Exception ex)
            {
                var msg = LogMessageResource.GenericError(requestPath, ex.Message);
                await BusinessLogicLogger.LogCriticalAsync(msg, requestPath);
                Logger.LogCritical(ex, msg);
                throw ex;
            }
        }

        private Task<List<UserCsvRecord>> ParseCsvAsync(IFormFile file, List<BulkUserCreateItemResult> errorCollector)
        {
            var records = new List<UserCsvRecord>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture));

            try
            {
                var parsedRecords = csv.GetRecords<UserCsvRecord>().ToList();

                foreach (var record in parsedRecords)
                {
                    if (string.IsNullOrWhiteSpace(record.UserName))
                    {
                        errorCollector.Add(new BulkUserCreateItemResult
                        {
                            UserName = "(no username)",
                            Success = false,
                            Error = "UserName is required"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.Password))
                    {
                        errorCollector.Add(new BulkUserCreateItemResult
                        {
                            UserName = record.UserName,
                            Success = false,
                            Error = "Password is required"
                        });
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(record.Email))
                    {
                        errorCollector.Add(new BulkUserCreateItemResult
                        {
                            UserName = record.UserName,
                            Success = false,
                            Error = "Email is required"
                        });
                        continue;
                    }

                    records.Add(record);
                }
            }
            catch (Exception ex)
            {
                errorCollector.Add(new BulkUserCreateItemResult
                {
                    UserName = "Parsing",
                    Success = false,
                    Error = $"CSV parsing error: {ex.Message}"
                });
            }

            return Task.FromResult(records);
        }
    }

    public class UserCsvRecord
    {
        public string UserName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Approved { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockUser { get; set; }
        /// <summary>
        /// Дата и время начала блокировки пользователя
        /// </summary>
        public DateTime? LockoutStart { get; set; }
        
        /// <summary>
        /// Дата и время окончания блокировки пользователя
        /// </summary>
        public DateTime? LockoutEnd { get; set; }
        public string Roles { get; set; } = string.Empty;
        public UserBusinessType UserBusinessType { get; set; }
        public Guid UserBusinessId { get; set; }
    }

    public class BulkUserCreateResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<BulkUserCreateItemResult> Results { get; set; } = new();
    }

    public class BulkUserCreateItemResult
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}
