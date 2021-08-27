using System;
using System.Linq;
using DaoDatabase;
using DaoDatabase.AutoMapping.Enums;
using DaoDatabase.AutoMapping.MapAtribute;
using Datacenter.Model.Utils;

namespace Datacenter.Model.Entity
{
    /// <summary>
    /// thiết bị
    /// </summary>
    [Table]
    public class Device : IEntity, ICacheModel
    {
        private Specification _specification;
        private DeviceSetupInfo _setupInfo;
        private DeviceSimInfo _simInfo;
        private DeviceStatus _status;

        /// <summary>
        ///     serial thiết bị
        /// </summary>
        [PrimaryKey(KeyGenerateType = KeyGenerateType.Manual)]
        public virtual long Serial { get; set; }

        /// <summary>
        ///     Id của thiết bị
        /// </summary>
        [BasicColumn]
        public virtual string Id { get; set; }

        /// <summary>
        ///     Thời gian khai báo thiết bị
        /// </summary>
        [BasicColumn]
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        ///     Thời gian hết hạn
        /// </summary>
        [BasicColumn]
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        ///     Ngày đóng phí
        /// </summary>
        [BasicColumn]
        public virtual DateTime PaidFee { get; set; }

        /// <summary>
        ///     Id công ty
        /// </summary>
        [BasicColumn]
        public virtual long CompanyId { get; set; }

        /// <summary>
        ///     Id đội xe
        /// </summary>
        [BasicColumn]
        public virtual long GroupId { get; set; }

        /// <summary>
        ///     Biển số
        /// </summary>
        [BasicColumn]
        public virtual string Bs { get; set; }

        /// <summary>
        ///     Sở giao thông vận tải đăng ký xe
        /// </summary>
        [BasicColumn]
        public virtual string Sgtvt { get; set; }

        /// <summary>
        ///     không được báo cáo  ( khi set == true thiết bị vẫn chạy nhưng sẽ ko dc báo cáo
        ///     trên web quản lý)
        /// </summary>
        [BasicColumn]
        public virtual bool NotView { get; set; }

        /// <summary>
        ///     truyền thông tin cho tổng cục đường bộ
        /// </summary>
        [BasicColumn]
        public virtual bool BgtTranportData { get; set; }

        /// <summary>
        ///     loại  hình kinh doanh
        /// </summary>
        [BasicColumn]
        public virtual DeviceActivityType ActivityType { get; set; }


        private string _ModelName;
        /// <summary>
        ///     tên loại xe
        /// </summary>
        [BasicColumn]
        public virtual string ModelName
        {
            get { return _ModelName; }
            set
            {
                _ModelName = value;
                UpdateDeviceType();
            }
        }

        /// <summary>
        ///     thông tin gắn với thiết bị dùng để quản lý phần đỏi hiết bị
        /// </summary>
        [BasicColumn]
        public virtual Guid Indentity { get; set; }

        /// <summary>
        ///     chưa các thông tin cache cần thiết cho quá trình xử lý
        /// </summary>
        public virtual DeviceTemp Temp { get; set; } = new DeviceTemp();

        /// <summary>
        /// bãng mẫu cây nhiên liệu
        /// </summary>
        [BasicColumn]
        public virtual string FuelSheet { set; get; }

        /// <summary>
        ///     chứa thông tin trạng thái hiện tại của thiết bị
        /// </summary>
        [HasOneColumn(Type = HasOneType.Parent)]
        public virtual DeviceStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        ///     chứa thông tin sim
        /// </summary>
        [HasOneColumn(Type = HasOneType.Parent)]
        public virtual DeviceSimInfo SimInfo
        {
            get { return _simInfo; }
            set { _simInfo = value; }
        }

        [HasOneColumn(Type = HasOneType.Parent)]
        // ReSharper disable once ConvertToAutoProperty
        public virtual Specification Specification
        {
            get { return _specification; }
            set { _specification = value; }
        }

        [HasOneColumn(Type = HasOneType.Parent)]
        public virtual DeviceSetupInfo SetupInfo
        {
            get { return _setupInfo; }
            set { _setupInfo = value; }
        }
        /// <summary>
        /// thiết bị đã được check hay chưa
        /// </summary>
        [BasicColumn(IsIndex = true)]
        public virtual bool Valid { get; set; }

        [BasicColumn()]
        public virtual string Phone { get; set; }
        [BasicColumn()]
        public virtual string Vin { get; set; }

        /// <summary>
        /// Đảo ngược tình trạng cửa
        /// </summary>
        [BasicColumn]
        public virtual bool InvertDoor { get; set; }

        /// <summary>
        /// Đảo ngược tình trạng máy lạnh
        /// </summary>
        [BasicColumn]
        public virtual bool InvertAir { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [BasicColumn]
        public virtual string Note { get; set; }

        /// <summary>
        /// Người lắp đặt
        /// </summary>
        [BasicColumn]
        public virtual string Installer { get; set; }

        /// <summary>
        /// Người quản lý/bão trì
        /// </summary>
        [BasicColumn]
        public virtual string Maintaincer { get; set; }

        private string _FuelParams = "";
        public virtual int[] FuelParamList { get; set; }
        private static char[] SEPS = new char[] { '|', ',', ';', ' ' };

        /// <summary>
        /// Tham số bình nhiên liệu, cách nhau dấu '|', ',', ';', ' '
        /// Tham số gồm : chiều cao đầu mút bình nhiên liệu, ml đầu mút
        /// </summary>
        [BasicColumn]
        public virtual string FuelParams
        {
            get
            {
                return _FuelParams;
            }
            set
            {
                _FuelParams = value;
                if (string.IsNullOrWhiteSpace(_FuelParams)) return;
                try
                {
                    String[] ss = _FuelParams.Split(SEPS, StringSplitOptions.RemoveEmptyEntries);
                    FuelParamList = new int[ss.Length];
                    for (int i = ss.Length - 1; i >= 0; i--)
                    {
                        if (!String.IsNullOrWhiteSpace(ss[i]))
                            FuelParamList[i] = int.Parse(ss[i]);
                        else
                            FuelParamList[i] = -1;
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Định mức nhiên liệu chạy trong 100KM (ml)
        /// </summary>
        [BasicColumn]
        public virtual float FuelQuotaKm{ set; get; }

        /// <summary>
        /// Định mức nhiên liệu chạy trong 1 giờ (ml)
        /// </summary>
        [BasicColumn]
        public virtual float FuelQuoteHour{ set; get; }

        /// <summary>
        /// Sổ điện thoại của chủ xe
        /// </summary>
        [BasicColumn]
        public virtual string OwnerPhone { get; set; }

        /// <summary>
        /// Gửi tin nhắn báo cho chủ xe
        /// </summary>
        [BasicColumn]
        public virtual bool SmsAlarm { get; set; }

        /// <summary>
        /// Số lần giới hạn đồng bộ, nếu vượt quá số lần này thì gửi tin nhắn thông báo
        /// </summary>
        [BasicColumn]
        public virtual int  OnlineTimeout { get; set; }

        /// <summary>
        /// Gửi email báo cho chủ xe
        /// </summary>
        [BasicColumn]
        public virtual bool EmailAlarm { get; set; }

        /// <summary>
        /// email của chủ xe
        /// </summary>
        [BasicColumn]
        public virtual string EmailAddess{ get; set; }

        /// <summary>
        /// CameraId
        /// </summary>
        [BasicColumn]
        public virtual string CameraId { get; set; }


        public virtual void FixNullObject()
        {
            //if (Status == null)
            //{
            //    Status = new DeviceStatus {Device = this, Serial = Serial};
            //    Status.FixNullObject();
            //}
            //if (SimInfo == null)
            //{
            //    SimInfo = new DeviceSimInfo {Device = this, Serial = Serial};
            //    SimInfo.FixNullObject();
            //}
            //if (SetupInfo == null)
            //{
            //    SetupInfo = new DeviceSetupInfo { Device = this, Serial = Serial };
            //    SetupInfo.FixNullObject();
            //}
            //if (Specification == null)
            //{
            //    Specification = new Specification { Device = this, Serial = Serial };
            //    Specification.FixNullObject();
            //}
        }

        /// <summary>
        /// Loai xe (chưa lưu trữ trong db), Biến tạm sử dụng nhận dạng loại xe
        /// </summary>
        public virtual DeviceType DeviceType { get; set; } = DeviceType.None;


        /// <summary>
        /// Cập nhật DeviceType dựa theo _ModelName
        /// </summary>
        private void UpdateDeviceType()
        {
            String loaixe = _ModelName ?? ""; loaixe = loaixe.Trim();
            if (loaixe.EndsWith("(XCT)")) DeviceType = DeviceType.ConstructionVehicle;
            else if (loaixe.EndsWith("(TX7)")) DeviceType = DeviceType.TaxiVehicle;
            else if (loaixe.EndsWith("(PĐ)") || loaixe.EndsWith("(PD)")) DeviceType = DeviceType.Dynamo;
            else if (loaixe.EndsWith("(BD)")) DeviceType = DeviceType.OilVehicle;
            else DeviceType = DeviceType.OtherVehicle;
        }


    }
}