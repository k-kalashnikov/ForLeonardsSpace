using Masofa.Common.Models.SystemCrical;

namespace Masofa.Common.Helper
{
    public static class ScheduleOptionsToCronHelper
    {
        public static string ToCronExpression(ScheduleTaskOptions options)
        {
            return options.Type switch
            {
                ScheduleType.Interval => ConvertInterval(options),
                ScheduleType.Daily => ConvertDaily(options),
                ScheduleType.Weekly => ConvertWeekly(options),
                ScheduleType.Monthly => ConvertMonthly(options),
                ScheduleType.Yearly => ConvertYearly(options),
                ScheduleType.Once => ConvertOnce(options),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string ConvertInterval(ScheduleTaskOptions options)
        {
            var frequency = Math.Max(1, options.Frequency);

            return options.Interval switch
            {
                ScheduleInterval.Seconds => $"0/{frequency} * * * * ?",
                ScheduleInterval.Minutes => $"0 0/{frequency} * * * ?",
                ScheduleInterval.Hours => $"0 0 0/{frequency} * * ?",
                ScheduleInterval.Days => $"0 0 0 1/{frequency} * ?",
                _ => "0 0 0 * * ?"
            };
        }

        private static string ConvertDaily(ScheduleTaskOptions options)
        {
            var time = options.TimeOfDay ?? new TimeSpan(0, 0, 0);
            return $"{time.Seconds} {time.Minutes} {time.Hours} * * ?";
        }

        private static string ConvertWeekly(ScheduleTaskOptions options)
        {
            var time = options.TimeOfDay ?? new TimeSpan(0, 0, 0);
            var days = string.Join(",", options.DaysOfWeek.Select(d => ((int)d).ToString()));

            if (string.IsNullOrEmpty(days))
                days = "1"; // По умолчанию - понедельник

            return $"{time.Seconds} {time.Minutes} {time.Hours} ? * {days}";
        }

        private static string ConvertMonthly(ScheduleTaskOptions options)
        {
            var time = options.TimeOfDay ?? new TimeSpan(0, 0, 0);
            var days = string.Join(",", options.DaysOfMonth.Distinct().Where(d => d >= 1 && d <= 31));

            if (string.IsNullOrEmpty(days))
                days = "1"; // По умолчанию - 1 число

            return $"{time.Seconds} {time.Minutes} {time.Hours} {days} * ?";
        }

        private static string ConvertYearly(ScheduleTaskOptions options)
        {
            var time = options.TimeOfDay ?? new TimeSpan(0, 0, 0);
            var months = string.Join(",", options.Months.Distinct().Where(m => m >= 1 && m <= 12));
            var day = options.DaysOfMonth.FirstOrDefault(d => d >= 1 && d <= 31) == 0
                ? 1
                : options.DaysOfMonth.First(d => d >= 1 && d <= 31);

            if (string.IsNullOrEmpty(months))
                months = "1"; // По умолчанию - январь

            return $"{time.Seconds} {time.Minutes} {time.Hours} {day} {months} ?";
        }

        private static string ConvertOnce(ScheduleTaskOptions options)
        {
            var time = options.TimeOfDay ?? new TimeSpan(0, 0, 0);
            return $"{time.Seconds} {time.Minutes} {time.Hours} * * ?";
        }
    }
}
