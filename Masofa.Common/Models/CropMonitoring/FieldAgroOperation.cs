using Masofa.Common.Helper;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.CropMonitoring
{
    public class FieldAgroOperation : BaseEntity
    {
        public Guid FieldId { get; set; }
        public Guid OperationId { get; set; }
        public LocalizationString OperationName { get; set; }
        public string? OperationTypeFullName { get; set; }
        public string? AgroOperationParamsJson { get; set; }

        public DateTime OriginalDate { get; set; }
        public string? Comment { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Type? AgroOperationParamsType { get; set; }

        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Type? OperationType
        {
            get
            {
                return TypeHelper.GetTypeFromAllAssemblies(OperationTypeFullName);
            }
        }
    }

    public class FieldAgroOperationHistory : BaseHistoryEntity<FieldAgroOperation> { }
}
