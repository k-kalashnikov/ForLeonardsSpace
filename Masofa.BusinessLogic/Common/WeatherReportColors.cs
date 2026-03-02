namespace Masofa.BusinessLogic.Common
{
    public class WeatherReportColors
    {
        public static SortedDictionary<double, string>? GetColorTable(string tableName) =>
            tableName.ToLowerInvariant() switch
            {
                "temperature" => GetTemperatureColors(),
                "radiation" => GetRadiationColors(),
                "fallout" => GetFalloutColors(),
                "humidity" => GetHumidityColors(),
                _ => null
            };

        public static SortedDictionary<double, string> GetTemperatureColors() =>
            new()
            {
                {-40, "00012F"},
                {-35, "02006A"},
                {-30, "050196"},
                {-25, "0C00DA"},
                {-20, "1400FF"},
                {-15, "3032FF"},
                {-10, "2E9AFC"},
                {-5, "66B0FF"},
                {0, "94CDFF"},
                {5, "34FE35"},
                {10, "36FE2E"},
                {15, "9CFE35"},
                {20, "FEFD2F"},
                {25, "FD9A2E"},
                {30, "FF332C"},
                {35, "CE0400"},
                {40, "9A0007"},
                {45, "690001"},
                {50, "2F0400"}
            };

        public static SortedDictionary<double, string> GetRadiationColors() =>
            new()
            {
                {4000, "FFFFFF"},
                {4500, "FEFD3B"},
                {5000, "FED72F"},
                {5500, "FDB02E"},
                {6000, "FD852E"},
                {6500, "FE522D"},
                {7000, "F0251F"},
                {7500, "C30401"},
                {8000, "5C0101"}
            };

        public static SortedDictionary<double, string> GetFalloutColors() =>
            new()
            {
                {0, "FFFFFF"},
                {1, "D3E4FB"},
                {2, "96C2FE"},
                {3, "9BB1D6"},
                {4, "748EBA"},
                {5, "49688F"}
            };

        public static SortedDictionary<double, string> GetHumidityColors() =>
            new()
            {
                {0, "FFFFFF"},
                {10, "E0F7FA"},
                {20, "E0F7FA"},
                {30, "B0E1FA"},
                {40, "87CEFA"},
                {50, "65A6D5"},
                {60, "4682B4"},
                {70, "213DC1"},
                {80, "0000C8"},
                {90, "0101A9"},
                {100, "00008B"}
            };

        public static SortedDictionary<bool, string> GetFrostDangerColors() =>
            new()
            {
                {true, "2E9AFC"},
                {false, "36FE2E"}
            };
    }
}
