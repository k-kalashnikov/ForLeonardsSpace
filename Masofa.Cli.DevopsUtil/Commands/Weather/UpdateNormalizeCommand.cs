using DocumentFormat.OpenXml.Vml;
using Masofa.Common.Attributes;
using Masofa.Common.Models.Era;
using Masofa.Common.Models.SystemCrical;
using Masofa.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Reflection;

namespace Masofa.Cli.DevopsUtil.Commands.Weather
{
    [BaseCommand("UpdateNormalizeCommand", "UpdateNormalizeCommand")]
    public class UpdateNormalizeCommand : IBaseCommand
    {
        public MasofaIBMWeatherDbContext MasofaIBMWeatherDbContext { get; init; }
        public MasofaEraDbContext MasofaEraDbContext { get; init; }
        public HttpClient HttpClient { get; init; }
        public UpdateNormalizeCommand(MasofaIBMWeatherDbContext masofaIBMWeatherDbContext, MasofaEraDbContext masofaEraDbContext)
        {
            MasofaIBMWeatherDbContext = masofaIBMWeatherDbContext;
            MasofaEraDbContext = masofaEraDbContext;
            HttpClient = new HttpClient();
        }

        public async Task Execute()
        {
            var stations = MasofaEraDbContext.EraWeatherStations
                .ToList();

            var years = new List<int>
            {
                2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025
            };
            var index = 1;
            Console.Clear();
            foreach (var station in stations)
            {
                try
                {
                    DrawProgressBar(0, index, stations.Count, "UpdateNormalizeCommand", $"Processing station {index}/{stations.Count} - {station.Id} Lat:{station.Point.Y} Lng: {station.Point.X}");
                    var eraNormalizedWeathers = MasofaEraDbContext.Era5DayNormalizedWeather.Where(x => x.WeatherStation == station.Id).ToList();
                    MasofaEraDbContext.Era5DayNormalizedWeather.RemoveRange(eraNormalizedWeathers);
                    eraNormalizedWeathers = new List<Era5DayNormalizedWeather>();
                    var lastMonth = 1;
                    var lineResult = 3;
                    for (var currentDay = new DateOnly(2021, 1, 1); currentDay < new DateOnly(2022, 1, 1); currentDay = currentDay.AddDays(1))
                    {
                        DrawProgressBar(0, index, stations.Count, "UpdateNormalizeCommand", $"Processing date {currentDay.Day}/{currentDay.Month} Lat:{station.Point.Y} Lng: {station.Point.X}");
                        var eraReportsWeathers = new List<Era5DayWeatherReport>();
                        foreach (var year in years)
                        {
                            var tempDay = new DateOnly(year, currentDay.Month, currentDay.Day);
                            var tempBody = new InputViewModel()
                            {
                                Location = new LocationViewModel()
                                {
                                    X = station.Point.X,
                                    Y = station.Point.Y
                                },
                                ReportType = ReportType.Day,
                                ReportConfigJson = new DayConfig()
                                {
                                    DateOnly = tempDay
                                }
                            };
                            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://185.100.234.107:28000/v1/weather");
                            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(tempBody), System.Text.Encoding.UTF8, "application/json");
                            var response = await SendRequestAsync<OutputWeatherReport>(request, CancellationToken.None);
                            if (response == null)
                            {
                                Console.WriteLine($"No data for station {station.Id} for date {tempDay}");
                                continue;
                            }
                            var eraReportWeather = new Era5DayWeatherReport
                            {
                                WeatherStation = station.Id,
                                TemperatureMin = response.TemperatureMin,
                                TemperatureMax = response.TemperatureMax,
                                TemperatureMinTotal = response.TemperatureMinTotal,
                                TemperatureMaxTotal = response.TemperatureMaxTotal,
                                SolarRadiationInfluence = response.SolarRadiationInfluence,
                                Fallout = response.Fallout,
                                Humidity = response.Humidity,
                                WindSpeed = response.WindSpeed,
                                WindDerection = response.WindDerection,
                                Date = tempDay
                            };
                            eraReportsWeathers.Add(eraReportWeather);
                        }
                        var eraNormalizedWeather = new Era5DayNormalizedWeather
                        {
                            WeatherStation = station.Id,
                            Day = currentDay.Day,
                            Month = currentDay.Month,
                            TemperatureMin = eraReportsWeathers.Average(x => x.TemperatureMin),
                            TemperatureMax = eraReportsWeathers.Average(x => x.TemperatureMax),
                            TemperatureMinTotal = eraReportsWeathers.Average(x => x.TemperatureMinTotal),
                            TemperatureMaxTotal = eraReportsWeathers.Average(x => x.TemperatureMaxTotal),
                            SolarRadiationInfluence = eraReportsWeathers.Average(x => x.SolarRadiationInfluence),
                            Fallout = eraReportsWeathers.Average(x => x.Fallout),
                            Humidity = eraReportsWeathers.Average(x => x.Humidity),
                            WindSpeed = eraReportsWeathers.Average(x => x.WindSpeed),
                            WindDerection = eraReportsWeathers.Average(x => x.WindDerection)
                        };
                        eraNormalizedWeathers.Add(eraNormalizedWeather);
                        PrintCalendar(lineResult, currentDay.Year, eraNormalizedWeathers.Select(en => new DateOnly(currentDay.Year, en.Month, en.Day)).ToList());
                    }
                    MasofaEraDbContext.Era5DayNormalizedWeather.AddRange(eraNormalizedWeathers);
                    MasofaEraDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    DrawProgressBar(0, index, stations.Count, "UpdateNormalizeCommand", ex.Message);
                    continue;
                }
                index++;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        private async Task<TResult> SendRequestAsync<TResult>(HttpRequestMessage message, CancellationToken cancellationToken) where TResult : class
        {
            try
            {
                // Добавим проверку на null
                if (HttpClient == null)
                {
                    throw new InvalidOperationException("HttpClient is null in BaseRepository");
                }

                var response = await HttpClient.SendAsync(message, cancellationToken);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var responseString = await response.Content.ReadAsStringAsync();
                        object result = (typeof(string) == typeof(TResult)) ? responseString : Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(responseString);
                        return (TResult)result;
                    case HttpStatusCode.NotFound:
                        throw new HttpRequestException("Page not found", null, HttpStatusCode.NotFound);
                    case HttpStatusCode.Unauthorized:
                        throw new HttpRequestException("Unauthorized user", null, HttpStatusCode.Unauthorized);
                    case HttpStatusCode.BadRequest:
                        var badRequestContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Model not valid: {badRequestContent}", null, HttpStatusCode.BadRequest);
                    case HttpStatusCode.InternalServerError:
                        var serverErrorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Internal Server Error: {serverErrorContent}", null, HttpStatusCode.InternalServerError);
                    default:
                        var defaultContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Unhandled code: {response.StatusCode}. With message: {defaultContent}", null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"BaseRepository.SendRequestAsync failed in: {ex}";
                Console.WriteLine(errorMessage);
                return null;
            }
        }

        private void DrawProgressBar(int line, int current, int total, string taksName, string message)
        {
            // Ограничиваем текущий прогресс
            current = Math.Min(current, total);
            int filledLength = ((int)Math.Round((double)current / total) * 100);

            string bar = new string('█', filledLength).PadRight(100);
            string text = $"{taksName}: [{bar}] {current} of {total.ToString()}";
            Console.SetCursorPosition(0, line);
            Console.Write(message.PadRight(Console.WindowWidth - 1)); // затираем старый текст
            Console.SetCursorPosition(0, line + 1);
            Console.Write(text.PadRight(Console.WindowWidth - 1));
        }


        public static void PrintCalendar(int line, int year, List<DateOnly> highlightedDates)
        {
            var highlightSet = new HashSet<DateOnly>(highlightedDates);

            Console.SetCursorPosition(0, line);
            const string dayHeader = "Вс Пн Вт Ср Чт Пт Сб";

            for (int row = 0; row < 3; row++) // 3 ряда по 4 месяца
            {
                // === Имена месяцев ===
                for (int col = 0; col < 4; col++)
                {
                    int month = row * 4 + col + 1;
                    if (month > 12)
                    {
                        Console.Write("                      "); // 22 пробела
                        continue;
                    }
                    string name = new DateTime(year, month, 1).ToString("MMMM");
                    Console.Write($"{name,-22} ");
                }
                Console.WriteLine();

                // === Заголовки дней недели ===
                for (int col = 0; col < 4; col++)
                {
                    int month = row * 4 + col + 1;
                    if (month > 12)
                    {
                        Console.Write("                      ");
                        continue;
                    }
                    Console.Write($"{dayHeader} ");
                }
                Console.WriteLine();

                // === Подготовка месяцев ===
                int[] currentDay = new int[4];
                int[] daysInMonth = new int[4];
                int[] firstDayOffset = new int[4]; // смещение от воскресенья (DayOfWeek = 0)

                for (int col = 0; col < 4; col++)
                {
                    int month = row * 4 + col + 1;
                    if (month <= 12)
                    {
                        daysInMonth[col] = DateTime.DaysInMonth(year, month);
                        var firstDay = new DateTime(year, month, 1);
                        firstDayOffset[col] = (int)firstDay.DayOfWeek; // 0=Вс, 1=Пн, ..., 6=Сб
                        currentDay[col] = 1;
                    }
                    else
                    {
                        daysInMonth[col] = 0;
                        currentDay[col] = 0;
                        firstDayOffset[col] = 0;
                    }
                }

                // === Печать недель ===
                bool hasMore;
                do
                {
                    hasMore = false;
                    for (int col = 0; col < 4; col++)
                    {
                        if (daysInMonth[col] == 0)
                        {
                            Console.Write("                      ");
                            continue;
                        }

                        // Печать 7 дней недели: Вс → Сб
                        for (int dow = 0; dow < 7; dow++)
                        {
                            if (currentDay[col] == 1 && dow < firstDayOffset[col])
                            {
                                // Пропуск до первого дня
                                Console.Write("   ");
                            }
                            else if (currentDay[col] <= daysInMonth[col])
                            {
                                var date = new DateOnly(year, row * 4 + col + 1, currentDay[col]);
                                string dayStr = currentDay[col].ToString().PadLeft(2);

                                if (highlightSet.Contains(date))
                                {
                                    var original = Console.ForegroundColor;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(dayStr + " ");
                                    Console.ForegroundColor = original;
                                }
                                else
                                {
                                    Console.Write(dayStr + " ");
                                }

                                currentDay[col]++;
                            }
                            else
                            {
                                Console.Write("   ");
                            }
                        }
                        Console.Write(" "); // разделитель между месяцами
                    }
                    Console.WriteLine();

                    // Проверяем, остались ли ещё дни
                    for (int col = 0; col < 4; col++)
                    {
                        if (currentDay[col] <= daysInMonth[col])
                            hasMore = true;
                    }
                } while (hasMore);

                Console.WriteLine(); // пустая строка между блоками
            }
        }
    }

    public class LocationViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class InputViewModel
    {
        public LocationViewModel Location { get; set; }
        public ReportType ReportType { get; set; }
        public object ReportConfigJson { get; set; }
    }

    public class HourConfig
    {
        public DateTime? DateTime { get; set; }
    }

    public class DayConfig
    {
        public DateOnly? DateOnly { get; set; }
    }

    public class WeekConfig
    {
        public DateOnly? DateOnlyStart { get; set; }
        public DateOnly? DateOnlyEnd { get; set; }
    }

    public class MonthConfig
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }

    public class YearConfig
    {
        public int? Year { get; set; }
    }

    public enum ReportType
    {
        Hour = 0,
        Day = 1,
        Week = 3,
        Month = 4,
        Year = 5
    }

    public class OutputWeatherReport
    {
        /// <summary>
        /// Температура средняя
        /// </summary>
        [NotMapped]
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureAverage
        {
            get
            {
                return (TemperatureMin + TemperatureMax) / 2.0;
            }
        }

        /// <summary>
        /// Температура максимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Температура минимальная (средняя)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Температура максимальная (максимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMinTotal { get; set; }

        /// <summary>
        /// Температура минимальная (минимальная)
        /// </summary>
        [ReportValue(ColorTable = "Temperature")]
        public double TemperatureMaxTotal { get; set; }

        /// <summary>
        /// Солнечное излучение
        /// </summary>
        [ReportValue(ColorTable = "Radiation")]
        public double SolarRadiationInfluence { get; set; }

        /// <summary>
        /// Осадки
        /// </summary>
        [ReportValue(ColorTable = "Fallout")]
        public double Fallout { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        [ReportValue(ColorTable = "Humidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Направление ветра
        /// </summary>
        public double WindDerection { get; set; }
    }
}
