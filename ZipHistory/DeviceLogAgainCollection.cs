using Datacenter.Model;
using Datacenter.Model.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZipHistory
{

    public class DeviceLogAgainCollection : ISerializerModal
    {
        public static int VERISON = 1;

        //Dữ liệu ghi xuống
        public List<DeviceLogMoving> list = new List<DeviceLogMoving>();
        public DeviceLogMoving first = new DeviceLogMoving();

        //Dữ liệu đọc lên
        public List<DeviceLogMoving> listout = new List<DeviceLogMoving>();
        public DeviceLogMoving firstout = new DeviceLogMoving();

        public DeviceLogAgainCollection() { }
        public DeviceLogAgainCollection(ICollection<DeviceLogMoving> data)
        {
            if (data == null)
            {
                list = new List<DeviceLogMoving>();
                return;
            }

            list = new List<DeviceLogMoving>(data);
            if (list != null)
                first = list.FirstOrDefault();
        }

        public void Deserializer(BinaryReader stream, int version)
        {
            listout.Clear();
            int len = stream.ReadInt32();
            if (len > 0)
            {
                firstout = new DeviceLogMoving();
                firstout.CompanyId = stream.ReadInt64();
                firstout.DbId = stream.ReadInt32();
                firstout.GroupId = stream.ReadInt64();
                firstout.Indentity = Guid.Parse(stream.ReadString());
                firstout.Serial = stream.ReadInt64();
                for (int i = 0; i < len; i++)
                {
                    DeviceLogMoving devlog = new DeviceLogMoving();
                    devlog.Deserializer(stream, version);
                    devlog.CompanyId = firstout.CompanyId;
                    devlog.DbId = firstout.DbId;
                    devlog.GroupId = firstout.GroupId;
                    devlog.Indentity = firstout.Indentity;
                    devlog.Serial = firstout.Serial;
                    listout.Add(devlog);
                }
            }
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(list.Count);
            if (list.Count > 0)
            {
                stream.Write(first.CompanyId);
                stream.Write(first.DbId);
                stream.Write(first.GroupId);
                stream.Write(first.Indentity.ToString());
                stream.Write(first.Serial);
                foreach (var item in list)
                {
                    item.Serializer(stream);
                }
            }
        }


        public byte[] Serializer()
        {
            MemoryStream ms = null;
            BinaryWriter stream = null;

            try
            {
                ms = new MemoryStream();
                stream = new BinaryWriter(ms);
                stream.Write(VERISON);
                Serializer(stream);
                stream.Flush();
                return ms.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null) stream.Close();
                if (ms != null) stream.Close();
            }
        }

        public void Deserializer(byte[] data)
        {
            MemoryStream ms = null;
            BinaryReader stream = null;

            try
            {
                ms = new MemoryStream(data);
                stream = new BinaryReader(ms);
                int version = stream.ReadInt32();
                Deserializer(stream, version);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (stream != null) stream.Close();
                if (ms != null) stream.Close();
            }
        }

    }


}
