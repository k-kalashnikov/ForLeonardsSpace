using System.Net;
using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.ViewModels.Account;
using Masofa.Web.Monolith.ViewModels.User;

namespace Masofa.Tests.IntegrationTest.Identity
{
    public class AccountTest
    {
        #region LoginByLoginPassword Tests

        [Fact]
        public async Task LoginByLoginPassword_WithValidCredentials_ShouldAuthenticateSuccessfully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            };

            // Act
            var act = async () => await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
            unitOfWork.IsAuth.Should().BeTrue();
        }

        [Fact]
        public async Task LoginByLoginPassword_WithInvalidCredentials_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.WRONG_PASSWORD
            };

            // Act
            var act = async () => await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.Unauthorized);
            unitOfWork.IsAuth.Should().BeFalse();
        }

        [Fact]
        public async Task LoginByLoginPassword_WithNonExistentUser_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.WRONG_USERNAME,
                Password = TestConstants.WRONG_PASSWORD
            };

            // Act
            var act = async () => await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.Unauthorized);
            unitOfWork.IsAuth.Should().BeFalse();
        }

        [Fact]
        public async Task LoginByLoginPassword_WithEmptyCredentials_ShouldThrowBadRequestException()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = "",
                Password = ""
            };

            // Act
            var act = async () => await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.StatusCode == HttpStatusCode.BadRequest);
            unitOfWork.IsAuth.Should().BeFalse();
        }

        #endregion

        #region ChangePassword Tests

        [Fact]
        public async Task ChangePassword_WithValidData_ShouldChangePasswordSuccessfully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            };

            // Сначала логинимся, чтобы получить токен
            await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Проверяем, что мы успешно залогинились
            unitOfWork.IsAuth.Should().BeTrue();

            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = TestConstants.TEST_PASSWORD,
                NewPassword = TestConstants.NEW_TEST_PASSWORD,
                ConfirmNewPassword = TestConstants.NEW_TEST_PASSWORD
            };

            // Act
            var result = await unitOfWork.AccountRepository.ChangePasswordAsync(changePasswordViewModel, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            // Проверяем, что можем залогиниться с новым паролем
            var newUnitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var newLoginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.NEW_TEST_PASSWORD
            };

            await newUnitOfWork.LoginAsync(newLoginViewModel, CancellationToken.None);
            newUnitOfWork.IsAuth.Should().BeTrue();

            // Восстанавливаем исходный пароль
            var restorePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = TestConstants.NEW_TEST_PASSWORD,
                NewPassword = TestConstants.TEST_PASSWORD,
                ConfirmNewPassword = TestConstants.TEST_PASSWORD
            };

            var restoreResult = await newUnitOfWork.AccountRepository.ChangePasswordAsync(restorePasswordViewModel, CancellationToken.None);
            restoreResult.Should().BeTrue();
        }

        #endregion

        #region GetProfileInfo Tests

        [Fact]
        public async Task GetProfileInfo_WhenAuthenticated_ShouldReturnProfileInfo()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            };

            // Сначала логинимся
            await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            // Act
            var profileInfo = await unitOfWork.AccountRepository.GetProfileInfoAsync(CancellationToken.None);

            // Assert
            profileInfo.Should().NotBeNull();
            profileInfo.UserName.Should().Be(TestConstants.TEST_USERNAME);
        }

        [Fact]
        public async Task GetProfileInfo_WhenNotAuthenticated_ShouldFail()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.GetProfileInfoAsync(CancellationToken.None);
            await act.Should().ThrowAsync<Exception>(); // Ожидаем ошибку от API
        }

        #endregion

        #region ForgotPassword Tests

        [Fact]
        public async Task ForgotPassword_WithValidEmail_ShouldSendResetLink()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var forgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Email = "test@example.com" // Используйте реальный email тестового пользователя
            };

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.ForgotPasswordAsync(forgotPasswordViewModel, CancellationToken.None);
            await act.Should().NotThrowAsync(); // Должно выполниться без ошибок
        }

        [Fact]
        public async Task ForgotPassword_WithInvalidEmail_ShouldHandleGracefully()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var forgotPasswordViewModel = new ForgotPasswordViewModel
            {
                Email = "nonexistent@example.com"
            };

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.ForgotPasswordAsync(forgotPasswordViewModel, CancellationToken.None);
            await act.Should().NotThrowAsync(); // API обычно не раскрывает существование email
        }

        #endregion

        #region ResetPassword Tests

        [Fact]
        public async Task ResetPassword_WithValidToken_ShouldResetPassword()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                UserId = Guid.NewGuid().ToString(), // Используйте реальный userId
                Token = "valid-token", // Используйте реальный токен из ForgotPassword
                NewPassword = "NewPass123!"
            };

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.ResetPasswordAsync(resetPasswordViewModel, CancellationToken.None);
            await act.Should().NotThrowAsync(); // Должно выполниться без ошибок при валидных данных
        }

        [Fact]
        public async Task ResetPassword_WithInvalidToken_ShouldFail()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                UserId = Guid.NewGuid().ToString(),
                Token = "invalid-token",
                NewPassword = "NewPass123!"
            };

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.ResetPasswordAsync(resetPasswordViewModel, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>(); // Ожидаем ошибку от API
        }

        #endregion

        #region Update Tests

        /* [Fact]
         public async Task Update_WhenAuthenticated_ShouldUpdateUser()
         {
             // Arrange
             var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
             var loginViewModel = new LoginAndPasswordViewModel
             {
                 UserName = TestConstants.TEST_USERNAME,
                 Password = TestConstants.TEST_PASSWORD
             };

             // Сначала логинимся
             await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

             var updateViewModel = new Masofa.Common.Models.Identity.User
             {
                 UserName = TestConstants.TEST_USERNAME,
                 Email = "updated@example.com",
                 FirstName = "UpdatedFirstName",
                 LastName = "UpdatedLastName"
             };

             // Act
             var updatedUser = await unitOfWork.AccountRepository.UpdateAsync(updateViewModel, CancellationToken.None);

             // Assert
             updatedUser.Should().NotBeNull();
             // Добавьте дополнительные проверки в зависимости от ожидаемого поведения API
         }*/

        [Fact]
        public async Task Update_WithInvalidData_ShouldFail()
        {
            // Arrange
            var unitOfWork = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            var loginViewModel = new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            };

            // Сначала логинимся
            await unitOfWork.LoginAsync(loginViewModel, CancellationToken.None);

            var updateViewModel = new UpdateViewModel
            {
                Email = "" // Пустое имя пользователя - невалидные данные
            };

            // Act & Assert
            var act = async () => await unitOfWork.AccountRepository.UpdateAsync(updateViewModel, CancellationToken.None);
            await act.Should().ThrowAsync<Exception>(); // Ожидаем ошибку от API
        }

        #endregion
    }
}
