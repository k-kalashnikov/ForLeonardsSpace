using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account; // для LoginAndPasswordViewModel
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.Identity
{
    public class UserDeviceTest
    {
        private readonly ITestOutputHelper _output;

        public UserDeviceTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CrudOperations_UserDevice_ShouldSucceedAndCleanup()
        {

            // Arrange
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);
            Console.WriteLine("Логин успешен. Токен установлен.");

            var uniqueDeviceId = "DEMO test-device-" + Guid.NewGuid().ToString("N")[..8];
            var initialDeviceName = "DEMO Initial Test Device";
            var updatedDeviceName = "DEMO Updated Test Device";

            var newDevice = new UserDevice
            {
                UserId = TestConstants.TEST_USER_GUID, // Используйте реальный ID пользователя
                DeviceId = uniqueDeviceId,
                DeviceName = initialDeviceName,
                Platform = "DEMO TestPlatform",
                OsVersion = "1.0",
                AppVersion = "1.0.0",
                PushToken = "fake-push-token",
                IsActive = true,
                RegisteredAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow
            };
            _output.WriteLine($"Подготовлена сущность UserDevice для создания: DeviceId='{uniqueDeviceId}', DeviceName='{initialDeviceName}'");

            Guid createdId = Guid.Empty;

            // --- 1. CREATE ---
            // Act
            createdId = await uow.UserDeviceRepository.CreateAsync(newDevice, CancellationToken.None);

            // Assert
            createdId.Should().NotBeEmpty();
            _output.WriteLine($"Создание прошло успешно. Получен ID: {createdId}");

            // --- 2. GET BY ID ---
            // Act
            var fetchedDevice = await uow.UserDeviceRepository.GetByIdAsync(createdId, CancellationToken.None);

            // Assert
            fetchedDevice.Should().NotBeNull();
            fetchedDevice.Id.Should().Be(createdId);
            fetchedDevice.DeviceId.Should().Be(uniqueDeviceId);
            fetchedDevice.DeviceName.Should().Be(initialDeviceName);
            _output.WriteLine($"Получение по ID прошло успешно. Найдена сущность: Id={fetchedDevice.Id}, DeviceId='{fetchedDevice.DeviceId}', DeviceName='{fetchedDevice.DeviceName}'");

            // --- 3. GET BY QUERY (с фильтром) ---
            var query = new BaseGetQuery<UserDevice>
            {
                Take = 10,
                Offset = 0,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FilterField = "DeviceId",
                        FilterValue = uniqueDeviceId,
                        FilterOperator = FilterOperator.Equals // Предполагаем, что FilterOperator определён
                    }
                }
            };
            _output.WriteLine($"Отправляем запрос с фильтром по DeviceId='{uniqueDeviceId}'");

            // Act
            var queriedDevices = await uow.UserDeviceRepository.GetByQueryAsync(query, CancellationToken.None);

            // Assert
            queriedDevices.Should().NotBeNull();
            queriedDevices.Should().ContainSingle(d => d.DeviceId == uniqueDeviceId);
            _output.WriteLine($"GetByQuery прошёл успешно. Найдено {queriedDevices.Count} сущностей, соответствующих фильтру.");

            // --- 4. UPDATE ---
            fetchedDevice.DeviceName = updatedDeviceName;
            fetchedDevice.IsActive = false;
            _output.WriteLine($"Подготовлены изменения: DeviceName='{updatedDeviceName}', IsActive={false}");

            // Act
            var updatedDevice = await uow.UserDeviceRepository.UpdateAsync(fetchedDevice, CancellationToken.None);

            // Assert
            updatedDevice.Should().NotBeNull();
            updatedDevice.Id.Should().Be(createdId);
            updatedDevice.DeviceName.Should().Be(updatedDeviceName);
            updatedDevice.IsActive.Should().BeFalse();
            _output.WriteLine($"Обновление прошло успешно. Обновлённая сущность: Id={updatedDevice.Id}, DeviceName='{updatedDevice.DeviceName}', IsActive={updatedDevice.IsActive}");

            // --- 5. VERIFY UPDATE via GET BY ID ---
            // Act
            var fetchedAfterUpdate = await uow.UserDeviceRepository.GetByIdAsync(createdId, CancellationToken.None);

            // Assert
            fetchedAfterUpdate.Should().NotBeNull();
            fetchedAfterUpdate.DeviceName.Should().Be(updatedDeviceName);
            fetchedAfterUpdate.IsActive.Should().BeFalse();
            _output.WriteLine($"Проверка после Update прошла успешно. Получена сущность: Id={fetchedAfterUpdate.Id}, DeviceName='{fetchedAfterUpdate.DeviceName}', IsActive={fetchedAfterUpdate.IsActive}");

            // --- 6. DELETE ---
            // Act
            var deleteResult = await uow.UserDeviceRepository.DeleteAsync(createdId, CancellationToken.None);

            // Assert
            deleteResult.Should().BeTrue();
            _output.WriteLine($"Удаление прошло успешно. Результат: {deleteResult}");

            // --- 7. VERIFY DELETE (optional: try to get by ID after delete) ---

            var userDeviceAfterDelete = await uow.UserDeviceRepository.GetByIdAsync(createdId, CancellationToken.None);
            userDeviceAfterDelete.Should().NotBeNull("GetById должен возвращать сущность даже после удаления.");
            userDeviceAfterDelete.Status.Should().Be(StatusType.Deleted, "Статус должен быть 'Deleted' после операции удаления.");
            userDeviceAfterDelete.Id.Should().Be(createdId);
            _output.WriteLine("Подтверждено: сущность не удалена физически, а помечена как 'Deleted'.");
        }

        [Fact]
        public async Task GetTotalCountAsync_WithQuery_ShouldReturnCount()
        {
            // Arrange
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);

            // Создаём простой запрос для получения общего количества (фильтры могут быть null)
            var query = new BaseGetQuery<UserDevice> { Take = null, Offset = 0 }; // Полный подсчет, без фильтрации

            // Act
            var count = await uow.UserDeviceRepository.GetTotalCountAsync(query, CancellationToken.None);

            // Assert
            count.Should().BeGreaterThanOrEqualTo(0); // Должно быть >= 0
            _output.WriteLine($"GetTotalCount прошёл успешно. Общее количество: {count}");
        }

        // Тесты для ImportFromCSV и ExportFromCSV опциональны, так как требуют файлов или сложной настройки
        // [Fact] public async Task ImportFromCSVAsync_... { ... }
        // [Fact] public async Task ExportFromCSVAsync_... { ... }
    }
}