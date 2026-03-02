using Masofa.Common.Models;

namespace Masofa.Web.Monolith.Helpers
{
    public class BackgroundTaskLocalization
    {
        private static readonly Dictionary<string, LocalizationString> _localizationStore = new()
        {
            
            ["LoadAlertsJob"] = new LocalizationString
            {
                ["ru-RU"] = "Задача загрузки оповещений",
                ["en-US"] = "Alert download task",
                ["ar-LB"] = "مهمة تنزيل التنبيه",
                ["uz-Latn-UZ"] = ""
            },
            ["LoadCurrentDataJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка текущего задания по данным",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["LoadForecastDataJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка прогноза",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["ArviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Arvi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["EviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Evi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["GndviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Gndvi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["MndwiSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Mndwi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["NdmiSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Ndmi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["NdviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Ndvi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["OrviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Orvi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["OsaviSeasonReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Osavi по посевам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["ArviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Arvi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["EviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Evi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["GndviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Gndvi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["MndwiSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Mndwi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["NdmiSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Ndmi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["NdviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Ndvi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["OrviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Orvi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["OsaviSharedReportJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание отчета индекса Osavi по регионам и культурам",
                ["en-US"] = "",
                ["ar-LB"] = "",
                ["uz-Latn-UZ"] = ""
            },
            ["Sentinel2ArchiveParsingJob"] = new LocalizationString
            {
                ["ru-RU"] = "Парсинг архивов Sentinel и извлечение данных",
                ["en-US"] = "Parsing Sentinel archives and extracting data",
                ["ar-LB"] = "تحليل أرشيفات Sentinel واستخراج البيانات",
                ["uz-Latn-UZ"] = "Sentinel arxivlarini tahlil qilish va ma’lumotlarni chiqarib olish"
            },
            ["Sentinel2ConvertTilesJob"] = new LocalizationString
            {
                ["ru-RU"] = "Публикация TIFF-слоёв Sentinel в GeoServer",
                ["en-US"] = "Publishing Sentinel TIFF layers to GeoServer",
                ["ar-LB"] = "نشر طبقات Sentinel TIFF على GeoServer",
                ["uz-Latn-UZ"] = "Sentinel TIFF qatlamlarini GeoServerda chop etish"
            },
            ["Sentinel2MediaLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка архивов снимков Sentinel (ZIP)",
                ["en-US"] = "Downloading Sentinel imagery archives (ZIP)",
                ["ar-LB"] = "تنزيل أرشيفات صور Sentinel (ZIP)",
                ["uz-Latn-UZ"] = "Sentinel tasvirlari arxivlarini (ZIP) yuklab olish"
            },
            ["Sentinel2MetadataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка метаданных продуктов Sentinel",
                ["en-US"] = "Downloading Sentinel product metadata",
                ["ar-LB"] = "تنزيل بيانات تعريف منتج Sentinel",
                ["uz-Latn-UZ"] = "Sentinel mahsulotlari meta-ma’lumotlarini yuklash"
            },
            ["Sentinel2PreviewImageJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание превью изображений Sentinel",
                ["en-US"] = "Generating Sentinel Image Previews",
                ["ar-LB"] = "إنشاء معاينات صور Sentinel",
                ["uz-Latn-UZ"] = "Sentinel tasvirlarining dastlabki ko‘rinishini tayyorlash"
            },
            ["Sentinel2SearchProductJob"] = new LocalizationString
            {
                ["ru-RU"] = "Поиск спутниковых продуктов Sentinel",
                ["en-US"] = "Search for Sentinel satellite products",
                ["ar-LB"] = "ابحث عن منتجات الأقمار الصناعية Sentinel",
                ["uz-Latn-UZ"] = "Sentinel sun’iy yo‘ldosh mahsulotlarini qidirish"
            },
            ["CreatePartitionJob"] = new LocalizationString
            {
                ["ru-RU"] = "Создание таблиц и разделов в БД, связанных с календарными датами",
                ["en-US"] = "Creating tables and sections in the database associated with calendar dates",
                ["ar-LB"] = "إنشاء الجداول والأقسام في قاعدة البيانات المرتبطة بتواريخ التقويم",
                ["uz-Latn-UZ"] = "Ma’lumotlar bazasida taqvim sanalari bilan bog‘liq jadvallar va bo‘limlarni yaratish"
            },
            ["HealthCheckJob"] = new LocalizationString
            {
                ["ru-RU"] = "Мониторинг состояния системы и сервисов",
                ["en-US"] = "Monitoring the status of the system and services",
                ["ar-LB"] = "مراقبة حالة النظام والخدمات",
                ["uz-Latn-UZ"] = "Tizim va xizmatlar holatini kuzatib borish"
            },
            ["HelloJob"] = new LocalizationString
            {
                ["ru-RU"] = "Тестирование фонового планировщика и обучение сотрудников",
                ["en-US"] = "Background scheduler testing and employee training",
                ["ar-LB"] = "اختبار جدول الخلفية وتدريب الموظفين",
                ["uz-Latn-UZ"] = "Orqa fon rejalashtiruvchisini sinash va xodimlarni o‘qitish"
            },
            ["EraWeatherDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка прогнозных данных ERA5",
                ["en-US"] = "Loading ERA5 forecast data",
                ["ar-LB"] = "تحميل بيانات توقعات ERA5",
                ["uz-Latn-UZ"] = "ERA5 prognoz ma’lumotlarini yuklab olish"
            },
            ["EraWeatherHistoricalDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка исторических данных ERA5",
                ["en-US"] = "Downloading ERA5 historical data",
                ["ar-LB"] = "تنزيل بيانات ERA5 التاريخية",
                ["uz-Latn-UZ"] = "ERA5 tarixiy ma’lumotlarini yuklab olish"
            },
            ["Era5WeatherReportsCalculationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Расчёт агрегированных метеопоказателей ERA5",
                ["en-US"] = "Calculation of aggregated meteorological indicators ERA5",
                ["ar-LB"] = "حساب المؤشرات الجوية المجمعة ERA5",
                ["uz-Latn-UZ"] = "ERA5 umumlashtirilgan ob-havo ko‘rsatkichlarini hisoblash"
            },
            ["Era5WeatherReportsTilesGenerationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Генерация тайлов метеоданных ERA5",
                ["en-US"] = "Generating ERA5 weather data tiles",
                ["ar-LB"] = "إنشاء مربعات بيانات الطقس ERA5",
                ["uz-Latn-UZ"] = "ERA5 ob-havo ma’lumotlari tayllari (plitkalar)ni hosil qilish"
            },
            ["UgmWeatherCurrentDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка текущих метеоданных UGM",
                ["en-US"] = "Downloading current UGM weather data",
                ["ar-LB"] = "تنزيل بيانات الطقس الحالية لـ UGM",
                ["uz-Latn-UZ"] = "O‘zGM joriy ob-havo ma’lumotlarini yuklab olish"
            },
            ["UgmWeatherForecastDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка прогнозных метеоданных UGM",
                ["en-US"] = "Downloading UGM weather forecast data",
                ["ar-LB"] = "تنزيل بيانات توقعات الطقس UGM",
                ["uz-Latn-UZ"] = "O‘zGM prognoz ob-havo ma’lumotlarini yuklab olish"
            },
            ["UgmCurrentWeatherReportsTilesGenerationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Генерация тайлов текущих данных UGM",
                ["en-US"] = "Generating tiles of current UGM data",
                ["ar-LB"] = "إنشاء مربعات من بيانات UGM الحالية",
                ["uz-Latn-UZ"] = "O‘zGM joriy ma’lumotlari tayllari (plitkalar)ni hosil qilish"
            },
            ["UgmForecastWeatherReportsTilesGenerationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Генерация тайлов прогнозных данных UGM",
                ["en-US"] = "Generating UGM forecast data tiles",
                ["ar-LB"] = "إنشاء مربعات بيانات توقعات UGM",
                ["uz-Latn-UZ"] = "O‘zGM prognoz ma’lumotlari tayllari (plitkalar)ni hosil qilish"
            },
            ["IBMWeatherDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка прогнозных данных IBM (место + время)",
                ["en-US"] = "Loading IBM forecast data (location + time)",
                ["ar-LB"] = "تحميل بيانات توقعات IBM (الموقع + الوقت)",
                ["uz-Latn-UZ"] = "IBM Prognoz ma’lumotlarini (joy va vaqt bo‘yicha) yuklab olish"
            },
            ["IBMWeatherHistoricalDataLoaderJob"] = new LocalizationString
            {
                ["ru-RU"] = "Загрузка исторических данных IBM (место + время)",
                ["en-US"] = "Download IBM Historical Data (Location + Time)",
                ["ar-LB"] = "تنزيل بيانات IBM التاريخية (الموقع + الوقت)",
                ["uz-Latn-UZ"] = "IBM tarixiy ma’lumotlarini (joy va vaqt bo‘yicha) yuklab olish"
            },
            ["IbmWeatherReportsCalculationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Расчёт агрегированных данных IBM по дню, неделе, месяцу и году",
                ["en-US"] = "Calculate IBM aggregated data by day, week, month, and year",
                ["ar-LB"] = "حساب بيانات IBM المجمعة حسب اليوم والأسبوع والشهر والسنة",
                ["uz-Latn-UZ"] = "IBM umumlashtirilgan ma’lumotlarini kun, hafta, oy va yil kesimida hisoblash"
            },
            ["IbmWeatherReportsTilesGenerationJob"] = new LocalizationString
            {
                ["ru-RU"] = "Генерация картографических тайлов IBM и загрузка их в GeoServer",
                ["en-US"] = "Generating IBM map tiles and loading them into GeoServer",
                ["ar-LB"] = "إنشاء بلاطات خرائط IBM وتحميلها في GeoServer",
                ["uz-Latn-UZ"] = "IBM kartografik tayllari (plitkalar)ni yaratish va ularni GeoServerga joylash"
            },
            ["IBMWeatherCreatePartitionJob"] = new LocalizationString
            {
                ["ru-RU"] = "Задача обработки данных",
                ["en-US"] = "Data Processing Task",
                ["ar-LB"] = "مهمة معالجة البيانات",
                ["uz-Latn-UZ"] = ""
            },
            ["AnomaliesPointAndTiffJob"] = new LocalizationString
            {
                ["ru-RU"] = "Расчет аномалий в точках по значениям индексов",
                ["en-US"] = "Calculation of anomalies at points based on index values",
                ["ar-LB"] = "حساب الشذوذ في النقاط بناءً على قيم المؤشرات",
                ["uz-Latn-UZ"] = "Indeks qiymatlariga asoslangan nuqtalardagi anomaliyalarni hisoblash"
            }
        };

        public static LocalizationString GetLocalization(string key)
        {
            if (_localizationStore.TryGetValue(key, out var localizedString))
            {
                return localizedString;
            }
            
            return new LocalizationString();
        }
    }
}
