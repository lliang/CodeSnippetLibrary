using System;

namespace CodeSnippetLibrary.Extension
{
    public static class TypeExtension
    {
        /// <summary>
        /// 判断当前类型是否实现了指定接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasInterface<T>(this Type type)
        {
            return type.GetInterface(typeof(T).FullName) != null;
        }

        /// <summary>
        /// 尝试从当前类型中读出作用在Class级别的指定特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            object[] attrs = type.GetCustomAttributes(typeof(T), false);
            T targetAttribute = null;

            foreach (var attr in attrs)
            {
                if (attr is T)
                {
                    targetAttribute = attr as T;
                    break;
                }
            }

            return targetAttribute;
        }

        /// <summary>
        /// 尝试从当前类型中读出作用在指定方法的指定特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type, string methodName) where T : Attribute
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            T targetAttribute = null;

            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!method.Name.Equals(methodName))
                {
                    continue;
                }

                var attrs = method.GetCustomAttributes(typeof(T), false);
                foreach (var attr in attrs)
                {
                    if (attr is T)
                    {
                        targetAttribute = attr as T;
                        break;
                    }
                }

                if (targetAttribute != null)
                {
                    break;
                }
            }

            return targetAttribute;
        }
    }
}
