using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Weather;

public partial class WeatherStationAgroClimaticZone
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid WeatherStationId { get; set; }

    public Guid AgroClimaticZonesId { get; set; }
}
