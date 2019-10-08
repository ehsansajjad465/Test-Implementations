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
            }
            catch (Exception ex)
            {
            }

            return default(T);
        }

        public static LabelFormMap TryConvertToRecieptMetaData(this string description, RecieptMetaData recieptMetaData)
        {
            try
            {
                return recieptMetaData.Attributes
                                 .Where(a => description.ToUpper().Contains(a.RecieptLabel.RowLabel))
                                 .FirstOrDefault();

            }
            catch (Exception ex)
            {
            }

            return default(LabelFormMap);
        }


        public static List<String> ToList<TEnum>(this TEnum obj)
            where TEnum : struct, IComparable, IFormattable, IConvertible // correct one
        {

            return Enum.GetValues(typeof(TEnum)).OfType<Enum>()
                .Select(x => x.Description()).ToList();
        }

        public static string Description(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

    }
}
