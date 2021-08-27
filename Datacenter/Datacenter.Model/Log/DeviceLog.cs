using System;
using System.Linq;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Components;
using System.IO;
using System.Collections.Generic;
using System.Data;

namespace Datacenter.Model.Log
{
    /// <summary>
    /// lịch sử thiết bị
    /// </summary>
    [Table]
    [Serializable()]
    public class DeviceLog:IndexLogDevice,IDbLog
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

        public static DataTable CreateTable()
        {
            DataTable table = new DataTable("DeviceLog");

            table.Columns.Add(new DataColumn() { ColumnName = "Id", DataType = typeof(Int64), AllowDBNull = false, AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 });
            //table.Columns.Add(new DataColumn() { ColumnName = "SpeedTrace", DataType = typeof(String), AllowDBNull = true, MaxLength=255  });
            table.Columns.Add(new DataColumn() { ColumnName = "DbId", DataType = typeof(Int32), AllowDBNull = true});
            table.Columns.Add(new DataColumn() { ColumnName = "Serial", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "Indentity", DataType = typeof(Guid), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "GroupId", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "CompanyId", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_ClientSend", DataType = typeof(DateTime), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_ServerRecv", DataType = typeof(DateTime), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_GpsStatus", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_TotalGpsDistance", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_TotalCurrentGpsDistance", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Fuel", DataType = typeof(Int32), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Temperature", DataType = typeof(Int16), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_GsmSignal", DataType = typeof(byte), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Power", DataType = typeof(byte), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Machine", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_AirMachine", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Sos", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_UseTemperature", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_UseFuel", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_UseRfid", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Door", DataType = typeof(bool), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_SpeedTrace", DataType = typeof(String), AllowDBNull = true, MaxLength = 255 });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Angle", DataType = typeof(Double), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DeviceStatus_Speed", DataType = typeof(Int32), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "GpsInfo_Lat", DataType = typeof(Single), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "GpsInfo_Lng", DataType = typeof(Single), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "GpsInfo_Address", DataType = typeof(String), AllowDBNull = true, MaxLength = 255 });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_DriverId", DataType = typeof(Int64), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_Name", DataType = typeof(String), AllowDBNull = true, MaxLength = 255 });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_Gplx", DataType = typeof(String), AllowDBNull = true, MaxLength = 255 });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_TimeBeginWorkInSession", DataType = typeof(DateTime), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_TimeWorkInDay", DataType = typeof(Int32), AllowDBNull = true });
            table.Columns.Add(new DataColumn() { ColumnName = "DriverStatus_TimeWork", DataType = typeof(Int32), AllowDBNull = true });

            return table;
        }

        public virtual void AddToTable(DataTable table)
        {
            if (DeviceStatus.ClientSend.Year <= 1970) return;

            DataRow row = table.NewRow();

            row["Id"]= Id;
            row["DbId"] = DbId;
            row["CompanyId"] = CompanyId;
            row["GroupId"] = GroupId;
            row["Serial"] = Serial;
            row["Indentity"] = Indentity;

            //row["SpeedTrace"] = DeviceStatus.SpeedTrace;
            row["DeviceStatus_ClientSend"] = DeviceStatus.ClientSend;
            row["DeviceStatus_ServerRecv"] = DeviceStatus.ServerRecv;
            row["DeviceStatus_GpsStatus"] = DeviceStatus.GpsStatus;
            row["DeviceStatus_TotalGpsDistance"] = DeviceStatus.TotalGpsDistance;
            row["DeviceStatus_TotalCurrentGpsDistance"] = DeviceStatus.TotalCurrentGpsDistance;
            row["DeviceStatus_Fuel"] = DeviceStatus.Fuel;
            row["DeviceStatus_Temperature"] = DeviceStatus.Temperature;
            row["DeviceStatus_GsmSignal"] = DeviceStatus.GsmSignal;
            row["DeviceStatus_Power"] = DeviceStatus.Power;
            row["DeviceStatus_Machine"] = DeviceStatus.Machine;
            row["DeviceStatus_AirMachine"] = DeviceStatus.AirMachine;
            row["DeviceStatus_Sos"] = DeviceStatus.Sos;
            row["DeviceStatus_UseTemperature"] = DeviceStatus.UseTemperature;
            row["DeviceStatus_UseFuel"] = DeviceStatus.UseFuel;
            row["DeviceStatus_UseRfid"] = DeviceStatus.UseRfid;
            row["DeviceStatus_Door"] = DeviceStatus.Door;
            row["DeviceStatus_SpeedTrace"] = DeviceStatus.SpeedTrace;
            row["DeviceStatus_Angle"] = DeviceStatus.Angle;
            row["DeviceStatus_Speed"] = DeviceStatus.Speed;

            float GpsInfo_Lat = DeviceStatus?.GpsInfo.Lat ?? 0f;
            if (float.IsNaN(GpsInfo_Lat) || float.IsInfinity(GpsInfo_Lat)) GpsInfo_Lat = 0f;
            if (-1E-10 < GpsInfo_Lat && GpsInfo_Lat < 1E-10) GpsInfo_Lat = 0f;
            row["GpsInfo_Lat"] = GpsInfo_Lat;

            float GpsInfo_Lng = DeviceStatus?.GpsInfo.Lng ?? 0f;
            if (float.IsNaN(GpsInfo_Lng) || float.IsInfinity(GpsInfo_Lng)) GpsInfo_Lng = 0f;
            if (-1E-10 < GpsInfo_Lng && GpsInfo_Lng < 1E-10) GpsInfo_Lng = 0f;
            row["GpsInfo_Lng"] = GpsInfo_Lng;

            row["GpsInfo_Address"] = DeviceStatus?.GpsInfo.Address;
            row["DriverStatus_DriverId"] = DriverStatus.DriverId;
            row["DriverStatus_Name"] = DriverStatus.Name;
            row["DriverStatus_Gplx"] = DriverStatus.Gplx;
            row["DriverStatus_TimeBeginWorkInSession"] = DriverStatus.TimeBeginWorkInSession;
            row["DriverStatus_TimeWorkInDay"] = DriverStatus.TimeWorkInDay;
            row["DriverStatus_TimeWork"] = DriverStatus.TimeWork;

            table.Rows.Add(row);
        }

        public static DataTable CreateTable(List<DeviceLog> src)
        {
            DataTable table = CreateTable();
            foreach (var item in src)
            {
                item.AddToTable(table);
            }
            return table;
        }

    }


    //public class DeviceLogCollection : ISerializerModal
    //{
    //    public static int VERISON = 1;
    //    public List<DeviceLog> list = new List<DeviceLog>();
    //    public DeviceLog first = new DeviceLog();

    //    public DeviceLogCollection() { }
    //    public DeviceLogCollection(ICollection<DeviceLog> data)
    //    {
    //        if(data==null)
    //        {
    //            list = new List<DeviceLog>();
    //            return;
    //        }

    //        list = new List<DeviceLog>(data);
    //        if (list != null)
    //            first = list.FirstOrDefault();
    //    }

    //    public void Deserializer(BinaryReader stream, int version)
    //    {
    //        list.Clear();
    //        int len = stream.ReadInt32(); 
    //        if (len > 0)
    //        {
    //            first = new DeviceLog();
    //            first.CompanyId = stream.ReadInt64();
    //            first.DbId = stream.ReadInt32();
    //            first.GroupId = stream.ReadInt64();
    //            first.Indentity = Guid.Parse(stream.ReadString());
    //            first.Serial = stream.ReadInt64();
    //            for (int i = 0; i < len; i++)
    //            {
    //                DeviceLog devlog = new DeviceLog();
    //                devlog.Deserializer(stream, version);
    //                devlog.CompanyId = first.CompanyId;
    //                devlog.DbId = first.DbId;
    //                devlog.GroupId = first.GroupId;
    //                devlog.Indentity = first.Indentity;
    //                devlog.Serial = first.Serial;
    //                list.Add(devlog);
    //            }
    //        }
    //    }

    //    public void Serializer(BinaryWriter stream)
    //    {
    //        stream.Write(list.Count);
    //        if (list.Count > 0)
    //        {
    //            stream.Write(first.CompanyId);
    //            stream.Write(first.DbId);
    //            stream.Write(first.GroupId);
    //            stream.Write(first.Indentity.ToString());
    //            stream.Write(first.Serial);
    //            foreach (var item in list)
    //            {
    //                item.Serializer(stream);
    //            }
    //        }
    //    }


    //    public byte[] Serializer()
    //    {
    //        MemoryStream ms=null;
    //        BinaryWriter stream = null;

    //        try
    //        {
    //            ms = new MemoryStream();
    //            stream = new BinaryWriter(ms);
    //            stream.Write(VERISON);
    //            Serializer(stream);
    //            stream.Flush();
    //            return ms.ToArray();
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            if (stream != null) stream.Close();
    //            if (ms != null) stream.Close();
    //        }
    //    }

    //    public void Deserializer(byte[] data)
    //    {
    //        MemoryStream ms = null;
    //        BinaryReader stream = null;

    //        try
    //        {
    //            ms = new MemoryStream(data);
    //            stream = new BinaryReader(ms);
    //            int version = stream.ReadInt32();
    //            Deserializer(stream, version);
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //        finally
    //        {
    //            if (stream != null) stream.Close();
    //            if (ms != null) stream.Close();
    //        }
    //    }


    //}

}