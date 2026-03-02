using System.Text.Json.Serialization;

namespace Masofa.Common.Models.Satellite.Auxiliary
{
    public class SpecialValue
    {
        [JsonPropertyName("SPECIAL_VALUE_TEXT")]
        public string Text { get; set; }

        [JsonPropertyName("SPECIAL_VALUE_INDEX")]
        public int Index { get; set; }

        public override string ToString()
        {
            return string.Join(":",
            [
                Text,
                Index.ToString()
            ]);
        }

        public static SpecialValue FromString(string str)
        {
            var parts = str?.Split(':');
            if (parts?.Length != 2) return null;

            return new SpecialValue
            {
                Text = parts[0],
                Index = int.Parse(parts[1])
            };
        }
    }
}
