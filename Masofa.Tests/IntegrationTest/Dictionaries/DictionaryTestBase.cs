using System.Net;
using System.Reflection;
using FluentAssertions;
using Masofa.Client.ApiClient;
using Masofa.Common.ViewModels.Account;
using Masofa.Tests.IntegrationTest.Dictionaries.Attributes;
using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.Dictionaries;

[Collection("Sequential")] // Если тесты могут конфликтовать
public abstract class DictionaryTestBase<TModel> where TModel : BaseEntity, new()
{
    protected readonly ITestOutputHelper _output;
    protected readonly string _baseUrl;
    protected string? _authToken;
    protected HttpClient? _sharedHttpClient;
    protected UnitOfWork? _unitOfWork;

    // Репозиторий конкретного типа, созданный в Setup
    protected BaseCrudRepository<TModel>? _repository;

    public DictionaryTestBase(ITestOutputHelper output)
    {
        _output = output;
        _baseUrl = TestConstants.BASE_URL;
        _sharedHttpClient = new HttpClient();
    }

    /// <summary>
    /// Подготавливает UnitOfWork и аутентифицирует пользователя.
    /// Должен быть вызван в начале каждого теста.
    /// </summary>
    protected virtual async Task SetupAsync(string repositoryBaseUrl)
    {
        _unitOfWork = new UnitOfWork(_sharedHttpClient, _baseUrl);
        await _unitOfWork.LoginAsync(new LoginAndPasswordViewModel
        {
            UserName = TestConstants.TEST_USERNAME,
            Password = TestConstants.TEST_PASSWORD
        }, CancellationToken.None);

        // Создаём репозиторий для конкретного типа TModel
        _repository = new BaseCrudRepository<TModel>(_sharedHttpClient, repositoryBaseUrl);
        // Устанавливаем токен, как это делает UnitOfWork.SetToken
        _repository.Token = _unitOfWork.AccountRepository.Token;
    }

    protected virtual TModel PopulateBaseEntityForCreate(TModel model)
    {
        model.Status = Masofa.Common.Models.StatusType.Active;
        model.CreateUser = TestConstants.TEST_USER_GUID;
        return model;
    }
    protected virtual TModel PrepareModelForUpdate(TModel existingModel, TModel updatedModel)
    {
        // Копируем неизменяемые поля
        updatedModel.Id = existingModel.Id; // Обязательно
        updatedModel.LastUpdateUser = TestConstants.TEST_USER_GUID;

        return updatedModel;
    }

    /// <summary>
    /// Освобождает ресурсы.
    /// </summary>
    public void Dispose()
    {
        _sharedHttpClient?.Dispose();
        _unitOfWork = null;
        _repository = null;
    }

    // --- GET Тесты ---

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnCorrectEntity()
    {

        // Arrange
        var baseUrl = GetRepositoryBaseUrl();
        await SetupAsync(baseUrl);

        // Получим *все* (или много) сущностей, чтобы сравнить с результатом GetById
        var fullQuery = new BaseGetQuery<TModel> { Take = 1000, Offset = 0, SortBy = "Id", Sort = SortType.ASC };
        _output.WriteLine($"[GetById] Retrieving up to 1000 {typeof(TModel).Name} entities for comparison...");
        var fullList = await _repository!.GetByQueryAsync(fullQuery, CancellationToken.None);
        _output.WriteLine($"[GetById] Retrieved {fullList.Count} entities from full list query.");

        if (fullList.Count == 0)
        {
            _output.WriteLine($"[GetById] No existing {typeof(TModel).Name} found. Skipping GetById test for this run.");
            return; // Пропускаем тест, если нет сущностей
        }

        // Выберем первый элемент из полного списка как существующий ID для теста
        var existingId = fullList[0].Id;
        var expectedEntity = fullList[0];
        _output.WriteLine($"[GetById] Selected entity with ID: {existingId} and Name: {GetEntityName(expectedEntity)} for GetById test.");

        // Act
        _output.WriteLine($"[GetById] Attempting to retrieve {typeof(TModel).Name} with ID: {existingId} using GetById...");
        var result = await _repository.GetByIdAsync(existingId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull($"Entity with ID {existingId} should exist when retrieved by ID.");
        result.Id.Should().Be(existingId, "GetById should return entity with the requested ID.");

        // Проверим, что GetById вернул правильную сущность (сравнив ID и Name)
        result.Id.Should().Be(expectedEntity.Id, "GetById result ID should match the ID from the full list query.");
        GetEntityName(result).Should().Be(GetEntityName(expectedEntity), "GetById result Name should match the Name from the full list query.");
        _output.WriteLine($"[GetById] GetById returned entity with ID: {result.Id} and Name: {GetEntityName(result)}. This matches the entity from the full list query.");

        // Проверим, что в полном списке нет дубликатов для этого конкретного ID
        var entitiesWithSameId = fullList.Where(e => e.Id == existingId).ToList();
        entitiesWithSameId.Count.Should().Be(1, $"Full list query should return only one entity with ID {existingId}, but found {entitiesWithSameId.Count}.");
        _output.WriteLine($"[GetById] Verified that full list query contains only one entity with ID {existingId}.");
    }

    [Fact]
    public async Task GetByQueryAsync_WithValidQuery_ShouldReturnListWithoutDuplicateNames()
    {
        // Проверяем наличие атрибута
        if (this.GetType().GetCustomAttribute(typeof(SkipNameDuplicateCheckAttribute)) != null)
        {
            _output.WriteLine($"[GetByQuery_Names] Skipping duplicate name check for {typeof(TModel).Name} as its test class {this.GetType().Name} is marked with SkipNameDuplicateCheckAttribute.");
            return; // Просто выходим, тест считается пройденным (зелёным)
        }

        // Подготовка
        var baseUrl = GetRepositoryBaseUrl();
        await SetupAsync(baseUrl);

        // Делаем запрос, чтобы получить *все* или большинство сущностей
        var query = new BaseGetQuery<TModel> { Take = 1000, Offset = 0, SortBy = "Id", Sort = SortType.ASC }; // ASC для предсказуемого порядка

        _output.WriteLine($"[GetByQuery_Names] Выполнение запроса для {typeof(TModel).Name} с Take=1000 для проверки дубликатов имён.");

        // Выполнение
        var result = await _repository!.GetByQueryAsync(query, CancellationToken.None);

        // Проверка
        result.Should().NotBeNull("GetByQueryAsync должен вернуть список.");
        result.Should().BeAssignableTo<List<TModel>>($"Результат должен быть списком {typeof(TModel).Name}.");

        // Извлекаем имена и ID из всех сущностей
        var nameIdPairs = result.Select(item => new { Name = GetEntityName(item), Id = item.Id }).ToList();

        // Выведем все имена и ID в output
        _output.WriteLine($"[GetByQuery_Names] Получено {result.Count} элементов {typeof(TModel).Name}. Извлеченные имена (или локализованные имена) и их ID:");
        foreach (var pair in nameIdPairs)
        {
            _output.WriteLine($"  - ID: {pair.Id}, Имя: {pair.Name}");
        }

        // Проверим, что имена уникальны
        var namesList = nameIdPairs.Select(p => p.Name).ToList();
        var distinctNames = namesList.Distinct().ToList();

        // Если есть дубликаты, найдём их и выведем ID
        if (distinctNames.Count != namesList.Count)
        {
            // Группируем пары по имени и находим те группы, где Count > 1
            var duplicateGroups = nameIdPairs
                .GroupBy(p => p.Name)
                .Where(g => g.Count() > 1)
                .ToList();

            _output.WriteLine($"[GetByQuery_Names] *** ОБНАРУЖЕНЫ ДУБЛИКАТЫ ИМЁН ***");
            foreach (var group in duplicateGroups)
            {
                _output.WriteLine($"[GetByQuery_Names] Имя '{group.Key}' встречается {group.Count()} раз(а). Затронутые ID:");
                foreach (var item in group)
                {
                    _output.WriteLine($"    - ID: {item.Id}");
                }
            }
        }

        distinctNames.Count.Should().Be(namesList.Count,
            $"Результат GetByQueryAsync не должен содержать сущности с дублирующимися именами (или локализованными именами). Обнаружено {namesList.Count - distinctNames.Count} дубликат(ов) имени(ён).");

        _output.WriteLine($"[GetByQuery_Names] Успешно проверено отсутствие дубликатов имён (или локализованных имён) в наборе результатов из {result.Count} элементов.");
    }

    [Fact]
    public async Task GetByQueryAsync_WithValidQuery_ShouldReturnListWithoutDuplicates()
    {
        // Проверяем наличие атрибута
        if (this.GetType().GetCustomAttribute(typeof(SkipNameDuplicateCheckAttribute)) != null)
        {
            _output.WriteLine($"[GetByQuery_Names] Skipping duplicate name check for {typeof(TModel).Name} as its test class {this.GetType().Name} is marked with SkipNameDuplicateCheckAttribute.");
            return; // Просто выходим, тест считается пройденным (зелёным)
        }
        // Arrange
        var baseUrl = GetRepositoryBaseUrl();
        await SetupAsync(baseUrl);

        var query = new BaseGetQuery<TModel> { Take = 1000, Offset = 0, SortBy = "Id", Sort = SortType.ASC }; // Увеличим Take, чтобы захватить больше данных

        _output.WriteLine($"[GetByQuery] Executing query for {typeof(TModel).Name} with Take=1000 to check for duplicates.");

        // Act
        var result = await _repository!.GetByQueryAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull("GetByQueryAsync should return a list.");
        result.Should().BeAssignableTo<List<TModel>>($"Result should be a list of {typeof(TModel).Name}.");

        // Проверим дубликаты по ID
        var distinctIds = result.Select(e => e.Id).Distinct().ToList();
        distinctIds.Count.Should().Be(result.Count, $"GetByQueryAsync result should not contain duplicate IDs. Found {result.Count - distinctIds.Count} duplicates.");

        // Выведем ID и Name (если есть) всех сущностей в результатах
        _output.WriteLine($"[GetByQuery] Retrieved {result.Count} items of {typeof(TModel).Name}.");
        foreach (var item in result)
        {
            _output.WriteLine($"  - ID: {item.Id}, Name: {GetEntityName(item)}");
        }

        _output.WriteLine($"[GetByQuery] Successfully verified no duplicate IDs in the result set of {result.Count} items.");
    }


    // --- Вспомогательный метод для получения Name или Names.ru-RU (если поле Name не существует) ---
    // Использует рефлексию и проверяет структуру LocalizationString, если поле Name нет.
    private string GetEntityName(TModel entity)
    {
        if (entity == null) return "NULL";

        // Сначала попробуем найти обычное поле Name
        var nameProperty = typeof(TModel).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
        if (nameProperty != null && nameProperty.CanRead)
        {
            var nameValue = nameProperty.GetValue(entity);
            return nameValue?.ToString() ?? "NULL_NAME_VALUE";
        }

        // Если Name нет, попробуем найти поле Names (LocalizationString)
        var namesProperty = typeof(TModel).GetProperty("Names", BindingFlags.Public | BindingFlags.Instance);
        if (namesProperty != null && namesProperty.CanRead)
        {
            var namesValue = namesProperty.GetValue(entity);
            if (namesValue != null)
            {
                var namesType = namesValue.GetType();

                // Проверим, является ли Names словарем
                if (namesType.IsGenericType && namesType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    // Убедимся, что ключ - string
                    var genericArgs = namesType.GetGenericArguments();
                    if (genericArgs[0] == typeof(string))
                    {
                        var dictionary = (IDictionary<string, object>)namesValue;
                        // Попробуем получить ru-RU, en-US или первый доступный ключ
                        if (dictionary.ContainsKey("ru-RU"))
                        {
                            return dictionary["ru-RU"]?.ToString() ?? "NULL_NAMES_RU_VALUE";
                        }
                        else if (dictionary.ContainsKey("en-US"))
                        {
                            return dictionary["en-US"]?.ToString() ?? "NULL_NAMES_EN_VALUE";
                        }
                        else
                        {
                            var firstKey = dictionary.Keys.FirstOrDefault();
                            return firstKey != null ? dictionary[firstKey]?.ToString() ?? "NULL_NAMES_FIRST_VALUE" : "NO_LOCALIZED_NAME_FOUND_IN_DICT";
                        }
                    }
                    else
                    {
                        return $"UNEXPECTED_DICTIONARY_KEY_TYPE_{genericArgs[0].Name}";
                    }
                }
                else
                {
                    // Если Names - это не Dictionary, пробуем как объект с публичными свойствами
                    // Используем GetProperties, чтобы НЕ включать индексаторы по умолчанию
                    var allNameProperties = namesType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                     .Where(p => p.GetIndexParameters().Length == 0); // Исключаем индексные свойства

                    // Попробуем найти конкретные языковые свойства
                    var ruProperty = allNameProperties.FirstOrDefault(p => p.Name == "ru-RU" || p.Name == "ru_RU" || p.Name == "ru");
                    if (ruProperty != null && ruProperty.CanRead)
                    {
                        var ruValue = ruProperty.GetValue(namesValue);
                        return ruValue?.ToString() ?? "NULL_NAMES_RU_VALUE";
                    }

                    var enProperty = allNameProperties.FirstOrDefault(p => p.Name == "en-US" || p.Name == "en_US" || p.Name == "en");
                    if (enProperty != null && enProperty.CanRead)
                    {
                        var enValue = enProperty.GetValue(namesValue);
                        return enValue?.ToString() ?? "NULL_NAMES_EN_VALUE";
                    }

                    // Попробуем первый доступный обычный (не индексный) язык
                    var firstLangProp = allNameProperties.FirstOrDefault(p => p.PropertyType == typeof(string));
                    if (firstLangProp != null)
                    {
                        var firstLangValue = firstLangProp.GetValue(namesValue);
                        return firstLangValue?.ToString() ?? "NULL_NAMES_FIRST_LANG_VALUE";
                    }

                    return "NO_LOCALIZED_NAME_FOUND_IN_OBJ_NO_INDEXER"; // Если не удалось извлечь из обычных свойств
                }
                return "NO_LOCALIZED_NAME_FOUND_IN_OBJ_GENERIC"; // Если все попытки не удались
            }
            return "NULL_NAMES_VALUE"; // Если объект Names null
        }

        return "NO_NAME_OR_NAMES_PROPERTY"; // Если ни Name, ни Names не найдены
    }
    [Fact]
    public async Task CrudLifecycle_WithValidModel_ShouldExecuteAllOperationsSuccessfully()
    {
        // --- ПОДГОТОВКА ---
        var baseUrl = GetRepositoryBaseUrl();
        await SetupAsync(baseUrl);

        var initialModel = CreateValidTestModel();
        TModel? createdModel = null;
        TModel? updatedModel = null;
        TModel? retrievedAfterCreate = null;
        TModel? retrievedAfterUpdate = null;
        bool deleteResult = false;

        _output.WriteLine($"[Жизненный цикл CRUD] Начало тестирования жизненного цикла для {typeof(TModel).Name}.");
        _output.WriteLine($"[Жизненный цикл CRUD] Исходная модель для создания: {System.Text.Json.JsonSerializer.Serialize(initialModel, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })}");

        try
        {
            // --- ВЫПОЛНЕНИЕ & ПРОВЕРКА: СОЗДАНИЕ ---
            _output.WriteLine("[Жизненный цикл CRUD] Шаг 1: Создание сущности...");
            Guid createdId = await _repository!.CreateAsync(initialModel, CancellationToken.None);
            createdId.Should().NotBeEmpty("CreateAsync должен возвращать корректный Guid.");
            _output.WriteLine($"[Жизненный цикл CRUD] Шаг 1 УСПЕШНО: Создана сущность {typeof(TModel).Name} с ID: {createdId}");

            // --- ВЫПОЛНЕНИЕ & ПРОВЕРКА: ЧТЕНИЕ (после создания) ---
            _output.WriteLine("[Жизненный цикл CRUD] Шаг 2: Получение созданной сущности...");
            retrievedAfterCreate = await _repository.GetByIdAsync(createdId, CancellationToken.None);
            retrievedAfterCreate.Should().NotBeNull($"Сущность с ID {createdId} должна существовать после создания.");
            retrievedAfterCreate.Id.Should().Be(createdId, "ID полученной сущности должен совпадать с ID созданной сущности.");
            _output.WriteLine($"[Жизненный цикл CRUD] Шаг 2 УСПЕШНО: Получена созданная сущность с ID: {retrievedAfterCreate.Id}");

            // Сохраняем ссылку на созданную модель для последующего обновления
            createdModel = retrievedAfterCreate;

            // --- ВЫПОЛНЕНИЕ & ПРОВЕРКА: ОБНОВЛЕНИЕ ---
            _output.WriteLine("[Жизненный цикл CRUD] Шаг 3: Обновление сущности...");
            var modelToUpdate = ModifyModelForUpdate(createdModel);
            updatedModel = await _repository.UpdateAsync(modelToUpdate, CancellationToken.None);
            updatedModel.Should().NotBeNull($"UpdateAsync должен возвращать обновлённую сущность {typeof(TModel).Name}.");
            updatedModel.Id.Should().Be(createdId, "ID обновлённой сущности должен совпадать с оригинальным ID.");
            _output.WriteLine($"[Жизненный цикл CRUD] Шаг 3 УСПЕШНО: Обновлена сущность с ID: {updatedModel.Id}");

            // --- ВЫПОЛНЕНИЕ & ПРОВЕРКА: ЧТЕНИЕ (после обновления) ---
            _output.WriteLine("[Жизненный цикл CRUD] Шаг 4: Получение обновлённой сущности...");
            retrievedAfterUpdate = await _repository.GetByIdAsync(createdId, CancellationToken.None);
            retrievedAfterUpdate.Should().NotBeNull($"Сущность с ID {createdId} должна существовать после обновления.");
            retrievedAfterUpdate.Id.Should().Be(createdId, "ID полученной обновлённой сущности должен совпадать с оригинальным ID.");
            // Можно добавить дополнительные проверки на изменённые поля, если необходимо
            _output.WriteLine($"[Жизненный цикл CRUD] Шаг 4 УСПЕШНО: Получена обновлённая сущность с ID: {retrievedAfterUpdate.Id}");

            // --- ВЫПОЛНЕНИЕ & ПРОВЕРКА: УДАЛЕНИЕ ---
            _output.WriteLine("[Жизненный цикл CRUD] Шаг 5: Удаление сущности...");
            deleteResult = await _repository.DeleteAsync(createdId, CancellationToken.None);
            deleteResult.Should().BeTrue("DeleteAsync должен возвращать true при успешном удалении.");
            _output.WriteLine($"[Жизненный цикл CRUD] Шаг 5 УСПЕШНО: Удалена сущность с ID: {createdId}");

            // --- ФИНАЛЬНАЯ ПРОВЕРКА: ПОДТВЕРЖДЕНИЕ УДАЛЕНИЯ ---
            _output.WriteLine("[Жизненный цикл CRUD] Финальная проверка: Проверка удаления сущности...");
            TModel? retrievedAfterDelete = null;
            Exception? deleteVerificationException = null;
            try
            {
                retrievedAfterDelete = await _repository.GetByIdAsync(createdId, CancellationToken.None);
            }
            catch (HttpRequestException httpEx)
            {
                deleteVerificationException = httpEx;
                // Если API возвращает 404, это тоже приемлемый способ показать удаление
                if (httpEx.StatusCode == HttpStatusCode.NotFound)
                {
                    _output.WriteLine($"[Жизненный цикл CRUD] Финальная проверка ИНФО: GetById вернул 404 NotFound для ID {createdId}, подтверждая удаление (сценарий жёсткого удаления).");
                }
                else
                {
                    _output.WriteLine($"[Жизненный цикл CRUD] Финальная проверка ПРЕДУПРЕЖДЕНИЕ: GetById выбросил HttpRequestException с неожиданным статусом {httpEx.StatusCode} для ID {createdId}.");
                }
            }

            // Проверяем поведение в зависимости от типа удаления
            if (deleteVerificationException is HttpRequestException { StatusCode: HttpStatusCode.NotFound })
            {
                // Жёсткое удаление: 404 означает успех
                // retrievedAfterDelete остается null, проверка пройдена
            }
            else if (retrievedAfterDelete != null)
            {
                // Мягкое удаление: сущность существует, но статус изменён
                retrievedAfterDelete.Status.Should().Be(StatusType.Deleted, "Статус сущности должен быть Deleted после мягкого удаления.");
                _output.WriteLine($"[Жизненный цикл CRUD] Финальная проверка УСПЕШНО: Сущность с ID {createdId} имеет статус Deleted, подтверждая мягкое удаление.");
            }
            else
            {
                // retrievedAfterDelete == null и исключения не было. Это необычно, но возможно.
                _output.WriteLine($"[Жизненный цикл CRUD] Финальная проверка ИНФО: GetById вернул null для ID {createdId} без выброса 404. Считаем, что удаление выполнено.");
            }

            _output.WriteLine($"[Жизненный цикл CRUD] ВСЕ ШАГИ УСПЕШНО: Тест жизненного цикла для {typeof(TModel).Name} завершён успешно.");
        }
        catch (Exception ex)
        {
            // --- СРАЗУ ОШИБКА & ЛОГИРОВАНИЕ ---
            _output.WriteLine($"[Жизненный цикл CRUD] ОШИБКА на шаге, связанном с ID сущности (если есть): {createdModel?.Id ?? updatedModel?.Id ?? retrievedAfterCreate?.Id ?? Guid.Empty}");
            _output.WriteLine($"[Жизненный цикл CRUD] ДЕТАЛИ ОШИБКИ: {ex}");
            throw; // Пробрасываем исключение, чтобы тест был помечен как упавший
        }
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCount()
    {
        // Arrange
        var baseUrl = GetRepositoryBaseUrl();
        await SetupAsync(baseUrl);

        _output.WriteLine($"[GetTotalCount] Getting total count for {typeof(TModel).Name}.");

        // Создаём простой запрос для получения общего количества (фильтры могут быть null)
        var query = new BaseGetQuery<TModel> { Take = null, Offset = 0 }; // Полный подсчет, без фильтрации

        // Act
        var count = await _repository!.GetTotalCountAsync(query, CancellationToken.None);

        // Assert
        count.Should().BeGreaterThanOrEqualTo(0, "GetTotalCountAsync should return a non-negative integer.");
        _output.WriteLine($"[GetTotalCount] Total count of {typeof(TModel).Name} is: {count}");
    }

    /* [Fact]
     public async Task ImportFromCSVAsync_WithValidFile_ShouldReturnTrue()
     {
         // Arrange
         var baseUrl = GetRepositoryBaseUrl();
         await SetupAsync(baseUrl);

         // Создаём простой CSV-контент для импорта.
         // Формат CSV должен соответствовать ожиданиям API для конкретного TModel.
         // Это может быть сложно обобщить. Часто импорт CSV используется для массового заполнения.
         // Для теста можно использовать пустой файл или файл с одной строкой-заглушкой,
         // если API позволяет (или ожидает) создание сущностей через CSV.
         // ВАЖНО: Этот тест может быть хрупким и зависеть от структуры CSV для конкретного справочника.
         // В реальности, возможно, импорт CSV используется не для создания новых уникальных сущностей,
         // а для обновления/заполнения существующих.
         // Для простоты, создадим пустой файл или файл с заголовками.
         var csvContentString = "id,comment,status\n"; // Пример заголовков (нужно адаптировать под модель)
         // Или просто: var csvContentString = "";
         var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContentString);
         var fileContent = new StreamContent(new MemoryStream(csvBytes));
         fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/csv"); // Или application/octet-stream

         using var formData = new MultipartFormDataContent();
         // Имя поля, как ожидает контроллер. В Swagger указано "formFile".
         formData.Add(fileContent, "formFile", "test_import.csv");

         _output.WriteLine($"[ImportFromCSV] Attempting to import {typeof(TModel).Name} from CSV file.");

         // Act
         bool importResult;
         try
         {
             importResult = await _repository!.ImportFromCSVAsync(formData, CancellationToken.None);
         }
         catch (System.Net.Http.HttpRequestException httpEx)
         {
             _output.WriteLine($"[ImportFromCSV] HttpRequestException: {httpEx.StatusCode} - {httpEx.Message}");
             // В зависимости от логики API, пустой файл или файл с неправильной структурой может вызвать ошибку.
             // Если это ожидаемое поведение для пустого/неправильного файла, тест может проверять конкретный код ошибки.
             // Если API ожидает валидный CSV, тест нужно будет адаптировать.
             // Пока что просто пробросим исключение, чтобы тест упал и показал ошибку.
             throw;
         }

         // Assert
         // Результат зависит от логики API. Он может возвращать true при успешной обработке,
         // false при ошибке в данных, или выбрасывать исключение.
         // Часто true означает, что файл принят на обработку.
         importResult.Should().BeTrue("ImportFromCSVAsync should return true if the file is accepted for processing.");
         _output.WriteLine($"[ImportFromCSV] Import operation returned: {importResult}");
     }

     [Fact]
     public async Task ExportFromCSVAsync_WithValidQuery_ShouldReturnCSVBytes()
     {
         // Arrange
         var baseUrl = GetRepositoryBaseUrl();
         await SetupAsync(baseUrl);

         // Создаём запрос для экспорта. Можно использовать пустой запрос (все сущности) или с фильтром.
         var query = new BaseGetQuery<TModel> {  }; // При необходимости добавьте фильтры

         _output.WriteLine($"[ExportFromCSV] Attempting to export {typeof(TModel).Name} to CSV with query.");

         // Act
         byte[] csvBytes;
         try
         {
             csvBytes = await _repository!.ExportFromCSVAsync(query, CancellationToken.None);
         }
         catch (System.Net.Http.HttpRequestException httpEx)
         {
             _output.WriteLine($"[ExportFromCSV] HttpRequestException: {httpEx.StatusCode} - {httpEx.Message}");
             // Если нет сущностей для экспорта, API может вернуть 404 или пустой файл.
             // Проверим это.
             if (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
             {
                 // Это может быть допустимым поведением, если нет данных.
                 // В этом случае, возможно, стоит пропустить тест или проверить длину ответа, если он возвращается.
                 // Пока что, для простоты, разрешим 404 и проверим, что исключение именно такое.
                 // Но лучше, чтобы API возвращал пустой CSV файл (0 байт) вместо 404.
                 // Для этого теста будем считать 404 ошибкой, если не указано иное.
                 throw; // Пробрасываем, чтобы тест упал.
             }
             else
             {
                 throw; // Пробрасываем другие ошибки.
             }
         }

         // Assert
         csvBytes.Should().NotBeNull("ExportFromCSVAsync should return byte array.");
         // csvBytes.Length.Should().BeGreaterThan(0, "Exported CSV should not be empty if entities exist."); // Может быть пустым, если нет данных или заголовки только.
         _output.WriteLine($"[ExportFromCSV] Export operation returned {csvBytes.Length} bytes.");
         // Опционально: Проверить, что возвращаемые байты действительно содержат CSV (например, проверить начало строки на наличие заголовков).
         if (csvBytes.Length > 0)
         {
             var csvContent = System.Text.Encoding.UTF8.GetString(csvBytes);
             _output.WriteLine($"[ExportFromCSV] Beginning of CSV content: {csvContent.Substring(0, Math.Min(100, csvContent.Length))}...");
             // Пример проверки: csvContent.Should().StartWith("id,comment,status"); // Зависит от структуры
         }
     }*/

    // --- Абстрактные методы для переопределения в наследниках ---

    /// <summary>
    /// Возвращает BaseUrl для конкретного репозитория (например, "http://.../administrative-unit").
    /// </summary>
    protected abstract string GetRepositoryBaseUrl();

    /// <summary>
    /// Создает валидную тестовую модель TModel.
    /// </summary>
    protected abstract TModel CreateValidTestModel();

    /// <summary>
    /// Изменяет существующую модель TModel для теста Update.
    /// </summary>
    protected abstract TModel ModifyModelForUpdate(TModel existingModel);
}