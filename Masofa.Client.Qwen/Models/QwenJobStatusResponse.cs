using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Client.Qwen.Models
{
    public class QwenJobStatusResponse
    {
        public string status { get; set; }
        public string? message { get; set; }
        public string? log_level { get; set; }
        public double? progress { get; set; }
    }
}
