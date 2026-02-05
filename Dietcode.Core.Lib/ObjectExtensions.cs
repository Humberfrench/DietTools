namespace Dietcode.Core.Lib
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Retorna o próprio valor se não for nulo; caso contrário, retorna um valor alternativo fornecido (para reference types).
        /// </summary>
        public static T OrThisValue<T>(this T? value, T fallback) where T : class
        {
            return value ?? fallback;
        }

        /// <summary>
        /// Retorna o próprio valor se não for nulo; caso contrário, retorna um valor alternativo fornecido (para value types).
        /// </summary>
        public static T OrThisValue<T>(this T? value, T fallback) where T : struct
        {
            return value ?? fallback;
        }

        /// <summary>
        /// Retorna a instância se não for nula; caso contrário, cria uma nova instância do tipo.
        /// </summary>
        public static T OrThisValue<T>(this T? value) where T : class, new()
        {
            return value ?? new T();
        }

        /// <summary>
        /// Retorna o valor se tiver sido atribuído; caso contrário, cria uma nova instância do tipo struct.
        /// </summary>
        public static T OrThisValue<T>(this T? value) where T : struct
        {
            return value ?? new T();
        }

        /// <summary>
        /// Retorna a lista atual se não for nula; caso contrário, retorna uma lista vazia.
        /// </summary>
        public static List<T> OrEmpty<T>(this List<T>? list)
        {
            return list ?? [];
        }

        /// <summary>
        /// Retorna o dicionário atual se não for nulo; caso contrário, retorna um dicionário vazio.
        /// </summary>
        public static Dictionary<TKey, TValue> OrEmpty<TKey, TValue>(this Dictionary<TKey, TValue>? dict)
            where TKey : notnull
        {
            return dict ?? [];
        }

        /// <summary>
        /// Retorna o array atual se não for nulo; caso contrário, retorna um array vazio.
        /// </summary>
        public static T[] OrEmpty<T>(this T[]? array)
        {
            return array ?? [];
        }

        /// <summary>
        /// Retorna o IEnumerable atual se não for nulo; caso contrário, retorna um array vazio.
        /// </summary>
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Retorna o IReadOnlyList atual se não for nulo; caso contrário, retorna um array vazio.
        /// </summary>
        public static IReadOnlyList<T> OrEmpty<T>(this IReadOnlyList<T>? source)
        {
            return source ?? Array.Empty<T>();
        }

        /// <summary>
        /// Retorna o ICollection atual se não for nulo; caso contrário, retorna um array vazio.
        /// </summary>
        public static ICollection<T> OrEmpty<T>(this ICollection<T>? source)
        {
            return source ?? new List<T>();
        }


    }
}
