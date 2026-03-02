namespace Masofa.Web.Monolith.ViewModels.WeatherReport
{
    public class PointExportViewModel
    {
        public required WeatherDataSource WeatherDataSource { get; set; }
        public required double Longitude { get; set; }
        public required double Latitude { get; set; }
        public required DateOnly Date { get; set; }
        public required ExportFileType ExportFileType { get; set; }
    }
}
