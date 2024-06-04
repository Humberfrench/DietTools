using System.Text;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {

        public static string FromBase64ToString(this string stringBase64)
        {
            var data = Convert.FromBase64String(stringBase64);
            return Encoding.UTF8.GetString(data);
        }


    }
}
