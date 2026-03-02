namespace Masofa.Common.Models.SystemCrical
{
    public class PosibleExecuteType
    {
        public LocalizationString Names { get; set; }
        public string ExecuteTypeName { get; set; }
        public SystemBackgroundTaskType TaskType { get; set; }
        public string GroupName { get; set; }
        public List<ParametrsJsonSchemaFields> ParametrsJsonSchema { get; set; }
    }

    public class ParametrsJsonSchemaFields
    {
        public string FieldName { get; set; }
        public LocalizationString FieldLabels { get; set; }
        public string DefaultValue { get; set; }
        public ParametrsJsonSchemaFieldsType FieldsType { get; set; }
        public Dictionary<string, string> PosibleValues { get; set; }
    }

    public enum ParametrsJsonSchemaFieldsType
    {
        InputText = 0,
        InputNumber = 1,
        InputString = 2,
        InputDate = 3,
        InputTime = 4,
        Select = 5,
        ComboBox = 6,
        //и т.д.
    }
}
