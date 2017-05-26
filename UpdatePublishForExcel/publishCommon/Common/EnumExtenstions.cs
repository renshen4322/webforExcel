using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon
{
    public static class EnumExtenstions
    {
        public static string GetDescription(this Type enumType, string item)
        {
            var fi = enumType.GetField(item);
            var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            var description = attribute == null ? item : ((DescriptionAttribute)attribute).Description;
            return description;
        }

        public static string GetDescription(this Enum enumType)
        {
            var attribute = enumType.GetType().GetField(enumType.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            var description = attribute == null ? enumType.ToString() : attribute.Description;
            return description;
        }
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return str;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}
