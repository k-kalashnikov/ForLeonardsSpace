namespace Masofa.Web.Monolith.ViewModels.WeatherReport
{
    public class LayerExportViewModel
    {
        public required WeatherDataSource WeatherDataSource { get; set; }
        public required string LayerName { get; set; }
        public required DateOnly Date { get; set; }
        public required ExportFileType ExportFileType { get; set; }
    }

    public enum WeatherDataSource
    {
        Era5 = 1,
        IBM = 2,
        UGM = 3
    }

    public enum ExportFileType
    {
        GeoJSON = 1,
        KML = 2,
        Shapefile = 3,
        CSV = 4
    }
}
