//using Masofa.Client.ApiClient;
//using Masofa.Client.ApiClient.Repositrories.CropMonitoring;
//using Masofa.Common.Models.SystemCrical;
//using Microsoft.AspNetCore.Http;

//namespace Masofa.Cli.DevopsUtil.Commands.CropMonitoring
//{
//    [BaseCommand("ImportFieldCommand", "ImportFieldCommand")]
//    public class ImportFieldCommand : IBaseCommand
//    {
//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public async Task Execute()
//        {
//            Console.WriteLine("Enter pls filePath");
//            var filePath = Console.ReadLine();
//            var httpClient = new HttpClient();
//            var unitOfWork = new UnitOfWork(httpClient, "https://localhost:7088");
//            await unitOfWork.LoginAsync(new Common.ViewModels.Account.LoginAndPasswordViewModel()
//            {
//                UserName = "Admin",
//                Password = "Zxcvbnmlkjh9"
//            }, CancellationToken.None);
//            var fileBytes = File.ReadAllBytes(filePath);
//            var result = new FieldImportResult()
//            {
//                Ids = new List<Guid>()
//            };
//            using (var memStream = new MemoryStream(fileBytes))
//            {
//                IFormFile file = new FormFile(memStream, 0, fileBytes.Length, "File", "tempFile");
//                result = await unitOfWork.FieldRepository.ImportFieldsAsync(new BusinessLogic.CropMonitoring.Fields.FieldImportViewModel()
//                {
//                    FieldExportType = BusinessLogic.CropMonitoring.Fields.FieldExportType.GeoJson,
//                    File = file
//                }, CancellationToken.None);
//            }
//            foreach (var item in result.Ids)
//            {
//                Console.WriteLine($"{item}");
//            }

//        }




//        public Task Execute(string[] args)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
