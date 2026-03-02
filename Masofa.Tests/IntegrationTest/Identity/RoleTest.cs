using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.ViewModels.Account;

namespace Masofa.Tests.IntegrationTest.Identity
{
    public class RoleTest
    {
        [Fact]
        public async Task GetRoles_ShouldReturnNonEmptyList()
        {
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);

            var roles = await uow.RoleRepository.GetRolesAsync(CancellationToken.None);
            roles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetUsersInRole_ValidRoleName_ShouldReturnUsers()
        {
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);

            var users = await uow.RoleRepository.GetUsersInRoleAsync("Admin", CancellationToken.None);
            users.Should().NotBeNull();
        }
    }
}