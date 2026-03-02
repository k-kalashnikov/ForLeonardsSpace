using Masofa.Common.Models.Satellite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.Client.EarthExplorer
{
    public class LandsatLoginResponseViewModel
    {
        public long requestId { get; set; }
        public string version { get; set; } = default!;
        public string? data { get; set; }
        public string? errorCode { get; set; }
        public string? errorMessage { get; set; }
    }

    //public class LoginData
    //{
    //    public string apiKey { get; set; } = default!;
    //}

    public class AccessTokenViewModel
    {
        public string access_token { get; set; }
    }

    public class LandsatServiceOptions
    {
        public string MetadataApiUrl { get; set; } = string.Empty;
        public string SearchApiUrl { get; set; } = string.Empty;
        public string TokenApiUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<int> Paths { get; set; } = new();
        public List<int> Rows { get; set; } = new();
        public SatelliteSearchConfig SatelliteSearchConfig { get; set; } = new();
    }

    public class LandsatProductMetadataDto
    {
        public string? SpacecraftId { get; set; }
        public string? SensorId { get; set; }
        public string? ProcessingLevel { get; set; }
        public string? AcquisitionDate { get; set; }
        public string? SceneCenterTime { get; set; }
        public string? Path { get; set; }
        public string? Row { get; set; }
        public string? CloudCover { get; set; }
        public string? DataType { get; set; }
        public string? CollectionCategory { get; set; }
    }
}
