using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CodeSnippetLibrary.Net
{
    public static class XmlSerializerExecutor
    {
        /// <summary>
        /// 将对象序列化为Xml流
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(ms, obj);
                ms.Position = 0;

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 将Xml流反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public static T Deserialize<T>(Stream stream, Encoding contentEncoding = null)
        {
            if (contentEncoding == null)
            {
                contentEncoding = Encoding.Default;
            }
            using (StreamReader reader = new StreamReader(stream, contentEncoding))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
                catch
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 将Xml字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="contentEncoding"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlString, Encoding contentEncoding = null)
        {
            if (contentEncoding == null)
            {
                contentEncoding = Encoding.Default;
            }
            using (MemoryStream ms = new MemoryStream(contentEncoding.GetBytes(xmlString)))
            {
                return Deserialize<T>(ms);
            }
        }
    }
}
