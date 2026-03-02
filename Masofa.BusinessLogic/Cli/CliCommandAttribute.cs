using Masofa.Common.Models;
using SharpKml.Dom;

namespace Masofa.BusinessLogic.Cli
{
    public class CliCommandAttribute : Attribute
    {
        public LocalizationString Names { get; set; }
        public LocalizationString Descriptions { get; set; }
    }
}
