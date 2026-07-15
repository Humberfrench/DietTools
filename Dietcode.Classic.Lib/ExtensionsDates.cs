namespace Dietcode.Classic.Lib
{
    public static partial class Extensions
    {
        public static string ToDateFormatted(this DateTime dateValue)
            => dateValue.ToString("dd/MM/yyyy");

        public static string ToTimeFormatted(this DateTime dateValue)
            => dateValue.ToString("HH:mm");

        public static string ToDateTimeFormatted(this DateTime dateValue)
            => dateValue.ToString("dd/MM/yyyy HH:mm");

        public static string ToDateTimeWithSecondsFormatted(this DateTime dateValue)
            => dateValue.ToString("dd/MM/yyyy HH:mm:ss");

        public static string ToDateFormatted(this DateTime? dateValue)
            => dateValue?.ToString("dd/MM/yyyy") ?? string.Empty;

        public static string ToTimeFormatted(this DateTime? dateValue)
            => dateValue?.ToString("HH:mm") ?? string.Empty;

        public static string ToDateTimeFormatted(this DateTime? dateValue)
            => dateValue?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;

        public static string ToDateTimeWithSecondsFormatted(this DateTime? dateValue)
            => dateValue?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty;

        public static string ToDateTimeFormatted(this DateTime dateValue, string separador)
            => dateValue.ToString($"dd/MM/yyyy{separador}HH:mm");

        public static string ToDateTimeWithSecondsFormatted(this DateTime dateValue, string separador)
            => dateValue.ToString($"dd/MM/yyyy{separador}HH:mm:ss");

        public static string ToDateTimeFormatted(this DateTime? dateValue, string separador)
            => dateValue?.ToString($"dd/MM/yyyy{separador}HH:mm") ?? string.Empty;

        public static string ToDateTimeWithSecondsFormatted(this DateTime? dateValue, string separador)
            => dateValue?.ToString($"dd/MM/yyyy{separador}HH:mm:ss") ?? string.Empty;

        public static DateTime ToDateTime(this string dataTexto)
        {
            if (DateTime.TryParse(dataTexto, out var data))
                return data;

            return DateTime.MinValue;
        }

        public static bool IsDiaNaoUtil(this DateTime date)
            => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || date.IsFeriadoFixo();

        public static bool IsDiaUtil(this DateTime date)
            => !date.IsDiaNaoUtil();

        public static bool IsFeriadoFixo(this DateTime date)
        {
            return (date.Month == 1 && date.Day == 1) ||
                   (date.Month == 4 && date.Day == 21) ||
                   (date.Month == 5 && date.Day == 1) ||
                   (date.Month == 9 && date.Day == 7) ||
                   (date.Month == 10 && date.Day == 12) ||
                   (date.Month == 11 && date.Day == 2) ||
                   (date.Month == 11 && date.Day == 15) ||
                   (date.Month == 12 && date.Day == 25);
        }

        public static DateTime ProximoDiaUtil(this DateTime date)
        {
            var next = date;

            while (next.IsDiaNaoUtil())
                next = next.AddDays(1);

            return next;
        }
    }
}
