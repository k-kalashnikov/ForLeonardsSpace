using System;
using System.Collections.Generic;
using NodaTime;

namespace Masofa.Depricated.DataAccess.DepricatedWeatherServerOne.Models;

public partial class WeatherDatesCompleted
{
    public LocalDateTime? Date { get; set; }

    public LocalDateTime? FinishedAt { get; set; }
}
