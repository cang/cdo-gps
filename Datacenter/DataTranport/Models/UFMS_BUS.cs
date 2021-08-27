using System.Collections.Generic;

namespace UFMS_BUS
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BaseMessage")]
    public partial class BaseMessage : global::ProtoBuf.IExtensible
    {
        public BaseMessage() { }

        private UFMS_BUS.BaseMessage.MsgType _msgType;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"msgType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public UFMS_BUS.BaseMessage.MsgType msgType
        {
            get { return _msgType; }
            set { _msgType = value; }
        }
        private UFMS_BUS.RegVehicle _msgRegVehicle;
        [global::ProtoBuf.ProtoMember(400, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.RegVehicle msgRegVehicle
        {
            get { return _msgRegVehicle; }
            set { _msgRegVehicle = value; }
        }
        private UFMS_BUS.RegDriver _msgRegDriver;
        [global::ProtoBuf.ProtoMember(410, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.RegDriver msgRegDriver
        {
            get { return _msgRegDriver; }
            set { _msgRegDriver = value; }
        }
        private UFMS_BUS.RegCompany _msgRegCompany;
        [global::ProtoBuf.ProtoMember(420, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.RegCompany msgRegCompany
        {
            get { return _msgRegCompany; }
            set { _msgRegCompany = value; }
        }
        private UFMS_BUS.WayPoint _msgWayPoint;
        [global::ProtoBuf.ProtoMember(100, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.WayPoint msgWayPoint
        {
            get { return _msgWayPoint; }
            set { _msgWayPoint = value; }
        }

        private UFMS_BUS.BusWayPoint _msgBusWayPoint;
        [global::ProtoBuf.ProtoMember(102, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.BusWayPoint msgBusWayPoint
        {
            get { return _msgBusWayPoint; }
            set { _msgBusWayPoint = value; }
        }

        private UFMS_BUS.BusWayPointBatch _msgBusWayPointBatch;
        [global::ProtoBuf.ProtoMember(103, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.BusWayPointBatch msgBusWayPointBatch
        {
            get { return _msgBusWayPointBatch; }
            set { _msgBusWayPointBatch = value; }
        }

        private UFMS_BUS.BusWayPoints _msgBusWayPoints;
        [global::ProtoBuf.ProtoMember(104, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.BusWayPoints msgBusWayPoints
        {
            get { return _msgBusWayPoints; }
            set { _msgBusWayPoints = value; }
        }

        private UFMS_BUS.StudentCheckInPoint _msgStudentCheckInPoint;
        [global::ProtoBuf.ProtoMember(105, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.StudentCheckInPoint msgStudentCheckInPoint
        {
            get { return _msgStudentCheckInPoint; }
            set { _msgStudentCheckInPoint = value; }
        }

        private UFMS_BUS.BusTicketPoint _msgBusTicketPoint;
        [global::ProtoBuf.ProtoMember(500, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.BusTicketPoint msgBusTicketPoint
        {
            get { return _msgBusTicketPoint; }
            set { _msgBusTicketPoint = value; }
        }

        private UFMS_BUS.BusTicketStartEndPoint _msgBusTicketStartEndPoint;
        [global::ProtoBuf.ProtoMember(501, IsRequired = true, Name = @"msg", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public UFMS_BUS.BusTicketStartEndPoint msgBusTicketStartEndPoint
        {
            get { return _msgBusTicketStartEndPoint; }
            set { _msgBusTicketStartEndPoint = value; }
        }


        [global::ProtoBuf.ProtoContract(Name = @"MsgType")]
        public enum MsgType
        {

            [global::ProtoBuf.ProtoEnum(Name = @"WayPoint", Value = 100)]
            WayPoint = 100,

            [global::ProtoBuf.ProtoEnum(Name = @"BusWayPoint", Value = 102)]
            BusWayPoint = 102,

            [global::ProtoBuf.ProtoEnum(Name = @"BusWayPointBatch", Value = 103)]
            BusWayPointBatch = 103,

            [global::ProtoBuf.ProtoEnum(Name = @"BusWayPoints", Value = 104)]
            BusWayPoints = 104,

            [global::ProtoBuf.ProtoEnum(Name = @"StudentCheckInPoint", Value = 105)]
            StudentCheckInPoint = 105,


            [global::ProtoBuf.ProtoEnum(Name = @"RegVehicle", Value = 400)]
            RegVehicle = 400,

            [global::ProtoBuf.ProtoEnum(Name = @"RegDriver", Value = 410)]
            RegDriver = 410,

            [global::ProtoBuf.ProtoEnum(Name = @"RegCompany", Value = 420)]
            RegCompany = 420,

            [global::ProtoBuf.ProtoEnum(Name = @"BusTicketPoint", Value = 500)]
            BusTicketPoint = 500,

            [global::ProtoBuf.ProtoEnum(Name = @"BusTicketStartEndPoint", Value = 501)]
            BusTicketStartEndPoint = 501,

            [global::ProtoBuf.ProtoEnum(Name = @"BusTicketRecoverPoint", Value = 502)]
            BusTicketRecoverPoint = 502,

        }


        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegVehicle")]
    public partial class RegVehicle : global::ProtoBuf.IExtensible
    {
        public RegVehicle() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        private UFMS_BUS.RegVehicle.VehicleType _vehicleType;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"vehicleType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public UFMS_BUS.RegVehicle.VehicleType vehicleType
        {
            get { return _vehicleType; }
            set { _vehicleType = value; }
        }
        private string _driver;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private string _company;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"company", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string company
        {
            get { return _company; }
            set { _company = value; }
        }

        private int _deviceModelNo = default(int);
        [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"deviceModelNo", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int deviceModelNo
        {
            get { return _deviceModelNo; }
            set { _deviceModelNo = value; }
        }

        private string _deviceModel = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"deviceModel", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string deviceModel
        {
            get { return _deviceModel; }
            set { _deviceModel = value; }
        }

        private string _deviceId = "";
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"deviceId", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string deviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        private string _sim = "";
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"sim", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string sim
        {
            get { return _sim; }
            set { _sim = value; }
        }

        private int _datetime = default(int);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }

        private string _vin = "";
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"vin", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string vin
        {
            get { return _vin; }
            set { _vin = value; }
        }
        [global::ProtoBuf.ProtoContract(Name = @"VehicleType")]
        public enum VehicleType
        {

            [global::ProtoBuf.ProtoEnum(Name = @"Khach", Value = 100)]
            Khach = 100,

            [global::ProtoBuf.ProtoEnum(Name = @"Bus", Value = 200)]
            Bus = 200,

            [global::ProtoBuf.ProtoEnum(Name = @"HopDong", Value = 300)]
            HopDong = 300,

            [global::ProtoBuf.ProtoEnum(Name = @"DuLich", Value = 400)]
            DuLich = 400,

            [global::ProtoBuf.ProtoEnum(Name = @"Container", Value = 500)]
            Container = 500
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegDriver")]
    public partial class RegDriver : global::ProtoBuf.IExtensible
    {
        public RegDriver() { }

        private string _driver;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }

        private string _name = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _datetimeIssue = default(int);
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"datetimeIssue", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetimeIssue
        {
            get { return _datetimeIssue; }
            set { _datetimeIssue = value; }
        }

        private int _datetimeExpire = default(int);
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"datetimeExpire", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        [global::System.ComponentModel.DefaultValue(default(int))]
        public int datetimeExpire
        {
            get { return _datetimeExpire; }
            set { _datetimeExpire = value; }
        }

        private string _regPlace = "";
        [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"regPlace", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string regPlace
        {
            get { return _regPlace; }
            set { _regPlace = value; }
        }

        private string _license = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"license", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string license
        {
            get { return _license; }
            set { _license = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"RegCompany")]
    public partial class RegCompany : global::ProtoBuf.IExtensible
    {
        public RegCompany() { }

        private string _company;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"company", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string company
        {
            get { return _company; }
            set { _company = value; }
        }

        private string _name = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _address = "";
        [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"address", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string address
        {
            get { return _address; }
            set { _address = value; }
        }

        private string _tel = "";
        [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"tel", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string tel
        {
            get { return _tel; }
            set { _tel = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"WayPoint")]
    public partial class WayPoint : global::ProtoBuf.IExtensible
    {
        public WayPoint() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }
        private string _driver = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private float _speed;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private int _datetime;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }
        private double _x;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }
        private float _z = default(float);
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float z
        {
            get { return _z; }
            set { _z = value; }
        }
        private float _heading = default(float);
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"heading", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float heading
        {
            get { return _heading; }
            set { _heading = value; }
        }
        private bool _ignition = default(bool);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"ignition", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool ignition
        {
            get { return _ignition; }
            set { _ignition = value; }
        }
        private bool _door = default(bool);
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"door", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool door
        {
            get { return _door; }
            set { _door = value; }
        }
        private bool _aircon = default(bool);
        [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"aircon", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool aircon
        {
            get { return _aircon; }
            set { _aircon = value; }
        }

        //For save data to raw table
        private double _maxValidSpeed = default(double);
        [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"maxvalidspeed", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(double))]
        public double maxvalidspeed
        {
            get { return _maxValidSpeed; }
            set { _maxValidSpeed = value; }
        }

        private float _vss = default(float);
        [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name = @"vss", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float vss
        {
            get { return _vss; }
            set { _vss = value; }
        }

        private string _location = default(string);
        [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name = @"location", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(string))]
        public string location
        {
            get { return _location; }
            set { _location = value; }
        }
        //End save data to raw table

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusWayPoint")]
    public partial class BusWayPoint : global::ProtoBuf.IExtensible
    {
        public BusWayPoint() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        private string _driver = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }
        private float _speed;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private int _datetime;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }
        private double _x;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }

        private float _z = default(float);
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float z
        {
            get { return _z; }
            set { _z = value; }
        }

        private float _heading = default(float);
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"heading", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float heading
        {
            get { return _heading; }
            set { _heading = value; }
        }

        private bool _ignition = default(bool);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"ignition", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool ignition
        {
            get { return _ignition; }
            set { _ignition = value; }
        }


        private bool _aircon = default(bool);
        [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"aircon", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool aircon
        {
            get { return _aircon; }
            set { _aircon = value; }
        }

        private bool _door_up = default(bool);
        [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"door_up", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool door_up
        {
            get { return _door_up; }
            set { _door_up = value; }
        }

        private bool _door_down = default(bool);
        [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"door_down", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool door_down
        {
            get { return _door_down; }
            set { _door_down = value; }
        }

        private bool _sos = default(bool);
        [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name = @"sos", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool sos
        {
            get { return _sos; }
            set { _sos = value; }
        }

        private bool _working = default(bool);
        [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name = @"working", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool working
        {
            get { return _working; }
            set { _working = value; }
        }

        private float _analog1 = default(float);
        [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name = @"analog1", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float analog1
        {
            get { return _analog1; }
            set { _analog1 = value; }
        }

        private float _analog2 = default(float);
        [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name = @"analog2", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float analog2
        {
            get { return _analog2; }
            set { _analog2 = value; }
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }


    }

    //dung de gui waypoint cua nhieu thiet bi cung luc
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusWayPoints")]
    public partial class BusWayPoints : global::ProtoBuf.IExtensible
    {
        public BusWayPoints() { }

        private List<BusWayPoint> _list;
        [global::ProtoBuf.ProtoMember(1, Name = @"Events", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public List<BusWayPoint> list
        {
            get { return _list; }
            set { _list = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }

    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"StudentCheckInPoint")]
    public partial class StudentCheckInPoint : global::ProtoBuf.IExtensible
    {
        public StudentCheckInPoint() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        private string _studentcode = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"studentcode", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string studentcode
        {
            get { return _studentcode; }
            set { _studentcode = value; }
        }
        private float _speed;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private int _datetime;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }
        private double _x;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }
        private double _y;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }

        private float _z = default(float);
        [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float z
        {
            get { return _z; }
            set { _z = value; }
        }

        private float _heading = default(float);
        [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"heading", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
        [global::System.ComponentModel.DefaultValue(default(float))]
        public float heading
        {
            get { return _heading; }
            set { _heading = value; }
        }

        private bool _working = default(bool);
        [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"working", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue(default(bool))]
        public bool working
        {
            get { return _working; }
            set { _working = value; }
        }


        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }


    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusWayPointBatch")]
    public partial class BusWayPointBatch : global::ProtoBuf.IExtensible
    {
        public BusWayPointBatch() { }

        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        private string _driver = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"driver", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string driver
        {
            get { return _driver; }
            set { _driver = value; }
        }

        [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusTrackPoint")]
        public partial class BusTrackPoint : global::ProtoBuf.IExtensible
        {
            public BusTrackPoint() { }

            private int _datetime;
            [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public int datetime
            {
                get { return _datetime; }
                set { _datetime = value; }
            }
            private float _speed;
            [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"speed", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            public float speed
            {
                get { return _speed; }
                set { _speed = value; }
            }
            private double _x;
            [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public double x
            {
                get { return _x; }
                set { _x = value; }
            }
            private double _y;
            [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
            public double y
            {
                get { return _y; }
                set { _y = value; }
            }

            private float _z = default(float);
            [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"z", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(float))]
            public float z
            {
                get { return _z; }
                set { _z = value; }
            }

            private float _heading = default(float);
            [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"heading", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(float))]
            public float heading
            {
                get { return _heading; }
                set { _heading = value; }
            }

            private bool _ignition = default(bool);
            [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"ignition", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool ignition
            {
                get { return _ignition; }
                set { _ignition = value; }
            }

            private bool _aircon = default(bool);
            [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name = @"aircon", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool aircon
            {
                get { return _aircon; }
                set { _aircon = value; }
            }
            private bool _door_up = default(bool);
            [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name = @"door_up", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool door_up
            {
                get { return _door_up; }
                set { _door_up = value; }
            }

            private bool _door_down = default(bool);
            [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name = @"door_down", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool door_down
            {
                get { return _door_down; }
                set { _door_down = value; }
            }

            private bool _sos = default(bool);
            [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name = @"sos", DataFormat = global::ProtoBuf.DataFormat.Default)]
            [global::System.ComponentModel.DefaultValue(default(bool))]
            public bool sos
            {
                get { return _sos; }
                set { _sos = value; }
            }

            private float _analog1 = default(float);
            [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name = @"analog1", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(float))]
            public float analog1
            {
                get { return _analog1; }
                set { _analog1 = value; }
            }

            private float _analog2 = default(float);
            [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name = @"analog2", DataFormat = global::ProtoBuf.DataFormat.FixedSize)]
            [global::System.ComponentModel.DefaultValue(default(float))]
            public float analog2
            {
                get { return _analog2; }
                set { _analog2 = value; }
            }
            private global::ProtoBuf.IExtension extensionObject;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
        }



        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }

    }


    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusTicketPoint")]
    public partial class BusTicketPoint : global::ProtoBuf.IExtensible
    {
        public BusTicketPoint() { }

        //Biển số xe
        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        //Mã số tuyến
        private string _routecode = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"routecode", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string routecode
        {
            get { return _routecode; }
            set { _routecode = value; }
        }

        //Ngày giờ thiết bị ghi nhận (unix time)
        private int _datetime;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }

        //Kinh độ
        private double _x;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }

        //Vĩ độ
        private double _y;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }

        //Tên trạm
        private string _stationname = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"stationname", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string stationname
        {
            get { return _stationname; }
            set { _stationname = value; }
        }

        //Mã loại vé
        private BusTicketType _tickettype;
        [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name = @"tickettype", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public BusTicketType tickettype
        {
            get { return _tickettype; }
            set { _tickettype = value; }
        }

        //Giá vé
        private double _fare;
        [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name = @"fare", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double fare
        {
            get { return _fare; }
            set { _fare = value; }
        }

        //Số serires vé
        private string _series = "";
        [global::ProtoBuf.ProtoMember(9, IsRequired = true, Name = @"series", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string series
        {
            get { return _series; }
            set { _series = value; }
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }


    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusTicketRecoverPoint")]
    public partial class BusTicketRecoverPoint : global::ProtoBuf.IExtensible
    {
        public BusTicketRecoverPoint() { }

        //Biển số xe
        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        //Mã số tuyến
        private string _routecode = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"routecode", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string routecode
        {
            get { return _routecode; }
            set { _routecode = value; }
        }

        //Ngày giờ thiết bị ghi nhận (unixtime)
        private int _date;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"date", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int date
        {
            get { return _date; }
            set { _date = value; }
        }

        //Chuyến số
        private int _tripno;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"tripno", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int tripno
        {
            get { return _tripno; }
            set { _tripno = value; }
        }


        //Thông tin các loại vé
        private List<BusTicketTotal> _list;
        [global::ProtoBuf.ProtoMember(5, Name = @"tickets", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public List<BusTicketTotal> list
        {
            get { return _list; }
            set { _list = value; }
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }


    }


    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"BusTicketStartEndPoint")]
    public partial class BusTicketStartEndPoint : global::ProtoBuf.IExtensible
    {
        public BusTicketStartEndPoint() { }

        //Biển số xe
        private string _vehicle;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"vehicle", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string vehicle
        {
            get { return _vehicle; }
            set { _vehicle = value; }
        }

        //Mã số tuyến
        private string _routecode = "";
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"routecode", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string routecode
        {
            get { return _routecode; }
            set { _routecode = value; }
        }

        //Ngày giờ thiết bị ghi nhận (unixtime)
        private int _datetime;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"datetime", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }

        //Kinh độ
        private double _x;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }

        //Vĩ độ
        private double _y;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }

        //Tên trạm
        private string _stationname = "";
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"stationname", DataFormat = global::ProtoBuf.DataFormat.Default)]
        [global::System.ComponentModel.DefaultValue("")]
        public string stationname
        {
            get { return _stationname; }
            set { _stationname = value; }
        }

        //Chuyến số
        private int _tripno;
        [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name = @"tripno", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int tripno
        {
            get { return _tripno; }
            set { _tripno = value; }
        }

        //Loại message (bắt đầu/kết thúc chuyến)
        private BusTicketPointType _pointtype;
        [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name = @"pointtype", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public BusTicketPointType pointtype
        {
            get { return _pointtype; }
            set { _pointtype = value; }
        }

        //Thông tin các loại vé
        private List<BusTicketTotal> _list;
        [global::ProtoBuf.ProtoMember(9, Name = @"tickets", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public List<BusTicketTotal> list
        {
            get { return _list; }
            set { _list = value; }
        }

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }


    }


    [global::ProtoBuf.ProtoContract(Name = @"BusTicketTotal")]
    public partial class BusTicketTotal : global::ProtoBuf.IExtensible
    {
        public BusTicketTotal() { }

        //Số lượng vé: bằng 0 khi bắt đầu chuyến mới
        private int _total;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"total", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int total
        {
            get { return _total; }
            set { _total = value; }
        }

        //Số serires vé
        private string _series;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"series", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string series
        {
            get { return _series; }
            set { _series = value; }
        }

        //Mã loại vé
        private BusTicketType _tickettype;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"tickettype", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public BusTicketType tickettype
        {
            get { return _tickettype; }
            set { _tickettype = value; }
        }
        //End save data to raw table

        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }


    [global::ProtoBuf.ProtoContract(Name = @"BusTicketType")]
    public enum BusTicketType
    {

        [global::ProtoBuf.ProtoEnum(Name = @"SET", Value = 1)]//vé tập
        SET = 1,

        [global::ProtoBuf.ProtoEnum(Name = @"TURN", Value = 2)] //vé lượt
        TURN = 2,

        [global::ProtoBuf.ProtoEnum(Name = @"STUDENT", Value = 3)]//vé sinh viên
        STUDENT = 103,

        [global::ProtoBuf.ProtoEnum(Name = @"FREE", Value = 4)]//vé miễn
        FREE = 4,

        [global::ProtoBuf.ProtoEnum(Name = @"EXAM", Value = 5)] //vé mùa thi
        EXAM = 5,
    }

    [global::ProtoBuf.ProtoContract(Name = @"BusTickePointType")]
    public enum BusTicketPointType
    {

        [global::ProtoBuf.ProtoEnum(Name = @"START", Value = 1)]
        START = 1,

        [global::ProtoBuf.ProtoEnum(Name = @"END", Value = 2)]
        END = 2,
    }
}