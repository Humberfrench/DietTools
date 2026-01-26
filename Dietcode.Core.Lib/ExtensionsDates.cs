namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {

        // --------------------------------------------------
        //  DATAS E HORAS
        // --------------------------------------------------

        public static string ToDateFormatted(this DateTime dateValue)
            => dateValue.ToString("dd/MM/yyyy");

        public static string ToTimeFormatted(this DateTime dateValue)
            => dateValue.ToString("HH:mm");

        public static string ToDateTimeFormatted(this DateTime dateValue)
            => $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm}";

        public static string ToDateTimeWithSecondsFormatted(this DateTime dateValue)
            => $"{dateValue:dd/MM/yyyy} | {dateValue:HH:mm:ss}";

        public static string ToDateFormatted(this DateTime? dateValue)
            => dateValue?.ToString("dd/MM/yyyy") ?? "";

        public static string ToTimeFormatted(this DateTime? dateValue)
            => dateValue?.ToString("HH:mm") ?? "";

        public static string ToDateTimeFormatted(this DateTime? dateValue)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm}"
                : "";

        public static string ToDateTimeWithSecondsFormatted(this DateTime? dateValue)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} | {dateValue.Value:HH:mm:ss}"
                : "";

        public static string ToDateTimeFormatted(this DateTime dateValue, string separador)
            => $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm}";

        public static string ToDateTimeWithSecondsFormatted(this DateTime dateValue, string separador)
            => $"{dateValue:dd/MM/yyyy} {separador} {dateValue:HH:mm:ss}";

        public static string ToDateTimeFormatted(this DateTime? dateValue, string separador)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm}"
                : "";

        public static string ToDateTimeWithSecondsFormatted(this DateTime? dateValue, string separador)
            => dateValue.HasValue
                ? $"{dateValue.Value:dd/MM/yyyy} {separador} {dateValue.Value:HH:mm:ss}"
                : "";

        //DATE ONLY
        public static string ToDateFormatted(this DateOnly dateValue) => dateValue.ToString("dd/MM/yyyy");

        public static string ToDateFormatted(this DateOnly? dateValue) => dateValue?.ToString("dd/MM/yyyy") ?? "";

        /// <summary>
        /// Converte um DateOnly para data Juliana no formato YYYYDDD.
        /// </summary>
        public static int ToJulianDate(this DateOnly date)
        {
            return date.Year * 1000 + date.DayOfYear;
        }

        /// <summary>
        /// Converte um DateOnly para data Juliana string (YYYYDDD).
        /// </summary>
        public static string ToJulianDateString(this DateOnly date)
        {
            return $"{date.Year}{date.DayOfYear:D3}";
        }
    }
}
