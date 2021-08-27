using System;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using Datacenter.Model.Utils;
using System.Reflection;
using System.IO;

namespace Datacenter.Model.Log
{
    public class DeviceTraceLogBase : IndexLogDevice, IDbLog
    {
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Auto)]
        public virtual long Id { get; set; }
        [ComponentColumn(Index = 0)]
        public virtual GpsLocation BeginLocation { set; get; }
        [ComponentColumn(Index = 1)]
        public virtual GpsLocation EndLocation { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime BeginTime { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual DateTime EndTime { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual TraceType Type { set; get; }
        [BasicColumn(IsIndex = true)]
        public virtual long DriverId { set; get; }
        [BasicColumn]
        public virtual string Note { set; get; }

        // các trường check ( ko lưu database)

        public virtual DateTime DriverTime { get; set; }

        public virtual void FixNullObject()
        {
            BeginTime = BeginTime.Fix();
            EndTime = EndTime.Fix();

        }
        [BasicColumn]
        public virtual int DbId { get; set; }

        //todo: chưa cài đặt
        [BasicColumn]
        public virtual long Distance { get; set; }

        public virtual T CopyTo<T>()
        {
            var ret = Activator.CreateInstance<T>();

            //var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            //foreach (var field in fields)
            //{
            //    var value = field.GetValue(this);
            //    field.SetValue(ret, value);
            //}

            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in props)
            {
                var value = field.GetValue(this);
                field.SetValue(ret, value);
            }

            return ret;
        }

        public override void Deserializer(BinaryReader stream, int version)
        {
            base.Deserializer(stream, version);

            Id = stream.ReadInt64();

            if (stream.ReadBoolean())
            {
                BeginLocation = new GpsLocation();
                BeginLocation.Deserializer(stream, version);
            }

            if (stream.ReadBoolean())
            {
                EndLocation = new GpsLocation();
                EndLocation.Deserializer(stream, version);
            }

            BeginTime = DateTime.FromBinary(stream.ReadInt64());
            EndTime = DateTime.FromBinary(stream.ReadInt64());

            Type = (TraceType)stream.ReadByte();
            DriverId = stream.ReadInt64();

            if(stream.ReadBoolean())
                Note = stream.ReadString();

            DriverTime = DateTime.FromBinary(stream.ReadInt64());

            Distance = stream.ReadInt64();
        }

        public override void Serializer(BinaryWriter stream)
        {
            base.Serializer(stream);
            stream.Write(Id);

            if (BeginLocation != null)
            {
                stream.Write(true);
                BeginLocation.Serializer(stream);
            }
            else
                stream.Write(false);

            if (EndLocation != null)
            {
                stream.Write(true);
                EndLocation.Serializer(stream);
            }
            else
                stream.Write(false);

            stream.Write(BeginTime.ToBinary());
            stream.Write(EndTime.ToBinary());

            stream.Write((byte)Type);
            stream.Write(DriverId);

            stream.Write(Note!=null);
            if(Note != null) stream.Write(Note);

            stream.Write(DriverTime.ToBinary());

            stream.Write(Distance);
        }


    }
}