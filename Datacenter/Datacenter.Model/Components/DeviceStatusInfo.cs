#region header

// /*********************************************************************************************/
// Project :Datacenter.Model
// FileName : DeviceStatusInfo.cs
// Time Create : 9:28 AM 22/06/2016
// Author:  Cang Do (dovancang@gmail.com)
// /********************************************************************************************/

#endregion

using System;
using System.IO;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Components
{
    [Serializable()]
    public class DeviceStatusInfo : ISerializerModal
    {
        [BasicColumn(IsIndex = true)]
        public virtual DateTime ClientSend { set; get; }

        [BasicColumn(IsIndex = true)]
        public virtual DateTime ServerRecv { set; get; }

        [BasicColumn]
        public virtual bool GpsStatus { get; set; }

        [ComponentColumn]
        public virtual GpsLocation GpsInfo { get; set; }

        [BasicColumn]
        public virtual long TotalGpsDistance { get; set; }

        [BasicColumn]
        public virtual long TotalCurrentGpsDistance { get; set; }


        [BasicColumn]
        public virtual int Fuel { get; set; }

        [BasicColumn]
        public virtual short Temperature { get; set; }

        [BasicColumn]
        public virtual byte GsmSignal { get; set; }

        [BasicColumn]
        public virtual byte Power { get; set; }

        [BasicColumn]
        public virtual bool Machine { get; set; }

        [BasicColumn]
        public virtual bool AirMachine { get; set; }

        [BasicColumn]
        public virtual bool Sos { get; set; }

        [BasicColumn]
        public virtual bool UseTemperature { get; set; }

        [BasicColumn]
        public virtual bool UseFuel { get; set; }

        [BasicColumn]
        public virtual bool UseRfid { get; set; }

        [BasicColumn]
        public virtual bool Door { get; set; }

        [BasicColumn]
        public virtual string SpeedTrace { get; set; }

        [BasicColumn]
        public virtual double Angle { get; set; }

        [BasicColumn]
        public virtual int Speed { get; set; }

        

        #region Implementation of IEntity

        /// <summary>
        ///     sửa các giá trị ko được phép null trước khi insert vào cơ sở dữ liệu
        /// </summary>
        public virtual void FixNullObject()
        {
            ClientSend = ClientSend.Fix();
            ServerRecv = ServerRecv.Fix();
        }

        #endregion

        public void Deserializer(BinaryReader stream, int version)
        {
            ClientSend = DateTime.FromBinary(stream.ReadInt64());
            ServerRecv = DateTime.FromBinary(stream.ReadInt64());
            GpsStatus = stream.ReadBoolean();
            TotalGpsDistance = stream.ReadInt64();
            TotalCurrentGpsDistance = stream.ReadInt64();
            Fuel = stream.ReadInt32();
            Temperature = stream.ReadInt16();
            GsmSignal = stream.ReadByte();
            Power = stream.ReadByte();
            Machine = stream.ReadBoolean();
            AirMachine = stream.ReadBoolean();
            Sos = stream.ReadBoolean();
            UseTemperature = stream.ReadBoolean();
            UseFuel = stream.ReadBoolean();
            UseRfid = stream.ReadBoolean();
            Door = stream.ReadBoolean();
            SpeedTrace = stream.ReadString();
            Angle = stream.ReadDouble();
            Speed = stream.ReadInt32();

            if(stream.ReadBoolean())
            {
                GpsInfo = new GpsLocation();
                GpsInfo.Deserializer(stream,version);
            }

            //Fix lỗi thời gian client gửi sai 
            if ((ClientSend - DateTime.Now).TotalHours > 24)
                ClientSend = ServerRecv;
        }

        public void Serializer(BinaryWriter stream)
        {
            stream.Write(ClientSend.ToBinary());
            stream.Write(ServerRecv.ToBinary());
            stream.Write(GpsStatus);
            stream.Write(TotalGpsDistance);
            stream.Write(TotalCurrentGpsDistance);
            stream.Write(Fuel);
            stream.Write(Temperature);
            stream.Write(GsmSignal);
            stream.Write(Power);
            stream.Write(Machine);
            stream.Write(AirMachine);
            stream.Write(Sos);
            stream.Write(UseTemperature);
            stream.Write(UseFuel);
            stream.Write(UseRfid);
            stream.Write(Door);
            stream.Write(SpeedTrace??String.Empty);
            stream.Write(Angle);
            stream.Write(Speed);


            if (GpsInfo != null)
            {
                stream.Write(true);
                GpsInfo.Serializer(stream);
            }
            else
                stream.Write(false);
        }

    }
}