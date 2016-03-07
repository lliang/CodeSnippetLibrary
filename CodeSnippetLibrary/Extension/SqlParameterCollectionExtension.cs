using System;
using System.Data.SqlClient;

namespace CodeSnippetLibrary.Extension
{
    public static class SqlParameterCollectionExtension
    {
        /// <summary>
        /// 当值为null或者为各属性空值时自动转换为DBNull.Value。警告，在某些情况下，例如插入枚举值时，Int的0将会被转为DBNull。错误地使用此方法可能导致数据出错。
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddWithNullableValue(this SqlParameterCollection collection, string parameterName, object value)
        {
            if (value == null)
            {
                collection.AddWithValue(parameterName, DBNull.Value);
                return;
            }

            var type = value.GetType();
            var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            collection.AddWithValue(parameterName, value.Equals(defaultValue) ? DBNull.Value : value);
        }
    }
}
