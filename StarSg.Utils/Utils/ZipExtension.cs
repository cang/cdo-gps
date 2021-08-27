#region header
// /*********************************************************************************************/
// Project :Core
// FileName : ZipExtension.cs
// Time Create : 9:08 AM 21/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.IO.Compression;
using System.Text;

namespace StarSg.Utils.Utils
{
    public static class ZipExtension
    {
        public static Encoding ENCODE_DEFAULT = Encoding.GetEncoding("ISO-8859-1");

        /// <summary>
        /// nén 1 mảng byte
        /// </summary>
        /// <param name="data">
        /// dữ liêu cần nén
        /// </param>
        /// <returns>
        /// trả về mảng dữ liệu đã nén lại
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public static byte[] Zip(this byte[] data)
        {
            using (var mStream = new MemoryStream())
            {
                using (var zStream = new ZipOutputStream(mStream))
                {
                    zStream.PutNextEntry(new ZipEntry("text") { DateTime = DateTime.Now, Size = data.Length });
                    zStream.SetLevel(9);
                    zStream.Write(data, 0, data.Length);
                    zStream.CloseEntry();
                    // Console.WriteLine("Mã hóa : {0} byte -> {1} byte ", data.Length, result.Length);
                    // var tmp = result.UnZip();
                    return mStream.ToArray();
                }
            }
        }


        /// <summary>
        /// giải nén 1 mảng dữ liệu
        /// </summary>
        /// <param name="data">
        /// dữ liệu nén
        /// </param>
        /// <returns>
        /// dữ liệu đã giải nén
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public static byte[] UnZip(this byte[] data)
        {
            using (MemoryStream tmpStream = new MemoryStream(), mStream = new MemoryStream(data))
            {
                using (var zStream = new ZipInputStream(mStream))
                {
                    var entry = zStream.GetNextEntry();
                    int tmp = -1;
                    do
                    {
                        if (tmp > -1) tmpStream.WriteByte((byte)tmp);
                        tmp = zStream.ReadByte();
                    }
                    while (tmp > -1);
                    return tmpStream.ToArray();
                    // Console.WriteLine("Giải mã hóa : {0} byte -> {1} byte ", data.Length, result.Length);
                }
            }
        }

        public static String CompressDeflateBase64(this string str)
        {
            return Convert.ToBase64String((CompressDeflate(Encoding.UTF8.GetBytes(str))));
        }

        public static String CompressDeflate(this string str)
        {
            return ENCODE_DEFAULT.GetString(CompressDeflate(Encoding.UTF8.GetBytes(str)));
        }

        public static byte[] CompressDeflate(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                //using (var gs = new GZipStream(mso, CompressionMode.Compress))
                using (var gs = new DeflateStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }

        public static string DecompressDeflateBase64(this String str)
        {
            return Encoding.UTF8.GetString(DecompressDeflate(Convert.FromBase64String(str)));
        }

        public static string DecompressDeflate(this String str)
        {
            return Encoding.UTF8.GetString(DecompressDeflate(ENCODE_DEFAULT.GetBytes(str)));
        }

        public static byte[] DecompressDeflate(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                //using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                using (var gs = new DeflateStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return mso.ToArray();
            }
        }

    }
}