#region header

// /*********************************************************************************************/
// Project :Core
// FileName : SerializerObject.cs
// Time Create : 9:14 AM 21/04/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Core.Utils
{
    public static class SerializerObject
    {
        /// <summary>
        /// chuyển đổi 1 đối tượng thành mảng byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(this object obj)
        {
            if (obj == null)
                return null;

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// chuyển đổi 1 mảng byte thành đối tượng
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static T ByteArrayToObject<T>(this byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return  (T)binForm.Deserialize(memStream);
            }
        }


    }
}