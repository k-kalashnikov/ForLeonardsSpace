using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Masofa.Common.Models;
using Masofa.Common.Models.Identity;
using Masofa.Common.Models.Satellite.Parse.Sentinel.Inspire;
using System.Xml.Linq;


namespace Masofa.Common.Resources
{
    /// <summary>
    /// Файл содержит метод для получения локализированного лога
    /// </summary>
    public class LogMessageResource //static
    {
        public static LocalizationString RequestStarted(System.String requestPath)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Начат запрос к маршруту {requestPath}"
                    },
                    {
                         "en-US", $"Started request to route {requestPath}"
                    },
                    {
                         "uz-Latn-UZ", $"{requestPath} yo‘nalishiga so‘rov yuborildi"
                    },
               };
        }
        public static LocalizationString RequestFinishedWithResult(System.String requestPath, System.String result)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Запрос к {requestPath} завершён успешно "
                    },
                    {
                         "en-US", $"Request to {requestPath} completed successfully"
                    },
                    {
                         "uz-Latn-UZ", $"{requestPath} so‘rovi muvaffaqiyatli yakunlandi"
                    },
               };
        }
        public static LocalizationString GenericError(System.String requestPath, System.String errorMessage)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Произошла непредвиденная ошибка в {requestPath}: {errorMessage}"
                    },
                    {
                         "en-US", $"Unexpected error occurred in {requestPath}: {errorMessage}"
                    },
                    {
                         "uz-Latn-UZ", $"{requestPath}da kutilmagan xato yuz berdi: {errorMessage}"
                    },
               };
        }
        public static LocalizationString EntityNotFound(System.String entityType, System.String id)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Объект {entityType} с идентификатором {id} не найден"
                    },
                    {
                         "en-US", $"Object {entityType} with ID {id} not found"
                    },
                    {
                         "uz-Latn-UZ", $"{id} identifikatorli {entityType} obyekti topilmadi"
                    },
               };
        }
        public static LocalizationString UserNotFound(System.String userName)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пользователь «{userName}» не найден"
                    },
                    {
                         "en-US", $"User “{userName}” not found"
                    },
                    {
                         "uz-Latn-UZ", $"{userName} foydalanuvchi topilmadi"
                    },
               };
        }
        public static LocalizationString ModelValidationFailed(System.String requestPath, System.String model)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Некорректные данные в {requestPath}: {model}"
                    },
                    {
                         "en-US", $"Invalid data in {requestPath}: {model}"
                    },
                    {
                         "uz-Latn-UZ", $"{requestPath}da noto‘g‘ri ma’lumotlar: {model}"
                    },
               };
        }
        public static LocalizationString TagRelationNotFound(System.String satelliteProductId, System.String tagId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Не найдена связь между тегом {tagId} и спутниковым снимком {satelliteProductId}"
                    },
                    {
                         "en-US", $"No link found between tag {tagId} and satellite image {satelliteProductId}"
                    },
                    {
                         "uz-Latn-UZ", $"{tagId} tegi va {satelliteProductId} sun’iy yo‘ldosh surati o‘rtasida bog‘lanish topilmadi"
                    },
               };
        }
        public static LocalizationString SatelliteProductNotFound(System.String satelliteProductId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Спутниковый снимок {satelliteProductId} не найден"
                    },
                    {
                         "en-US", $"Satellite image {satelliteProductId} not found"
                    },
                    {
                         "uz-Latn-UZ", $"{satelliteProductId} sun’iy yo‘ldosh tasviri topilmadi"
                    },
               };
        }
        public static LocalizationString FieldNotFoundOrNoPolygon(System.String fieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Поле {fieldId} не найдено или не имеет полигона"
                    },
                    {
                         "en-US", $"Field {fieldId} not found or has no polygon"
                    },
                    {
                         "uz-Latn-UZ", $"{fieldId} dala topilmadi yoki uning poligoni mavjud emas"
                    },
               };
        }
        public static LocalizationString InvalidDateRange()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Дата начала должна быть раньше даты окончания"
                    },
                    {
                         "en-US", $"Start date must be earlier than end date"
                    },
                    {
                         "uz-Latn-UZ", $"Boshlanish sanasi tugash sanasidan oldin bo‘lishi kerak"
                    },
               };
        }
        public static LocalizationString NoWeatherStationsNearby()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Нет доступных метеостанций в радиусе 10 км"
                    },
                    {
                         "en-US", $"No weather stations available within 10 km radius"
                    },
                    {
                         "uz-Latn-UZ", $"10 km radiusda meteorologik stansiyalar mavjud emas"
                    },
               };
        }
        public static LocalizationString TileOrLayerNotFound()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Слой карты не найден на сервере"
                    },
                    {
                         "en-US", $"Map layer not found on server"
                    },
                    {
                         "uz-Latn-UZ", $"Xarita qatlami serverda mavjud emas"
                    },
               };
        }
        public static LocalizationString NoFieldsForGlobalBoundaries()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Нет полей с полигонами для расчёта границ"
                    },
                    {
                         "en-US", $"No fields with polygons available for boundary calculation"
                    },
                    {
                         "uz-Latn-UZ", $"Qidiruv chegaralarini hisoblash uchun poligonli maydonlar mavjud emas"
                    },
               };
        }
        public static LocalizationString ActiveConfigNotFound()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Активная конфигурация не найдена — создана новая по умолчанию"
                    },
                    {
                         "en-US", $"Active configuration not found — a new default one created"
                    },
                    {
                         "uz-Latn-UZ", $"Faol sozlama topilmadi - standart yangi sozlama yaratildi"
                    },
               };
        }
        public static LocalizationString SystemBackgroundTaskStarted(System.String taskId, System.String name)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Фоновая задача {name} - {taskId} запущена"
                    },
                    {
                         "en-US", $"Background task {name} - {taskId} started"
                    },
                    {
                         "uz-Latn-UZ", $"{name} - {taskId} fon vazifasi ishga tushirildi"
                    },
               };
        }
        public static LocalizationString SystemBackgroundTaskStopped(System.String taskId, System.String name)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Фоновая задача {name} - {taskId} остановлена"
                    },
                    {
                         "en-US", $"Background task {name} - {taskId} stopped"
                    },
                    {
                         "uz-Latn-UZ", $"{name} - {taskId} fon vazifasi to‘xtatildi"
                    },
               };
        }
        public static LocalizationString BusinessEventSerialized(System.String messageBody)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Событие бизнес-логики сериализовано: {messageBody}"
                    },
                    {
                         "en-US", $"The business logic event is serialized: {messageBody}"
                    },
                    {
                         "uz-Latn-UZ", $"Biznes mantiqi hodisasi seriyalashtirildi: {messageBody}"
                    },
               };
        }
        public static LocalizationString ZipFileIsEmpty()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"ZIP-файл пуст"
                    },
                    {
                         "en-US", $"Zip File is empty"
                    },
                    {
                         "uz-Latn-UZ", $"ZIP fayl bo‘sh"
                    },
               };
        }
        public static LocalizationString KmlFileIsEmpty()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"KML-файл пуст"
                    },
                    {
                         "en-US", $"KML File is empty"
                    },
                    {
                         "uz-Latn-UZ", $"KML fayl bo‘sh"
                    },
               };
        }
        public static LocalizationString InvalidLogin()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Неверный логин"
                    },
                    {
                         "en-US", $"Invalid login credentials"
                    },
                    {
                         "uz-Latn-UZ", $"Login noto‘g‘ri"
                    },
               };
        }
        public static LocalizationString UserEmailNotFound()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пользователь с указанным email не найден"
                    },
                    {
                         "en-US", $"User with specified email not found"
                    },
                    {
                         "uz-Latn-UZ", $"Ko‘rsatilgan elektron pochta manziliga ega foydalanuvchi topilmadi"
                    },
               };
        }
        public static LocalizationString RoleNotFound(System.String roleName)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Роль «{roleName}» не найдена"
                    },
                    {
                         "en-US", $"Role «{roleName}» not found"
                    },
                    {
                         "uz-Latn-UZ", $"«{roleName}» roli topilmadi"
                    },
               };
        }
        public static LocalizationString UserIsNotDefined()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пользователь не определён"
                    },
                    {
                         "en-US", $"User not defined"
                    },
                    {
                         "uz-Latn-UZ", $"Foydalanuvchi aniqlanmagan"
                    },
               };
        }
        //public static LocalizationString RequestStarted(System.String requestPath)
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Начало запроса в {requestPath}"
        //            },
        //            {
        //                 "en-US", $"Начало запроса в {requestPath}"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Начало запроса в {requestPath}"
        //            },
        //       };
        //}
        //public static LocalizationString RequestFinishedWithResult(System.String requestPath, System.String result)
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Завершение запроса в {requestPath} с результатом: {result}"
        //            },
        //            {
        //                 "en-US", $"Завершение запроса в {requestPath} с результатом: {result}"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Завершение запроса в {requestPath} с результатом: {result}"
        //            },
        //       };
        //}
        //public static LocalizationString GenericError(System.String requestPath, System.String errorMessage)
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Произошло что-то не так в {requestPath}: {errorMessage}"
        //            },
        //            {
        //                 "en-US", $"Произошло что-то не так в {requestPath}: {errorMessage}"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Произошло что-то не так в {requestPath}: {errorMessage}"
        //            },
        //       };
        //}
        //public static LocalizationString UserNotFound(System.String Author)
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Пользователь '{Author}' не найден"
        //            },
        //            {
        //                 "en-US", $"Пользователь '{Author}' не найден"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Пользователь '{Author}' не найден"
        //            },
        //       };
        //}
        public static LocalizationString UavPhotoNotFound(System.String UavPhotoId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"UAVPhoto с Id = {UavPhotoId} не найден"
                    },
                    {
                         "en-US", $"UAVPhoto с Id = {UavPhotoId} не найден"
                    },
                    {
                         "uz-Latn-UZ", $"UAVPhoto с Id = {UavPhotoId} не найден"
                    },
               };
        }
        public static LocalizationString LocaleNotSupported(System.String locale, System.String Id)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Локаль '{locale}' не поддерживается LocalizationFile. Пропуск сезона '{Id}'"
                    },
                    {
                         "en-US", $"Локаль '{locale}' не поддерживается LocalizationFile. Пропуск сезона '{Id}'"
                    },
                    {
                         "uz-Latn-UZ", $"Локаль '{locale}' не поддерживается LocalizationFile. Пропуск сезона '{Id}'"
                    },
               };
        }
        public static LocalizationString ReportGenerated(System.String locale, System.String templateName)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Сформирован отчет для локали {locale} с использованием шаблона {templateName}"
                    },
                    {
                         "en-US", $"Сформирован отчет для локали {locale} с использованием шаблона {templateName}"
                    },
                    {
                         "uz-Latn-UZ", $"Сформирован отчет для локали {locale} с использованием шаблона {templateName}"
                    },
               };
        }
        public static LocalizationString FieldRecalculationCompleted(System.Int32 Count)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Завершён пересчёт для {Count} импортированных полей"
                    },
                    {
                         "en-US", $"Завершён пересчёт для {Count} импортированных полей"
                    },
                    {
                         "uz-Latn-UZ", $"Завершён пересчёт для {Count} импортированных полей"
                    },
               };
        }
        public static LocalizationString OperationStarted(System.String operationName, System.String details)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Начало операции: {operationName} {details}"
                    },
                    {
                         "en-US", $"Начало операции: {operationName} {details}"
                    },
                    {
                         "uz-Latn-UZ", $"Начало операции: {operationName} {details}"
                    },
               };
        }
        public static LocalizationString OperationEnded(System.String operationName, System.Int32 processedCount, System.String details)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Завершена операция: {operationName}. Обработано: {processedCount} {details}"
                    },
                    {
                         "en-US", $"Завершена операция: {operationName}. Обработано: {processedCount} {details}"
                    },
                    {
                         "uz-Latn-UZ", $"Завершена операция: {operationName}. Обработано: {processedCount} {details}"
                    },
               };
        }
        public static LocalizationString OperationEndedNoItems(System.String operationName, System.String details)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Завершена операция: {operationName} {details}"
                    },
                    {
                         "en-US", $"Завершена операция: {operationName} {details}"
                    },
                    {
                         "uz-Latn-UZ", $"Завершена операция: {operationName} {details}"
                    },
               };
        }
        public static LocalizationString OperationError(System.String operationName, System.String Message, System.String details)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка в операции {operationName}: {Message} {details}"
                    },
                    {
                         "en-US", $"Ошибка в операции {operationName}: {Message} {details}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка в операции {operationName}: {Message} {details}"
                    },
               };
        }
        public static LocalizationString NewProductProcessingStarted(System.String ProductId, System.String SatelliteType)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Обработка нового продукта {ProductId} для спутника {SatelliteType}"
                    },
                    {
                         "en-US", $"Обработка нового продукта {ProductId} для спутника {SatelliteType}"
                    },
                    {
                         "uz-Latn-UZ", $"Обработка нового продукта {ProductId} для спутника {SatelliteType}"
                    },
               };
        }
        public static LocalizationString FieldMappingsAdded(System.Int32 Count, System.String ProductId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Добавлено {Count} маппингов полей для продукта {ProductId}"
                    },
                    {
                         "en-US", $"Добавлено {Count} маппингов полей для продукта {ProductId}"
                    },
                    {
                         "uz-Latn-UZ", $"Добавлено {Count} маппингов полей для продукта {ProductId}"
                    },
               };
        }
        public static LocalizationString NoFieldIntersections(System.String ProductId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Нет пересечений для продукта {ProductId}"
                    },
                    {
                         "en-US", $"Нет пересечений для продукта {ProductId}"
                    },
                    {
                         "uz-Latn-UZ", $"Нет пересечений для продукта {ProductId}"
                    },
               };
        }
        public static LocalizationString FieldRecalculationStarted(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пересчет продуктов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Пересчет продуктов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Пересчет продуктов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString LandsatRecalculationStarted(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пересчет Landsat продуктов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Пересчет Landsat продуктов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Пересчет Landsat продуктов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString StacFeatureProcessingError(System.String FeatureId, System.String Message)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка при обработке STAC feature {FeatureId}: {Message}"
                    },
                    {
                         "en-US", $"Ошибка при обработке STAC feature {FeatureId}: {Message}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка при обработке STAC feature {FeatureId}: {Message}"
                    },
               };
        }
        public static LocalizationString LandsatMappingsAdded(System.Int32 Count, System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Добавлено {Count} Landsat маппингов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Добавлено {Count} Landsat маппингов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Добавлено {Count} Landsat маппингов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString SentinelRecalculationStarted(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пересчет Sentinel продуктов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Пересчет Sentinel продуктов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Пересчет Sentinel продуктов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString SentinelProductProcessingError(System.String SatellateProductId, System.String Message)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка при обработке Sentinel продукта {SatellateProductId}: {Message}"
                    },
                    {
                         "en-US", $"Ошибка при обработке Sentinel продукта {SatellateProductId}: {Message}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка при обработке Sentinel продукта {SatellateProductId}: {Message}"
                    },
               };
        }
        public static LocalizationString SentinelMappingsAdded(System.Int32 Count, System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Добавлено {Count} Sentinel маппингов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Добавлено {Count} Sentinel маппингов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Добавлено {Count} Sentinel маппингов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString FieldProductsRecalculated(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пересчитаны продукты для поля {FieldId}"
                    },
                    {
                         "en-US", $"Пересчитаны продукты для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Пересчитаны продукты для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString GlobalBoundariesRecalculationStarted()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Начинаем пересчет глобальных границ поиска"
                    },
                    {
                         "en-US", $"Начинаем пересчет глобальных границ поиска"
                    },
                    {
                         "uz-Latn-UZ", $"Начинаем пересчет глобальных границ поиска"
                    },
               };
        }
        public static LocalizationString GlobalBoundariesRecalculated(System.Int32 Count)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Глобальные границы успешно пересчитаны. Поля: {Count}"
                    },
                    {
                         "en-US", $"Глобальные границы успешно пересчитаны. Поля: {Count}"
                    },
                    {
                         "uz-Latn-UZ", $"Глобальные границы успешно пересчитаны. Поля: {Count}"
                    },
               };
        }
        public static LocalizationString OldConfigsDeactivated(System.Int32 Count)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Деактивировано {Count} старых конфигураций"
                    },
                    {
                         "en-US", $"Деактивировано {Count} старых конфигураций"
                    },
                    {
                         "uz-Latn-UZ", $"Деактивировано {Count} старых конфигураций"
                    },
               };
        }
        public static LocalizationString ProductMappingRemovalStarted(System.String ProductId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Удаление маппинга для продукта {ProductId}"
                    },
                    {
                         "en-US", $"Удаление маппинга для продукта {ProductId}"
                    },
                    {
                         "uz-Latn-UZ", $"Удаление маппинга для продукта {ProductId}"
                    },
               };
        }
        public static LocalizationString ActiveConfigRetrieved()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Получение активной конфигурации поиска"
                    },
                    {
                         "en-US", $"Получение активной конфигурации поиска"
                    },
                    {
                         "uz-Latn-UZ", $"Получение активной конфигурации поиска"
                    },
               };
        }
        public static LocalizationString DefaultConfigCreated()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Создана конфигурация по умолчанию"
                    },
                    {
                         "en-US", $"Создана конфигурация по умолчанию"
                    },
                    {
                         "uz-Latn-UZ", $"Создана конфигурация по умолчанию"
                    },
               };
        }
        public static LocalizationString LandsatMBRRetrieved()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"MBR для Landsat API успешно получен"
                    },
                    {
                         "en-US", $"MBR для Landsat API успешно получен"
                    },
                    {
                         "uz-Latn-UZ", $"MBR для Landsat API успешно получен"
                    },
               };
        }
        public static LocalizationString LandsatMBRRequested()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Получение MBR для Landsat API"
                    },
                    {
                         "en-US", $"Получение MBR для Landsat API"
                    },
                    {
                         "uz-Latn-UZ", $"Получение MBR для Landsat API"
                    },
               };
        }
        public static LocalizationString ProductsForFieldRetrieved(System.Int32 Count, System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Получено {Count} продуктов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Получено {Count} продуктов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Получено {Count} продуктов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString ProductsForFieldRequested(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Получение продуктов для поля {FieldId}"
                    },
                    {
                         "en-US", $"Получение продуктов для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Получение продуктов для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString SentinelWKTRetrieved()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"WKT для Sentinel2 API успешно получен"
                    },
                    {
                         "en-US", $"WKT для Sentinel2 API успешно получен"
                    },
                    {
                         "uz-Latn-UZ", $"WKT для Sentinel2 API успешно получен"
                    },
               };
        }
        public static LocalizationString SentinelWKTRequested()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Получение WKT для Sentinel2 API"
                    },
                    {
                         "en-US", $"Получение WKT для Sentinel2 API"
                    },
                    {
                         "uz-Latn-UZ", $"Получение WKT для Sentinel2 API"
                    },
               };
        }
        //public static LocalizationString ModelValidationFailed(System.String requestPath, System.String query)
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Модель недопустима в {requestPath}. Модель: {query}"
        //            },
        //            {
        //                 "en-US", $"Модель недопустима в {requestPath}. Модель: {query}"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Модель недопустима в {requestPath}. Модель: {query}"
        //            },
        //       };
        //}
        public static LocalizationString EntityNotFound(System.Type Model, System.String id)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Сущность с типом {Model} с Id = {id} не найдена"
                    },
                    {
                         "en-US", $"Сущность с типом {Model} с Id = {id} не найдена"
                    },
                    {
                         "uz-Latn-UZ", $"Сущность с типом {Model} с Id = {id} не найдена"
                    },
               };
        }
        public static LocalizationString HistoryNotFound(System.String Name)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"История для сущности с типом {Name} не найдена"
                    },
                    {
                         "en-US", $"История для сущности с типом {Name} не найдена"
                    },
                    {
                         "uz-Latn-UZ", $"История для сущности с типом {Name} не найдена"
                    },
               };
        }
        public static LocalizationString TableDataModelValidationFailed(System.String requestPath)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Модель недопустима в {requestPath}"
                    },
                    {
                         "en-US", $"Модель недопустима в {requestPath}"
                    },
                    {
                         "uz-Latn-UZ", $"Модель недопустима в {requestPath}"
                    },
               };
        }
        public static LocalizationString FileNotFound(System.String id)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Файл с Id {id} не найден"
                    },
                    {
                         "en-US", $"Файл с Id {id} не найден"
                    },
                    {
                         "uz-Latn-UZ", $"Файл с Id {id} не найден"
                    },
               };
        }
        public static LocalizationString DocumentationNotFoundOrNotPublic(System.String documentationId, System.String requestPath)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Документация {documentationId} не найдена или не является публичной в {requestPath}"
                    },
                    {
                         "en-US", $"Документация {documentationId} не найдена или не является публичной в {requestPath}"
                    },
                    {
                         "uz-Latn-UZ", $"Документация {documentationId} не найдена или не является публичной в {requestPath}"
                    },
               };
        }
        public static LocalizationString DocumentationHasNoFile(System.String documentationId, System.String requestPath)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Документация {documentationId} не содержит файла в {requestPath}"
                    },
                    {
                         "en-US", $"Документация {documentationId} не содержит файла в {requestPath}"
                    },
                    {
                         "uz-Latn-UZ", $"Документация {documentationId} не содержит файла в {requestPath}"
                    },
               };
        }
        public static LocalizationString FileStorageItemNotFound(System.String FileStorageId, System.String requestPath)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Файл {FileStorageId} не найден в {requestPath}"
                    },
                    {
                         "en-US", $"Файл {FileStorageId} не найден в {requestPath}"
                    },
                    {
                         "uz-Latn-UZ", $"Файл {FileStorageId} не найден в {requestPath}"
                    },
               };
        }
        public static LocalizationString NoTagRelationsFound(System.String ownerId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Не найдено связей тегов для ownerId: {ownerId}"
                    },
                    {
                         "en-US", $"Не найдено связей тегов для ownerId: {ownerId}"
                    },
                    {
                         "uz-Latn-UZ", $"Не найдено связей тегов для ownerId: {ownerId}"
                    },
               };
        }
        public static LocalizationString FieldNotFound(System.String fieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Поле с ID {fieldId} не найдено"
                    },
                    {
                         "en-US", $"Поле с ID {fieldId} не найдено"
                    },
                    {
                         "uz-Latn-UZ", $"Поле с ID {fieldId} не найдено"
                    },
               };
        }
        public static LocalizationString FileIsEmptyOrNull()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Файл пуст или равен null"
                    },
                    {
                         "en-US", $"Файл пуст или равен null"
                    },
                    {
                         "uz-Latn-UZ", $"Файл пуст или равен null"
                    },
               };
        }
        public static LocalizationString UserNotFoundById(System.String userId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пользователь с Id = {userId} не найден"
                    },
                    {
                         "en-US", $"Пользователь с Id = {userId} не найден"
                    },
                    {
                         "uz-Latn-UZ", $"Пользователь с Id = {userId} не найден"
                    },
               };
        }
        public static LocalizationString LandsatProductsError(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка получения продуктов Landsat для поля {FieldId}"
                    },
                    {
                         "en-US", $"Ошибка получения продуктов Landsat для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка получения продуктов Landsat для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString Sentinel2ProductsError(System.String FieldId)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка получения продуктов Sentinel2 для поля {FieldId}"
                    },
                    {
                         "en-US", $"Ошибка получения продуктов Sentinel2 для поля {FieldId}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка получения продуктов Sentinel2 для поля {FieldId}"
                    },
               };
        }
        public static LocalizationString ProductsNotFoundOnServer()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Продукты не найдены на сервере"
                    },
                    {
                         "en-US", $"Продукты не найдены на сервере"
                    },
                    {
                         "uz-Latn-UZ", $"Продукты не найдены на сервере"
                    },
               };
        }
        public static LocalizationString LayerNotFoundOnServer()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Слой не найден на сервере"
                    },
                    {
                         "en-US", $"Слой не найден на сервере"
                    },
                    {
                         "uz-Latn-UZ", $"Слой не найден на сервере"
                    },
               };
        }
        public static LocalizationString UserNotFoundByPinfl(System.String pinfl)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Пользователь с Pinfl = {pinfl} не найден"
                    },
                    {
                         "en-US", $"Пользователь с Pinfl = {pinfl} не найден"
                    },
                    {
                         "uz-Latn-UZ", $"Пользователь с Pinfl = {pinfl} не найден"
                    },
               };
        }
        //public static LocalizationString InvalidLogin()
        //{
        //    return new Dictionary<string, string>()
        //       {
        //            {
        //                 "ru-RU", $"Неверный логин"
        //            },
        //            {
        //                 "en-US", $"Неверный логин"
        //            },
        //            {
        //                 "uz-Latn-UZ", $"Неверный логин"
        //            },
        //       };
        //}
        public static LocalizationString ModelIsEmptyOrNull()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Модель пуста или равна null"
                    },
                    {
                         "en-US", $"Модель пуста или равна null"
                    },
                    {
                         "uz-Latn-UZ", $"Модель пуста или равна null"
                    },
               };
        }
        public static LocalizationString ModelDeserializationError(System.String Message)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка десериализации модели: {Message}"
                    },
                    {
                         "en-US", $"Ошибка десериализации модели: {Message}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка десериализации модели: {Message}"
                    },
               };
        }
        public static LocalizationString JobStarted(System.String jobName)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Запуск джобы: {jobName}"
                    },
                    {
                         "en-US", $"Запуск джобы: {jobName}"
                    },
                    {
                         "uz-Latn-UZ", $"Запуск джобы: {jobName}"
                    },
               };
        }
        public static LocalizationString JobCompleted(System.String jobName)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Джоба {jobName} успешно завершена"
                    },
                    {
                         "en-US", $"Джоба {jobName} успешно завершена"
                    },
                    {
                         "uz-Latn-UZ", $"Джоба {jobName} успешно завершена"
                    },
               };
        }
        public static LocalizationString JobError(System.String jobName, System.String Message)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка в джобе {jobName}: {Message}"
                    },
                    {
                         "en-US", $"Ошибка в джобе {jobName}: {Message}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка в джобе {jobName}: {Message}"
                    },
               };
        }
        public static LocalizationString TwoFACodeSent(System.String Email)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"2FA код отправлен на {Email}"
                    },
                    {
                         "en-US", $"2FA код отправлен на {Email}"
                    },
                    {
                         "uz-Latn-UZ", $"2FA код отправлен на {Email}"
                    },
               };
        }
        public static LocalizationString TwoFACodeSendError(System.String Email)
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Ошибка отправки 2FA кода на {Email}"
                    },
                    {
                         "en-US", $"Ошибка отправки 2FA кода на {Email}"
                    },
                    {
                         "uz-Latn-UZ", $"Ошибка отправки 2FA кода на {Email}"
                    },
               };
        }
        public static LocalizationString EmailConfirmed()
        {
            return new Dictionary<string, string>()
               {
                    {
                         "ru-RU", $"Email подтверждён!"
                    },
                    {
                         "en-US", $"Email подтверждён!"
                    },
                    {
                         "uz-Latn-UZ", $"Email подтверждён!"
                    },
               };
        }
    }
}

