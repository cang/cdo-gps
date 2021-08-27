using System;
using System.Linq;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using System.IO;
using System.Collections.Generic;

namespace Datacenter.Model.Log
{
    /// <summary>
    /// lịch sử thiết bị
    /// </summary>
    [Table]
    [Serializable()]
    public class DeviceLogMoving:IndexLogDevice,IDbLog,ISerializerModal
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        [ComponentColumn]
        public virtual DeviceStatusInfo DeviceStatus { get; set; }
        [ComponentColumn]
        public virtual DriverStatusInfo DriverStatus { get; set; }
        //[BasicColumn]
        //public virtual string SpeedTrace { get; set; }
        public virtual void FixNullObject()
        {
        }

        [BasicColumn]
        public virtual int DbId { get; set; }

        public override void Deserializer(BinaryReader stream, int version)
        {
            base.Deserializer(stream, version);
            Id = stream.ReadInt64();
            if(stream.ReadBoolean())
            {
                DeviceStatus = new DeviceStatusInfo();
                DeviceStatus.Deserializer(stream, version);
            }
            if (stream.ReadBoolean())
            {
                DriverStatus = new DriverStatusInfo();
                DriverStatus.Deserializer(stream, version);
            }
        }

        public override void Serializer(BinaryWriter stream)
        {
            base.Serializer(stream);
            stream.Write(Id);

            if (DeviceStatus != null)
            {
                stream.Write(true);
                DeviceStatus.Serializer(stream);
            }
            else
                stream.Write(false);

            if (DriverStatus != null)
            {
                stream.Write(true);
                DriverStatus.Serializer(stream);
            }
            else
                stream.Write(false);
        }
    }


    public class DeviceLogCollection : ISerializerModal
    {
        public static int VERISON = 1;

        //Dữ liệu ghi xuống
        public List<DeviceLogMoving> list = new List<DeviceLogMoving>();
        public DeviceLogMoving first = new DeviceLogMoving();

        //Dữ liệu đọc lên
        public List<DeviceLog> listout = new List<DeviceLog>();
        public DeviceLog firstout = new DeviceLog();

        public DeviceLogCollection() { }
        public DeviceLogCollection(ICollection<DeviceLogMoving> data)
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
                firstout = new DeviceLog();
                firstout.CompanyId = stream.ReadInt64();
                firstout.DbId = stream.ReadInt32();
                firstout.GroupId = stream.ReadInt64();
                firstout.Indentity = Guid.Parse(stream.ReadString());
                firstout.Serial = stream.ReadInt64();
                for (int i = 0; i < len; i++)
                {
                    DeviceLog devlog = new DeviceLog();
                    devlog.Deserializer(stream, version);
                    devlog.CompanyId = firstout.CompanyId;
                    devlog.DbId = firstout.DbId;
                    devlog.GroupId = firstout.GroupId;
                    devlog.Indentity = firstout.Indentity;
                    devlog.Serial = firstout.Serial;//giá trị này cần cập nhật từ bên ngoài nếu có sử dụng, lý do : thiết bị đổi serial
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