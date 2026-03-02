//// ⚠️ Этот файл сгенерирован автоматически. Не редактируй вручную.
//using Masofa.Common.Models.Dictionaries;
//using Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models;

//namespace Masofa.Cli.DevopsUtil.Converters
//{
//     /// <summary>
//     /// Содержит implicit-операторы для преобразования моделей
//     /// </summary>
//     public static class ImplicitConverters
//     {

//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AdministrativeUnit → Masofa.Common.Models.Dictionaries.AdministrativeUnit
//        /// </summary>
//        public static Common.Models.Dictionaries.AdministrativeUnit Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AdministrativeUnit source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AdministrativeUnit();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Level = source.Level;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroclimaticZone → Masofa.Common.Models.Dictionaries.AgroclimaticZone
//        /// </summary>
//        public static Common.Models.Dictionaries.AgroclimaticZone Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroclimaticZone source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AgroclimaticZone();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroMachineType → Masofa.Common.Models.Dictionaries.AgroMachineType
//        /// </summary>
//        public static Common.Models.Dictionaries.AgroMachineType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroMachineType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AgroMachineType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.IsSoilSafe = source.IsSoilSafe;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroOperation → Masofa.Common.Models.Dictionaries.AgroOperation
//        /// </summary>
//        public static Common.Models.Dictionaries.AgroOperation Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroOperation source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AgroOperation();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgrotechnicalMeasure → Masofa.Common.Models.Dictionaries.AgrotechnicalMeasure
//        /// </summary>
//        public static Common.Models.Dictionaries.AgrotechnicalMeasure Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgrotechnicalMeasure source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AgrotechnicalMeasure();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.CropId = source.CropId;
//             target.VarietyId = source.VarietyId;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroTerm → Masofa.Common.Models.Dictionaries.AgroTerm
//        /// </summary>
//        public static Common.Models.Dictionaries.AgroTerm Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.AgroTerm source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.AgroTerm();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.DescrEn = source.DescrEn;
//             target.DescrRu = source.DescrRu;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidContent → Masofa.Common.Models.Dictionaries.BidContent
//        /// </summary>
//        public static Common.Models.Dictionaries.BidContent Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidContent source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.BidContent();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidState → Masofa.Common.Models.Dictionaries.BidState
//        /// </summary>
//        public static Common.Models.Dictionaries.BidState Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidState source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.BidState();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidType → Masofa.Common.Models.Dictionaries.BidType
//        /// </summary>
//        public static Common.Models.Dictionaries.BidType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.BidType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.BidType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.BusinessType → Masofa.Common.Models.Dictionaries.BusinessType
//        /// </summary>
//        public static Common.Models.Dictionaries.BusinessType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.BusinessType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.BusinessType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.ClimaticStandard → Masofa.Common.Models.Dictionaries.ClimaticStandard
//        /// </summary>
//        public static Common.Models.Dictionaries.ClimaticStandard Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.ClimaticStandard source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.ClimaticStandard();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.RegionId = source.RegionId;
//             target.Month = source.Month;
//             target.Day = source.Day;
//             target.TempAvg = source.TempAvg;
//             target.TempMin = source.TempMin;
//             target.TempMax = source.TempMax;
//             target.PrecDayAvg = source.PrecDayAvg;
//             target.RadDayAvg = source.RadDayAvg;
//             target.HumAvg = source.HumAvg;
//             target.CoefSel = source.CoefSel;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Crop → Masofa.Common.Models.Dictionaries.Crop
//        /// </summary>
//        public static Common.Models.Dictionaries.Crop Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Crop source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Crop();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.NameLa = source.NameLa;
//             target.IsMonitoring = source.IsMonitoring;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.CropPeriod → Masofa.Common.Models.Dictionaries.CropPeriod
//        /// </summary>
//        public static Common.Models.Dictionaries.CropPeriod Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.CropPeriod source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.CropPeriod();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.RegionId = source.RegionId;
//             target.CropId = source.CropId;
//             target.VarietyId = source.VarietyId;
//             target.DayStart = source.DayStart;
//             target.DayEnd = source.DayEnd;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.CropPeriodVegetationIndex → Masofa.Common.Models.Dictionaries.CropPeriodVegetationIndex
//        /// </summary>
//        public static Common.Models.Dictionaries.CropPeriodVegetationIndex Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.CropPeriodVegetationIndex source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.CropPeriodVegetationIndex();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.CropPeriodId = source.CropPeriodId;
//             target.VegetationIndexId = source.VegetationIndexId;
//             target.Value = source.Value;
//             target.Min = source.Min;
//             target.Max = source.Max;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.DicitonaryType → Masofa.Common.Models.Dictionaries.DicitonaryType
//        /// </summary>
//        public static Common.Models.Dictionaries.DicitonaryType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.DicitonaryType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.DicitonaryType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Disease → Masofa.Common.Models.Dictionaries.Disease
//        /// </summary>
//        public static Common.Models.Dictionaries.Disease Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Disease source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Disease();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.NameLa = source.NameLa;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.EntomophageType → Masofa.Common.Models.Dictionaries.EntomophageType
//        /// </summary>
//        public static Common.Models.Dictionaries.EntomophageType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.EntomophageType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.EntomophageType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.NameLa = source.NameLa;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.ExperimentalFarmingMethod → Masofa.Common.Models.Dictionaries.ExperimentalFarmingMethod
//        /// </summary>
//        public static Common.Models.Dictionaries.ExperimentalFarmingMethod Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.ExperimentalFarmingMethod source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.ExperimentalFarmingMethod();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Name = source.Name;
//             target.CropId = source.CropId;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Fertilizer → Masofa.Common.Models.Dictionaries.Fertilizer
//        /// </summary>
//        public static Common.Models.Dictionaries.Fertilizer Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Fertilizer source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Fertilizer();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.IsEco = source.IsEco;
//             target.IsOrganic = source.IsOrganic;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.FertilizerType → Masofa.Common.Models.Dictionaries.FertilizerType
//        /// </summary>
//        public static Common.Models.Dictionaries.FertilizerType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.FertilizerType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.FertilizerType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.FieldUsageStatus → Masofa.Common.Models.Dictionaries.FieldUsageStatus
//        /// </summary>
//        public static Common.Models.Dictionaries.FieldUsageStatus Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.FieldUsageStatus source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.FieldUsageStatus();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Firm → Masofa.Common.Models.Dictionaries.Firm
//        /// </summary>
//        public static Common.Models.Dictionaries.Firm Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Firm source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Firm();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Inn = source.Inn;
//             target.Egrpo = source.Egrpo;
//             target.Site = source.Site;
//             target.Phones = source.Phones;
//             target.Email = source.Email;
//             target.Chief = source.Chief;
//             target.MainRegionId = source.MainRegionId;
//             target.PhysicalAddress = source.PhysicalAddress;
//             target.MailingAddress = source.MailingAddress;
//             target.ShortName = source.ShortName;
//             target.FullName = source.FullName;
//             target.InternationalName = source.InternationalName;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.FlightTarget → Masofa.Common.Models.Dictionaries.FlightTarget
//        /// </summary>
//        public static Common.Models.Dictionaries.FlightTarget Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.FlightTarget source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.FlightTarget();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.IrrigationMethod → Masofa.Common.Models.Dictionaries.IrrigationMethod
//        /// </summary>
//        public static Common.Models.Dictionaries.IrrigationMethod Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.IrrigationMethod source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.IrrigationMethod();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.IsWaterSafe = source.IsWaterSafe;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.IrrigationSource → Masofa.Common.Models.Dictionaries.IrrigationSource
//        /// </summary>
//        public static Common.Models.Dictionaries.IrrigationSource Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.IrrigationSource source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.IrrigationSource();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.MeasurementUnit → Masofa.Common.Models.Dictionaries.MeasurementUnit
//        /// </summary>
//        public static Common.Models.Dictionaries.MeasurementUnit Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.MeasurementUnit source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.MeasurementUnit();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.FullNameEn = source.FullNameEn;
//             target.FullNameRu = source.FullNameRu;
//             target.SiUnit = source.SiUnit;
//             target.Factor = source.Factor;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.MeliorativeMeasureType → Masofa.Common.Models.Dictionaries.MeliorativeMeasureType
//        /// </summary>
//        public static Common.Models.Dictionaries.MeliorativeMeasureType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.MeliorativeMeasureType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.MeliorativeMeasureType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Person → Masofa.Common.Models.Dictionaries.Person
//        /// </summary>
//        public static Common.Models.Dictionaries.Person Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Person source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Person();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Pinfl = source.Pinfl;
//             target.UserId = source.UserId;
//             target.Telegram = source.Telegram;
//             target.Phones = source.Phones;
//             target.Email = source.Email;
//             target.MainRegionId = source.MainRegionId;
//             target.PhysicalAddress = source.PhysicalAddress;
//             target.MailingAddress = source.MailingAddress;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Pesticide → Masofa.Common.Models.Dictionaries.Pesticide
//        /// </summary>
//        public static Common.Models.Dictionaries.Pesticide Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Pesticide source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Pesticide();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;
//             target.IntCode = source.IntCode;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.PesticideType → Masofa.Common.Models.Dictionaries.PesticideType
//        /// </summary>
//        public static Common.Models.Dictionaries.PesticideType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.PesticideType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.PesticideType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.IntCode = source.IntCode;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.PestType → Masofa.Common.Models.Dictionaries.PestType
//        /// </summary>
//        public static Common.Models.Dictionaries.PestType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.PestType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.PestType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.NameLa = source.NameLa;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.ProductQualityStandard → Masofa.Common.Models.Dictionaries.ProductQualityStandard
//        /// </summary>
//        public static Common.Models.Dictionaries.ProductQualityStandard Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.ProductQualityStandard source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.ProductQualityStandard();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.ProviderWeatherCondition → Masofa.Common.Models.Dictionaries.ProviderWeatherCondition
//        /// </summary>
//        public static Common.Models.Dictionaries.ProviderWeatherCondition Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.ProviderWeatherCondition source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.ProviderWeatherCondition();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.RegionId = source.RegionId;
//             target.Lat = source.Lat;
//             target.Lng = source.Lng;
//             target.Radius = source.Radius;
//             target.WeatherStationTypeId = source.WeatherStationTypeId;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Region → Masofa.Common.Models.Dictionaries.Region
//        /// </summary>
//        public static Common.Models.Dictionaries.Region Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Region source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Region();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.ParentId = source.ParentId;
//             target.Level = source.Level;
//             target.NameMhobt = source.NameMhobt;
//             target.ShortNameEn = source.ShortNameEn;
//             target.ShortNameRu = source.ShortNameRu;
//             target.NameAdminEn = source.NameAdminEn;
//             target.NameAdminRu = source.NameAdminRu;
//             target.Population = source.Population;
//             target.AgroclimaticZoneId = source.AgroclimaticZoneId;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.RegionMap → Masofa.Common.Models.Dictionaries.RegionMap
//        /// </summary>
//        public static Common.Models.Dictionaries.RegionMap Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.RegionMap source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.RegionMap();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Lat = source.Lat;
//             target.Lng = source.Lng;
//             target.MozaikX = source.MozaikX;
//             target.MozaikY = source.MozaikY;
//             target.Polygon = (NetTopologySuite.Geometries.Polygon?)source.Polygon;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.RegionType → Masofa.Common.Models.Dictionaries.RegionType
//        /// </summary>
//        public static Common.Models.Dictionaries.RegionType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.RegionType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.RegionType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.SoilType → Masofa.Common.Models.Dictionaries.SoilType
//        /// </summary>
//        public static Common.Models.Dictionaries.SoilType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.SoilType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.SoilType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.SolarRadiationInfluence → Masofa.Common.Models.Dictionaries.SolarRadiationInfluence
//        /// </summary>
//        public static Common.Models.Dictionaries.SolarRadiationInfluence Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.SolarRadiationInfluence source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.SolarRadiationInfluence();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.RegionId = source.RegionId;
//             target.CropId = source.CropId;
//             target.VarietyId = source.VarietyId;
//             target.DayStart = source.DayStart;
//             target.DayEnd = source.DayEnd;
//             target.RadNorm = source.RadNorm;
//             target.VegetationPeriodId = source.VegetationPeriodId;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.SystemDataSource → Masofa.Common.Models.Dictionaries.SystemDataSource
//        /// </summary>
//        public static Common.Models.Dictionaries.SystemDataSource Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.SystemDataSource source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.SystemDataSource();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.TaskStatus → Masofa.Common.Models.Dictionaries.TaskStatus
//        /// </summary>
//        public static Common.Models.Dictionaries.TaskStatus Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.TaskStatus source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.TaskStatus();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.UavCameraType → Masofa.Common.Models.Dictionaries.UavCameraType
//        /// </summary>
//        public static Common.Models.Dictionaries.UavCameraType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.UavCameraType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.UavCameraType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.UavDataType → Masofa.Common.Models.Dictionaries.UavDataType
//        /// </summary>
//        public static Common.Models.Dictionaries.UavDataType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.UavDataType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.UavDataType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.Variety → Masofa.Common.Models.Dictionaries.Variety
//        /// </summary>
//        public static Common.Models.Dictionaries.Variety Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.Variety source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.Variety();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.CropId = source.CropId;
//             target.NameLa = source.NameLa;
//             target.RipeningPeriod = source.RipeningPeriod;
//             target.AverageYield = source.AverageYield;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.VarietyFeature → Masofa.Common.Models.Dictionaries.VarietyFeature
//        /// </summary>
//        public static Common.Models.Dictionaries.VarietyFeature Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.VarietyFeature source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.VarietyFeature();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.VegetationIndex → Masofa.Common.Models.Dictionaries.VegetationIndex
//        /// </summary>
//        public static Common.Models.Dictionaries.VegetationIndex Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.VegetationIndex source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.VegetationIndex();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.Name = source.Name;
//             target.DescriptionEn = source.DescriptionEn;
//             target.DescriptionRu = source.DescriptionRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.VegetationPeriod → Masofa.Common.Models.Dictionaries.VegetationPeriod
//        /// </summary>
//        public static Common.Models.Dictionaries.VegetationPeriod Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.VegetationPeriod source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.VegetationPeriod();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.RegionId = source.RegionId;
//             target.VarietyId = source.VarietyId;
//             target.ClassId = source.ClassId;
//             target.DayStart = source.DayStart;
//             target.DayEnd = source.DayEnd;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.WaterResource → Masofa.Common.Models.Dictionaries.WaterResource
//        /// </summary>
//        public static Common.Models.Dictionaries.WaterResource Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.WaterResource source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WaterResource();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.WeatherStation → Masofa.Common.Models.Dictionaries.WeatherStation
//        /// </summary>
//        public static Common.Models.Dictionaries.WeatherStation Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.WeatherStation source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherStation();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             target.FirmId = source.FirmId;
//             target.RegionId = source.RegionId;
//             target.ClassId = source.ClassId;
//             target.IsAuto = source.IsAuto;
//             target.Lat = source.Lat;
//             target.Lng = source.Lng;
//             target.Radius = source.Radius;
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//        /// <summary>
//        /// Неявное преобразование Masofa.Depricated.DataAccess.DepricatedUdictServerTwo.Models.WeatherStationType → Masofa.Common.Models.Dictionaries.WeatherStationType
//        /// </summary>
//        public static Common.Models.Dictionaries.WeatherStationType Convert(Depricated.DataAccess.DepricatedUdictServerTwo.Models.WeatherStationType source)
//         {
//             if (source == null) return null;
//            var target = new Common.Models.Dictionaries.WeatherStationType();

//             // === ПРОВЕРЬ МАППИНГ ПОЛЕЙ ===
//             //target.NameUz = source.NameUz;
//             target.NameEn = source.NameEn;
//             target.NameRu = source.NameRu;
//             target.Visible = source.Visible;
//             target.OrderCode = source.OrderCode;
//             target.ExtData = source.ExtData;
//             target.Comment = source.Comment;
//             target.Id = source.Id;


//             return target;
//         }
//     }

//}
