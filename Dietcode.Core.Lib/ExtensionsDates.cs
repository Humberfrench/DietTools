using System.Globalization;

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

        public static DateTime ToDateTime(this string dataTexto)
        {
            return DateTime.ParseExact(
                dataTexto,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
        }

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

        //DIAS UTEIS
        #region Final De Semana e Feriado
        private static readonly HashSet<(int Month, int Day)> FeriadosFixos =
        [
            (1, 1),   // Confraternização Universal
            (4, 21),  // Tiradentes
            (5, 1),   // Dia do Trabalho
            (9, 7),   // Independência
            (10, 12), // Nossa Sra Aparecida
            (11, 2),  // Finados
            (11, 15), // Proclamação da República
            (12, 25), // Natal
        ];

        public static bool IsDiaNaoUtil(this DateTime date)
            => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
               || IsFeriadoFixo(date);

        private static bool IsFeriadoFixo(DateTime date)
            => FeriadosFixos.Contains((date.Month, date.Day));

        public static bool IsDiaNaoUtil(this DateOnly date)
       => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
          || date.IsFeriadoFixo();

        public static bool IsDiaUtil(this DateOnly date)
            => !date.IsDiaNaoUtil();

        public static bool IsFeriadoFixo(this DateOnly date)
            => FeriadosFixos.Contains((date.Month, date.Day));

        public static DateTime ProximoDiaUtil(this DateTime date)
        {
            // se você quiser sempre retornar só a data (00:00:00), use: var d = date.Date;
            var d = date;

            do
            {
                d = d.AddDays(1);
            }
            while (d.IsDiaNaoUtil());

            return d;
        }

        public static DateOnly ProximoDiaUtil(this DateOnly date)
        {
            var d = date;

            do
            {
                d = d.AddDays(1);
            }
            while (d.IsDiaNaoUtil());

            return d;
        }

        #endregion

    }
}
