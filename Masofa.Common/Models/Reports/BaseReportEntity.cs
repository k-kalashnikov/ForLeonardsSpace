using Masofa.Common.Models.MasofaAnaliticReport;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Reports
{
    public abstract class BaseReportEntity<TReportModel> : BaseEntity
        where TReportModel : class, new()
    {
        /// <summary>
        /// Печатная форма отчёта, которая храниться в MinIO
        /// </summary>
        [NotMapped]
        public LocalizationFileStorageItem LocalizationFile { get; set; }

        /// <summary>
        /// Объект в котором хранятся данные отчёта
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotMapped]
        public TReportModel ReportBody { get; set; } = new();

        /// <summary>
        /// Cереализация ReportBody
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public string? ReportBodyJson
        {
            get
            {
                return ReportBody == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(ReportBody);
            }
            set
            {
                if (value != null)
                {
                    ReportBody = Newtonsoft.Json.JsonConvert.DeserializeObject<TReportModel>(value);
                }
            }
        }
    }
}
