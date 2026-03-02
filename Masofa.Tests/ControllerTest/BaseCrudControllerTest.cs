using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Services.FileStorage;

namespace Masofa.Tests.ControllerTest;

public abstract class BaseCrudControllerTest<TModel, TDbContext> : BaseControllerTest // Наследуемся от BaseControllerTest
    where TModel : BaseEntity, new()
    where TDbContext : DbContext
{
    protected Mock<TDbContext> DbContextMock { get; set; } = null!;
    protected TDbContext DbContextInstance { get; set; } = null!;
    protected Mock<IFileStorageProvider> FileStorageMock { get; set; } = null!;
    protected Mock<DbSet<TModel>> DbSetMock { get; set; } = null!;
    protected List<TModel> TestData { get; set; } = null!;

    // Добавляем инициализацию специфичных для CRUD зависимостей
    protected override void InitializeBaseMocks() // Переопределяем метод инициализации
    {
        base.InitializeBaseMocks(); // Вызываем инициализацию базовых моков из родителя

        // Создаём InMemoryDbContextOptions
        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Создаём **реальный экземпляр** DbContext
        DbContextInstance = (TDbContext)Activator.CreateInstance(typeof(TDbContext), options);

        FileStorageMock = new Mock<IFileStorageProvider>();
        TestData = new List<TModel>();

        // Мокируем DbSet
        var queryableData = TestData.AsQueryable();

        DbSetMock = new Mock<DbSet<TModel>>();
        DbSetMock.As<IQueryable<TModel>>().Setup(m => m.Provider).Returns(queryableData.Provider);
        DbSetMock.As<IQueryable<TModel>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        DbSetMock.As<IQueryable<TModel>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        DbSetMock.As<IQueryable<TModel>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

        // Подменяем DbSet<TModel> в **реальном** DbContext (или моке, если нужно)
        DbContextMock = new Mock<TDbContext>(options);
        DbContextMock.Setup(x => x.Set<TModel>()).Returns(DbSetMock.Object);
    }

    // Конструктор вызывает base() -> InitializeBaseMocks()
    protected BaseCrudControllerTest() : base()
    {
        // Дополнительная инициализация, если нужна
    }

    /// <summary>
    /// Создаёт экземпляр контроллера, наследника BaseCrudController, с подменёнными зависимостями
    /// </summary>
    protected TController CreateCrudController<TController>()
        where TController : BaseCrudController<TModel, TDbContext>
    {
        // Явно передаём все зависимости, требуемые BaseCrudController:
        // 1. fileStorageProvider (FileStorageMock.Object)
        // 2. dbContext (DbContextInstance)
        // 3. logger (LoggerMock.Object) - из базового класса
        // 4. configuration (ConfigurationMock.Object) - из базового класса
        // 5. mediator (MediatorMock.Object) - из базового класса
        // 6. businessLogicLogger (BusinessLogicLoggerMock.Object) - из базового класса
        // 7. httpContextAccessor (HttpContextAccessorMock.Object) - из базового класса
        var controller = (TController)Activator.CreateInstance(
            typeof(TController),
            FileStorageMock.Object,
            DbContextInstance, // или DbContextMock.Object, в зависимости от подхода
            LoggerMock.Object,
            ConfigurationMock.Object,
            MediatorMock.Object,
            BusinessLogicLoggerMock.Object,
            HttpContextAccessorMock.Object
        );

        // Устанавливаем HttpContext вручную, чтобы User.Identity.Name был доступен
        var httpContext = new DefaultHttpContext();
        httpContext.User = CreateTestUser();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        return controller;
    }

    /// <summary>
    /// Создаёт экземпляр TModel с обязательными полями, чтобы избежать DbUpdateException
    /// По умолчанию просто new TModel(), но можно переопределить, если есть обязательные поля
    /// </summary>
    protected virtual TModel CreateValidModel()
    {
        return new TModel
        {
            Id = Guid.NewGuid(),
            CreateUser = Guid.NewGuid(),
            LastUpdateUser = Guid.NewGuid()
        };
    }

    /// <summary>
    /// Создаёт несколько экземпляров TModel для тестов
    /// </summary>
    protected virtual List<TModel> CreateValidModels(int count = 5)
    {
        var models = new List<TModel>();
        for (int i = 0; i < count; i++)
        {
            models.Add(CreateValidModel());
        }
        return models;
    }

    // ————————————————————————————————————————————————————————————————————————————————————————
    // CRUD Тесты
    // ————————————————————————————————————————————————————————————————————————————————————————

    #region GetByQuery

    [Fact]
    public async Task GetByQuery_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();

        var expected = new List<TModel> { new TModel(), new TModel() };
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default))
                    .ReturnsAsync(expected);

        // Act
        var result = await controller.GetByQuery(query);

        // Assert
        var okResult = Assert.IsType<ActionResult<List<TModel>>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(expected.Count, okResult.Value.Count);
    }

    [Fact]
    public async Task GetByQuery_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();
        // Добавляем ошибку в ModelState, чтобы сделать его невалидным
        controller.ModelState.AddModelError("SomeProperty", "Some error message");

        // Не настраиваем Mediator, так как он не должен вызываться при невалидной модели

        // Act
        var result = await controller.GetByQuery(query);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // .Result, потому что ActionResult<T>
                                                                                     // Проверяем, что возвращаемый объект - это сам ModelState (или его сериализованное представление)
        Assert.IsType<SerializableError>(badRequestResult.Value); // SerializableError - это тип, в который преобразуется ModelState
                                                                  // Можно дополнительно проверить наличие конкретной ошибки:
        Assert.True(controller.ModelState.ContainsKey("SomeProperty"));
        Assert.Contains("Some error message", controller.ModelState["SomeProperty"].Errors.Select(e => e.ErrorMessage));

        // Проверяем, что Mediator.Send НЕ был вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default), Times.Never);
    }

    [Fact]
    public async Task GetByQuery_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();

        var exceptionMessage = "Test exception from mediator";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.GetByQuery(query);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region GetById

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        var expected = new TModel { Id = id };
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetByIdRequest<TModel, TDbContext>>(), default))
                    .ReturnsAsync(expected);

        // Act
        var result = await controller.GetById(id);

        // Assert
        var okResult = Assert.IsType<ActionResult<TModel>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(id, okResult.Value.Id);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetByIdRequest<TModel, TDbContext>>(), default))
                    .ReturnsAsync((TModel)null);

        // Act
        var result = await controller.GetById(id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        var exceptionMessage = "Test exception from mediator in GetById";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetByIdRequest<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.GetById(id);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseGetByIdRequest<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region Create

    /* [Fact]
     public async Task Create_ValidModel_ReturnsCreatedId()
     {
         // Arrange
         var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();

         var model = new TModel();
         controller.ModelState.Clear();

         var expectedId = Guid.NewGuid();
         bool mediatorCalled = false;

         MediatorMock.Setup(m => m.Send(It.IsAny<BaseCreateCommand<TModel, TDbContext>>(), default))
                     .Callback(() => mediatorCalled = true)
                     .ReturnsAsync(expectedId);

         // Act
         var result = await controller.Create(model);

         // Assert
         Assert.True(mediatorCalled, "Mediator.Send was not called");
         var okResult = Assert.IsType<ActionResult<Guid>>(result);
         Assert.Equal(expectedId, okResult.Value);
     }*/

    [Fact]
    public async Task Create_ValidModel_ReturnsCreatedId()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();

        var model = new TModel { Id = Guid.NewGuid() }; // Убедимся, что модель инициализирована
        controller.ModelState.Clear(); // Убедимся, что ModelState валиден

        var expectedId = Guid.NewGuid();
        bool mediatorCalled = false;

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseCreateCommand<TModel, TDbContext>>(), default))
                    .Callback(() => mediatorCalled = true)
                    .ReturnsAsync(expectedId);

        // Act
        var result = await controller.Create(model);

        // Assert
        Assert.True(mediatorCalled, "Mediator.Send was not called for Create command");
        var okResult = Assert.IsType<ActionResult<Guid>>(result);
        Assert.Equal(expectedId, okResult.Value);

        // Проверяем, что был вызван Mediator с правильной командой и данными
        MediatorMock.Verify(m => m.Send(It.Is<BaseCreateCommand<TModel, TDbContext>>(
            cmd => cmd.Model.Id == model.Id && cmd.Author == controller.User.Identity.Name), default), Times.Once);
    }

    [Fact]
    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel();
        // Добавляем ошибку в ModelState
        controller.ModelState.AddModelError("SomeProperty", "Some error message");

        // Не настраиваем Mediator

        // Act
        var result = await controller.Create(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.IsType<SerializableError>(badRequestResult.Value);

        // Проверяем, что Mediator.Send НЕ был вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseCreateCommand<TModel, TDbContext>>(), default), Times.Never);
    }

    [Fact]
    public async Task Create_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel { Id = Guid.NewGuid() };
        controller.ModelState.Clear();

        var exceptionMessage = "Test exception from mediator in Create";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseCreateCommand<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.Create(model);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseCreateCommand<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region Update

    /*[Fact]
    public async Task Update_ValidModel_ReturnsUpdatedModel()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel { Id = Guid.NewGuid() };

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseUpdateCommand<TModel, TDbContext>>(), default))
                    .ReturnsAsync(model);

        // Act
        var result = await controller.Update(model);

        // Assert
        var okResult = Assert.IsType<ActionResult<TModel>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(model.Id, okResult.Value.Id);
    }*/

    [Fact]
    public async Task Update_ValidModel_ReturnsUpdatedModel()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel { Id = Guid.NewGuid() };
        controller.ModelState.Clear(); // Убедимся, что ModelState валиден

        var updatedModel = new TModel { Id = model.Id, Status = 0 }; // Предполагаем, что у TModel есть Name
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseUpdateCommand<TModel, TDbContext>>(), default))
                    .ReturnsAsync(updatedModel);

        // Act
        var result = await controller.Update(model);

        // Assert
        var okResult = Assert.IsType<ActionResult<TModel>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(updatedModel.Id, okResult.Value.Id);
        Assert.Equal(updatedModel.Status, okResult.Value.Status);

        // Проверяем, что был вызван Mediator с правильной командой и данными
        MediatorMock.Verify(m => m.Send(It.Is<BaseUpdateCommand<TModel, TDbContext>>(
            cmd => cmd.Model.Id == model.Id && cmd.Author == controller.User.Identity.Name), default), Times.Once);
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel { Id = Guid.NewGuid() };
        // Добавляем ошибку в ModelState
        controller.ModelState.AddModelError("SomeProperty", "Some error message");

        // Не настраиваем Mediator

        // Act
        var result = await controller.Update(model);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.IsType<SerializableError>(badRequestResult.Value);

        // Проверяем, что Mediator.Send НЕ был вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseUpdateCommand<TModel, TDbContext>>(), default), Times.Never);
    }

    [Fact]
    public async Task Update_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var model = new TModel { Id = Guid.NewGuid() };
        controller.ModelState.Clear();

        var exceptionMessage = "Test exception from mediator in Update";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseUpdateCommand<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.Update(model);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseUpdateCommand<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region Delete

    /*[Fact]
    public async Task Delete_ExistingId_ReturnsTrue()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseDeleteCommand<TModel, TDbContext>>(), default))
                    .ReturnsAsync(true);

        // Act
        var result = await controller.Delete(id);

        // Assert
        var okResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(okResult.Value);
    }*/

    [Fact]
    public async Task Delete_ExistingId_ReturnsTrue()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseDeleteCommand<TModel, TDbContext>>(), default))
                    .ReturnsAsync(true);

        // Act
        var result = await controller.Delete(id);

        // Assert
        var okResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(okResult.Value);

        // Проверяем, что был вызван Mediator с правильной командой и данными
        MediatorMock.Verify(m => m.Send(It.Is<BaseDeleteCommand<TModel, TDbContext>>(
            cmd => cmd.Id == id && cmd.Author == controller.User.Identity.Name), default), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsFalse() // Или может быть NotFound, в зависимости от реализации BaseDeleteCommand
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        // Возвращаем false из Mediator, что может означать, что сущность не была найдена или не удалена
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseDeleteCommand<TModel, TDbContext>>(), default))
                    .ReturnsAsync(false);

        // Act
        var result = await controller.Delete(id);

        // Assert
        var okResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.False(okResult.Value);

        // Проверяем, что был вызван Mediator с правильной командой и данными
        MediatorMock.Verify(m => m.Send(It.Is<BaseDeleteCommand<TModel, TDbContext>>(
            cmd => cmd.Id == id && cmd.Author == controller.User.Identity.Name), default), Times.Once);
    }

    [Fact]
    public async Task Delete_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var id = Guid.NewGuid();

        var exceptionMessage = "Test exception from mediator in Delete";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseDeleteCommand<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.Delete(id);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseDeleteCommand<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region GetTotalCount

    /*
        [Fact]
        public virtual async Task GetTotalCount_ReturnsCount()
        {
            // Arrange
            var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();

            var testData = CreateValidModels(5);
            foreach (var item in testData)
            {
                DbContextInstance.Add(item);
            }
            await DbContextInstance.SaveChangesAsync();

            // Act
            var result = await controller.GetTotalCount();

            // Assert
            var okResult = Assert.IsType<ActionResult<int>>(result);
            Assert.Equal(testData.Count, okResult.Value);
        }*/

    [Fact]
    public async Task GetTotalCount_ValidRequest_ReturnsCount()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();

        var testData = new List<TModel> { new TModel(), new TModel(), new TModel() };
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default))
                    .ReturnsAsync(testData);

        // Act
        var result = await controller.GetTotalCount(query);

        // Assert
        var okResult = Assert.IsType<ActionResult<int>>(result);
        Assert.Equal(testData.Count, okResult.Value);
    }

    [Fact]
    public async Task GetTotalCount_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();
        // Добавляем ошибку в ModelState
        controller.ModelState.AddModelError("SomeProperty", "Some error message");

        // Не настраиваем Mediator

        // Act
        var result = await controller.GetTotalCount(query);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.IsType<SerializableError>(badRequestResult.Value);

        // Проверяем, что Mediator.Send НЕ был вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default), Times.Never);
    }

    [Fact]
    public async Task GetTotalCount_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var query = new BaseGetQuery<TModel>();

        var exceptionMessage = "Test exception from mediator in GetTotalCount";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.GetTotalCount(query);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseGetRequest<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion

    #region ImportFromCSV

    /*[Fact]
    public async Task ImportFromCSV_ValidFile_ReturnsTrue()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var file = new Mock<IFormFile>().Object;

        MediatorMock.Setup(m => m.Send(It.IsAny<BaseImportFromCSVCommand<TModel, TDbContext>>(), default))
                    .Returns(Task.CompletedTask);

        // Act
        var result = await controller.ImportFromCSV(file);

        // Assert
        var okResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(okResult.Value);
    }*/

    [Fact]
    public async Task ImportFromCSV_ValidFile_ReturnsTrue()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var file = new Mock<IFormFile>().Object;

        bool mediatorCalled = false;
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseImportFromCSVCommand<TModel, TDbContext>>(), default))
                    .Callback(() => mediatorCalled = true)
                    .Returns(Task.CompletedTask);

        // Act
        var result = await controller.ImportFromCSV(file);

        // Assert
        Assert.True(mediatorCalled, "Mediator.Send was not called for ImportFromCSV command");
        var okResult = Assert.IsType<ActionResult<bool>>(result);
        Assert.True(okResult.Value);

        // Проверяем, что был вызван Mediator с правильной командой и данными
        MediatorMock.Verify(m => m.Send(It.Is<BaseImportFromCSVCommand<TModel, TDbContext>>(
            cmd => cmd.FormFile == file && cmd.Author == controller.User.Identity.Name), default), Times.Once);
    }

    // В контроллере нет явной проверки ModelState для IFormFile, поэтому нет теста на невалидную модель
    // Но можно протестировать, если в BaseImportFromCSVCommand будет добавлена валидация

    [Fact]
    public async Task ImportFromCSV_MediatorThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var controller = CreateCrudController<TestableCrudController<TModel, TDbContext>>();
        var file = new Mock<IFormFile>().Object;

        var exceptionMessage = "Test exception from mediator in ImportFromCSV";
        MediatorMock.Setup(m => m.Send(It.IsAny<BaseImportFromCSVCommand<TModel, TDbContext>>(), default))
                    .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await controller.ImportFromCSV(file);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result); // .Result, потому что ActionResult<T>
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal(exceptionMessage, statusCodeResult.Value);

        // Проверяем, что Mediator.Send БЫЛ вызван
        MediatorMock.Verify(m => m.Send(It.IsAny<BaseImportFromCSVCommand<TModel, TDbContext>>(), default), Times.Once);
    }

    #endregion
}

// ————————————————————————————————————————————————————————————————————————————————————————
// Вспомогательный обобщённый класс для тестирования
// ————————————————————————————————————————————————————————————————————————————————————————

public class TestableCrudController<TModel, TDbContext> : BaseCrudController<TModel, TDbContext>
    where TModel : BaseEntity, new()
    where TDbContext : DbContext
{
    public TestableCrudController(
        IFileStorageProvider fileStorageProvider,
        TDbContext dbContext,
        ILogger logger,
        IConfiguration configuration,
        IMediator mediator,
        IBusinessLogicLogger businessLogicLogger,
        IHttpContextAccessor httpContextAccessor) : base(
            fileStorageProvider,
            dbContext,
            logger,
            configuration,
            mediator,
            businessLogicLogger,
            httpContextAccessor)
    {
    }
}