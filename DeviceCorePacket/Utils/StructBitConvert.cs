using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CorePacket.Utils
{
    public static class StructBitConvert
    {
        /// <summary>
        ///     The to bit.
        /// </summary>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <returns>
        /// </returns>
        private static IDictionary<int, bool> ToBit(int val)
        {
            var result = new Dictionary<int, bool>();
            var tmp = val;
            var index = 0;
            do
            {
                result.Add(index++, tmp%2 == 1);
                tmp = tmp/2;
            } while (tmp > 0);

            return result;
        }

        /// <summary>
        ///     The to init.
        /// </summary>
        /// <param name="list">
        ///     The list.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        private static int ToInit(IDictionary<int, bool> list)
        {
            return list.Sum(m => (int) Math.Pow(2, m.Key)*(m.Value ? 1 : 0));
        }

        /// <summary>
        ///     The to struct.
        /// </summary>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <typeparam name="T">
        ///     class convert
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        ///     class convert
        /// </returns>
        public static T ToStruct<T>(int val) where T : class
        {
            var type = typeof (T);

            var properties = type.GetProperties();
            var listbool = ToBit(val);
            var result = Activator.CreateInstance<T>();
            foreach (var m in properties)
            {
                var attrs = m.GetCustomAttribute<BitField>();
                if (attrs != null && listbool.ContainsKey(attrs.Index))
                {
                    m.SetValue(result, listbool[attrs.Index]);
                }
            }

            return result;
        }

        /// <summary>
        ///     The to int.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int ToInt<T>(T obj) where T : class
        {
            var listbool = new Dictionary<int, bool>();

            var type = typeof (T);

            var properties = type.GetProperties();

            foreach (var m in properties)
            {
                var att = m.GetCustomAttribute<BitField>();
                if (att != null)
                {
                    if (!listbool.ContainsKey(att.Index))
                    {
                        listbool.Add(att.Index, (bool) m.GetValue(obj));
                    }
                }
            }

            return ToInit(listbool.Reverse().ToDictionary(m => m.Key, m => m.Value));
        }
    }
}