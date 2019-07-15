using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public static class EnumExtension
    {
        public static string GetValue(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static T TryFromEnumStringValue<T>(this string description) where T : struct
        {
            try
            {
                return (T)typeof(T)
                    .GetFields()
                    .First(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                 .Cast<DescriptionAttribute>()
                                 .Any(a => description.ToUpper().Contains(a.Description))
                    )
                    .GetValue(null);
            }catch(Exception ex)
            {
            }

            return default(T);
        }
    }
}
