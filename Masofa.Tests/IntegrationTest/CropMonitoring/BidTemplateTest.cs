using FluentAssertions;
using Masofa.BusinessLogic.CropMonitoring.Fields;
using Masofa.Client.ApiClient;
using Masofa.Client.ApiClient.Repositrories.CropMonitoring;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.CropMonitoring
{
    [Collection("Sequential")]
    public class BidTemplateTest
    {
        private readonly ITestOutputHelper _output;
        private readonly string _baseUrl;
        private string? _authToken;
        private HttpClient? _sharedHttpClient;

        public BidTemplateTest(ITestOutputHelper output)
        {
            _output = output;
            _baseUrl = TestConstants.BASE_URL;
            _sharedHttpClient = new HttpClient();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            if (_authToken != null)
            {
                _output.WriteLine("Using cached auth token.");
                return _authToken;
            }

            _output.WriteLine("Obtaining new auth token...");
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            };

            try
            {
                await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);
                _authToken = unitOfWork.AccountRepository.Token;

                if (string.IsNullOrEmpty(_authToken))
                {
                    throw new InvalidOperationException("Login was successful, but token is null or empty.");
                }

                _output.WriteLine("Auth token obtained successfully.");
                return _authToken;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Failed to obtain auth token: {ex}");
                throw new InvalidOperationException("Failed to authenticate for integration tests.", ex);
            }
        }

        private async Task<BidTemplateRepository> CreateAuthenticatedBidTemplateRepositoryAsync()
        {
            var token = await GetAuthTokenAsync();
            var bidTemplateRepository = new BidTemplateRepository(_sharedHttpClient, $"{_baseUrl}/cropMonitoring/Bid");
            bidTemplateRepository.Token = token;
            _output.WriteLine("Authenticated BidTemplateRepository created.");
            return bidTemplateRepository;
        }

        #region ImportFromFiles

        /*[Fact]
        public async Task ImportFromFilesAsync_WithValidFiles_ShouldReturnImportResult()
        {
            // Arrange
            var bidTemplateRepository = await CreateAuthenticatedBidTemplateRepositoryAsync();

            // Создаем тестовый файл (например, простой текстовый файл)
            var fileContent1 = "This is a test KML file content for import.";
            var fileBytes1 = System.Text.Encoding.UTF8.GetBytes(fileContent1);
            var fileStream1 = new MemoryStream(fileBytes1);
            var streamContent1 = new StreamContent(fileStream1);
            streamContent1.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream"); // Или подходящий тип

            using var formData = new MultipartFormDataContent();
            // ВАЖНО: Имя части формы должно совпадать с именем параметра в контроллере - "files" (множественное число)
            formData.Add(streamContent1, "files", "test_import_file1.txt");

            // Добавим еще один файл, если API ожидает массив
            var fileContent2 = "This is a second test file.";
            var fileBytes2 = System.Text.Encoding.UTF8.GetBytes(fileContent2);
            var fileStream2 = new MemoryStream(fileBytes2);
            var streamContent2 = new StreamContent(fileStream2);
            streamContent2.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            formData.Add(streamContent2, "files", "test_import_file2.txt");

            var exportType = FieldExportType.Kml; // Используем подходящий тип экспорта, если он применим

            _output.WriteLine("Attempting to import files...");

            // Act
            var result = await bidTemplateRepository.ImportFromFilesAsync(exportType, formData, CancellationToken.None);

            // Assert
            // Результат зависит от логики API. Может возвращать список идентификаторов импортированных сущностей или сообщений об ошибках.
            result.Should().NotBeNull("Import result list should not be null.");
            result.Should().BeAssignableTo<List<string>>("Result should be a list of strings.");
            // Проверим, что возвращенные строки могут быть GUID или содержать информацию об импорте
            // result.Should().AllBeOfType<string>(); // Уже проверено выше через BeAssignableTo
            _output.WriteLine($"ImportFromFilesAsync returned {result.Count} results.");
            foreach (var res in result)
            {
                _output.WriteLine($"  - {res}");
            }
        }*/

        #endregion

        #region GetSchema

        [Fact]
        public async Task GetSchemaAsync_WhenAuthenticated_ShouldReturnSchema()
        {
            // Arrange
            var bidTemplateRepository = await CreateAuthenticatedBidTemplateRepositoryAsync();

            // Act
            var schema = await bidTemplateRepository.GetSchemaAsync(CancellationToken.None);

            // Assert
            schema.Should().NotBeNull("Schema should be returned.");
            schema.Should().BeAssignableTo<string>("Schema should be a string.");
            // schema.Should().NotBeEmpty("Schema should not be empty."); // Зависит от реализации API
            _output.WriteLine($"GetSchemaAsync returned a schema of length: {schema.Length}");
        }

        #endregion

        #region GetByQuery

        [Fact]
        public async Task GetByQueryAsync_WithValidQuery_ShouldReturnListOfBidTemplate()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            var query = new BaseGetQuery<BidTemplate>
            {
                Take = 5,
                Offset = 0,
                Sort = SortType.ASC,
                SortBy = "Id"
            };

            _output.WriteLine("Executing Standard GetByQueryAsync with a query to get up to 5 BidTemplates...");

            // Act
            var result = await bidTemplateRepository.GetByQueryAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Result list should not be null.");
            result.Should().BeAssignableTo<List<BidTemplate>>("Result should be a list of BidTemplate.");
            result.Count.Should().BeLessThanOrEqualTo(5, "Query requested up to 5 items.");
            if (result.Count > 0)
            {
                var firstBidTemplate = result[0];
                firstBidTemplate.Id.Should().NotBeEmpty("Each BidTemplate should have a valid ID.");
            }
            _output.WriteLine($"Standard GetByQueryAsync test completed successfully. Retrieved {result.Count} BidTemplates.");
        }

        #endregion


        #region GetById

        [Fact]
        public async Task GetByIdAsync_WithValidExistingId_ShouldReturnBidTemplate()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            // Подготовим JSON-объект для LocalizationString, который будет использован в valuesJson
            // Это объект, который будет представлен как строка в поле valuesJson
            var localizationValuesObject = new Dictionary<string, string>
    {
        { "en", "Default Crop Name EN" },
        { "ru", "Имя культуры RU" }
        // Добавьте другие языки по необходимости
    };
            // Сериализуем его в строку
            var valuesJsonString = System.Text.Json.JsonSerializer.Serialize(localizationValuesObject);

            // Подготовим корректный JSON для поля DataJson, соответствующий BidTemplateSchemaVersion3
            // ВАЖНО: Не включаем 'supportedLanguages', так как readOnly. Только 'valuesJson'.
            var localizationStringJson = @"{ ""valuesJson"": """" }"; // <-- Пустая строка в valuesJson

            var bidTemplateSchemaVersion3Json = $@"{{
    ""cropId"": ""{TestConstants.EXISTING_CROP_ID}"",
    ""cropNames"": {localizationStringJson},
    ""schemaVersion"": 2,
    ""contentVersion"": 1,
    ""version"": ""1.0"",
    ""blocks"": []
}}";

            // Сначала создадим BidTemplate для теста
            var createModel = new BidTemplate
            {
                CropId = TestConstants.EXISTING_CROP_ID,
                SchemaVersion = 2, // Должно совпадать с версией в dataJson
                ContentVersion = 1, // Должно совпадать с версией в dataJson
                DataJson = bidTemplateSchemaVersion3Json, // <-- ИСПРАВЛЕНО: Правильный JSON без supportedLanguages
                Comment = "Test BidTemplate for GetById",
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = TestConstants.TEST_USER_GUID,
                LastUpdateUser = TestConstants.TEST_USER_GUID
            };

            _output.WriteLine($"Attempting to create BidTemplate with DataJson: {createModel.DataJson}"); // Логируем JSON

            var createdId = await bidTemplateRepository.CreateAsync(createModel, CancellationToken.None); // Используем CreateAsync, который возвращает Guid
            _output.WriteLine($"Created temporary BidTemplate with ID: {createdId} for Standard GetById test.");

            // Act
            var result = await bidTemplateRepository.GetByIdAsync(createdId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull($"BidTemplate with ID {createdId} should exist via standard GetById.");
            result.Id.Should().Be(createdId);
            _output.WriteLine($"Standard GetByIdAsync test completed successfully. Retrieved BidTemplate ID: {result.Id}");
        }

        #endregion

        #region Create

        /*[Fact]
        public async Task CreateAsync_WithValidModel_ShouldCreateBidTemplateAndReturnId()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            var createModel = new BidTemplate
            {
                CropId = TestConstants.EXISTING_CROP_ID,
                SchemaVersion = 2,
                ContentVersion = 1,
                DataJson = "{}", // Простой JSON
                Comment = "Test BidTemplate for Create",
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = TestConstants.TEST_USER_GUID,
                LastUpdateUser = TestConstants.TEST_USER_GUID
            };

            _output.WriteLine($"Attempting to create BidTemplate...");

            // Act
            Guid createdId;
            try
            {
                createdId = await bidTemplateRepository.CreateAsync(createModel, CancellationToken.None); // Используем CreateEntityAsync
                _output.WriteLine($"BidTemplate created successfully with ID: {createdId}");
            }
            catch (Exception ex) when (ex is not Xunit.Sdk.XunitException)
            {
                _output.WriteLine($"Standard CreateEntityAsync failed unexpectedly: {ex}");
                throw;
            }

            // Assert
            createdId.Should().NotBeEmpty("Created BidTemplate ID should not be empty Guid");

            _output.WriteLine("Verifying created BidTemplate exists via standard GetById...");
            BidTemplate? retrievedBidTemplate = null;
            try
            {
                retrievedBidTemplate = await bidTemplateRepository.GetByIdAsync(createdId, CancellationToken.None);
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _output.WriteLine($"BidTemplate with ID {createdId} not found immediately. Retrying once...");
                await Task.Delay(500);
                retrievedBidTemplate = await bidTemplateRepository.GetByIdAsync(createdId, CancellationToken.None);
            }

            retrievedBidTemplate.Should().NotBeNull($"BidTemplate with ID {createdId} should be retrievable after creation via standard GetById");
            retrievedBidTemplate.Id.Should().Be(createdId);
            retrievedBidTemplate.Comment.Should().Be(createModel.Comment);

            _output.WriteLine($"Standard CreateEntityAsync test completed successfully. Created and verified BidTemplate ID: {createdId}");
        }
        */
        #endregion

        #region Update

        /*[Fact]
        public async Task UpdateAsync_WithValidModel_ShouldUpdateBidTemplateAndReturnUpdatedEntity()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            // 1. Создадим BidTemplate
            var createModel = new BidTemplate
            {
                CropId = TestConstants.EXISTING_CROP_ID,
                SchemaVersion = 2,
                ContentVersion = 1,
                DataJson = "{}",
                Comment = "Test BidTemplate for Update (initial)",
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = Guid.NewGuid(),
                LastUpdateUser = Guid.NewGuid()
            };

            var createdBidTemplateId = await bidTemplateRepository.CreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary BidTemplate with ID: {createdBidTemplateId} for Standard Update test.");

            // 2. Получим созданную BidTemplate
            var existingBidTemplate = await bidTemplateRepository.GetByIdAsync(createdBidTemplateId, CancellationToken.None);
            existingBidTemplate.Should().NotBeNull();

            // 3. Подготовим модель для обновления
            var updateModel = new BidTemplate
            {
                Id = existingBidTemplate.Id, // Обязательно указываем Id
                CropId = existingBidTemplate.CropId, // Копируем из существующей
                SchemaVersion = existingBidTemplate.SchemaVersion, // Копируем из существующей
                ContentVersion = existingBidTemplate.ContentVersion, // Копируем из существующей
                DataJson = existingBidTemplate.DataJson, // Копируем из существующей
                Comment = "Test BidTemplate for Update (UPDATED)", // Изменяем комментарий
                Status = existingBidTemplate.Status, // Копируем из существующей
                CreateAt = existingBidTemplate.CreateAt, // Не меняем дату создания
                LastUpdateAt = DateTime.UtcNow, // Обновляем дату обновления
                CreateUser = existingBidTemplate.CreateUser, // Копируем из существующей
                LastUpdateUser = Guid.NewGuid() // Обновляем пользователя обновления
                // Копируем остальные поля из existingBidTemplate
            };

            _output.WriteLine($"Attempting to update BidTemplate ID: {createdBidTemplateId} with standard Update method.");

            // Act
            var updatedBidTemplate = await bidTemplateRepository.UpdateAsync(updateModel, CancellationToken.None);

            // Assert
            updatedBidTemplate.Should().NotBeNull("Updated BidTemplate should be returned.");
            updatedBidTemplate.Id.Should().Be(createdBidTemplateId, "Updated BidTemplate ID should match the requested ID.");
            updatedBidTemplate.Comment.Should().Be(updateModel.Comment, "Comment should be updated.");
            // updatedBidTemplate.Status.Should().Be(updateModel.Status, "Status should be updated if changed.");

            _output.WriteLine($"Standard UpdateAsync test completed successfully for BidTemplate ID: {createdBidTemplateId}");
        }
        */
        #endregion

        #region Delete

       /* [Fact]
        public async Task DeleteAsync_WithValidExistingId_ShouldDeleteBidTemplateAndReturnTrue()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            // 1. Создадим BidTemplate
            var createModel = new BidTemplate
            {
                CropId = TestConstants.EXISTING_CROP_ID,
                SchemaVersion = 2,
                ContentVersion = 1,
                DataJson = "{}",
                Comment = "Test BidTemplate for Delete",
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = Guid.NewGuid(),
                LastUpdateUser = Guid.NewGuid()
            };

            var createdBidTemplateId = await bidTemplateRepository.CreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary BidTemplate with ID: {createdBidTemplateId} for Standard Delete test.");

            // 2. Проверим, что BidTemplate существует
            var bidTemplateBeforeDelete = await bidTemplateRepository.GetByIdAsync(createdBidTemplateId, CancellationToken.None);
            bidTemplateBeforeDelete.Should().NotBeNull();

            _output.WriteLine($"Attempting to delete BidTemplate ID: {createdBidTemplateId} with standard Delete method.");

            // Act
            var deleteResult = await bidTemplateRepository.DeleteAsync(createdBidTemplateId, CancellationToken.None); // Используем DeleteEntityAsync

            // Assert
            deleteResult.Should().BeTrue("Delete operation should return true on success.");

            // 3. Проверим, что BidTemplate больше не существует (проверка на мягкие удаления)
            var bidTemplateAfterDelete = await bidTemplateRepository.GetByIdAsync(createdBidTemplateId, CancellationToken.None);
            bidTemplateAfterDelete.Should().NotBeNull("GetById should return the entity even after soft-delete.");
            bidTemplateAfterDelete.Status.Should().Be(StatusType.Deleted, "BidTemplate status should be changed to Deleted after the delete operation.");
            bidTemplateAfterDelete.Id.Should().Be(createdBidTemplateId);

            _output.WriteLine($"Standard DeleteAsync test completed successfully for BidTemplate ID: {createdBidTemplateId}");
        }*/

        #endregion

        #region GetTotalCount

        [Fact]
        public async Task GetTotalCountAsync_ShouldReturnTotalCount()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidTemplateRepository = unitOfWork.BidTemplateRepository;

            _output.WriteLine("Getting total count of BidTemplates...");

            // Создаём простой запрос для получения общего количества (фильтры могут быть null)
            var query = new BaseGetQuery<BidTemplate> { Take = null, Offset = 0 }; // Полный подсчет, без фильтрации

            // Act
            var count = await bidTemplateRepository.GetTotalCountAsync(query, CancellationToken.None);

            // Assert
            count.Should().BeGreaterThanOrEqualTo(0, "Total count should be zero or more.");
            _output.WriteLine($"Standard GetTotalCountAsync test completed successfully. Total count: {count}");
        }

        #endregion
    }
}
