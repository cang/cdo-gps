using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StarSg.Utils.Utils
{

    public class Crypto
    {
        /// <summary>
        /// Convert byte's array to hex's string
        /// </summary>
        /// <param name="hashValue">Byte's array</param>
        /// <returns>hex's string</returns>
        private static string HashToString(byte[] hashValue)
        {
            string strHex = string.Empty;

            foreach (byte b in hashValue)
            {
                strHex += string.Format("{0:X2}", b);
            }

            return strHex;
        }

        /// <summary>
        /// Encrypt input string with sh1 algorithm
        /// </summary>
        /// <param name="inputString">string to be encrypted</param>
        /// <returns>Encrypted string</returns>
        public static string GetSH1(string inputString)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();

            byte[] data = encoding.GetBytes(inputString);

            SHA1 sh1 = new SHA1CryptoServiceProvider();

            return HashToString(sh1.ComputeHash(data));
        }

    }
}
