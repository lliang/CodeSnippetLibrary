using System.Text;
using System.Security.Cryptography;

namespace CodeSnippetLibrary.Algorithm
{
    /// <summary>
    /// 哈希算法
    /// </summary>
    public class HashAlgorithm
    {
        /// <summary>
        /// 获取Hash字符串
        /// </summary>
        /// <param name="values">字符串数组</param>
        /// <returns>String Hash字符串</returns>
        public static string GetHashString(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                return null;
            }
            var keyBulider = new StringBuilder(values.Length);
            foreach (var value in values)
            {
                keyBulider.Append(value);
            }

            var keyString = keyBulider.ToString();
            var keyByteArray = Encoding.ASCII.GetBytes(keyString);
            var keyByteHashArray = new MD5CryptoServiceProvider().ComputeHash(keyByteArray);
            return ByteArrayToString(keyByteHashArray);
        }

        /// <summary>
        /// 将字节数组转化为字符串
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>String字符串</returns>
        private static string ByteArrayToString(byte[] byteArray)
        {
            var output = new StringBuilder(byteArray.Length);
            for (int i = 0; i < byteArray.Length; i++)
            {
                output.Append(byteArray[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}
