using System.Net;
using FluentAssertions;
using Masofa.BusinessLogic.CropMonitoring.Bids;
using Masofa.Client.ApiClient;
using Masofa.Client.ApiClient.Repositrories.CropMonitoring;
using Masofa.Common.Models.CropMonitoring;
using Masofa.Common.ViewModels.Account;
using Masofa.Web.Monolith.ViewModels.Bids;
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.CropMonitoring
{
    [Collection("Sequential")]
    public class BidTest
    {
        private readonly ITestOutputHelper _output;
        private readonly string _baseUrl;
        private string? _authToken;
        private HttpClient? _sharedHttpClient;

        public BidTest(ITestOutputHelper output)
        {
            _output = output;
            _baseUrl = TestConstants.BASE_URL;
            // Создаем один HttpClient для всего тестового класса, чтобы избежать проблем с подключениями
            _sharedHttpClient = new HttpClient();
        }

        /// <summary>
        /// Получает токен аутентификации, выполнив логин через UnitOfWork.
        /// </summary>
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

        /// <summary>
        /// Создает экземпляр BidRepository с установленным токеном аутентификации.
        /// </summary>
        private async Task<BidRepository> CreateAuthenticatedBidRepositoryAsync()
        {
            var token = await GetAuthTokenAsync();
            var bidRepository = new BidRepository(_sharedHttpClient, $"{_baseUrl}/cropMonitoring/Bid");
            bidRepository.Token = token;
            _output.WriteLine("Authenticated BidRepository created.");
            return bidRepository;
        }

        #region CRUD Scenarios

        [Fact]
        public async Task CRUD_CustomMethods_ShouldPerformAllOperations()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();
            Guid createdBidId = Guid.Empty;

            try
            {
                // 1. Create
                var createModel = new BidCreateViewModel
                {
                    BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                    ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                    DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                    Comment = $"Demo test CRUD Custom",
                    Description = $"Demo test CRUD Custom",
                    Lat = TestConstants.TEST_LAT,
                    Lng = TestConstants.TEST_LNG,
                    Number = 0,
                    FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                    Customer = $"Demo test CRUD Custom",
                    CropId = TestConstants.EXISTING_CROP_ID,
                    BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                    IsUnvalidBid = true,
                    Publish = false,
                    //BidState = BidStateType.New,
                    Polygon = ""
                };

                _output.WriteLine("Step 1: Creating Bid using CustomCreate...");
                createdBidId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
                createdBidId.Should().NotBeEmpty("CustomCreate should return a valid ID.");
                _output.WriteLine($"Bid created successfully with ID: {createdBidId}");

                // 2. GetById
                _output.WriteLine("Step 2: Retrieving Bid using CustomGetById...");
                var retrievedBid = await bidRepository.CustomGetByIdAsync(createdBidId, CancellationToken.None);
                retrievedBid.Should().NotBeNull();
                retrievedBid.Id.Should().Be(createdBidId);
                retrievedBid.BidState.Should().Be(BidStateType.New);
                _output.WriteLine($"Bid retrieved successfully. ID: {retrievedBid.Id}, State: {retrievedBid.BidState}");

                // 3. GetByQuery
                _output.WriteLine("Step 3: Retrieving Bids using CustomGetByQuery...");
                var query = new BaseGetQuery<BidGetViewModel>
                {
                    Take = 1,
                    Offset = 0,
                    Filters = new List<FieldFilter>
                    {
                        new FieldFilter
                        {
                            FilterField = "Id",
                            FilterValue = createdBidId,
                            FilterOperator = FilterOperator.Equals
                        }
                    }
                };
                var queryResult = await bidRepository.CustomGetByQueryAsync(query, CancellationToken.None);
                queryResult.Should().ContainSingle(b => b.Id == createdBidId, "Query should find the created Bid.");
                _output.WriteLine($"Query returned {queryResult.Count} Bids, including the created one.");

                // 4. Update
                _output.WriteLine("Step 4: Updating Bid using CustomUpdate...");
                var updateModel = new BidUpdateViewModel
                {
                    Id = createdBidId,
                    BidTypeId = createModel.BidTypeId,
                    ForemanId = createModel.ForemanId,
                    DeadlineDate = createModel.DeadlineDate,
                    Lat = createModel.Lat,
                    Lng = createModel.Lng,
                    Number = createModel.Number,
                    FieldPlantingDate = createModel.FieldPlantingDate,
                    Customer = createModel.Customer,
                    CropId = createModel.CropId,
                    BidTemplateId = createModel.BidTemplateId,
                    IsUnvalidBid = createModel.IsUnvalidBid,
                    Status = createModel.Status,
                    Comment = $"{createModel.Comment} UPDATED",
                    Description = createModel.Description,
                    BidState = BidStateType.InProgress
                };

                var updatedBid = await bidRepository.CustomUpdateAsync(updateModel, CancellationToken.None);
                updatedBid.Should().NotBeNull();
                updatedBid.Id.Should().Be(createdBidId);
                updatedBid.Comment.Should().Contain("UPDATED");
                updatedBid.BidState.Should().Be(BidStateType.InProgress);
                _output.WriteLine($"Bid updated successfully. New State: {updatedBid.BidState}");

                // Verification: Check if update is persistent via GetById
                var verifiedUpdatedBid = await bidRepository.CustomGetByIdAsync(createdBidId, CancellationToken.None);
                verifiedUpdatedBid.Should().NotBeNull();
                verifiedUpdatedBid.Id.Should().Be(createdBidId);
                verifiedUpdatedBid.BidState.Should().Be(BidStateType.InProgress);
                _output.WriteLine($"Update verified via CustomGetById.");
            }
            finally
            {
                // 5. Delete
                if (createdBidId != Guid.Empty)
                {
                    _output.WriteLine("Step 5: Deleting Bid using Delete...");
                    var deleteResult = await bidRepository.DeleteAsync(createdBidId, CancellationToken.None);
                    deleteResult.Should().BeTrue("Delete should return true on success.");

                    // Проверяем, что сущность помечена как удалённая, а не физически удалена
                    var bidAfterDelete = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
                    bidAfterDelete.Should().NotBeNull("GetById should return the entity even after soft-delete.");
                    bidAfterDelete.Status.Should().Be(StatusType.Deleted, "Bid status should be changed to Deleted after the delete operation.");
                    bidAfterDelete.Id.Should().Be(createdBidId);
                }
                else
                {
                    _output.WriteLine("Bid was not created, skipping deletion step.");
                }
            }
        }

        [Fact]
        public async Task CRUD_StandartMethods_ShouldPerformAllOperations()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();
            Guid createdBidId = Guid.Empty;

            try
            {
                // 1. Create
                var createModel = new Common.Models.CropMonitoring.Bid
                {
                    BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                    DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                    Comment = $"Demo test CRUD Custom",
                    Description = $"Demo test CRUD Custom",
                    Lat = TestConstants.TEST_LAT,
                    Lng = TestConstants.TEST_LNG,
                    Number = 0,
                    FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                    Customer = $"Demo test CRUD Custom",
                    CropId = TestConstants.EXISTING_CROP_ID,
                    BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                    IsUnvalidBid = true,
                    //BidState = BidStateType.New
                };

                _output.WriteLine("Step 1: Creating Bid using CustomCreate...");
                createdBidId = await bidRepository.CreateAsync(createModel, CancellationToken.None);
                createdBidId.Should().NotBeEmpty("CustomCreate should return a valid ID.");
                _output.WriteLine($"Bid created successfully with ID: {createdBidId}");

                // 2. GetById
                _output.WriteLine("Step 2: Retrieving Bid using CustomGetById...");
                var retrievedBid = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
                retrievedBid.Should().NotBeNull();
                retrievedBid.Id.Should().Be(createdBidId);
                retrievedBid.BidState.Should().Be(BidStateType.New);
                _output.WriteLine($"Bid retrieved successfully. ID: {retrievedBid.Id}, State: {retrievedBid.BidState}");

                // 3. GetByQuery
                _output.WriteLine("Step 3: Retrieving Bids using CustomGetByQuery...");
                var query = new BaseGetQuery<Common.Models.CropMonitoring.Bid>
                {
                    Take = 1,
                    Offset = 0,
                    Filters = new List<FieldFilter>
                    {
                        new FieldFilter
                        {
                            FilterField = "Id",
                            FilterValue = createdBidId,
                            FilterOperator = FilterOperator.Equals
                        }
                    }
                };
                var queryResult = await bidRepository.GetByQueryAsync(query, CancellationToken.None);
                queryResult.Should().ContainSingle(b => b.Id == createdBidId, "Query should find the created Bid.");
                _output.WriteLine($"Query returned {queryResult.Count} Bids, including the created one.");

                // 4. Update
                _output.WriteLine("Step 4: Updating Bid using CustomUpdate...");
                var updateModel = new Common.Models.CropMonitoring.Bid
                {
                    Id = createdBidId,
                    BidTypeId = createModel.BidTypeId,
                    ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                    WorkerId = TestConstants.EXISTING_WORKER_ID,
                    DeadlineDate = createModel.DeadlineDate,
                    Lat = createModel.Lat,
                    Lng = createModel.Lng,
                    Number = createModel.Number,
                    FieldPlantingDate = createModel.FieldPlantingDate,
                    Customer = createModel.Customer,
                    CropId = createModel.CropId,
                    BidTemplateId = createModel.BidTemplateId,
                    IsUnvalidBid = createModel.IsUnvalidBid,
                    //Status = createModel.Status,
                    Comment = $"{createModel.Comment} UPDATED",
                    Description = createModel.Description,
                    BidState = BidStateType.InProgress
                };

                var updatedBid = await bidRepository.UpdateAsync(updateModel, CancellationToken.None);
                updatedBid.Should().NotBeNull();
                updatedBid.Id.Should().Be(createdBidId);
                updatedBid.Comment.Should().Contain("UPDATED");
                updatedBid.BidState.Should().Be(BidStateType.InProgress);
                _output.WriteLine($"Bid updated successfully. New State: {updatedBid.BidState}");

                // Verification: Check if update is persistent via GetById
                var verifiedUpdatedBid = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
                verifiedUpdatedBid.Should().NotBeNull();
                verifiedUpdatedBid.Id.Should().Be(createdBidId);
                verifiedUpdatedBid.BidState.Should().Be(BidStateType.InProgress);
                _output.WriteLine($"Update verified via CustomGetById.");
            }
            finally
            {
                // 5. Delete
                if (createdBidId != Guid.Empty)
                {
                    _output.WriteLine("Step 5: Deleting Bid using Delete...");
                    var deleteResult = await bidRepository.DeleteAsync(createdBidId, CancellationToken.None);
                    deleteResult.Should().BeTrue("Delete should return true on success.");

                    // Проверяем, что сущность помечена как удалённая, а не физически удалена
                    var bidAfterDelete = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
                    bidAfterDelete.Should().NotBeNull("GetById should return the entity even after soft-delete.");
                    bidAfterDelete.Status.Should().Be(StatusType.Deleted, "Bid status should be changed to Deleted after the delete operation.");
                    bidAfterDelete.Id.Should().Be(createdBidId);
                }
                else
                {
                    _output.WriteLine("Bid was not created, skipping deletion step.");
                }
            }
        }

        #endregion

        #region SaveResult

       /* [Fact]
        public async Task SaveResultAsync_WithValidFileAndExistingBidId_ShouldSaveFileAndReturnTrue()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            // Сначала создадим Bid, чтобы получить существующий ID
            _output.WriteLine("Creating a temporary Bid for SaveResult test...");
            var createModel = new BidCreateViewModel
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for SaveResult test",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Polygon = "",
                Status = StatusType.Active,
                //CreateAt = DateTime.UtcNow,
                //LastUpdateAt = DateTime.UtcNow
            };

            var bidId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary Bid with ID: {bidId}");

            // Создаем тестовый файл (например, простой ZIP-файл в виде байтов)
            // Сигнатура ZIP-файла: PK
            var fileBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00 };
            var fileContent = new StreamContent(new MemoryStream(fileBytes));

            using var formData = new MultipartFormDataContent();
            // ВАЖНО: Имя части формы должно совпадать с именем параметра в контроллере - "bidResultFile"
            formData.Add(fileContent, "bidResultFile", "test_result.zip");

            _output.WriteLine($"Attempting to save result for Bid ID: {bidId}");

            // Act
            var result = await bidRepository.SaveResultAsync(bidId, formData, CancellationToken.None);

            // Assert
            result.Should().BeTrue("SaveResultAsync should return true on successful file save.");
            _output.WriteLine($"SaveResultAsync completed successfully for Bid ID: {bidId}");

            // Optional: Проверим, что состояние Bid изменилось на Finished
            // Для этого нам нужно получить Bid через CustomGetByIdAsync или GetByQuery
            // CustomGetByIdAsync возвращает BidGetViewModel, где BidStateType доступен
            var updatedBid = await bidRepository.CustomGetByIdAsync(bidId, CancellationToken.None);
            updatedBid.Should().NotBeNull();
            updatedBid.BidState.Should().Be(BidStateType.Finished, "Bid state should be updated to Finished after saving result.");
            _output.WriteLine($"Verified Bid ID: {bidId} state is now Finished.");
        }

        [Fact]
        public async Task SaveResultAsync_WithValidFileButNonExistentBidId_ShouldThrowNotFound()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();
            var nonExistentBidId = Guid.NewGuid();
            _output.WriteLine($"Testing SaveResultAsync with non-existent Bid ID: {nonExistentBidId}");

            var fileBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
            var fileContent = new StreamContent(new MemoryStream(fileBytes));

            using var formData = new MultipartFormDataContent();
            formData.Add(fileContent, "bidResultFile", "test_result.zip");

            // Act
            Func<Task> act = async () => await bidRepository.SaveResultAsync(nonExistentBidId, formData, CancellationToken.None);

            // Assert
            // Ожидаем, что API вернет 404 Not Found, что должно привести к HttpRequestException
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.NotFound);

            _output.WriteLine("Test completed successfully. Expected 404 Not Found exception was thrown for non-existent Bid ID.");
        }

        [Fact]
        public async Task SaveResultAsync_WithoutFile_ShouldHandleGracefully()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            // Создаем Bid
            var createModel = new BidCreateViewModel
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for SaveResult test (no file)",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Polygon = "",
                Status = StatusType.Active,
                //CreateAt = DateTime.UtcNow,
                //LastUpdateAt = DateTime.UtcNow
            };

            var bidId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary Bid with ID: {bidId} for test without file");

            // Создаем пустой MultipartFormDataContent
            using var emptyFormData = new MultipartFormDataContent();

            _output.WriteLine($"Attempting to save result for Bid ID: {bidId} without a file");

            // Act & Assert
            // Поведение может отличаться. API может вернуть 400 Bad Request или обработать как ошибку.
            // Мы ожидаем, что это приведет к исключению.
            Func<Task> act = async () => await bidRepository.SaveResultAsync(bidId, emptyFormData, CancellationToken.None);

            // В зависимости от реализации бэкенда, это может быть 400 или 500
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.InternalServerError);

            _output.WriteLine("Test completed successfully. Expected exception was thrown for request without file.");
        }*/

        #endregion

        #region GetTotal

        [Fact]
        public async Task GetTotalCountAsync_ShouldReturnTotalCount()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            _output.WriteLine("Getting total count of Bids...");

            // Создаём простой запрос для получения общего количества (фильтры могут быть null)
            var query = new BaseGetQuery<Bid> { Take = null, Offset = 0 }; // Полный подсчет, без фильтрации

            // Act
            var count = await bidRepository.GetTotalCountAsync(query, CancellationToken.None);

            // Assert
            count.Should().BeGreaterThanOrEqualTo(0, "Total count should be zero or more.");
            _output.WriteLine($"Standard GetTotalCountAsync test completed successfully. Total count: {count}");
        }

        #endregion

        #region ImportFromCSV



        #endregion


        /*
        #region CustomGetByQuery

        [Fact]
        public async Task CustomGetByQueryAsync_WithValidQuery_ShouldReturnListOfBidGetViewModel()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            var query = new BaseGetQuery<BidGetViewModel>
            {
                Take = 5, // Получим первые 5 записей
                Offset = 0,
            };

            _output.WriteLine("Executing CustomGetByQueryAsync with a query to get up to 5 Bids...");

            // Act
            var result = await bidRepository.CustomGetByQueryAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Result list should not be null.");
            result.Should().BeAssignableTo<List<BidGetViewModel>>("Result should be a list of BidGetViewModel.");

            // Проверим, что результат является списком
            var resultList = result as List<BidGetViewModel>;
            resultList.Should().NotBeNull();

            // Проверим ограничение по количеству (может быть меньше 5, если всего записей меньше)
            resultList!.Count.Should().BeLessThanOrEqualTo(5, "Query requested up to 5 items.");

            // Если список не пуст, проверим структуру первого элемента
            if (resultList.Count > 0)
            {
                var firstBid = resultList[0];
                firstBid.Id.Should().NotBeEmpty("Each Bid should have a valid ID.");
                // firstBid.Number.Should().BeGreaterOrEqualTo(0); // Проверка, если Number всегда >= 0
                _output.WriteLine($"First retrieved Bid ID: {firstBid.Id}");
            }

            _output.WriteLine($"Test completed successfully. Retrieved {resultList!.Count} Bids.");
        }

        [Fact]
        public async Task CustomGetByQueryAsync_WithQueryForNonExistentItems_ShouldReturnEmptyList()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            // Создадим фильтр, который вряд ли найдет что-то
            // Например, фильтр по несуществующему ID
            var nonExistentId = Guid.NewGuid();
            var query = new BaseGetQuery<BidGetViewModel>
            {
                Take = 10,
                Offset = 0,
                Sort = SortType.ASC,
                SortBy = "Id",
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FilterField = "Id", // Предполагаем, что можно фильтровать по Id
                        FilterValue = nonExistentId,
                        FilterOperator = FilterOperator.Equals
                    }
                }
            };

            _output.WriteLine($"Executing CustomGetByQueryAsync with a query that should return no results (filtering by non-existent ID: {nonExistentId})...");

            // Act
            var result = await bidRepository.CustomGetByQueryAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Result list should not be null even if empty.");
            result.Should().BeAssignableTo<List<BidGetViewModel>>("Result should be a list of BidGetViewModel.");

            var resultList = result as List<BidGetViewModel>;
            resultList.Should().NotBeNull();
            resultList!.Count.Should().Be(0, "Query should return no results for a non-existent ID filter.");

            _output.WriteLine("Test completed successfully. Empty list was returned as expected.");
        }

        #endregion

        #region CustomGetById

        [Fact]
        public async Task CustomGetByIdAsync_WithValidExistingId_ShouldReturnBidGetViewModel()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            _output.WriteLine("Getting a list of Bids to find an existing ID...");
            var queryForList = new BaseGetQuery<BidGetViewModel> { Take = 1 };
            var bidsList = await bidRepository.CustomGetByQueryAsync(queryForList, CancellationToken.None);

            if (bidsList == null || bidsList.Count == 0)
            {
                _output.WriteLine("No existing Bids found. Creating a new one for the test...");
                // Если нет существующих Bid, создадим один
                var createModel = new BidCreateViewModel
                {
                    BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                    ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                    DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                    Comment = $"{TestConstants.TEST_COMMENT} for GetByID test",
                    Description = TestConstants.TEST_DESCRIPTION,
                    Lat = TestConstants.TEST_LAT,
                    Lng = TestConstants.TEST_LNG,
                    Number = 0,
                    FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                    Customer = TestConstants.TEST_CUSTOMER,
                    CropId = TestConstants.EXISTING_CROP_ID,
                    BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                    IsUnvalidBid = true,
                    BidState = BidStateType.New,
                    Polygon = "",
                    Status = StatusType.Active,
                    //CreateAt = DateTime.UtcNow,
                    //LastUpdateAt = DateTime.UtcNow
                };

                var createdId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
                _output.WriteLine($"Created temporary Bid with ID: {createdId} for GetByID test.");

                // Теперь используем этот ID
                var retrievedBid = await bidRepository.CustomGetByIdAsync(createdId, CancellationToken.None);
                retrievedBid.Should().NotBeNull();
                retrievedBid.Id.Should().Be(createdId);
                _output.WriteLine($"Test completed successfully. Retrieved Bid ID: {retrievedBid.Id}");
                return;
            }

            // Если список не пуст, используем первый ID
            var existingBidId = bidsList.First().Id;
            _output.WriteLine($"Using existing Bid ID: {existingBidId}");

            // Act
            var result = await bidRepository.CustomGetByIdAsync(existingBidId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull($"Bid with ID {existingBidId} should exist.");
            result.Id.Should().Be(existingBidId);
            _output.WriteLine($"Test completed successfully. Retrieved Bid ID: {result.Id}");
        }

        [Fact]
        public async Task CustomGetByIdAsync_WithInvalidNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();
            var nonExistentId = Guid.NewGuid(); // Генерируем заведомо несуществующий ID
            _output.WriteLine($"Testing CustomGetByIdAsync with non-existent ID: {nonExistentId}");

            // Act
            BidGetViewModel? result = null;
            Exception? caughtException = null;
            try
            {
                result = await bidRepository.CustomGetByIdAsync(nonExistentId, CancellationToken.None);
            }
            catch (Exception ex)
            {
                caughtException = ex;
                _output.WriteLine($"Exception caught: {ex.GetType().Name} - {ex.Message}");
            }

            // Assert
            // В зависимости от реализации API, метод может:
            // 1. Вернуть null (если API возвращает 200 OK с null телом)
            // 2. Выбросить HttpRequestException с кодом 404 (если API возвращает 404 Not Found)
            if (caughtException != null)
            {
                // Если было исключение, проверим, что это ожидаемый тип
                caughtException.Should().BeOfType<System.Net.Http.HttpRequestException>();
                var httpEx = (System.Net.Http.HttpRequestException)caughtException;
                httpEx.StatusCode.Should().Be(HttpStatusCode.NotFound);
                _output.WriteLine("Test completed successfully. Expected 404 Not Found exception was thrown.");
            }
            else
            {
                // Если исключения не было, результат должен быть null
                result.Should().BeNull($"Bid with non-existent ID {nonExistentId} should not be found and return null.");
                _output.WriteLine("Test completed successfully. Null was returned for non-existent ID.");
            }
        }

        #endregion

        #region CustomCreate

        [Fact]
        public async Task CustomCreateAsync_WithValidModel_ShouldCreateBidAndReturnId()
        {
            // Arrange
            _output.WriteLine("Starting CustomCreateAsync test...");
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            var createModel = new BidCreateViewModel
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = TestConstants.TEST_COMMENT,
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Polygon = "",
                Status = Masofa.Common.Models.StatusType.Active,
                //CreateAt = DateTime.UtcNow,
                //LastUpdateAt = DateTime.UtcNow
            };

            _output.WriteLine($"Attempting to create Bid with model: {System.Text.Json.JsonSerializer.Serialize(createModel, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

            // Act
            Guid createdId;
            try
            {
                createdId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
                _output.WriteLine($"Bid created successfully with ID: {createdId}");
            }
            catch (Exception ex) when (ex is not Xunit.Sdk.XunitException)
            {
                _output.WriteLine($"CustomCreateAsync failed unexpectedly: {ex}");
                throw;
            }

            // Assert
            createdId.Should().NotBeEmpty("Created Bid ID should not be empty Guid");

            _output.WriteLine("Verifying created Bid exists...");
            BidGetViewModel? retrievedBid = null;
            try
            {
                retrievedBid = await bidRepository.CustomGetByIdAsync(createdId, CancellationToken.None);
            }
            catch (System.Net.Http.HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                _output.WriteLine($"Bid with ID {createdId} not found immediately. This might be due to eventual consistency. Retrying once...");
                await Task.Delay(500); // Небольшая задержка
                retrievedBid = await bidRepository.CustomGetByIdAsync(createdId, CancellationToken.None);
            }

            retrievedBid.Should().NotBeNull($"Bid with ID {createdId} should be retrievable after creation");

            retrievedBid.Id.Should().Be(createdId);
            retrievedBid.BidState.Should().Be(BidStateType.New);

            _output.WriteLine($"Test completed successfully. Created and verified Bid ID: {createdId}");
        }

        #endregion

        #region CustomUpdate

        [Fact]
        public async Task CustomUpdateAsync_WithValidModel_ShouldUpdateBidAndReturnUpdatedEntity()
        {
            // Arrange
            var bidRepository = await CreateAuthenticatedBidRepositoryAsync();

            var originalComment = $"{TestConstants.TEST_COMMENT} for Custom Update test (initial)";
            var originalDescription = TestConstants.TEST_DESCRIPTION;
            var originalCustomer = TestConstants.TEST_CUSTOMER;
            var originalBidState = BidStateType.New;
            var originalDeadlineDate = TestConstants.TEST_DEADLINE_DATE;
            var originalFieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE;
            var originalLat = TestConstants.TEST_LAT;
            var originalLng = TestConstants.TEST_LNG;
            var originalNumber = 0L;
            var originalStatus = StatusType.Active;
            var originalCreateAt = DateTime.UtcNow;
            var originalCreateUser = Guid.NewGuid();
            var originalLastUpdateUser = Guid.NewGuid();

            var createModel = new BidCreateViewModel
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = originalDeadlineDate,
                Comment = originalComment,
                Description = originalDescription,
                Lat = originalLat,
                Lng = originalLng,
                Number = originalNumber,
                FieldPlantingDate = originalFieldPlantingDate,
                Customer = originalCustomer,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = originalBidState,
                Polygon = "",
                Status = originalStatus,
                //CreateAt = originalCreateAt,
                //LastUpdateAt = originalCreateAt,
                //CreateUser = originalCreateUser,
                //LastUpdateUser = originalLastUpdateUser
            };

            var createdBidId = await bidRepository.CustomCreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary Bid with ID: {createdBidId} for Custom Update test.");

            var existingBidViewModel = await bidRepository.CustomGetByIdAsync(createdBidId, CancellationToken.None);
            existingBidViewModel.Should().NotBeNull();
            existingBidViewModel.Id.Should().Be(createdBidId);
            existingBidViewModel.BidState.Should().Be(originalBidState);

            var updateModel = new BidUpdateViewModel
            {
                Id = createdBidId,
                // --- Копируем обязательные/важные поля из createModel ---
                BidTypeId = createModel.BidTypeId,
                ForemanId = createModel.ForemanId,
                DeadlineDate = createModel.DeadlineDate,
                Lat = createModel.Lat,
                Lng = createModel.Lng,
                Number = createModel.Number,
                FieldPlantingDate = createModel.FieldPlantingDate,
                Customer = createModel.Customer,
                CropId = createModel.CropId,
                BidTemplateId = createModel.BidTemplateId,
                IsUnvalidBid = createModel.IsUnvalidBid,
                Status = createModel.Status,
                //CreateAt = createModel.CreateAt, // Оставляем дату создания как есть
                //LastUpdateAt = DateTime.UtcNow, // Обновляем дату обновления
                //CreateUser = createModel.CreateUser, // Оставляем создателя как есть
                //LastUpdateUser = createModel.LastUpdateUser, // Обновляем пользователя обновления

                ParentId = createModel.ParentId,
                WorkerId = createModel.WorkerId,
                StartDate = createModel.StartDate,
                EndDate = createModel.EndDate,
                FieldId = createModel.FieldId,
                RegionId = createModel.RegionId,
                VarietyId = createModel.VarietyId,
                FileResultId = createModel.FileResultId,

                // --- Изменяем то, что хотим обновить ---
                Comment = $"{TestConstants.TEST_COMMENT} for Custom Update test (UPDATED)",
                Description = $"{TestConstants.TEST_DESCRIPTION} - Updated via CustomUpdate",
                BidState = BidStateType.InProgress // Изменим статус
            };

            _output.WriteLine($"Attempting to update Bid ID: {createdBidId} using CustomUpdateAsync.");

            // Act
            var updatedBid = await bidRepository.CustomUpdateAsync(updateModel, CancellationToken.None);

            // Assert
            updatedBid.Should().NotBeNull("Updated Bid should be returned by CustomUpdateAsync.");
            updatedBid.Id.Should().Be(createdBidId, "Updated Bid ID should match the requested ID.");
            // Проверяем, что измененные поля действительно обновились
            updatedBid.Comment.Should().Be(updateModel.Comment, "Comment should be updated.");
            updatedBid.BidState.Should().Be(updateModel.BidState, "BidState should be updated.");
            updatedBid.Description.Should().Be(updateModel.Description, "Description should be updated.");
            // Проверяем, что неизмененные поля остались как были (если API их не сбросил)
            updatedBid.BidTypeId.Should().Be(updateModel.BidTypeId, "BidTypeId should remain the same.");
            updatedBid.DeadlineDate.Should().Be(updateModel.DeadlineDate, "DeadlineDate should remain the same.");
            updatedBid.Lat.Should().Be(updateModel.Lat, "Lat should remain the same.");
            updatedBid.Lng.Should().Be(updateModel.Lng, "Lng should remain the same.");
            updatedBid.Customer.Should().Be(updateModel.Customer, "Customer should remain the same.");
            updatedBid.CropId.Should().Be(updateModel.CropId, "CropId should remain the same.");

            _output.WriteLine($"CustomUpdateAsync test completed successfully for Bid ID: {createdBidId}");
        }

        #endregion

        #region GetByQuery

        [Fact]
        public async Task GetByQueryAsync_WithValidQuery_ShouldReturnListOfBid()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            var query = new BaseGetQuery<Masofa.Common.Models.CropMonitoring.Bid>
            {
                Take = 5,
                Offset = 0,
                Sort = SortType.ASC,
                SortBy = "Id"
            };

            _output.WriteLine("Executing Standard GetByQueryAsync with a query to get up to 5 Bids...");

            // Act
            var result = await bidRepository.GetByQueryAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Result list should not be null.");
            result.Should().BeAssignableTo<List<Masofa.Common.Models.CropMonitoring.Bid>>("Result should be a list of Bid.");
            result.Count.Should().BeLessThanOrEqualTo(5, "Query requested up to 5 items.");
            if (result.Count > 0)
            {
                var firstBid = result[0];
                firstBid.Id.Should().NotBeEmpty("Each Bid should have a valid ID.");
            }
            _output.WriteLine($"Standard GetByQueryAsync test completed successfully. Retrieved {result.Count} Bids.");
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetByIdAsync_WithValidExistingId_ShouldReturnBid()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            // Сначала создадим Bid для теста
            var createModel = new Masofa.Common.Models.CropMonitoring.Bid
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for Standard GetByID test",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = Guid.NewGuid(),
                LastUpdateUser = Guid.NewGuid()
            };

            var createdId = await bidRepository.CreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary Bid with ID: {createdId} for Standard GetById test.");

            // Act
            var result = await bidRepository.GetByIdAsync(createdId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull($"Bid with ID {createdId} should exist via standard GetById.");
            result.Id.Should().Be(createdId);
            _output.WriteLine($"Standard GetByIdAsync test completed successfully. Retrieved Bid ID: {result.Id}");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            var nonExistentId = Guid.NewGuid();
            _output.WriteLine($"Testing Standard GetByIdAsync with non-existent ID: {nonExistentId}");

            // Act & Assert
            Func<Task> act = async () => await bidRepository.GetByIdAsync(nonExistentId, CancellationToken.None);
            await act.Should().ThrowAsync<System.Net.Http.HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.NotFound);

            _output.WriteLine("Standard GetByIdAsync test completed successfully. Expected 404 Not Found exception was thrown.");
        }

        #endregion

        #region Create

        [Fact]
        public async Task CreateAsync_WithValidModel_ShouldCreateBidAndReturnId()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            var createModel = new Masofa.Common.Models.CropMonitoring.Bid
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for Standard Create test",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = TestConstants.TEST_USER_GUID,
                LastUpdateUser = TestConstants.TEST_USER_GUID
            };

            _output.WriteLine($"Attempting to create Bid with standard Create method using model: {System.Text.Json.JsonSerializer.Serialize(createModel, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

            // Act
            Guid createdId;
            try
            {
                createdId = await bidRepository.CreateAsync(createModel, CancellationToken.None);
                _output.WriteLine($"Bid created successfully with ID: {createdId}");
            }
            catch (Exception ex) when (ex is not Xunit.Sdk.XunitException)
            {
                _output.WriteLine($"Standard CreateAsync failed unexpectedly: {ex}");
                throw;
            }

            // Assert
            createdId.Should().NotBeEmpty("Created Bid ID should not be empty Guid");

            _output.WriteLine("Verifying created Bid exists via standard GetById...");
            Masofa.Common.Models.CropMonitoring.Bid? retrievedBid = null;
            try
            {
                retrievedBid = await bidRepository.GetByIdAsync(createdId, CancellationToken.None);
            }
            catch (System.Net.Http.HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                _output.WriteLine($"Bid with ID {createdId} not found immediately. Retrying once...");
                await Task.Delay(500); // Небольшая задержка
                retrievedBid = await bidRepository.GetByIdAsync(createdId, CancellationToken.None);
            }

            retrievedBid.Should().NotBeNull($"Bid with ID {createdId} should be retrievable after creation via standard GetById");
            retrievedBid.Id.Should().Be(createdId);
            retrievedBid.BidState.Should().Be(BidStateType.New);

            _output.WriteLine($"Standard CreateAsync test completed successfully. Created and verified Bid ID: {createdId}");
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateAsync_WithValidModel_ShouldUpdateBidAndReturnUpdatedEntity()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            // 1. Создадим Bid
            var createModel = new Masofa.Common.Models.CropMonitoring.Bid
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for Standard Update test (initial)",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = Guid.NewGuid(),
                LastUpdateUser = Guid.NewGuid()
            };

            var createdBidId = await bidRepository.CreateAsync(createModel, CancellationToken.None);
            _output.WriteLine($"Created temporary Bid with ID: {createdBidId} for Standard Update test.");

            // 2. Получим созданную Bid
            var existingBid = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
            existingBid.Should().NotBeNull();

            // 3. Подготовим модель для обновления (изменяем нужные поля)
            var updateModel = new Masofa.Common.Models.CropMonitoring.Bid
            {
                Id = existingBid.Id, // Обязательно указываем Id
                // Копируем остальные поля из existingBid
                BidTypeId = existingBid.BidTypeId,
                ForemanId = existingBid.ForemanId,
                DeadlineDate = existingBid.DeadlineDate,
                Comment = $"{TestConstants.TEST_COMMENT} for Standard Update test (UPDATED)", // Меняем комментарий
                Description = existingBid.Description,
                Lat = existingBid.Lat,
                Lng = existingBid.Lng,
                Number = existingBid.Number,
                FieldPlantingDate = existingBid.FieldPlantingDate,
                Customer = existingBid.Customer,
                CropId = existingBid.CropId,
                BidTemplateId = existingBid.BidTemplateId,
                IsUnvalidBid = existingBid.IsUnvalidBid,
                BidState = BidStateType.InProgress, // Меняем статус
                Status = existingBid.Status,
                CreateAt = existingBid.CreateAt, // Не меняем дату создания
                LastUpdateAt = DateTime.UtcNow, // Обновляем дату обновления
                CreateUser = existingBid.CreateUser, // Не меняем создателя
                LastUpdateUser = Guid.NewGuid() // Обновляем пользователя обновления
                // Остальные поля по необходимости
            };

            _output.WriteLine($"Attempting to update Bid ID: {createdBidId} with standard Update method.");

            // Act
            var updatedBid = await bidRepository.UpdateAsync(updateModel, CancellationToken.None);

            // Assert
            updatedBid.Should().NotBeNull("Updated Bid should be returned.");
            updatedBid.Id.Should().Be(createdBidId, "Updated Bid ID should match the requested ID.");
            updatedBid.Comment.Should().Be(updateModel.Comment, "Comment should be updated.");
            updatedBid.BidState.Should().Be(updateModel.BidState, "BidState should be updated.");

            _output.WriteLine($"Standard UpdateAsync test completed successfully for Bid ID: {createdBidId}");
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_WithValidExistingId_ShouldDeleteBidAndReturnTrue()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
            await unitOfWork.LoginAsync(new LoginAndPasswordViewModel { UserName = TestConstants.TEST_USERNAME, Password = TestConstants.TEST_PASSWORD }, CancellationToken.None);

            var bidRepository = unitOfWork.BidRepository;

            var createModel = new Masofa.Common.Models.CropMonitoring.Bid
            {
                BidTypeId = TestConstants.EXISTING_BID_TYPE_ID,
                ForemanId = TestConstants.EXISTING_FOREMAN_ID,
                DeadlineDate = TestConstants.TEST_DEADLINE_DATE,
                Comment = $"{TestConstants.TEST_COMMENT} for Standard Delete test",
                Description = TestConstants.TEST_DESCRIPTION,
                Lat = TestConstants.TEST_LAT,
                Lng = TestConstants.TEST_LNG,
                Number = 0,
                FieldPlantingDate = TestConstants.TEST_FIELD_PLANTING_DATE,
                Customer = TestConstants.TEST_CUSTOMER,
                CropId = TestConstants.EXISTING_CROP_ID,
                BidTemplateId = TestConstants.EXISTING_BID_TEMPLATE_ID,
                IsUnvalidBid = true,
                BidState = BidStateType.New,
                Status = StatusType.Active,
                CreateAt = DateTime.UtcNow,
                LastUpdateAt = DateTime.UtcNow,
                CreateUser = Guid.NewGuid(),
                LastUpdateUser = Guid.NewGuid()
            };

            var createdBidId = await bidRepository.CreateAsync(createModel, CancellationToken.None);

            var bidBeforeDelete = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
            bidBeforeDelete.Should().NotBeNull();
            bidBeforeDelete.Status.Should().NotBe(StatusType.Deleted);

            // Act
            var deleteResult = await bidRepository.DeleteAsync(createdBidId, CancellationToken.None);

            // Assert
            deleteResult.Should().BeTrue("Delete operation should return true on success.");

            // Проверяем, что сущность помечена как удалённая, а не физически удалена
            var bidAfterDelete = await bidRepository.GetByIdAsync(createdBidId, CancellationToken.None);
            bidAfterDelete.Should().NotBeNull("GetById should return the entity even after soft-delete.");
            bidAfterDelete.Status.Should().Be(StatusType.Deleted, "Bid status should be changed to Deleted after the delete operation.");
            bidAfterDelete.Id.Should().Be(createdBidId);
        }

        #endregion
        */
    }
}
