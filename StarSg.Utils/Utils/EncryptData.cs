//using System;

//namespace Core.Utils
//{
//    public static class EncryptData
//    {
//        private const int PCcitt = 0x1021;

//        private static readonly ushort[] TableChecksum;

//        static EncryptData()
//        {
//            TableChecksum = new ushort[256];

//            // generate Table 
//            for (var i = 0; i < 256; i++)
//            {
//                var crc1 = 0;
//                var c = i << 8;
//                for (var j = 0; j < 8; j++)
//                {
//                    if (((crc1 ^ c) & 0x8000) != 0)
//                        crc1 = (ushort) ((crc1 << 1) ^ PCcitt);
//                    else
//                        crc1 = (ushort) (crc1 << 1);
//                    c = (ushort) (c << 1);
//                }
//                TableChecksum[i] = (ushort) crc1;
//            }
//        }

//        /// <summary>
//        ///     mã hóa 1 chuỗi thành mã 64
//        /// </summary>
//        /// <param name="data">
//        /// </param>
//        /// <returns>
//        ///     The <see cref="string" />.
//        /// </returns>
//        public static string Encrypt64Code(this string data)
//        {
//            var result = string.Empty;
//            var tp = ulong.Parse(data);
//            if (tp == 0)
//            {
//                result += Convert.ToChar(64);
//            }
//            else
//            {
//                while (tp > 0)
//                {
//                    result += Convert.ToChar(tp%63 + 64);
//                    tp /= 63; // 64;
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        ///     giải mã 1 chuỗi mã 64
//        /// </summary>
//        /// <param name="s">
//        /// </param>
//        /// <returns>
//        ///     The <see cref="ulong" />.
//        /// </returns>
//        public static ulong DeEncypt64Code(this string s)
//        {
//            var i = 0;
//            ulong t = 0;
//            s = s.Trim();
//            while (i < s.Length)
//            {
//                t += (ulong) ((Convert.ToInt32(s[i]) - 64)*Math.Pow(63, i));
//                i++;
//            }

//            return t;
//        }

//        private static ushort GetCrcCheck(ushort crc, char c)
//        {
//            var tmp = (ushort) ((crc >> 8) ^ (ushort) (0x00ff & c));
//            crc = (ushort) ((crc << 8) ^ TableChecksum[tmp]);
//            return crc;
//        }

//        /// <summary>
//        ///     tạo checksum từ 1 đoạn dữ liệu
//        /// </summary>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        public static string AddCheckSum(this string val)
//        {
//            var checksum = val.CheckSum();
//            return val + ";" + checksum;
//        }

//        public static string CheckSum(this string val)
//        {
//            ushort checksum = 0xffff;
//            int i;

//            for (i = 0; i < val.Length; i++)
//            {
//                checksum = GetCrcCheck(checksum, val[i]);
//            }
//            return checksum.ToString().Encrypt64Code();
//        }
//    }
//}