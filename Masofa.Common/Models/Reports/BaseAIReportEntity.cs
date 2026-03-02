using System.ComponentModel.DataAnnotations;

namespace Masofa.Common.Models.Reports
{
    public abstract class BaseAIReportEntity<TReportModel> : BaseReportEntity<TReportModel>
        where TReportModel : class, new()
    {
        /// <summary>
        /// Rating
        /// </summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Point { get; set; }

        /// <summary>
        /// Comment
        /// </summary>
        public string? Comment { get; set; }
    }
}
