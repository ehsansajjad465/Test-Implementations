using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace TestConsole.Zeeshan
{
    public static class EnumExtensions
    {

        public static T TryFromEnumStringValue<T>(this string description) where T : struct
        {
            try
            {
                return (T)typeof(T)
                    .GetRuntimeFields()
                    .First(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .Cast<DescriptionAttribute>()
                                 .Any(a => description.ToUpper().Contains(a.Description))
                    )
                    .GetValue(null);
            }
            catch (Exception ex)
            {
            }

            return default(T);
        }
    }
}