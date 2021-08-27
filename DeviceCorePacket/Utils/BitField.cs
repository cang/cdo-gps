using System;

namespace CorePacket.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BitField : Attribute
    {
        /// <summary>
        ///     Gets or sets the index.
        /// </summary>
        public int Index { get; set; }
    }
}