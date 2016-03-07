using System;
using System.Data.SqlClient;

namespace CodeSnippetLibrary.Extension
{
    public static class SqlDataReaderExtension
    {
        public static object SetDataValue(object value)
        {
            return value ?? DBNull.Value;
        }

        public static int ReadInt(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
        }

        public static long ReadLong(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt64(reader[name]);
        }

        public static Guid ReadGuid(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? Guid.Empty : (Guid)reader[name];
        }

        public static bool ReadBoolean(this SqlDataReader reader, string name)
        {
            return !reader.IsDBNull(reader.GetOrdinal(name)) && (bool)reader[name];
        }

        public static float ReadFloat(this SqlDataReader reader, string name)
        {
            float number;
            return reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : float.TryParse(reader[name].ToString(), out number) ? number : 0;
        }

        public static double ReadDouble(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : (double)reader[name];
        }

        public static DateTime ReadDateTime(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : (DateTime)reader[name];
        }

        public static DateTime ReadDateTimeUtc(this SqlDataReader reader, string name)
        {
            var dt = reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : (DateTime)reader[name];
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
        }

        public static string ReadString(this SqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : reader[name].ToString();
        }

        public static TEnum ReadEnum<TEnum>(this SqlDataReader reader, string name) where TEnum : struct
        {
            var dbValue = 0;
            if (!reader.IsDBNull(reader.GetOrdinal(name)))
            {
                dbValue = Convert.ToInt32(reader[name]);
            }

            TEnum value = (TEnum)Convert.ChangeType(dbValue, Enum.GetUnderlyingType(typeof(TEnum)));
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new InvalidCastException(string.Format("Spcified value {0} is not defined in enum {1}.", dbValue, typeof(TEnum).Name));
            }

            return value;
        }

        public static TEnum ReadEnum<TEnum>(this SqlDataReader reader, string name, object defaultValue) where TEnum : struct
        {
            if (reader.IsDBNull(reader.GetOrdinal(name)))
            {
                return (TEnum)defaultValue;
            }

            int dbValue = (int)reader[name];

            TEnum value = (TEnum)Convert.ChangeType(dbValue, Enum.GetUnderlyingType(typeof(TEnum)));
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                throw new InvalidCastException(string.Format("Spcified value {0} is not defined in enum {1}.", dbValue, typeof(TEnum).Name));
            }

            return value;
        }
    }
}
