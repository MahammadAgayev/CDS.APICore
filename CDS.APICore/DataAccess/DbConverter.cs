using System;
using System.Data;

namespace CDS.APICore.DataAccess
{
    public static class DbConverter
    {
        public static T Convert<T>(object o, Func<object, T> converter)
        {
            return o == DBNull.Value ? default : converter(o);
        }

        public static T Convert<T>(DataRow row, string column, Func<object, T> converter)
        {
            return Convert(row[column], converter);
        }

        public static string ToString(DataRow row, string column)
        {
            return Convert(row, column, System.Convert.ToString);
        }

        public static int ToInt32(DataRow row, string column)
        {
            return Convert(row, column, System.Convert.ToInt32);
        }

        public static decimal ToDecimal(DataRow row, string column)
        {
            return Convert(row, column, System.Convert.ToDecimal);
        }

        public static DateTime ToDateTime(DataRow row, string column)
        {
            return Convert(row, column, System.Convert.ToDateTime);
        }
    }
}
