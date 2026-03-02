using Xunit.Abstractions;

namespace Masofa.Tests.IntegrationTest.Dictionaries;


public class AdministrativeUnitTest : DictionaryTestBase<AdministrativeUnit>
{
    public AdministrativeUnitTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AdministrativeUnit";
    }
    protected override AdministrativeUnit CreateValidTestModel()
    {
        var names = new LocalizationString();
        names["ru-RU"] = $"Demo Тестовая единица";
        names["en-US"] = $"Demo Test Unit";
        names["uz-Latn-UZ"] = $"Demo Test birligi";

        var model = new AdministrativeUnit
        {
            Names = names,
            Visible = true,
            Comment = $"Demo test AdministrativeUnit"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override AdministrativeUnit ModifyModelForUpdate(AdministrativeUnit existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AdministrativeUnit
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class AgroclimaticZoneTest : DictionaryTestBase<AgroclimaticZone>
{
    public AgroclimaticZoneTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AgroclimaticZone";
    }

    protected override AgroclimaticZone CreateValidTestModel()
    {
        var model = new AgroclimaticZone
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test AgroclimaticZone"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }


    protected override AgroclimaticZone ModifyModelForUpdate(AgroclimaticZone existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AgroclimaticZone
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class AgroMachineTypeTest : DictionaryTestBase<AgroMachineType>
{
    public AgroMachineTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AgroMachineType";
    }

    protected override AgroMachineType CreateValidTestModel()
    {
        var model = new AgroMachineType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test AgroMachineType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override AgroMachineType ModifyModelForUpdate(AgroMachineType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AgroMachineType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class AgroOperationTest : DictionaryTestBase<AgroOperation>
{
    public AgroOperationTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AgroOperation";
    }
    protected override AgroOperation CreateValidTestModel()
    {
        var model = new AgroOperation
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test AgroOperation"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override AgroOperation ModifyModelForUpdate(AgroOperation existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AgroOperation
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class AgrotechnicalMeasureTest : DictionaryTestBase<AgrotechnicalMeasure>
{
    public AgrotechnicalMeasureTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AgrotechnicalMeasure";
    }
    protected override AgrotechnicalMeasure CreateValidTestModel()
    {
        var model = new AgrotechnicalMeasure
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test AgrotechnicalMeasure"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override AgrotechnicalMeasure ModifyModelForUpdate(AgrotechnicalMeasure existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AgrotechnicalMeasure
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class AgroTermTest : DictionaryTestBase<AgroTerm>
{
    public AgroTermTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/AgroTerm";
    }
    protected override AgroTerm CreateValidTestModel()
    {
        var model = new AgroTerm
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test AgroTerm"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override AgroTerm ModifyModelForUpdate(AgroTerm existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new AgroTerm
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class BidContentTest : DictionaryTestBase<BidContent>
{
    public BidContentTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/BidContent";
    }
    protected override BidContent CreateValidTestModel()
    {
        var model = new BidContent
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test BidContent"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override BidContent ModifyModelForUpdate(BidContent existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new BidContent
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class BidStateTest : DictionaryTestBase<BidState>
{
    public BidStateTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/BidState";
    }
    protected override BidState CreateValidTestModel()
    {
        var model = new BidState
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test BidState"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override BidState ModifyModelForUpdate(BidState existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new BidState
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class BidTypeTest : DictionaryTestBase<BidType>
{
    public BidTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/BidType";
    }
    protected override BidType CreateValidTestModel()
    {
        var model = new BidType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test BidType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override BidType ModifyModelForUpdate(BidType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new BidType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class BusinessTypeTest : DictionaryTestBase<BusinessType>
{
    public BusinessTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/BusinessType";
    }
    protected override BusinessType CreateValidTestModel()
    {
        var model = new BusinessType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test BusinessType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override BusinessType ModifyModelForUpdate(BusinessType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new BusinessType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class ClimaticStandardTest : DictionaryTestBase<ClimaticStandard>
{
    public ClimaticStandardTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/ClimaticStandard";
    }
    protected override ClimaticStandard CreateValidTestModel()
    {
        var model = new ClimaticStandard
        {
            Comment = $"Demo test ClimaticStandard"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override ClimaticStandard ModifyModelForUpdate(ClimaticStandard existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new ClimaticStandard
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class CropTest : DictionaryTestBase<Crop>
{
    public CropTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Crop";
    }
    protected override Crop CreateValidTestModel()
    {
        var model = new Crop
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Crop"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Crop ModifyModelForUpdate(Crop existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Crop
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class CropPeriodTest : DictionaryTestBase<CropPeriod>
{
    public CropPeriodTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/CropPeriod";
    }
    protected override CropPeriod CreateValidTestModel()
    {
        var model = new CropPeriod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test CropPeriod"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override CropPeriod ModifyModelForUpdate(CropPeriod existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new CropPeriod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class CropPeriodVegetationIndexTest : DictionaryTestBase<CropPeriodVegetationIndex>
{
    public CropPeriodVegetationIndexTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/CropPeriodVegetationIndex";
    }
    protected override CropPeriodVegetationIndex CreateValidTestModel()
    {
        var model = new CropPeriodVegetationIndex
        {
            Comment = $"Demo test CropPeriodVegetationIndex"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override CropPeriodVegetationIndex ModifyModelForUpdate(CropPeriodVegetationIndex existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new CropPeriodVegetationIndex
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class DicitonaryTypeTest : DictionaryTestBase<DicitonaryType>
{
    public DicitonaryTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/DicitonaryType";
    }
    protected override DicitonaryType CreateValidTestModel()
    {
        var model = new DicitonaryType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test DicitonaryType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override DicitonaryType ModifyModelForUpdate(DicitonaryType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new DicitonaryType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class DiseaseTest : DictionaryTestBase<Disease>
{
    public DiseaseTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Disease";
    }
    protected override Disease CreateValidTestModel()
    {
        var model = new Disease
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Disease"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Disease ModifyModelForUpdate(Disease existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Disease
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class EntomophageTypeTest : DictionaryTestBase<EntomophageType>
{
    public EntomophageTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/EntomophageType";
    }
    protected override EntomophageType CreateValidTestModel()
    {
        var model = new EntomophageType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test EntomophageType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override EntomophageType ModifyModelForUpdate(EntomophageType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new EntomophageType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

// not names
public class ExperimentalFarmingMethodTest : DictionaryTestBase<ExperimentalFarmingMethod>
{
    public ExperimentalFarmingMethodTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/ExperimentalFarmingMethod";
    }
    protected override ExperimentalFarmingMethod CreateValidTestModel()
    {
        var model = new ExperimentalFarmingMethod
        {
            Name = "Demo test name ExperimentalFarmingMethod",
            Visible = true,
            Comment = $"Demo test ExperimentalFarmingMethod"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override ExperimentalFarmingMethod ModifyModelForUpdate(ExperimentalFarmingMethod existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new ExperimentalFarmingMethod
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class FertilizerTest : DictionaryTestBase<Fertilizer>
{
    public FertilizerTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Fertilizer";
    }
    protected override Fertilizer CreateValidTestModel()
    {
        var model = new Fertilizer
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Fertilizer"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Fertilizer ModifyModelForUpdate(Fertilizer existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Fertilizer
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class FertilizerTypeTest : DictionaryTestBase<FertilizerType>
{
    public FertilizerTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/FertilizerType";
    }
    protected override FertilizerType CreateValidTestModel()
    {
        var model = new FertilizerType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test FertilizerType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override FertilizerType ModifyModelForUpdate(FertilizerType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new FertilizerType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class FieldUsageStatusTest : DictionaryTestBase<FieldUsageStatus>
{
    public FieldUsageStatusTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/FieldUsageStatus";
    }
    protected override FieldUsageStatus CreateValidTestModel()
    {
        var model = new FieldUsageStatus
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test FieldUsageStatus"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override FieldUsageStatus ModifyModelForUpdate(FieldUsageStatus existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new FieldUsageStatus
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

/*public class FirmTest : DictionaryTestBase<Firm>
{
    public FirmTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Firm";
    }
    protected override Firm CreateValidTestModel()
    {
        var model = new Firm
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Firm"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Firm ModifyModelForUpdate(Firm existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Firm
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}*/

public class FlightTargetTest : DictionaryTestBase<FlightTarget>
{
    public FlightTargetTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/FlightTarget";
    }
    protected override FlightTarget CreateValidTestModel()
    {
        var model = new FlightTarget
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test FlightTarget"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override FlightTarget ModifyModelForUpdate(FlightTarget existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new FlightTarget
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class IrrigationMethodTest : DictionaryTestBase<IrrigationMethod>
{
    public IrrigationMethodTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/IrrigationMethod";
    }
    protected override IrrigationMethod CreateValidTestModel()
    {
        var model = new IrrigationMethod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test IrrigationMethod"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override IrrigationMethod ModifyModelForUpdate(IrrigationMethod existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new IrrigationMethod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class IrrigationSourceTest : DictionaryTestBase<IrrigationSource>
{
    public IrrigationSourceTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/IrrigationSource";
    }
    protected override IrrigationSource CreateValidTestModel()
    {
        var model = new IrrigationSource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test IrrigationSource"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override IrrigationSource ModifyModelForUpdate(IrrigationSource existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new IrrigationSource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class MeasurementUnitTest : DictionaryTestBase<MeasurementUnit>
{
    public MeasurementUnitTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/MeasurementUnit";
    }
    protected override MeasurementUnit CreateValidTestModel()
    {
        var model = new MeasurementUnit
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test MeasurementUnit"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override MeasurementUnit ModifyModelForUpdate(MeasurementUnit existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new MeasurementUnit
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class MeliorativeMeasureTypeTest : DictionaryTestBase<MeliorativeMeasureType>
{
    public MeliorativeMeasureTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/MeliorativeMeasureType";
    }
    protected override MeliorativeMeasureType CreateValidTestModel()
    {
        var model = new MeliorativeMeasureType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test MeliorativeMeasureType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override MeliorativeMeasureType ModifyModelForUpdate(MeliorativeMeasureType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new MeliorativeMeasureType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

/*public class PersonTest : DictionaryTestBase<Person>
{
    public PersonTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Person";
    }
    protected override Person CreateValidTestModel()
    {
        var model = new Person
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Person"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Person ModifyModelForUpdate(Person existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Person
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}*/

public class PesticideTest : DictionaryTestBase<Pesticide>
{
    public PesticideTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Pesticide";
    }
    protected override Pesticide CreateValidTestModel()
    {
        var model = new Pesticide
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Pesticide"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Pesticide ModifyModelForUpdate(Pesticide existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Pesticide
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class PesticideTypeTest : DictionaryTestBase<PesticideType>
{
    public PesticideTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/PesticideType";
    }
    protected override PesticideType CreateValidTestModel()
    {
        var model = new PesticideType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test PesticideType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override PesticideType ModifyModelForUpdate(PesticideType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new PesticideType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class PestTypeTest : DictionaryTestBase<PestType>
{
    public PestTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/PestType";
    }
    protected override PestType CreateValidTestModel()
    {
        var model = new PestType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test PestType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override PestType ModifyModelForUpdate(PestType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new PestType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class ProductQualityStandardTest : DictionaryTestBase<ProductQualityStandard>
{
    public ProductQualityStandardTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/ProductQualityStandard";
    }
    protected override ProductQualityStandard CreateValidTestModel()
    {
        var model = new ProductQualityStandard
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test ProductQualityStandard"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override ProductQualityStandard ModifyModelForUpdate(ProductQualityStandard existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new ProductQualityStandard
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class ProviderWeatherConditionTest : DictionaryTestBase<ProviderWeatherCondition>
{
    public ProviderWeatherConditionTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/ProviderWeatherCondition";
    }
    protected override ProviderWeatherCondition CreateValidTestModel()
    {
        var model = new ProviderWeatherCondition
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test ProviderWeatherCondition"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override ProviderWeatherCondition ModifyModelForUpdate(ProviderWeatherCondition existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new ProviderWeatherCondition
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class RegionTest : DictionaryTestBase<Region>
{
    public RegionTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Region";
    }
    protected override Region CreateValidTestModel()
    {
        var model = new Region
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Region"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Region ModifyModelForUpdate(Region existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Region
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

// not names
public class RegionMapTest : DictionaryTestBase<RegionMap>
{
    public RegionMapTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/RegionMap";
    }
    protected override RegionMap CreateValidTestModel()
    {
        var model = new RegionMap
        {
            Visible = true,
            Comment = $"Demo test RegionMap"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override RegionMap ModifyModelForUpdate(RegionMap existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new RegionMap
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class RegionTypeTest : DictionaryTestBase<RegionType>
{
    public RegionTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/RegionType";
    }
    protected override RegionType CreateValidTestModel()
    {
        var model = new RegionType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test RegionType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override RegionType ModifyModelForUpdate(RegionType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new RegionType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class SoilTypeTest : DictionaryTestBase<SoilType>
{
    public SoilTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/SoilType";
    }
    protected override SoilType CreateValidTestModel()
    {
        var model = new SoilType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test SoilType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override SoilType ModifyModelForUpdate(SoilType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new SoilType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

// not names
public class SolarRadiationInfluenceTest : DictionaryTestBase<SolarRadiationInfluence>
{
    public SolarRadiationInfluenceTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/SolarRadiationInfluence";
    }
    protected override SolarRadiationInfluence CreateValidTestModel()
    {
        var model = new SolarRadiationInfluence
        {
            Visible = true,
            Comment = $"Demo test SolarRadiationInfluence"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override SolarRadiationInfluence ModifyModelForUpdate(SolarRadiationInfluence existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new SolarRadiationInfluence
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class SystemDataSourceTest : DictionaryTestBase<SystemDataSource>
{
    public SystemDataSourceTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/SystemDataSource";
    }
    protected override SystemDataSource CreateValidTestModel()
    {
        var model = new SystemDataSource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test SystemDataSource"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override SystemDataSource ModifyModelForUpdate(SystemDataSource existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new SystemDataSource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class TaskStatusTest : DictionaryTestBase<Common.Models.Dictionaries.TaskStatus>
{
    public TaskStatusTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/TaskStatus";
    }
    protected override Common.Models.Dictionaries.TaskStatus CreateValidTestModel()
    {
        var model = new Common.Models.Dictionaries.TaskStatus
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Common.Models.Dictionaries.TaskStatus"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Common.Models.Dictionaries.TaskStatus ModifyModelForUpdate(Common.Models.Dictionaries.TaskStatus existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Common.Models.Dictionaries.TaskStatus
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class UavCameraTypeTest : DictionaryTestBase<UavCameraType>
{
    public UavCameraTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/UavCameraType";
    }
    protected override UavCameraType CreateValidTestModel()
    {
        var model = new UavCameraType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test UavCameraType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override UavCameraType ModifyModelForUpdate(UavCameraType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new UavCameraType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class UavDataTypeTest : DictionaryTestBase<UavDataType>
{
    public UavDataTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/UavDataType";
    }
    protected override UavDataType CreateValidTestModel()
    {
        var model = new UavDataType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test UavDataType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override UavDataType ModifyModelForUpdate(UavDataType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new UavDataType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class VarietyTest : DictionaryTestBase<Variety>
{
    public VarietyTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/Variety";
    }
    protected override Variety CreateValidTestModel()
    {
        var model = new Variety
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test Variety"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override Variety ModifyModelForUpdate(Variety existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new Variety
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class VarietyFeatureTest : DictionaryTestBase<VarietyFeature>
{
    public VarietyFeatureTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/VarietyFeature";
    }
    protected override VarietyFeature CreateValidTestModel()
    {
        var model = new VarietyFeature
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test VarietyFeature"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override VarietyFeature ModifyModelForUpdate(VarietyFeature existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new VarietyFeature
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

// not names
public class VegetationIndexTest : DictionaryTestBase<VegetationIndex>
{
    public VegetationIndexTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/VegetationIndex";
    }
    protected override VegetationIndex CreateValidTestModel()
    {
        var model = new VegetationIndex
        {
            Visible = true,
            Comment = $"Demo test VegetationIndex"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override VegetationIndex ModifyModelForUpdate(VegetationIndex existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new VegetationIndex
        {
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class VegetationPeriodTest : DictionaryTestBase<VegetationPeriod>
{
    public VegetationPeriodTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/VegetationPeriod";
    }
    protected override VegetationPeriod CreateValidTestModel()
    {
        var model = new VegetationPeriod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test VegetationPeriod"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override VegetationPeriod ModifyModelForUpdate(VegetationPeriod existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new VegetationPeriod
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class WaterResourceTest : DictionaryTestBase<WaterResource>
{
    public WaterResourceTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/WaterResource";
    }
    protected override WaterResource CreateValidTestModel()
    {
        var model = new WaterResource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test WaterResource"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override WaterResource ModifyModelForUpdate(WaterResource existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new WaterResource
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class WeatherStationTest : DictionaryTestBase<WeatherStation>
{
    public WeatherStationTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/WeatherStation";
    }
    protected override WeatherStation CreateValidTestModel()
    {
        var model = new WeatherStation
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test WeatherStation"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override WeatherStation ModifyModelForUpdate(WeatherStation existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new WeatherStation
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}

public class WeatherStationTypeTest : DictionaryTestBase<WeatherStationType>
{
    public WeatherStationTypeTest(ITestOutputHelper output) : base(output) { }

    protected override string GetRepositoryBaseUrl()
    {
        return $"{_baseUrl}/dictionaries/WeatherStationType";
    }
    protected override WeatherStationType CreateValidTestModel()
    {
        var model = new WeatherStationType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Test Unit",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Visible = true,
            Comment = $"Demo test WeatherStationType"
        };
        // Вызываем вспомогательный метод для заполнения общих полей
        return PopulateBaseEntityForCreate(model);
    }

    protected override WeatherStationType ModifyModelForUpdate(WeatherStationType existingModel)
    {
        // Создаём новую сущность, копируя все поля из существующей
        var updatedModel = new WeatherStationType
        {
            Names = new LocalizationString
            {
                ["ru-RU"] = $"Demo Тестовая единица",
                ["en-US"] = $"Demo Test Unit (Updated)",
                ["uz-Latn-UZ"] = $"Demo Test birligi"
            },
            Comment = existingModel.Comment + " (Updated)", // Изменяем нужное поле
            LastUpdateUser = TestConstants.TEST_USER_GUID, // Обновляем пользователя
        };
        return PrepareModelForUpdate(existingModel, updatedModel);
    }
}
