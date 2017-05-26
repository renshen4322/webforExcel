using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon
{
    public static partial class ControllerExtension
    {
        /// <summary>
        /// 数据转换
        /// </summary>
        public static Decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            decimal result;
            if (Decimal.TryParse(value, out result)) return result;
            return 0;
        }

        /// <summary>
        /// 数据转换ToInt32
        /// </summary>
        public static int ToInt32(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            Int32 result;
            if (Int32.TryParse(value, out result)) return result;
            return 0;
        }

        /// <summary>
        /// 数据转换ToDateTime
        /// </summary>
        public static DateTime? ToDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            DateTime result;
            if (DateTime.TryParse(value, out result)) return result;
            return null;
        }

        /// <summary>
        /// 数据转换ToDouble
        /// </summary>
        public static Double ToDouble(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            Double result;
            if (Double.TryParse(value, out result)) return result;
            return 0;
        }
        /// <summary>
        /// 数据转换ToBoolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            bool result;
            if (Boolean.TryParse(value, out result)) return result;
            return false;
        }
    }
}
