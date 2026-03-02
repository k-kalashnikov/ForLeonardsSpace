//using Microsoft.Extensions.Logging;
//using System.Collections.Concurrent;

//namespace Masofa.BusinessLogic.Services.CliStateMachine
//{
//    public class CliStateMachineService
//    {
//        public CliStateMachineService(IServiceProvider serviceProvider, 
//            ILogger<CliStateMachineService> logger, 
//            ConcurrentDictionary<Guid, CliStateMachineItem> runningCommands, 
//            ConcurrentDictionary<Guid, CancellationTokenSource> cancellationTokenSources)
//        {
//            ServiceProvider = serviceProvider;
//            Logger = logger;
//            RunningCommands = runningCommands;
//            CancellationTokenSources = cancellationTokenSources;
//        }

//        private IServiceProvider ServiceProvider { get; set; }
//        private ILogger Logger { get; set; }
//        private ConcurrentDictionary<Guid, CliStateMachineItem> RunningCommands { get; set; }
//        private ConcurrentDictionary<Guid, CancellationTokenSource> CancellationTokenSources { get; set; }
//    }

//    public class CliStateMachineItem
//    {
//        public Guid Id { get; set; }
//        public string CommandName { get; set; } = null!;
//        public DateTime StartTime { get; set; }
//        public DateTime? EndTime { get; set; }
//        public CommandStatus Status { get; set; }
//        public string? ParametersJson { get; set; }
//        public string?  ex.Message { get; set; }
//        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : DateTime.UtcNow - StartTime;
//    }

//    public enum CommandStatus
//    {
//        Running,
//        Cancelling,
//        Cancelled,
//        Completed,
//        Failed
//    }
//}
