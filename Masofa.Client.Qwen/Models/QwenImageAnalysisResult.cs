using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Client.Qwen.Models
{
    public class QwenImageAnalysisResult
    {
        public string archive_name { get; set; }
        public string image { get; set; }
        public string plant_type { get; set; }
        public bool anomaly_presence { get; set; }
        public string? anomaly_description { get; set; }
        public string? problem_type { get; set; }
        public string? anomaly1 { get; set; }
        public string? anomaly2 { get; set; }
        public string? classification_description { get; set; }
        public string? recommendations { get; set; }
    }
}
