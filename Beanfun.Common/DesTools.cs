using System.Security.Cryptography;
using System.Text;

namespace Beanfun.Common
{
    public static class DesTools
    {
        /// <summary>
        /// 解谜数据
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string Decrypt(string value, string key)
        {
            string str = string.Empty;
            try
            {
                byte[] inputByteArray = Convert.FromHexString(value);
                using DESCryptoServiceProvider des = new();

                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = Encoding.ASCII.GetBytes(key);

                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.None;

                using MemoryStream ms = new();

                using CryptoStream cs = new(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                str = Encoding.Default.GetString(ms.ToArray());

                str = str.Replace("\0", "").Trim();
            }
            catch (Exception)
            {
            }

            return str;
        }
    }
}