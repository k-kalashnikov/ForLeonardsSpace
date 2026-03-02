https://api.weather.com/v3/location/point

GET https://api.weather.com/v3/location/point?
  geocode=55.7558,37.6173
  &language=en-US
  &format=json
  &apiKey=yourApiKey

  {
  "location": {
    "latitude": 55.7558,
    "longitude": 37.6173,
    "city": "Moscow",
    "country": "Russia",
    "countryCode": "RU",
    "displayName": "Moscow",
    "adminDistrict": "Moscow City",
    "adminDistrictCode": "MOW",
    "iataCode": "SVO",           // если есть, IATA-код аэропорта
    "icaoCode": "UUEE",          // если есть, ICAO-код станции/аэропорта
    "pwsId": "IMOSCOW77",        // если есть, идентификатор личной станции (pws)
    "locId": "RUXX0024",         // legacy/внутренний код IBM
    "placeId": "25d0c...f0ee4a81cd",
    "postalKey": "101000:RU",
    ...
    "type": "city" // или "pws","airport" и т.д.
  }
}


1.
Endpoint: /v3/wx/conditions/historical/dailysummary/30day

json
[
  {
    "id": "string",
    "v3-wx-conditions-historical-dailysummary-30day": {
      "dayOfWeek": [ "string", ... ],
      "iconCodeDay": [ "integer", ... ],
      "iconCodeNight": [ "integer", ... ],
      "precip24Hour": [ "number", ... ],
      "rain24Hour": [ "number", ... ],
      "snow24Hour": [ "number", ... ],
      "temperatureMax": [ "integer", ... ],
      "temperatureMin": [ "integer", ... ],
      "validTimeLocal": [ "string (ISO8601)", ... ],
      "wxPhraseLongDay": [ "string", ... ],
      "wxPhraseLongNight": [ "string", ... ]
    }
  }
]
2.
Endpoint: /v3/wx/conditions/historical/hourly/1day

json
[
  {
    "id": "string",
    "v3-wx-conditions-historical-hourly-1day": {
      "cloudCeiling": [ "integer", ... ],
      "dayOfWeek": [ "string", ... ],
      "dayOrNight": [ "string", ... ],
      "iconCode": [ "integer", ... ],
      "precip24Hour": [ "number", ... ],
      "pressureAltimeter": [ "number", ... ],
      "relativeHumidity": [ "integer", ... ],
      "snow24Hour": [ "number", ... ],
      "sunriseTimeLocal": [ "string (ISO8601)", ... ],
      "sunsetTimeLocal": [ "string (ISO8601)", ... ],
      "temperature": [ "integer", ... ],
      "uvDescription": [ "string", ... ],
      "uvIndex": [ "integer", ... ],
      "validTimeLocal": [ "string (ISO8601)", ... ],
      "windDirection": [ "integer", ... ],
      "windSpeed": [ "integer", ... ],
      "wxPhraseLong": [ "string", ... ]
    }
  }
]
3.
Endpoint: /v1/geocode/{lat}/{lon}/observations.json

json
{
  "metadata": {
    "language": "string",
    "version": "string",
    "latitude": "number",
    "longitude": "number",
    "units": "string",
    "status_code": "integer"
  },
  "observation": {
    "temp": "integer",
    "feels_like": "integer",
    "dewPt": "integer",
    "rh": "integer",
    "wspd": "integer",
    "wdir": "integer",
    "wdir_cardinal": "string",
    "gust": "integer | null",
    "pressure": "number",
    "wx_phrase": "string",
    "precip_total": "number",
    "uv_index": "integer",
    "uv_desc": "string",
    "obs_id": "string",
    "obs_name": "string",
    "max_temp": "integer",
    "min_temp": "integer",
    "icon_extd": "integer"
  }
}
4.
Endpoint: /v3/wx/forecast/daily/7day

json
{
  "calendarDayTemperatureMax": [ "integer", ... ],
  "calendarDayTemperatureMin": [ "integer", ... ],
  "dayOfWeek": [ "string", ... ],
  "narrative": [ "string", ... ],
  "sunriseTimeLocal": [ "string (ISO8601)", ... ],
  "sunsetTimeLocal": [ "string (ISO8601)", ... ],
  "validTimeLocal": [ "string (ISO8601)", ... ],
  "daypart": [
    {
      "dayOrNight": [ "string", ... ],
      "daypartName": [ "string", ... ],
      "temperature": [ "integer", ... ],
      "precipChance": [ "integer", ... ],
      "qpf": [ "number", ... ],
      "qpfSnow": [ "number", ... ],
      "windSpeed": [ "integer", ... ],
      "wxPhraseLong": [ "string", ... ]
    }
  ]
}
5.
Endpoint: /tile.weather.com/map/{layer}/{z}/{x}/{y}.png

Тип данных: PNG/JPEG-изображение, не JSON

6.
Endpoint: /v3/wx/hod/r1/direct

json
[
  {
    "requestedLatitude": "number",
    "requestedLongitude": "number",
    "latitude": "number",
    "longitude": "number",
    "gridpointId": "string",
    "validTimeUtc": "string (ISO8601)",
    "temperature": "number",
    "temperatureDewPoint": "number",
    "evapotranspiration": "number",
    "uvIndex": "integer",
    "precip1Hour": "number",
    "windSpeed": "number",
    "windDirection": "number"
    // ... другие погодные параметры, которые вы запросили через products
  }
]


    migrationBuilder.Sql(@"
        CREATE TABLE IF NOT EXISTS ""IBMWeatherData"" (
            ""Id"" uuid NOT NULL,
            ""IBMMeteoStationId"" uuid NOT NULL,
            ""ValidTimeUtc"" timestamp with time zone NOT NULL,
            ""ValidTimeLocal"" timestamp with time zone NOT NULL,
            ""Temperature"" integer NULL,
            ""FeelsLike"" integer NULL,
            ""DewPoint"" integer NULL,
            ""Humidity"" integer NULL,
            ""Pressure"" double precision NULL,
            ""WindSpeed"" integer NULL,
            ""WindDirection"" integer NULL,
            ""WindGust"" integer NULL,
            ""Precipitation"" double precision NULL,
            ""UvIndex"" integer NULL,
            ""WeatherPhrase"" text NULL,
            ""IconCode"" integer NULL,
            ""TemperatureMax"" integer NULL,
            ""TemperatureMin"" integer NULL,
            ""DayOrNight"" text NULL,
            ""PrecipChance"" integer NULL,
            ""Qpf"" double precision NULL,
            ""QpfSnow"" double precision NULL,
            ""CloudCeiling"" integer NULL,
            ""RelativeHumidity"" integer NULL,
            ""PressureAltimeter"" double precision NULL,
            ""Snow24Hour"" double precision NULL,
            ""SunriseTimeLocal"" timestamp with time zone NULL,
            ""SunsetTimeLocal"" timestamp with time zone NULL,
            ""UvDescription"" text NULL,
            ""DayOfWeek"" text NULL,
            ""IconCodeDay"" integer NULL,
            ""IconCodeNight"" integer NULL,
            ""Precip24Hour"" double precision NULL,
            ""Rain24Hour"" double precision NULL,
            ""WxPhraseLongDay"" text NULL,
            ""WxPhraseLongNight"" text NULL,
            ""WxPhraseLong"" text NULL,
            ""TemperatureDewPoint"" double precision NULL,
            ""Evapotranspiration"" double precision NULL,
            ""Precip1Hour"" double precision NULL,
            ""RequestedLatitude"" double precision NULL,
            ""RequestedLongitude"" double precision NULL,
            ""GridpointId"" text NULL,
            ""CreateAt"" timestamp with time zone NOT NULL,
            ""Status"" integer NOT NULL,
            ""LastUpdateAt"" timestamp with time zone NOT NULL,
            ""CreateUser"" uuid NOT NULL,
            ""LastUpdateUser"" uuid NOT NULL,
            ""Names"" text NOT NULL,
            CONSTRAINT ""PK_IBMWeatherData"" PRIMARY KEY (""Id"", ""ValidTimeUtc"")
        ) PARTITION BY RANGE (""ValidTimeUtc"");
    ");

    migrationBuilder.CreateIndex(
        name: "idx_ibmweather_data_valid_time_utc",
        table: "IBMWeatherData",
        column: "ValidTimeUtc");
}


1. Переделать запрос пораметров от пользователя в командах CLI.DevOps
    1. [text](Mono/Masofa.Cli.DevopsUtil/Commands/IBMWeather/LoadStationsCommand.cs) 
    1. [text](Mono/Masofa.Cli.DevopsUtil/Commands/IBMWeather/LoadHistoricalDataCommand.cs) 
    1. [text](Mono/Masofa.Cli.DevopsUtil/Commands/IBMWeather/LoadCurrentDataCommand.cs) 
    1. [text](Mono/Masofa.Cli.DevopsUtil/Commands/IBMWeather/LoadForecastDataCommand.cs) 
    1. [text](Mono/Masofa.Cli.DevopsUtil/Commands/IBMWeather/LoadAlertsCommand.cs)

1. Проверить работу команд 
    1. [text](Mono/Masofa.BusinessLogic/IBMWeather/LoadStationsCommand.cs) 
        1. Загрузка по двум типам параметров
    1. [text](Mono/Masofa.BusinessLogic/IBMWeather/LoadHistoricalDataCommand.cs) 
        1. Сохранение правильных параметров
        1. Сохранение партиций через Raw
    1. [text](Mono/Masofa.BusinessLogic/IBMWeather/LoadCurrentDataCommand.cs) 
        1. Сохранение правильных параметров
        1. Сохранение партиций через Raw
    1. [text](Mono/Masofa.BusinessLogic/IBMWeather/LoadForecastDataCommand.cs) 
        1. Сохранение правильных параметров
        1. Сохранение партиций через Raw

1. Переделать миграцию и скрипт