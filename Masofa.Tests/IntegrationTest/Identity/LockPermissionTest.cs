using System.Text.Json;
using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.Models.Identity;
using Masofa.Common.ViewModels.Account;

namespace Masofa.Tests.IntegrationTest.Identity
{
    public class LockPermissionTest
    {
        [Fact]
        public async Task CRUD_LockPermission()
        {
            // Arrange
            var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
            await uow.LoginAsync(new LoginAndPasswordViewModel
            {
                UserName = TestConstants.TEST_USERNAME,
                Password = TestConstants.TEST_PASSWORD
            }, CancellationToken.None);

            var permission = new LockPermission
            {
                UserId = TestConstants.EXISTING_WORKER_ID,
                EntityTypeName = "DEMO TestEntity",
                EntityAction = "Read",
                LockPermissionType = LockPermissionType.Action // 1
            };
            var json = JsonSerializer.Serialize(permission, BaseCrudRepository<LockPermission>.DefaultJsonOptions);
            Console.WriteLine("Отправляемый JSON:");
            Console.WriteLine(json);
            // create
            var id = await uow.LockPermissionRepository.CreateAsync(permission, CancellationToken.None);
            id.Should().NotBeEmpty();
            Console.WriteLine($"Создана новая запись LockPermission с ID: {id}");

            // getById
            var result = await uow.LockPermissionRepository.GetByIdAsync(id, CancellationToken.None);
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.UserId.Should().Be(permission.UserId);
            result.EntityTypeName.Should().Be(permission.EntityTypeName);
            result.LockPermissionType.Should().Be(LockPermissionType.Action);
            Console.WriteLine("Получение по ID прошло успешно. Данные совпадают.");

            //update
            var fetched = await uow.LockPermissionRepository.GetByIdAsync(id, CancellationToken.None);
            fetched.EntityAction = "Update";
            fetched.LockPermissionType = LockPermissionType.Entity;
            var updated = await uow.LockPermissionRepository.UpdateAsync(fetched, CancellationToken.None);
            updated.EntityAction.Should().Be("Update");
            updated.LockPermissionType.Should().Be(LockPermissionType.Entity);
            Console.WriteLine("Обновление сущности выполнено успешно.");

            // getByQuery
            var query = new BaseGetQuery<LockPermission>
            {
                Take = 10,
                Offset = 0,
                Filters = new List<FieldFilter>
                {
                    new FieldFilter
                    {
                        FilterField = "UserId",
                        FilterValue = TestConstants.EXISTING_WORKER_ID,
                        FilterOperator = FilterOperator.Equals
                    }
                }
            };

            var results = await uow.LockPermissionRepository.GetByQueryAsync(query, CancellationToken.None);
            results.Should().NotBeEmpty();
            results.Should().Contain(p => p.Id == id);
            Console.WriteLine("Запрос по фильтру вернул ожидаемую запись.");


            // delete
            var deleteResult = await uow.LockPermissionRepository.DeleteAsync(id, CancellationToken.None);
            deleteResult.Should().BeTrue("Delete должен возвращать true при успешном удалении");
            Console.WriteLine("Выполнено удаление.");

            // Проверяем, что сущность помечена как удалённая, а не физически удалена
            var bidAfterDelete = await uow.LockPermissionRepository.GetByIdAsync(id, CancellationToken.None);
            bidAfterDelete.Should().NotBeNull("GetById должен возвращать сущность даже после удаления.");
            bidAfterDelete.Status.Should().Be(StatusType.Deleted, "Статус должен быть 'Deleted' после операции удаления.");
            bidAfterDelete.Id.Should().Be(id);
            Console.WriteLine("Подтверждено: сущность не удалена физически, а помечена как 'Deleted'.");
        }


        //[Fact]
        //public async Task GetTotalCount_ShouldReturnCorrectCount()
        //{
        //    // Arrange
        //    var uow = new UnitOfWork(new HttpClient(), TestConstants.BASE_URL);
        //    await uow.LoginAsync(new LoginAndPasswordViewModel
        //    {
        //        UserName = TestConstants.TEST_USERNAME,
        //        Password = TestConstants.TEST_PASSWORD
        //    }, CancellationToken.None);

        //    var permission = new LockPermission
        //    {
        //        UserId = Guid.Parse(EXISTING_USER_ID),
        //        EntityTypeName = "CountTest",
        //        LockPermissionType = LockPermissionType.Entity
        //    };
        //    var id = await uow.LockPermissionRepository.CreateAsync(permission, CancellationToken.None);

        //    var query = new BaseGetQuery<LockPermission>
        //    {
        //        Filters = new List<FieldFilter>
        //        {
        //            new FieldFilter
        //            {
        //                FilterField = "UserId",
        //                FilterValue = EXISTING_USER_ID,
        //                FilterOperator = FilterOperator.Equals
        //            }
        //        }
        //    };

        // Act
        //var countBefore = await uow.LockPermissionRepository.GetTotalCountAsync(query, CancellationToken.None);
        //var countAfterDelete = 0;

        //try
        //{
        //    await uow.LockPermissionRepository.DeleteAsync(id, CancellationToken.None);
        //    countAfterDelete = await uow.LockPermissionRepository.GetTotalCountAsync(query, CancellationToken.None);
        //}
        //catch
        //{
        //    // Игнорируем, если удаление не удалось — главное, чтобы countBefore был >= 1
        //}

        //// Assert
        //countBefore.Should().BeGreaterOrEqualTo(1);
        //if (countAfterDelete != 0) // если удаление прошло
        //    countAfterDelete.Should().Be(countBefore - 1);
        //}
    }
}