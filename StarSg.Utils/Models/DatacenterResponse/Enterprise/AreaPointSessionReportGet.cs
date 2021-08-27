using System;
using System.Collections.Generic;
using StarSg.Core;
using StarSg.Utils.Models.Tranfer;

namespace StarSg.Utils.Models.DatacenterResponse.Enterprise
{
    public class AreaSessionGet:BaseResponse
    {
        public List<AreaSessionTranfer> Datas { get; set; } 
    }

    public class PointSessionGet : BaseResponse
    {
        public List<PointSessionTranfer> Datas { get; set; }
    }

    public class FuelSessionGet : BaseResponse
    {
        public List<FuelSessionTranfer> Datas { get; set; }
    }

    public class GuestSessionGet : BaseResponse
    {
        public List<GuestSessionTranfer> Datas { get; set; }
    }

    public class AreaSessionTranfer
    {
        public long     Id { set; get; }
        public string   Bs { set; get; }
        public string   Name { set; get; }
        public string   Type { set; get; }
        public string   Note { set; get; }
        public string   Address { set; get; }

        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public int      TotalSeconds { get; set; }
    }

    public class PointSessionTranfer
    {
        public long Id { set; get; }
        public string Bs { set; get; }
        public string Name { set; get; }
        public string Type { set; get; }
        public string Note { set; get; }
        public string Address { set; get; }

        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TotalSeconds { get; set; }

        public float Lat { get; set; }
        public float Lng { get; set; }
    }

    public class FuelSessionTranfer
    {
        public long Id { set; get; }
        public long Serial { get; set; }
        public string Bs { set; get; }
        public DateTime Time { get; set; }

        public string Type { set; get; }

        /// <summary>
        /// Nhiên liệu ban đầu
        /// </summary>
        public float BeginValue { get; set; }

        /// <summary>
        /// Nhiên liệu thay đổi
        /// </summary>
        public float ChangeValue { get; set; }

        /// <summary>
        /// Nhiên liệu còn lại
        /// </summary>
        public float RemainValue { get; set; }

        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Address { set; get; }
    }

    public class GuestSessionTranfer
    {
        public long Id { set; get; }
        public long Serial { get; set; }
        public string Bs { set; get; }

        /// <summary>
        /// Mã Tài xế
        /// </summary>
        public long DriverId { get; set; }

        /// <summary>
        /// Tài xế
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// DT Tài xế
        /// </summary>
        public string DriverPhone { get; set; }

        public bool  HasGuest { set; get; }

        public DateTime BeginTime { get; set; }
        public GpsPoint BeginLocation { get; set; }

        public DateTime EndTime { get; set; }
        public GpsPoint EndLocation { get; set; }

        public int TimeSeconds { get; set; }
        public double DistanceKm { get; set; }

        public string Note { set; get; }
    }

}