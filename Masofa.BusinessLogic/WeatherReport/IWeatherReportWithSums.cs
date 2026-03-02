using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masofa.BusinessLogic.WeatherReport
{
    public interface IWeatherReportWithSums
    {
        public double? SumOfActiveTemperaturesBase5 { get; set; }
        public double? SumOfActiveTemperaturesBase7 { get; set; }
        public double? SumOfActiveTemperaturesBase10 { get; set; }
        public double? SumOfActiveTemperaturesBase12 { get; set; }
        public double? SumOfActiveTemperaturesBase15 { get; set; }
        public double? SumOfSolarRadiation { get; set; }
        public double? SumOfFallout { get; set; }
    }

    public class WeatherReportWithSums : IWeatherReportWithSums
    {
        public double? SumOfActiveTemperaturesBase5 { get; set; }
        public double? SumOfActiveTemperaturesBase7 { get; set; }
        public double? SumOfActiveTemperaturesBase10 { get; set; }
        public double? SumOfActiveTemperaturesBase12 { get; set; }
        public double? SumOfActiveTemperaturesBase15 { get; set; }
        public double? SumOfSolarRadiation { get; set; }
        public double? SumOfFallout { get; set; }
    }
}
