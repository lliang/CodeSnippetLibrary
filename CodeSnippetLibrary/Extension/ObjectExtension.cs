using System;

namespace CodeSnippetLibrary.Extension
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 读取首个标记了指定特性的属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TV GetPropertyValueMarkedByAttribute<T, TV>(this object obj) where T : Attribute
        {
            object propertyValue = null;

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                bool found = false;

                var attributes = property.GetCustomAttributes(typeof(T), false);
                foreach (var attr in attributes)
                {
                    if (attr is T)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }
                if (property.CanRead)
                {
                    propertyValue = property.GetValue(obj, null);
                }

                break;
            }

            return propertyValue is TV ? (TV)propertyValue : default(TV);
        }
    }
}
