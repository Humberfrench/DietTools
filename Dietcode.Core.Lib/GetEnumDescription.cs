using System.ComponentModel;
using System.Reflection;

namespace Dietcode.Core.Lib
{
    public static class GetEnumDescription
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var enumType = value.GetType();
            var member = enumType.GetMember(value.ToString());

            if (member.Length == 0)
                return value.ToString();

            var attribute = member[0]
                .GetCustomAttribute<DescriptionAttribute>(inherit: false);

            return attribute?.Description ?? value.ToString();
        }
    }
}
