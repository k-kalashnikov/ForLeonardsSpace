namespace Masofa.Common
{
    public class GeoServerOptions
    {
        public string GeoServerUrl { get; set; } = "localhost:8080";
        public string Workspace { get; set; } = "workspace";
        public string LayerName { get; set; } = "layer";
        public string Volume { get; set; } = "/data";
        public string UserName { get; set; } = "admin";
        public string Password { get; set; } = "geoserver";
    }
}
