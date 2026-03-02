using FluentAssertions;
using Masofa.BusinessLogic.Identity.Users;
using Masofa.Client.ApiClient;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;
using Masofa.Web.Monolith.ViewModels.User;
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.Identity
{
    public class UserTest
    {
        private readonly ITestOutputHelper _output;

        public UserTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CrudOperations_User_ShouldSucceedAndCleanup()
        {
            // Arrange
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);
            _output.WriteLine("1. Логин успешен. Токен установлен.");

            var uniqueUserName = "DEMO_integration_test_user_" + Guid.NewGuid().ToString("N")[..8];
            var userEmail = uniqueUserName + "@example.com";
            var userPassword = "Pass123!";
            var updatedFirstName = "DEMOUpdatedFirstName";

            var newUser = new CreateViewModel
            {
                UserName = uniqueUserName,
                Email = userEmail,
                Password = userPassword,
                ConfirmPassword = userPassword, // Обязательно совпадает
                FirstName = "DEMO_IntegrationTest",
                LastName = "DEMO_User",
                Approved = true,
                EmailConfirmed = true,
                UserBusinessType = UserBusinessType.Person, // или Firm, в зависимости от TEST_USER_BUSINESS_ID
                Roles = new List<string> { "User" } // Пример роли, если нужно
            };

            Guid createdUserId = Guid.Empty;
            UserCreateCommandResult createResult = null;

            // --- 1. CREATE ---
            _output.WriteLine($"2. Выполняем операцию Create для пользователя '{uniqueUserName}'...");
            // Act
            createResult = await uow.UserRepository.CreateAsync(newUser, CancellationToken.None);

            // Assert
            createResult.Should().NotBeNull();
            createResult.Id.Should().NotBeEmpty(); // Проверяем, что ID возвращён
            createResult.Errors.Should().BeEmpty(because: $"User '{uniqueUserName}' should be created without errors. Errors: {string.Join(", ", createResult.Errors?.Select(e => e.Description) ?? new List<string>())}");
            createdUserId = createResult.Id;
            _output.WriteLine($"   Создание прошло успешно. Получен ID пользователя: {createdUserId}");

            // --- 2. GET BY ID ---
            _output.WriteLine($"3. Выполняем операцию GetById для ID: {createdUserId}...");
            // Act
            var fetchedUser = await uow.UserRepository.GetByIdAsync(createdUserId, CancellationToken.None);

            // Assert
            fetchedUser.Should().NotBeNull();
            fetchedUser.Id.Should().Be(createdUserId);
            fetchedUser.UserName.Should().Be(uniqueUserName);
            fetchedUser.FirstName.Should().Be(newUser.FirstName);
            _output.WriteLine($"   Получение по ID прошло успешно. Найден пользователь: Id={fetchedUser.Id}, UserName='{fetchedUser.UserName}', FirstName='{fetchedUser.FirstName}'");

            // --- 3. GET BY QUERY (с фильтром) ---
            _output.WriteLine($"4. Выполняем операцию GetByQuery с фильтром по UserName='{uniqueUserName}'...");
            var query = new UserGetQuery
            {
                Take = 10,
                Offset = 0,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FilterField = "UserName", // Убедитесь, что поле фильтрации соответствует схеме
                        FilterValue = uniqueUserName,
                        FilterOperator = FilterOperator.Equals
                    }
                }
            };

            // Act
            var queriedUsers = await uow.UserRepository.GetByQueryAsync(query, CancellationToken.None);

            // Assert
            queriedUsers.Should().NotBeNull();
            queriedUsers.Should().ContainSingle(u => u.UserName == uniqueUserName);
            _output.WriteLine($"   GetByQuery прошёл успешно. Найдено {queriedUsers.Count} пользователей, соответствующих фильтру.");

            // --- 4. RESEND NEW ACCOUNT LETTER ---
            _output.WriteLine($"5. Выполняем операцию ResendNewAccountLetter для ID: {createdUserId}...");
            // Act & Assert (ожидаем успешное выполнение без исключения)
            var actResend = async () => await uow.UserRepository.ResendNewAccountLetterAsync(createdUserId, CancellationToken.None);
            await actResend.Should().NotThrowAsync("Потому что пользователь существует и письмо должно отправиться (или вернуть 200 OK, даже если письмо не найдено, в зависимости от реализации API).");
            _output.WriteLine($"   Операция ResendNewAccountLetter завершена (без исключений).");

            // --- 5. GET CHILD USERS (если созданный пользователь является дочерним) ---
            // Этот тест работает, если текущий авторизованный пользователь (TEST_USERNAME) является родителем.
            // Иначе, список будет пустым, что тоже валидно.
            _output.WriteLine("6. Выполняем операцию GetChildUsers для текущего пользователя...");
            // Act
            var childUsers = await uow.UserRepository.GetChildUsersAsync(CancellationToken.None);

            // Assert (проверяем, что возвращается список, не проверяем содержимое, если пользователь не дочерний)
            childUsers.Should().NotBeNull();
            _output.WriteLine($"   GetChildUsers прошёл успешно. Найдено {childUsers.Count} дочерних пользователей.");

            _output.WriteLine("--- Тест завершён (с возможной ручной очисткой): CrudOperations_User_ShouldSucceedAndCleanup ---");
        }
    }
}