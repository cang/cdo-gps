﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using NodeServerPlatform.Core;
using NodeServerPlatform.Core.Utils;

namespace Core.Models
{
    [Export(typeof (NodeSharePacketModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [NodeShareOpCode(104)]
    public class P104RemoveSerial : NodeSharePacketModel
    {
        public P104RemoveSerial(byte[] data) : base(data)
        {
        }

        public P104RemoveSerial()
        {
        }

        public List<long> SerialList { get; set; } = new List<long>();

        public override bool Deserializer()
        {
            //2 byte đầu định nghĩa chiều dài mãng serial, mỗi serial dài 32 byte
            int lenght = ReadInt16();
            for (var i = 0; i < lenght; i++)
            {
                SerialList.Add(ReadInt64());
            }
            return true;
        }

        public override byte[] Serializer()
        {
            WriteInt16(SerialList.Count);
            foreach (var serial in SerialList)
            {
                WriteInt64(serial);
            }
            return base.Serializer();
        }
    }
}